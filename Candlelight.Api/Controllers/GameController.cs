using Candlelight.Application.Services;
using Candlelight.Core.Dtos.Game;
using Candlelight.Core.Dtos.Query;
using Microsoft.AspNetCore.Mvc;

namespace Candlelight.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController(GameService gameService) : ControllerBase
{
    private readonly GameService _gameService = gameService;
    /// <summary>
    /// Get a paginated list of games.
    /// Example: GET /api/GetGamesFromDbPaginatedQuery?page=1&pageSize=10
    /// </summary>
    [HttpGet("GetGamesFromDbPaginatedQuery")]
    [ActionName("GetPaginatedSteamGameDetailsFromDb")]
    public async Task<IActionResult> GetPaginatedSteamGameDetailsFromDb([FromQuery] PaginatedQuery query)
    {
        var (games, totalGames) = await _gameService.GetSteamGameDetailsFromDbAsync(query.Page, query.PageSize);

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
    public async Task<IActionResult> GetGame(int appId)
    {
        var game = await _gameService.GetOrFetchSteamGameDetailsByIdAsync(appId);
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
}