namespace Candlelight.Core.Entities;
public class ModFavourite : BaseEntity
{
    public Guid ModId { get; set; }
    public Guid UserId { get; set; }
    public Mod Mod { get; set; } = default!;
    public AppUser User { get; set; } = default!;
}
