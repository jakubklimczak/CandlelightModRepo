using Candlelight.Core.Entities;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Candlelight.Application.Services;

public class UserContextResolver(UserManagementService userService)
{
    private readonly UserManagementService _userService = userService;

    public async Task<AppUser?> ResolveUserAsync(ClaimsPrincipal principal)
    {
        var userId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (Guid.TryParse(userId, out var id))
        {
            return await _userService.GetUserByIdAsync(id);
        }

        var steamId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(steamId))
        {
            Console.WriteLine(Guid.Parse(steamId));
            return await _userService.GetUserByIdAsync(Guid.Parse(steamId));
        }

        return null;
    }
}
