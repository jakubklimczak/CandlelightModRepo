using System.ComponentModel.DataAnnotations;
namespace Candlelight.Core.Helpers;

public static class EmailValidationHelper
{
    public static bool IsEmailValid(string email)
    {
        if (email.Length is 0 or > 255)
        {
            return false;
        }
        var emailAttribute = new EmailAddressAttribute();
        return emailAttribute.IsValid(email);
    }
}