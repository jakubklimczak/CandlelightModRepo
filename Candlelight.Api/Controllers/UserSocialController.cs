using Candlelight.Application.Services;
using Candlelight.Core.Dtos.User;
using Microsoft.AspNetCore.Mvc;

namespace Candlelight.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserSocialController(UserSocialService userSocialService) : ControllerBase
{
    private readonly UserSocialService _userSocialService = userSocialService;

    [HttpGet("UserProfile/{userId}")]
    [ProducesResponseType(typeof(UserProfileDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUserProfile(Guid userId)
    {
        var profile = await _userSocialService.GetUserProfileByIdAsync(userId);
        if (profile == null) return NotFound();

        return Ok(profile);
    }
}