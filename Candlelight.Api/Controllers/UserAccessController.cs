using Candlelight.Application.Services;
using Candlelight.Core.Entities.Forms;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Authentication;
using Candlelight.Core.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.WebUtilities;
using AuthenticationService = Candlelight.Application.Services.AuthenticationService;
using Candlelight.Core.Helpers;

namespace Candlelight.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserAccessController(AuthenticationService authenticationService, UserManagementService userManagementService, SteamService steamService) : ControllerBase
{
    private readonly AuthenticationService _authenticationService = authenticationService;
    private readonly UserManagementService _userManagementService = userManagementService;
    private readonly SteamService _steamService = steamService;

    [HttpOptions]
    [Route("SendRegistrationForm")]
    [Route("SendLoginForm")]
    [Route("GetUsersList")]
    public IActionResult HandlePreflight()
    {
        Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");
        Response.Headers.Add("Access-Control-Allow-Methods", "POST, OPTIONS");
        Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
        Response.Headers.Add("Access-Control-Allow-Credentials", "true");

        return Ok();
    }

    [HttpPost]
    [ActionName("SendRegisterForm")]
    [Route("SendRegistrationForm")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> PostAsync([FromBody] RegistrationForm form)
    {
        try
        {
            if (!AuthenticationService.IsRegistrationFormValid(form))
            {
                return BadRequest("Data model is invalid.");
            }

            var user = await _authenticationService.RegisterUser(form);
            return Ok(new {id = user.Id });
        }
        catch (Exception ex) 
        { 
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message }); 
        }
    }

    [HttpPost]
    [Route("SendLoginForm")]
    [ActionName("SendLoginForm")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> PostAsync([FromBody] LoginForm form)
    {
        try
        {
            if (!AuthenticationService.IsLoginFormValid(form))
            {
                return BadRequest("Data model is invalid.");
            }
            var result = await _authenticationService.AttemptLogin(form);
            if (result == null)
            {
                return Unauthorized(form);
            }
            var token = _authenticationService.GenerateJwtToken(result);
            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    }

    [HttpGet]
    [Route("GetUsersList")]
    [ActionName("GetUsersList")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAsync()
    {
        try
        {
            return Ok(await _userManagementService.GetAllUsersAsync());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    }

    [HttpPost("LinkSteam")]
    public async Task<IActionResult> LinkSteam([FromBody] LinkSteamRequest model)
    {
        var user = await _userManagementService.GetUserByIdAsync(model.UserId);
        if (user == null) return NotFound("User not found");

        var steamId = model.SteamId;
        var existingUser = await _userManagementService.GetUserBySteamIdAsync(steamId);
        if (existingUser != null) return BadRequest("Steam account already linked to another user");

        user.SteamId = steamId;
        await _userManagementService.UpdateUserAsync(user);

        return Ok(new { SteamId = steamId });
    }

    [Authorize(Policy = "JwtOnly")]
    [HttpGet("CurrentUser")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (userId is null || !Guid.TryParse(userId, out var guid))
            return Unauthorized();

        var user = await _userManagementService.GetUserByIdAsync(guid);

        if (user is null)
            return NotFound("User not found");

        return Ok(new
        {
            User = user
        });
    }


    [HttpGet("DebugToken")]
    public IActionResult DebugToken([FromHeader(Name = "Authorization")] string token)
    {
        return Ok(new { token });
    }

    [HttpGet("SteamLogin")]
    public IActionResult SteamLogin([FromQuery] string? returnUrl = "/")
    {
        var redirectUri = Url.Action(nameof(SteamCallback), "UserAccess", new { returnUrl }, Request.Scheme);
        return Challenge(new AuthenticationProperties { RedirectUri = redirectUri }, "Steam");
    }

    [HttpGet("SteamCallback")]
    public async Task<IActionResult> SteamCallback([FromQuery] string? returnUrl = "/")
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded)
        {
            var error = result.Failure?.Message ?? "Unknown Steam auth error";
            return BadRequest(new { error });
        }

        var steamIdUrl = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (steamIdUrl == null)
            return BadRequest("Steam ID not found");

        var steamId = steamIdUrl;
        if (steamIdUrl.StartsWith("https://steamcommunity.com/openid/id/"))
        {
            steamId = steamId.Replace("https://steamcommunity.com/openid/id/", "");
        }
        var user = await _userManagementService.GetUserBySteamIdAsync(steamId);

        if (user == null)
        {
            var steamUser = await _steamService.GetPlayerSummaryAsync(steamIdUrl); 
            if (steamUser == null)
                return BadRequest("Steam user not found");
            var newUserId = Guid.NewGuid();

            var avatarUrl = steamUser.AvatarFull;
            var avatarFilename = $"{newUserId}.jpg";
            var avatarsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "avatars");

            if (!Directory.Exists(avatarsFolder))
                Directory.CreateDirectory(avatarsFolder);

            var avatarFilePath = Path.Combine(avatarsFolder, avatarFilename);

            try
            {
                using var avatarResponse = await _steamService.GetAvatarPhotoFromLinkAsync(avatarUrl);
                avatarResponse.EnsureSuccessStatusCode();

                var avatarBytes = await avatarResponse.Content.ReadAsByteArrayAsync();
                await System.IO.File.WriteAllBytesAsync(avatarFilePath, avatarBytes);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to download avatar image: {ex.Message}");
            }

            user = new AppUser
            {
                Id = newUserId,
                SteamId = steamId,
                UserName = steamUser.PersonaName,
                Email = $"{steamId}@steam.auto",
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                UserProfile = new UserProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = newUserId,
                    DisplayName = steamUser.PersonaName,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow,
                    CreatedBy = newUserId,
                    AvatarFilename = avatarFilename,
                }
            };

            var randomPassword = GenerateSecureRandomPassword();
            var hashedPassword = CryptographyHelper.HashPassword(user, randomPassword);
            user.PasswordHash = hashedPassword;

            await _userManagementService.CreateUserAsync(user);
        }

        var token = _authenticationService.GenerateJwtToken(user);

        var redirectUrlWithToken = QueryHelpers.AddQueryString(returnUrl ?? "/", "token", token);
        return Redirect(redirectUrlWithToken);
    }

    [Authorize(Policy = "JwtOnly")]
    [HttpGet("GetCurrentUserId")]
    public async Task<IActionResult> GetCurrentUserId([FromServices] UserContextResolver resolver)
    {
        var user = await resolver.ResolveUserAsync(HttpContext.User);
        if (user == null) return Unauthorized();
        return Ok(new { id = user.Id });
    }

    private static string GenerateSecureRandomPassword()
    {
        return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
    }
}
