using Candlelight.Backend.Entities.Forms;
using Candlelight.Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Candlelight.Backend.Controllers;

/*
 * Handles registration process.
 */
[ApiController]
public class UserAccessController(AuthenticationService authenticationService) : ControllerBase
{
    private readonly AuthenticationService _authenticationService = authenticationService;

    [HttpPost]
    [ActionName("SendRegisterForm")]
    [Route("api/[controller]/SendRegistrationForm")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> PostAsync([FromBody] RegistrationForm form)
    {
        try
        {
            if (!_authenticationService.IsRegistrationFormValid(form))
            {
                return BadRequest("Data model is invalid.");
            }

            await _authenticationService.RegisterUser(form);
            return Ok(form);
        }
        catch (Exception ex) 
        { 
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message }); 
        }
    }

    [HttpPost]
    [Route("api/[controller]/SendLoginForm")]
    [ActionName("SendLoginForm")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> PostAsync([FromBody] LoginForm form)
    {
        try
        {
            if (!_authenticationService.IsLoginFormValid(form))
            {
                return BadRequest("Data model is invalid.");
            }
            var result = await _authenticationService.AttemptLogin(form);
            if (result == null)
            {
                return Unauthorized(form);
            }
            return Ok(form);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    }
}
