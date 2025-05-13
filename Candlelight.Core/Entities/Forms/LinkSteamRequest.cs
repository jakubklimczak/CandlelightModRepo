namespace Candlelight.Core.Entities.Forms;

public class LinkSteamRequest
{
    public required Guid UserId { get; set; }
    public required string SteamId { get; set; }
}