namespace Candlelight.Core.Entities;

public class Mod : BaseEntity
{
    public Guid GameId { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string DescriptionSnippet { get; set; } = default!;
    public string ThumbnailUrl { get; set; } = default!;
    public string AuthorUsername { get; set; } = default!;

    public Game Game { get; set; } = default!;
    public List<ModVersion> Versions { get; set; } = default!;
    public List<ModReview> Reviews { get; set; } = [];
    public List<ModFavourite> Favourites { get; set; } = [];

}
