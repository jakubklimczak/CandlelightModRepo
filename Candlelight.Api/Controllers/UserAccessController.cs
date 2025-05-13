using Candlelight.Application.Services;
using Candlelight.Core.Entities.Forms;
using Microsoft.AspNetCore.Mvc;

namespace Candlelight.Api.Controllers;

/*
 * Handles registration process.
 */
[ApiController]
[Route("api/[controller]")]
public class UserAccessController(AuthenticationService authenticationService, UserManagementService userManagementService) : ControllerBase
{
    private readonly AuthenticationService _authenticationService = authenticationService;
    private readonly UserManagementService _userManagementService = userManagementService;

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

            await _authenticationService.RegisterUser(form);
            return Ok(new { message = "User registered successfully" });
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

        return Ok(new { Message = "Steam account linked successfully" });
    }


    /*
    [HttpGet("SteamLogin")]
    [Route("SteamLogin")]
    public IActionResult SteamLogin()
    {
        var redirectUrl = Url.Action(nameof(SteamCallback), "", null, Request.Scheme);
        return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, "Steam");
    }

    [HttpGet("SteamCallback")]
    [Route("SteamCallback")]
    public async Task<IActionResult> SteamCallback()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded) return Unauthorized();

        var steamId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // TODO: Check if user exists in DB, create account if necessary
        // Redirect to frontend with session token

        return Ok(new { SteamId = steamId, Message = "Login successful!" });
    }

    [HttpPost("Logout")]
    [Route("Logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { Message = "Logged out successfully" });
    }
    */
}
