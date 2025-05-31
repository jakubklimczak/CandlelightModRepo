using Candlelight.Api.Attributes;
using Candlelight.Application.Services;
using Candlelight.Core.Entities;
using Candlelight.Core.Entities.Forms;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Candlelight.Api.Controllers;

/// <summary>
/// Controller responsible for Steam integration.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SteamController(SteamService steamService) : ControllerBase
{
    private readonly SteamService _steamService = steamService;

    /// <summary>
    /// Debug - returns Steam API key. If empty - the application will not function properly.
    /// </summary>
    [HttpGet]
    [Route("Key")]
    public IActionResult GetSteamApiKey()
    {
        var apiKey = _steamService.GetSteamApiKey();
        return Ok(new { ApiKey = apiKey });
    }

    /// <summary>
    /// Endpoint which reads the top 100 games from Steam and adds them to the database. Seeder function, should preferably run on a scheduler.
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("FetchAndSaveTopGames")]
    [ActionName("FetchAndSaveTopGamesAsync")]
    public async Task<IActionResult> FetchAndSaveTopGamesAsync([CurrentUser] AppUser user)
    {
        await _steamService.FetchAndSaveTopGamesAsync(user.Id);
        return Ok("Fetching and saving top games started.");
    }

    /// <summary>
    /// Returns Steam user details by the Steam user ID
    /// </summary>
    [HttpGet("SteamUser/{steamId}")]
    public async Task<IActionResult> GetSteamUser(string steamId)
    {
        var userInfo = await _steamService.GetPlayerSummaryAsync(steamId);
        if (userInfo == null) return NotFound();

        return Ok(userInfo);
    }
}