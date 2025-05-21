namespace Candlelight.Core.Entities;

public class SoftDeletableEntity : BaseEntity
{
    public required bool IsSoftDeleted { get; set; } = false;
}

