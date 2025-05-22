using Candlelight.Core.Entities.Steam;

namespace Candlelight.Core.Entities;

public class Game : BaseEntity
{
    public int? SteamAppId { get; set; } = default!;
    public SteamGameDetails? SteamGameDetails { get; set; } = default!;
    public List<Mod> Mods { get; set; } = default!;
    public List<GameFavourite> Favourites { get; set; } = default!;
}

