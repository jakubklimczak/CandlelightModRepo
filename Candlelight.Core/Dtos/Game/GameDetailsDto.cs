using Candlelight.Core.Entities.Steam;

namespace Candlelight.Core.Dtos.Game;
public class GameDetailsDto
{
    public Guid Id { get; set; }
    public int? AppId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? DescriptionSnippet { get; set; }
    public string? Description { get; set; }
    public string? HeaderImage { get; set; }
    public string? Developer { get; set; }
    public string? Publisher { get; set; }
    public bool? IsFree { get; set; }
    public decimal? Price { get; set; }
    public string? Currency { get; set; }
    public int? MetacriticScore { get; set; }
    public string? Website { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public int ModCount { get; set; }
    public int FavouriteCount { get; set; }
    public bool IsCustom { get; set; }

    public List<Genre> Genres { get; set; } = [];
    public List<Category> Categories { get; set; } = [];
    public List<Platform> Platforms { get; set; } = [];
}