using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Candlelight.Core.Dtos.Mod;

public class ModUploadForm
{
    [FromForm] public Guid GameId { get; set; }
    [FromForm] public string Name { get; set; } = default!;
    [FromForm] public string Description { get; set; } = default!;
    [FromForm] public string DescriptionSnippet { get; set; } = default!;
    [FromForm] public string AuthorUsername { get; set; } = default!;
    [FromForm] public string ThumbnailUrl { get; set; } = default!;
    [FromForm] public string Version { get; set; } = default!;
    [FromForm] public string Changelog { get; set; } = default!;
    [FromForm] public IFormFile File { get; set; } = default!;
}