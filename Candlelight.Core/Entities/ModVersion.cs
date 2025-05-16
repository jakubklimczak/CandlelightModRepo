namespace Candlelight.Core.Entities;

public class ModVersion : BaseEntity
{
    public Guid ModId { get; set; }
    public string Version { get; set; } = default!;
    public string Changelog { get; set; } = default!;
    public string FileUrl { get; set; } = default!;

    public Mod Mod { get; set; } = default!;
}

