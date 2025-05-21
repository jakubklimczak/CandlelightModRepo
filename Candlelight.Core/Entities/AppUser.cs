using Microsoft.AspNetCore.Identity;

namespace Candlelight.Core.Entities;

public class AppUser : IdentityUser<Guid>
{
    public string? SteamId { get; set; }

    public required DateTime Created { get; set; }

    public required DateTime LastUpdated { get; set; }

    public UserProfile UserProfile { get; set; } = default!;
}