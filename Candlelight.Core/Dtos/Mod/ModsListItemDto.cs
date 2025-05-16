namespace Candlelight.Core.Dtos.Mod;

public class ModListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string DescriptionSnippet { get; set; } = default!;
    public string ThumbnailUrl { get; set; } = default!;
    public Guid AuthorId { get; set; } = default!;
    public string Author { get; set; } = default!;
    public DateTime LastUpdatedDate { get; set; }
}