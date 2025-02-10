using System.ComponentModel.DataAnnotations;

namespace Candlelight.Core.Entities.Steam;

public class GameDetails
{
    [Key]
    public int AppId { get; set; }  // Steam's ID

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public string? ShortDescription { get; set; }

    public string? DetailedDescription { get; set; }

    public string? HeaderImage { get; set; } // URL to game banner

    public string? Website { get; set; }

    public bool IsFree { get; set; }

    public decimal? Price { get; set; }  // free games = null

    public string? Currency { get; set; }

    public string? Developer { get; set; }

    public string? Publisher { get; set; }

    public int MetacriticScore { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public List<Genre> Genres { get; set; } = [];

    public List<Category> Categories { get; set; } = [];

    public List<Platform> Platforms { get; set; } = [];
}
