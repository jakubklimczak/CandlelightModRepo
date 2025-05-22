using Candlelight.Core.Entities;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Candlelight.Application.Services;

public class UserContextResolver(UserManagementService userService)
{
    private readonly UserManagementService _userService = userService;

    public async Task<AppUser?> ResolveUserAsync(ClaimsPrincipal principal)
    {
        // Try JWT-based login
        var jwtUserId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (Guid.TryParse(jwtUserId, out var userId))
        {
            return await _userService.GetUserByIdAsync(userId);
        }

        // Try Steam-based login
        var steamId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(steamId))
        {
            return await _userService.GetUserBySteamIdAsync(steamId);
        }

        return null;
    }
}
