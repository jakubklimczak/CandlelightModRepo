namespace Candlelight.Core.Entities.Forms;

public class LoginForm
{
    public required string UserEmail { get; set; }
    public required string PasswordString { get; set; }
}