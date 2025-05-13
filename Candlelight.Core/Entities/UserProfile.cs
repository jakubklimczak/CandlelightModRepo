using System.ComponentModel.DataAnnotations;

namespace Candlelight.Core.Entities;

public class UserProfile : BaseEntity
{
    public required Guid UserId { get; set; }

    [MaxLength(30)]
    public string? DisplayName { get; set; }

    [MaxLength(255)]
    public string? AvatarFilename { get; set; }

    [MaxLength(200)]
    public string? Bio { get; set; }

    [MaxLength(30)]
    public string? BackgroundColour { get; set; }
}