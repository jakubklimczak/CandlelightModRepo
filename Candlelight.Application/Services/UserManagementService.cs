using Candlelight.Infrastructure.Persistence.Data;
using Candlelight.Core.Entities;
using Candlelight.Core.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Candlelight.Application.Services;

public class UserManagementService(DataContext context)
{
    private readonly DataContext _context = context;

    public async Task<AppUser> CreateUserAsync(string userName, string userEmail, string passwordString)
    {
        AppUser newAppUser = new()
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            Email = userEmail,
            PasswordHash = "",
            Created = DateTime.Now,
            LastUpdated = DateTime.Now
        };

        var hashedPassword = CryptographyHelper.HashPassword(newAppUser, passwordString);
        newAppUser.PasswordHash = hashedPassword;

        UserProfile newProfile = new()
        {
            Id = Guid.NewGuid(),
            UserId = newAppUser.Id,
            DisplayName = newAppUser.UserName,
            AvatarFilename = null,
            BackgroundColour = null,
            Bio = null,
            CreatedAt = DateTime.Now,
            LastUpdatedAt = DateTime.Now,
            CreatedBy = newAppUser.Id,
        };

        await _context.Users.AddAsync(newAppUser);
        await _context.UserProfiles.AddAsync(newProfile);
        await _context.SaveChangesAsync();

        return newAppUser;
    }

    public async Task<AppUser?> UpdateUserAsync(AppUser updatedUser)
    {
        var user = await GetUserByIdAsync(updatedUser.Id);
        if (user == null)
            return null;

        if (!string.IsNullOrEmpty(updatedUser.UserName))
            user.UserName = updatedUser.UserName;

        if (!string.IsNullOrEmpty(updatedUser.Email))
            user.Email = updatedUser.Email;

        if (!string.IsNullOrEmpty(updatedUser.PasswordHash))
            user.PasswordHash = updatedUser.PasswordHash;

        user.SteamId = updatedUser.SteamId;

        user.LastUpdated = DateTime.Now;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return user;
    }


    public async Task<AppUser?> GetUserByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }       
    
    public async Task<AppUser?> GetUserBySteamIdAsync(string steamId)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.SteamId == steamId);
    }    
    
    public async Task<AppUser?> GetUserByNameAsync(string userName)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.UserName == userName);
    }    
    
    public async Task<AppUser?> GetUserByEmailAsync(string userEmail)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Email == userEmail);
    }

    public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }
}