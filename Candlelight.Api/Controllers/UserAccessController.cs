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
using Candlelight.Api.Attributes;

namespace Candlelight.Api.Controllers;

/// <summary>
/// Controller handling user creation and authorisation.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserAccessController(AuthenticationService authenticationService, UserManagementService userManagementService, SteamService steamService) : ControllerBase
{
    private readonly AuthenticationService _authenticationService = authenticationService;
    private readonly UserManagementService _userManagementService = userManagementService;
    private readonly SteamService _steamService = steamService;

    //TODO: check if needed
    /// <summary>
    /// Preflight.
    /// </summary>
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

    /// <summary>
    /// Registers a user with a Candlelight account.
    /// </summary>
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

    /// <summary>
    /// Logs in the user with their Candlelight account.
    /// </summary>
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

    /// <summary>
    /// Returns all users with their info.
    /// </summary>
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

    /// <summary>
    /// Used to link Candlelight account to Steam account.
    /// </summary>
    [HttpGet("LinkSteam")]
    [Authorize(Policy = "JwtOnly")]
    public IActionResult LinkSteam([FromQuery] string? returnUrl = "/profile")
    {
        var redirectUri = Url.Action("LinkSteamCallback", "UserAccess", new { returnUrl = returnUrl }, Request.Scheme);
        var steamLoginUrl = Url.Action("SteamLogin", "UserAccess", new { returnUrl = redirectUri }, Request.Scheme);
        return Ok(new { url = steamLoginUrl });
    }

    /// <summary>
    /// Callback after linking Candlelight account with Steam.
    /// </summary>
    [HttpGet("LinkSteamCallback")]
    [Authorize]
    public async Task<IActionResult> LinkSteamCallback([CurrentUser] AppUser user, [FromQuery] string? returnUrl = "/profile")
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded)
            return BadRequest(new { error = "Steam authentication failed." });

        var steamIdUrl = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(steamIdUrl))
            return BadRequest(new { error = "Steam ID not found." });

        var steamId = steamIdUrl.Replace("https://steamcommunity.com/openid/id/", "");

        // Ensure the Steam ID isn't already linked to another account
        var existingUser = await _userManagementService.GetUserBySteamIdAsync(steamId);
        if (existingUser != null && existingUser.Id != user.Id)
            return BadRequest(new { error = "Steam account is already linked to another user." });

        var steamUser = await _steamService.GetPlayerSummaryAsync(steamIdUrl);
        if (steamUser != null && !string.IsNullOrWhiteSpace(steamUser.AvatarFull))
        {
            var avatarFilename = $"{user.Id}.jpg";
            var avatarsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "avatars");
            Directory.CreateDirectory(avatarsFolder);
            var avatarFilePath = Path.Combine(avatarsFolder, avatarFilename);

            try
            {
                using var avatarResponse = await _steamService.GetAvatarPhotoFromLinkAsync(steamUser.AvatarFull);
                avatarResponse.EnsureSuccessStatusCode();
                var avatarBytes = await avatarResponse.Content.ReadAsByteArrayAsync();
                await System.IO.File.WriteAllBytesAsync(avatarFilePath, avatarBytes);

                user.UserProfile!.AvatarFilename = avatarFilename;
            }
            catch
            {
                Console.WriteLine("Failed to fetch new avatar.");
            }
            user.UserProfile!.DisplayName = steamUser.PersonaName;
        }

        user.SteamId = steamId;
        user.LastUpdated = DateTime.UtcNow;
        user.UserProfile!.LastUpdatedAt = DateTime.UtcNow;

        await _userManagementService.UpdateUserAsync(user);

        return Redirect(returnUrl ?? "/profile");
    }


    /// <summary>
    /// Returns info about currently logged-in user.
    /// </summary>
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

    /// <summary>
    /// Used for debugging JWT.
    /// </summary>
    [HttpGet("DebugToken")]
    public IActionResult DebugToken([FromHeader(Name = "Authorization")] string token)
    {
        return Ok(new { token });
    }

    /// <summary>
    /// Used to log in using Steam.
    /// </summary>
    [HttpGet("SteamLogin")]
    public IActionResult SteamLogin([FromQuery] string? returnUrl = "/")
    {
        var redirectUri = Url.Action(nameof(SteamCallback), "UserAccess", new { returnUrl }, Request.Scheme);
        return Challenge(new AuthenticationProperties { RedirectUri = redirectUri }, "Steam");
    }

    /// <summary>
    /// Callback used when logging in by Steam. Do not use manually unless debugging.
    /// </summary>
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
                    FavouritesVisible = true,
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

    /// <summary>
    /// Returns the ID of the currently logged-in user.
    /// </summary>
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

    /// <summary>
    /// Logs out the currently authenticated Steam user (cookie-based). JWT users are handled on the frontend.
    /// </summary>
    [HttpPost("Logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        if (User.Identity?.AuthenticationType == CookieAuthenticationDefaults.AuthenticationScheme)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        return Ok(new { message = "Successfully logged out." });
    }
}
