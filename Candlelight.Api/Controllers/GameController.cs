using Candlelight.Api.Attributes;
using Candlelight.Application.Services;
using Candlelight.Core.Dtos.Game;
using Candlelight.Core.Dtos.Query;
using Candlelight.Core.Entities;
using Candlelight.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Candlelight.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController(GameService gameService) : ControllerBase
{
    private readonly GameService _gameService = gameService;

    [HttpGet("GetGamesFromDbPaginatedQuery")]
    [ActionName("GetPaginatedSteamGameDetailsFromDb")]
    public async Task<IActionResult> GetPaginatedSteamGameDetailsFromDb(
        [FromQuery] PaginatedQuery query, 
        [FromQuery] bool showOnlyFavourites,
        [FromQuery] bool showOnlyOwned,
        [FromQuery] GamesSortingOptions sortBy, 
        [FromQuery] string? searchTerm = null
        )
    {
        // TODO: Implement favourite games and owned games
        var (games, totalGames) = await _gameService.GetSteamGameDetailsFromDbAsync(query.Page, query.PageSize, sortBy, searchTerm);

        var mappedGameResults = games.Select(game => new GameListItemDto()
        {
            AppId = game.AppId,
            Name = game.Name,
            HeaderImage = game.HeaderImage,
            Developer = game.Developer,
            Publisher = game.Publisher
        });

        var response = new PaginatedResponse<GameListItemDto>()
        {
            TotalItems = totalGames,
            Page = query.Page,
            PageSize = query.PageSize,
            Items = mappedGameResults.ToList(),
        };

        return Ok(response);
    }

    [HttpGet("GetGame/{appId:int}")]
    [ActionName("GetGame")]
    [Authorize]
    public async Task<IActionResult> GetGame(int appId, [CurrentUser] AppUser user)
    {
        var game = await _gameService.GetOrFetchSteamGameDetailsByIdAsync(appId, user.Id);
        if (game == null)
        {
            return NotFound($"Game with AppId {appId} not found.");
        }

        return Ok(game);
    }

    [HttpGet("GetGameFromDb/{appId:int}")]
    [ActionName("GetGameFromDb")]
    public async Task<IActionResult> GetGameFromDb(int appId)
    {
        var game = await _gameService.GetSteamGameDetailsByIdAsync(appId);
        if (game == null)
        {
            return NotFound($"Game with AppId {appId} not found.");
        }

        return Ok(game);
    }

    [HttpPost("{gameId}/favourite")]
    [Authorize]
    public async Task<IActionResult> AddFavourite(Guid gameId, [CurrentUser] AppUser user)
    {
        var userId = user.Id;
        if (!await _gameService.MarkGameAsFavourite(gameId, userId))
            return BadRequest("This game is already in favourites!");
        return Ok();
    }

    [HttpDelete("{gameId}/favourite")]
    [Authorize]
    public async Task<IActionResult> RemoveFavourite(Guid gameId, [CurrentUser] AppUser user)
    {
        var userId = user.Id;
        if (!await _gameService.RemoveGameFromFavourites(gameId, userId))
            return BadRequest("This game is not in your favourites!");
        return Ok();
    }
}