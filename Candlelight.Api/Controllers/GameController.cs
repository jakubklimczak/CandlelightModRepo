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
    [ActionName("GetGamesFromDb")]
    public async Task<IActionResult> GetGamesFromDb([FromQuery] PaginatedQuery query)
    {
        var (games, totalGames) = await _gameService.GetGamesFromDbAsync(query.Page, query.PageSize);

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

    [HttpGet("GetGame/{appId}")]
    [ActionName("GetGame")]
    public async Task<IActionResult> GetGame(int appId)
    {
        var game = await _gameService.GetOrFetchGameByIdAsync(appId);
        if (game == null)
        {
            return NotFound($"Game with AppId {appId} not found.");
        }

        return Ok(game);
    }

    [HttpGet("GetGameFromDb/{appId}")]
    [ActionName("GetGameFromDb")]
    public async Task<IActionResult> GetGameFromDb(int appId)
    {
        var game = await _gameService.GetGameByIdAsync(appId);
        if (game == null)
        {
            return NotFound($"Game with AppId {appId} not found.");
        }

        return Ok(game);
    }
}