using Candlelight.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Candlelight.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SteamController(SteamService steamService) : ControllerBase
{
    private readonly SteamService _steamService = steamService;
    [HttpGet]
    [Route("Key")]
    public IActionResult GetSteamApiKey()
    {
        var apiKey = _steamService.GetSteamApiKey();
        return Ok(new { ApiKey = apiKey });
    }

    /// <summary>
    /// Save top games from steam. Default max: 10000
    /// Example: GET /api/FetchAndSaveTopGames?maxGames=10
    /// </summary>
    [HttpPost("FetchAndSaveTopGames")]
    [ActionName("FetchAndSaveTopGamesAsync")]
    public async Task<IActionResult> FetchAndSaveTopGamesAsync([FromQuery] int maxGames)
    {
        await _steamService.FetchAndSaveTopGamesAsync();
        return Ok("Fetching and saving top games started.");
    }
}