using System.ComponentModel.DataAnnotations;
namespace Candlelight.Backend.Helpers;

public static class EmailValidationHelper
{
    public static bool IsEmailValid(string email)
    {
        var emailAttribute = new EmailAddressAttribute();
        return emailAttribute.IsValid(email);
    }
}