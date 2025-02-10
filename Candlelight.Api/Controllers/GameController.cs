using Candlelight.Application.Services;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> GetGamesFromDb([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var (games, totalGames) = await _gameService.GetGamesFromDbAsync(page, pageSize);

        var response = new
        {
            TotalGames = totalGames,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalGames / pageSize),
            Games = games
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