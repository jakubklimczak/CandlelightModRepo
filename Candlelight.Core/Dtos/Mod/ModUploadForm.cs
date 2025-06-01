using Microsoft.AspNetCore.Http;

namespace Candlelight.Core.Dtos.Mod;

public class ModUploadForm
{
    public Guid GameId { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string DescriptionSnippet { get; set; } = default!;
    public string Version { get; set; } = default!;
    public string Changelog { get; set; } = default!;

    public List<string>? SupportedVersions { get; set; }
    public List<Guid>? Dependencies { get; set; }

    public IFormFile File { get; set; } = default!;
    public List<IFormFile>? Images { get; set; }
    public string? SelectedThumbnail { get; set; }
}