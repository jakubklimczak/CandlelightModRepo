using Candlelight.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Candlelight.Core.Helpers;

public static class CryptographyHelper
{
    private static readonly PasswordHasher<AppUser> _passwordHasher = new();

    public static string HashPassword(AppUser appUser, string password)
    {
        return _passwordHasher.HashPassword(appUser, password);
    }

    public static bool VerifyPassword(AppUser appUser, string hashedPassword, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(appUser, hashedPassword, password);
        return result == PasswordVerificationResult.Success;
    }
}