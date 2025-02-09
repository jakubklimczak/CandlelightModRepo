namespace Candlelight.Core.Entities.Forms;

public class RegistrationForm
{
    public required string UserName { get; set; }
    public required string UserEmail { get; set; }
    public required string PasswordString { get; set; }
    public required string ConfirmPasswordString { get; set; }
}