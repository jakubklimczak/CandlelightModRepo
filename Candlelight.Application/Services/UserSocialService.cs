using Candlelight.Core.Dtos.Game;
using Candlelight.Core.Dtos.Mod;
using Candlelight.Core.Dtos.User;
using Candlelight.Core.Entities;
using Candlelight.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Candlelight.Application.Services;

public class UserSocialService(UserManagementService userManagementService, DataContext context)
{
    private readonly UserManagementService _userManagementService = userManagementService;
    private readonly DataContext _context = context;

    public async Task<UserProfileDto?> GetUserPublicProfileByIdAsync(Guid userId)
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
            FavouritesVisible = profile.FavouritesVisible,
        };
    }

    public async Task<UserProfileDto?> GetUserPrivateProfileByIdAsync(Guid userId)
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
            FavouritesVisible = profile.FavouritesVisible,
        };
    }

    public async Task<List<GameListItemDto>> GetUserFavouriteGamesAsync(Guid userId)
    {
        return await _context.GameFavourites
            .Where(f => f.UserId == userId)
            .Select(f => new GameListItemDto
            {
                Id = f.GameId,
                Name = f.Game.SteamGameDetails != null 
                    ? f.Game.SteamGameDetails.Name
                    : f.Game.CustomGameDetails!.Name,
                HeaderImage = f.Game.SteamGameDetails != null
                    ? f.Game.SteamGameDetails.HeaderImage
                    : f.Game.CustomGameDetails!.CoverImage,

            })
            .ToListAsync();
    }

    public async Task<List<ModListItemDto>> GetUserFavouriteModsAsync(Guid userId)
    {
        return await _context.ModFavourites
            .Where(f => f.UserId == userId)
            .Select(f => new ModListItemDto
            {
                Id = f.ModId,
                Name = f.Mod.Name,
                DescriptionSnippet = f.Mod.DescriptionSnippet,
                ThumbnailUrl = f.Mod.ThumbnailUrl,
                Author = f.Mod.CreatedByUser.UserName!,
                AuthorId = f.CreatedBy,
                LastUpdatedDate = f.Mod.LastUpdatedAt,
                TotalDownloads = f.Mod.Versions.Sum(v => v.DownloadCount),
                TotalFavourited = f.Mod.Favourites.Count,
                TotalReviews = f.Mod.Reviews.Count,
                AverageRating = f.Mod.Reviews.Average(r => r.Rating)
            })
            .ToListAsync();
    }

    public async Task<bool> AreUsersFavouritesVisible(Guid userId)
    {
        return await _context.UserProfiles.Where(up => up.UserId == userId).AnyAsync(up => up.FavouritesVisible);
    }

    public async Task UpdateUserProfileAsync(Guid userId, UpdateProfileForm form)
    {
        var user = await _context.Users
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.UserProfile == null) throw new Exception("User not found");

        var profile = user.UserProfile;

        if (form.DisplayName != null) profile.DisplayName = form.DisplayName;
        if (form.Bio != null) profile.Bio = form.Bio;
        if (form.BackgroundColour != null) profile.BackgroundColour = form.BackgroundColour;

        if (form.Avatar != null)
        {
            var avatarsFolder = Path.Combine("wwwroot", "avatars");
            var newExtension = Path.GetExtension(form.Avatar.FileName);
            var newFilename = $"{userId}{newExtension}";
            var newPath = Path.Combine(avatarsFolder, newFilename);

            var existingFiles = Directory.GetFiles(avatarsFolder, $"{userId}.*");
            foreach (var file in existingFiles)
            {
                System.IO.File.Delete(file);
            }

            await using var stream = new FileStream(newPath, FileMode.Create);
            await form.Avatar.CopyToAsync(stream);

            profile.AvatarFilename = newFilename;
        }

        profile.LastUpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}