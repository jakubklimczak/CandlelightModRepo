using Candlelight.Api.Attributes;
using Candlelight.Application.Services;
using Candlelight.Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("FetchAndSaveTopGames")]
    [ActionName("FetchAndSaveTopGamesAsync")]
    public async Task<IActionResult> FetchAndSaveTopGamesAsync([CurrentUser] AppUser user)
    {
        await _steamService.FetchAndSaveTopGamesAsync(user.Id);
        return Ok("Fetching and saving top games started.");
    }

    [HttpGet("SteamUser/{steamId}")]
    public async Task<IActionResult> GetSteamUser(string steamId)
    {
        var userInfo = await _steamService.GetPlayerSummaryAsync(steamId);
        if (userInfo == null) return NotFound();

        return Ok(userInfo);
    }
}