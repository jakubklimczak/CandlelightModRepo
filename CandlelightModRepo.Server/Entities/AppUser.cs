namespace Candlelight.Server.Entities;

public class AppUser
{
    public Guid UserId { get; set; }
    public required string UserName { get; set; }
    public required string UserEmail { get; set; }
}