using Candlelight.Core.Entities.Steam;
using System.ComponentModel.DataAnnotations.Schema;

namespace Candlelight.Core.Entities;

public class Game : BaseEntity
{
    public int? SteamAppId { get; set; } = default!;
    public Guid? SteamGameDetailsId { get; set; }
    public SteamGameDetails? SteamGameDetails { get; set; } = default!;
    public Guid? CustomGameDetailsId { get; set; }
    public CustomGameDetails? CustomGameDetails { get; set; }
    public List<Mod> Mods { get; set; } = default!;
    public List<GameFavourite> Favourites { get; set; } = default!;

    [NotMapped]
    public bool IsCustom => CustomGameDetailsId.HasValue;
}

