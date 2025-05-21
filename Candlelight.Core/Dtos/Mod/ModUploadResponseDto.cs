namespace Candlelight.Core.Dtos.Mod;

public class ModUploadResponseDto
{
    public Guid ModId { get; set; }
    public Guid ModVersionId { get; set; }
    public string DownloadUrl { get; set; } = default!;
}

