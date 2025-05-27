using Candlelight.Core.Dtos.User;
using Candlelight.Core.Entities;
using Candlelight.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Candlelight.Application.Services;

public class UserSocialService(UserManagementService userManagementService, DataContext context)
{
    private readonly UserManagementService _userManagementService = userManagementService;
    private readonly DataContext _context = context;
    public async Task<UserProfileDto?> GetUserProfileByIdAsync(Guid userId)
    {
        var user = await _context.Users.Where(u => u.Id == userId).Include(u => u.UserProfile).FirstOrDefaultAsync();
        if (user?.UserProfile == null) return null;

        var profile = user.UserProfile;
        return new UserProfileDto
        {
            UserId = user.Id,
            DisplayName = profile.DisplayName ?? "",
            Bio = profile.Bio,
            AvatarFilename = profile.AvatarFilename,
            CreatedAt = profile.CreatedAt,
            LastUpdatedAt = profile.LastUpdatedAt,
            ProfileId = profile.Id,
            BackgroundColour = profile.BackgroundColour,
        };
    }

    public async Task<List<GameFavourite>> GetUserFavouriteGamesAsync(Guid userId)
    {
        return await _context.GameFavourites.Where(f => f.UserId == userId).ToListAsync();
    }

    public async Task<List<ModFavourite>> GetUserFavouriteModsAsync(Guid userId)
    {
        return await _context.ModFavourites.Where(f => f.UserId == userId).ToListAsync();
    }
}