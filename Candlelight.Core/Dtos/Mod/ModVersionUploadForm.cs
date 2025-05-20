using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Candlelight.Core.Dtos.Mod;

public class ModVersionUploadForm
{
    [FromForm] public Guid ModId { get; set; } = default!;
    [FromForm] public string Version { get; set; } = default!;
    [FromForm] public string Changelog { get; set; } = default!;
    [FromForm] public IFormFile File { get; set; } = default!;
}

