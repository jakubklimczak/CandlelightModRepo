using Candlelight.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Candlelight.Core.Helpers;

public static class CryptographyHelper
{
    private static readonly PasswordHasher<UserInfo> _passwordHasher = new();

    public static string HashPassword(UserInfo user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public static bool VerifyPassword(UserInfo user, string hashedPassword, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, password);
        return result == PasswordVerificationResult.Success;
    }
}