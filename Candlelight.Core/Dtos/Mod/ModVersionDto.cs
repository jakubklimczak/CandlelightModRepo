namespace Candlelight.Core.Dtos.Mod;

public class ModVersionDto
{
    public required Guid Id { get; set; }
    public required Guid ModId { get; set; }
    public required string ModName { get; set; }
    public required string Version { get; set; } = default!;
    public required string Changelog { get; set; } = default!;
    public required string FileUrl { get; set; } = default!;
    public required Guid CreatedBy { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime LastUpdatedAt { get; set; }
    public List<string>? SupportedVersions { get; set; }
    public List<Guid>? Dependencies { get; set; }
}
