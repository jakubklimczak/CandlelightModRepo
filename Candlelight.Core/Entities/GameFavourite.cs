namespace Candlelight.Core.Entities;
public class GameFavourite : BaseEntity
{
    public Guid GameId { get; set; }
    public Guid UserId { get; set; }
    public Game Game { get; set; } = default!;
    public AppUser User { get; set; } = default!;
}