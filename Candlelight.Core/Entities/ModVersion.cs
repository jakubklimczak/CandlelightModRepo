namespace Candlelight.Core.Entities;

public class ModVersion : BaseEntity
{
    public Guid ModId { get; set; }
    public string Version { get; set; } = default!;
    public string Changelog { get; set; } = default!;
    public string FileUrl { get; set; } = default!;
    public List<string>? SupportedVersions { get; set; } = default!;
    public long FileSizeBytes { get; set; } = default!;
    public List<Guid>? Dependencies { get; set; } = default!;
    public int DownloadCount { get; set; } = default!;
    public string FileType { get; set; } = default!;

    public Mod Mod { get; set; } = default!;
}

