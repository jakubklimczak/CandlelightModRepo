using Candlelight.Core.Entities;
using Candlelight.Core.Entities.Forms;
using Candlelight.Core.Helpers;

namespace Candlelight.Application.Services;

public class AuthenticationService(UserManagementService userService)
{
    private readonly UserManagementService _userService = userService;

    public static bool IsRegistrationFormValid(RegistrationForm form)
    {
        if (form.Equals(null) || form.UserEmail.Equals(null) || form.UserName.Equals(null) || form.PasswordString.Equals(null) || form.ConfirmPasswordString.Equals(null))
        {
            return false;
        }

        if (!form.PasswordString.Equals(form.ConfirmPasswordString))
        {
            return false;
        }

        if (form.UserName.Length is < 6 or > 30)
        {
            return false;
        }

        if (form.PasswordString.Length < 8)
        {
            return false;
        }

        if (!EmailValidationHelper.IsEmailValid(form.UserEmail))
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

    public async Task<UserInfo> RegisterUser(RegistrationForm form)
    {
        var result = await _userService.CreateUserAsync(form.UserName, form.UserEmail, form.PasswordString);
        return result;
    }

    public async Task<UserInfo?> AttemptLogin(LoginForm form)
    {
        var user = await _userService.GetUserByEmailAsync(form.UserEmail);
        return ValidateLoginInfo(user, form) ? user : null;
    }

    public static bool ValidateLoginInfo(UserInfo? user, LoginForm form)
    {
        return user != null && CryptographyHelper.VerifyPassword(user, user.PasswordHash, form.PasswordString);
    }
}