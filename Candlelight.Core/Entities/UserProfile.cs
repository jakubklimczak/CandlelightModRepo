using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Candlelight.Core.Entities;

public class UserProfile : BaseEntity
{

    [MaxLength(30)]
    public string? DisplayName { get; set; }

    [MaxLength(255)]
    public string? AvatarFilename { get; set; }

    [MaxLength(200)]
    public string? Bio { get; set; }

    [MaxLength(30)]
    public string? BackgroundColour { get; set; }

    public required Guid UserId { get; set; }

    [ForeignKey(nameof(AppUser))]
    public AppUser User { get; set; } = null!;
}