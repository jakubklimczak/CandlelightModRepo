using Candlelight.Core.Entities.Forms;
using Candlelight.Application.Services;
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
            return Ok(new { message = "User logged in successfully" });
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
}
