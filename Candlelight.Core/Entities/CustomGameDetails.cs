using System.ComponentModel.DataAnnotations;

namespace Candlelight.Core.Entities;
public class CustomGameDetails : BaseEntity
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? CoverImage { get; set; } // xyz.jpg; located in wwwroot/custom-covers/

    public string? Developer { get; set; }

    public string? Publisher { get; set; }

    public Guid GameId { get; set; }

    public Game Game { get; set; } = null!;
}