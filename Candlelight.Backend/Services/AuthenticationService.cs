using Candlelight.Backend.Entities;
using Candlelight.Backend.Entities.Forms;
using Candlelight.Backend.Helpers;

namespace Candlelight.Backend.Services;

public class AuthenticationService(UserManagementService userService)
{
    private readonly UserManagementService _userService = userService;

    public bool IsRegistrationFormValid(RegistrationForm form)
    {
        if (form.Equals(null) || form.UserEmail.Equals(null) || form.UserName.Equals(null) || form.PasswordString.Equals(null) || form.ConfirmPasswordString.Equals(null))
        {
            return false;
        }

        if (!form.PasswordString.Equals(form.ConfirmPasswordString))
        {
            return false;
        }

        if (form.UserName.Length < 6)
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
}