namespace Candlelight.Core.Dtos.User;

public class UserProfileDto
{
    public Guid ProfileId { get; set; }
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? AvatarFilename { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public string? BackgroundColour { get; set; }
    public bool FavouritesVisible { get; set; }
}