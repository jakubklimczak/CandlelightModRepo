using Candlelight.Backend.Entities.Forms;
using Candlelight.Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Candlelight.Server.Controllers;

/*
 * Handles registration process.
 */
[ApiController]
[Route("api/[register]")]
public class UserRegistrationController(AuthenticationService authenticationService) : ControllerBase
{
    private readonly AuthenticationService _authenticationService = authenticationService;

    [HttpPost(Name = "SendRegistrationForm")]
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
}
