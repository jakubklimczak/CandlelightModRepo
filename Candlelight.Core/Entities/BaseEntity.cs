namespace Candlelight.Core.Entities;

public class BaseEntity
{
    public required Guid Id { get; set; }

    public required DateTime Created { get; set; }

    public required DateTime LastUpdated { get; set; }
}

