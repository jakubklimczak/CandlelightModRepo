using System.ComponentModel.DataAnnotations;

namespace Candlelight.Core.Entities;
public class ModReview : SoftDeletableEntity
{
    public Guid ModId { get; set; }
    public Guid UserId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    public string? Comment { get; set; }

    public Mod Mod { get; set; } = default!;
    public AppUser User { get; set; } = default!;
}
