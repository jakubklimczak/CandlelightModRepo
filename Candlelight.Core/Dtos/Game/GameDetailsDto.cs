namespace Candlelight.Core.Dtos.Game;
public class GameDetailsDto
{
    public int AppId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? HeaderImage { get; set; }
    public string? Developer { get; set; }
    public string? Publisher { get; set; }
    public bool IsFree { get; set; }
    public decimal? Price { get; set; }
    public string? Currency { get; set; }
    public int MetacriticScore { get; set; }
    public DateTime? ReleaseDate { get; set; }

    public List<string> Genres { get; set; } = [];
    public List<string> Categories { get; set; } = [];
    public List<string> Platforms { get; set; } = [];
}