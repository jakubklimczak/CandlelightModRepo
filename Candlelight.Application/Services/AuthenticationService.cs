using Candlelight.Core.Entities;
using Candlelight.Core.Entities.Forms;
using Candlelight.Core.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Candlelight.Application.Services;

public class AuthenticationService(UserManagementService userService, IConfiguration configuration)
{
    private readonly UserManagementService _userService = userService;
    private readonly string _jwtKey = configuration["Jwt:Key"] ?? throw new ArgumentException("JWT Key is missing");
    private readonly string _issuer = configuration["Jwt:Issuer"] ?? "DefaultIssuer";
    private readonly string _audience = configuration["Jwt:Audience"] ?? "DefaultAudience";

    public static bool IsRegistrationFormValid(RegistrationForm form)
    {
        if (form.Equals(null) || form.Email.Equals(null) || form.UserName.Equals(null) || form.Password.Equals(null) || form.ConfirmPassword.Equals(null))
        {
            return false;
        }

        if (!form.Password.Equals(form.ConfirmPassword))
        {
            return false;
        }

        if (form.UserName.Length is < 6 or > 30)
        {
            return false;
        }

        if (form.Password.Length < 8)
        {
            return false;
        }

        if (!EmailValidationHelper.IsEmailValid(form.Email))
        {
            return false;
        }

        return true;
    }

    public static bool IsLoginFormValid(LoginForm form)
    {
        if (form.Equals(null) || form.UserEmail.Equals(null) || form.PasswordString.Equals(null))
        {
            return false;
        }

        if (!EmailValidationHelper.IsEmailValid(form.UserEmail))
        {
            return false;
        }

        return true;
    }

    public async Task<AppUser> RegisterUser(RegistrationForm form)
    {
        var result = await _userService.CreateUserAsync(form.UserName, form.Email, form.Password);
        return result;
    }

    public async Task<AppUser?> AttemptLogin(LoginForm form)
    {
        var user = await _userService.GetUserByEmailAsync(form.UserEmail);
        return ValidateLoginInfo(user, form) ? user : null;
    }

    public static bool ValidateLoginInfo(AppUser? user, LoginForm form)
    {
        return user != null && CryptographyHelper.VerifyPassword(user, user.PasswordHash!, form.PasswordString);
    }

    public string GenerateJwtToken(AppUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}