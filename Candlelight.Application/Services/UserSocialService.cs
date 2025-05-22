using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Candlelight.Core.Dtos.User;

namespace Candlelight.Application.Services;

public class UserSocialService(UserManagementService userManagementService)
{
    private readonly UserManagementService _userManagementService = userManagementService;

    public async Task<UserProfileDto?> GetUserProfileByIdAsync(Guid userId)
    {
        var user = await _userManagementService.GetUserByIdAsync(userId);
        if (user == null) return null;

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
}