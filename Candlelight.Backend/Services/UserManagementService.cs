using Candlelight.Backend.Data;
using Candlelight.Backend.Entities;
using Candlelight.Backend.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Candlelight.Backend.Services;

public class UserManagementService(DataContext context)
{
    private readonly DataContext _context = context;

    public async Task<UserInfo> CreateUserAsync(string userName, string userEmail, string passwordString)
    {
        UserInfo newUser = new()
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            UserEmail = userEmail,
            PasswordHash = "",
            Created = DateTime.Now,
            LastUpdated = DateTime.Now
        };

        var encodedPassword = EncodingHelper.Base64Encode(passwordString);
        var hashedPassword = CryptographyHelper.HashPassword(newUser, encodedPassword);
        newUser.PasswordHash = hashedPassword;

        UserProfile newProfile = new()
        {
            Id = Guid.NewGuid(),
            UserId = newUser.Id,
            DisplayName = newUser.UserName,
            AvatarFilename = null,
            BackgroundColour = null,
            Bio = null,
            Created = DateTime.Now,
            LastUpdated = DateTime.Now
        };

        await _context.Users.AddAsync(newUser);
        await _context.UserProfiles.AddAsync(newProfile);
        await _context.SaveChangesAsync();

        return newUser;
    }

    public async Task<UserInfo?> GetUserByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }    
    
    public async Task<UserInfo?> GetUserByNameAsync(string userName)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.UserName == userName);
    }    
    
    public async Task<UserInfo?> GetUserByEmailAsync(string userEmail)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.UserEmail == userEmail);
    }
}