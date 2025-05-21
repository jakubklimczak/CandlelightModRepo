namespace Candlelight.Core.Dtos.Mod;

public class ModDetailsResponseDto
{
    public required Guid Id { get; set; }
    public required Guid GameId { get; set; }
    public required string AuthorUsername { get; set; }
    public required string Name { get; set; }
    public required string ThumbnailUrl { get; set; }
    public required string Description { get; set; }
    public required string GameName { get; set; }
    public required Guid CreatedBy { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime LastUpdatedAt { get; set; }
    public required double AverageRating { get; set; }
    public required int ReviewCount { get; set; }
    public required int FavouriteCount { get; set; }
    public required List<ModVersionDto> Versions { get; set; } 
}

