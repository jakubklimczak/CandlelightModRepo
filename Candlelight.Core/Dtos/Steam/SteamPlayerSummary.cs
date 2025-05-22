namespace Candlelight.Core.Dtos.Steam;

public class SteamPlayerSummary
{
    public string SteamId { get; set; } = string.Empty;
    public string PersonaName { get; set; } = string.Empty;
    public string ProfileUrl { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public string AvatarMedium { get; set; } = string.Empty;
    public string AvatarFull { get; set; } = string.Empty;
    public string? RealName { get; set; }
    public int PersonaState { get; set; }
    public long? TimeCreated { get; set; }
    public string? LocCountryCode { get; set; }
}