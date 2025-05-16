using System.ComponentModel.DataAnnotations;

namespace Candlelight.Core.Entities;

public class BaseEntity
{
    [Key]
    public required Guid Id { get; set; }

    public required Guid CreatedBy { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required DateTime LastUpdatedAt { get; set; }
}

