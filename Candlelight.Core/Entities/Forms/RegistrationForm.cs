namespace Candlelight.Core.Entities.Forms;

public class RegistrationForm
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
}