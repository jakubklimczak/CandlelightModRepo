using Candlelight.Core.Entities;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Candlelight.Application.Services;

public class UserContextResolver(UserManagementService userService)
{
    private readonly UserManagementService _userService = userService;

    public async Task<AppUser?> ResolveUserAsync(ClaimsPrincipal principal)
    {
        // Default: JWT token based approach
        var userId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (Guid.TryParse(userId, out var id))
        {
            return await _userService.GetUserByIdAsync(id);
        }

        // Fallback only for Steam cookie-authenticated users
        var steamUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(steamUserId, out var steamGuid))
        {
            return await _userService.GetUserByIdAsync(steamGuid);
        }

        return null;
    }
}
