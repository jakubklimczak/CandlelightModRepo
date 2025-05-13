namespace Candlelight.Core.Dtos.Game;

public class GameListItemDto
{
    public int AppId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? HeaderImage { get; set; }
    public string? Developer { get; set; }
    public string? Publisher { get; set; }
}
