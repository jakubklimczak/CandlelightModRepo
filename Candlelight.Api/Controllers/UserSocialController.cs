using Candlelight.Api.Attributes;
using Candlelight.Application.Services;
using Candlelight.Core.Dtos.Game;
using Candlelight.Core.Dtos.Mod;
using Candlelight.Core.Dtos.User;
using Candlelight.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Candlelight.Api.Controllers;

/// <summary>
/// Controller used for all social features.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserSocialController(UserSocialService userSocialService) : ControllerBase
{
    private readonly UserSocialService _userSocialService = userSocialService;

    /// <summary>
    /// Returns the public profile of the user with provided {userId}.
    /// </summary>
    [HttpGet("UserProfile/User/{userId}")]
    [ProducesResponseType(typeof(UserProfileDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetUserProfile(Guid userId)
    {
        var profile = await _userSocialService.GetUserPublicProfileByIdAsync(userId);
        if (profile == null) return NotFound();

        return Ok(profile);
    }

    /// <summary>
    /// Returns the profile of the current user with all public and private properties.
    /// </summary>
    [HttpGet("UserProfile/CurrentUser")]
    [ProducesResponseType(typeof(UserProfileDto), 200)]
    [Authorize(Policy = "JwtOnly")]
    public async Task<IActionResult> GetUserProfile([CurrentUser] AppUser user)
    {
        var profile = await _userSocialService.GetUserPrivateProfileByIdAsync(user.Id);
        return Ok(profile);
    }

    /// <summary>
    /// Returns the list of user's favourite games. If private, it is only returned if the current user is also the owner.
    /// </summary>
    [HttpGet("FavouriteGames/{userId}")]
    [ProducesResponseType(typeof(List<GameListItemDto>), 200)]
    [Authorize(Policy = "JwtOnly")]
    public async Task<IActionResult> GetFavouriteGames(Guid userId, [CurrentUser] AppUser user)
    {
        if (userId != user.Id && !(await _userSocialService.AreUsersFavouritesVisible(userId)))
        {
            return Unauthorized(userId);
        }
        var result = await _userSocialService.GetUserFavouriteGamesAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Returns the list of user's favourite mods. If private, it is only returned if the current user is also the owner.
    /// </summary>
    [HttpGet("FavouriteMods/{userId}")]
    [ProducesResponseType(typeof(List<ModListItemDto>), 200)]
    [Authorize(Policy = "JwtOnly")]
    public async Task<IActionResult> GetFavouriteMods(Guid userId, [CurrentUser] AppUser user)
    {
        if (userId != user.Id && !(await _userSocialService.AreUsersFavouritesVisible(userId)))
        {
            return Unauthorized(userId);
        }
        var result = await _userSocialService.GetUserFavouriteModsAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Updates the profile of the current user.
    /// </summary>
    [HttpPut("UserProfile/Update")]
    [Authorize(Policy = "JwtOnly")]
    public async Task<IActionResult> UpdateUserProfile([CurrentUser] AppUser user, [FromForm] UpdateProfileForm form)
    {
        await _userSocialService.UpdateUserProfileAsync(user.Id, form);
        return Ok();
    }

}