namespace Candlelight.Backend.Entities;

public class UserInfo
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string UserEmail { get; set; }
    public required string PasswordHash { get; set; }
}