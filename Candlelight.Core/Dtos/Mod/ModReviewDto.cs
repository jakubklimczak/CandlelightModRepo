namespace Candlelight.Core.Dtos.Mod;

public class ModReviewDto
{
    public Guid Id { get; set; }
    public Guid ModId { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? ReviewText { get; set; } = string.Empty;
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}

