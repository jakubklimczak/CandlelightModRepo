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
        [FromQuery] bool showOnlySteam,
        [FromQuery] bool showOnlyCustom,
        [FromQuery] GamesSortingOptions sortBy, 
        [FromQuery] string? searchTerm = null
        )
    {
        // TODO: Implement favourite games and owned games
        if (showOnlySteam)
        {
            var (games, totalGames) =
                await _gameService.GetSteamGameDetailsFromDbAsync(query.Page, query.PageSize, sortBy, searchTerm);

            var mappedGameResults = games.Select(game => new GameListItemDto
            {
                Id = game.GameId,
                AppId = game.AppId,
                Name = game.Name,
                HeaderImage = game.HeaderImage,
                Developer = game.Developer,
                Publisher = game.Publisher,
                IsCustom = false
            });

            return Ok(new PaginatedResponse<GameListItemDto>
            {
                TotalItems = totalGames,
                Page = query.Page,
                PageSize = query.PageSize,
                Items = mappedGameResults.ToList(),
            });
        }
        else if (showOnlyCustom)
        {
            var (games, totalGames) =
                await _gameService.GetCustomGameDetailsFromDbAsync(query.Page, query.PageSize, sortBy, searchTerm);

            var mappedGameResults = games.Select(game => new GameListItemDto
            {
                Id = game.GameId,
                Name = game.Name,
                Description = game.Description,
                HeaderImage = string.IsNullOrEmpty(game.CoverImage) ? null : $"{game.CoverImage}",
                Developer = game.Developer,
                Publisher = game.Publisher,
                IsCustom = true
            });

            return Ok(new PaginatedResponse<GameListItemDto>
            {
                TotalItems = totalGames,
                Page = query.Page,
                PageSize = query.PageSize,
                Items = mappedGameResults.ToList(),
            });
        }
        else
        {
            var (games, totalGames) =
                await _gameService.GetAllGamesFromDbAsync(query.Page, query.PageSize, sortBy, searchTerm);

            var mappedGameResults = games.Select(game =>
            {
                if (game.SteamGameDetails is not null)
                {
                    return new GameListItemDto
                    {
                        Id = game.Id,
                        AppId = game.SteamAppId,
                        Name = game.SteamGameDetails.Name,
                        HeaderImage = game.SteamGameDetails.HeaderImage,
                        Developer = game.SteamGameDetails.Developer,
                        Publisher = game.SteamGameDetails.Publisher,
                        IsCustom = false
                    };
                }
                else if (game.CustomGameDetails is not null)
                {
                    return new GameListItemDto
                    {
                        Id = game.Id,
                        Name = game.CustomGameDetails.Name,
                        Description = game.CustomGameDetails.Description,
                        HeaderImage = string.IsNullOrEmpty(game.CustomGameDetails.CoverImage) ? null : $"{game.CustomGameDetails.CoverImage}",
                        Developer = game.CustomGameDetails.Developer,
                        Publisher = game.CustomGameDetails.Publisher,
                        IsCustom = true
                    };
                }

                return null!;
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            }).Where(g => g is not null).ToList();

            return Ok(new PaginatedResponse<GameListItemDto>
            {
                TotalItems = totalGames,
                Page = query.Page,
                PageSize = query.PageSize,
                Items = mappedGameResults
            });
        }
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

    [HttpPost("{gameId}/Favourite")]
    [Authorize]
    public async Task<IActionResult> AddFavourite(Guid gameId, [CurrentUser] AppUser user)
    {
        var userId = user.Id;
        if (!await _gameService.MarkGameAsFavourite(gameId, userId))
            return BadRequest("This game is already in favourites!");
        return Ok();
    }

    [HttpDelete("{gameId}/Favourite")]
    [Authorize]
    public async Task<IActionResult> RemoveFavourite(Guid gameId, [CurrentUser] AppUser user)
    {
        var userId = user.Id;
        if (!await _gameService.RemoveGameFromFavourites(gameId, userId))
            return BadRequest("This game is not in your favourites!");
        return Ok();
    }

    [HttpPost("AddCustom")]
    [Authorize]
    public async Task<IActionResult> AddCustomGame([FromForm] CustomGameDto dto, [CurrentUser] AppUser user, IFormFile? coverImage)
    {
        if (coverImage != null && !coverImage.ContentType.StartsWith("image/"))
        {
            return BadRequest("Only image files are allowed.");
        }

        var userId = user.Id;

        var game = await _gameService.AddCustomGameAsync(dto, userId, coverImage);

        return Ok(game.Id);
    }

}