using Candlelight.Application.Services;
using Candlelight.Core.Dtos.Mod;
using Candlelight.Core.Dtos.Query;
using Candlelight.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Candlelight.Api.Controllers;

/*
 * Handles modifications in the application.
 */
[ApiController]
[Route("api/[controller]")]
public class ModController(
    ModService modService) : ControllerBase
{
    [HttpGet("GetModsByGameId")]
    public async Task<IActionResult> GetModsByGameId([FromQuery] Guid gameId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var (mods, totalCount) = await modService.GetModsByGameIdAsync(gameId, page, pageSize);

        var result = new PaginatedResponse<ModListItemDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount,
            Items = mods.Select(mod => new ModListItemDto
            {
                Id = mod.Id,
                Name = mod.Name,
                DescriptionSnippet = mod.Description,
                ThumbnailUrl = mod.ThumbnailUrl,
                Author = mod.AuthorUsername,
                AuthorId = mod.CreatedBy,
                LastUpdatedDate = mod.LastUpdatedAt
            }).ToList()
        };

        return Ok(result);
    }

    [HttpPost("UploadNewMod")]
    [RequestSizeLimit(500_000_000)] // 500 MB max TODO: adjust as needed. make endpoint for bigger mods
    public async Task<IActionResult> UploadNewMod(
        [FromForm] ModUploadForm dto)
    {
        if (dto.File.Length == 0)
            return BadRequest("No file uploaded.");

        var now = DateTime.UtcNow;
        var userId = /* TODO: get from auth */ Guid.Empty;

        var mod = new Mod
        {
            Id = Guid.NewGuid(),
            GameId = dto.GameId,
            Name = dto.Name,
            Description = dto.Description,
            DescriptionSnippet = dto.DescriptionSnippet,
            AuthorUsername = dto.AuthorUsername,
            ThumbnailUrl = dto.ThumbnailUrl,
            CreatedAt = now,
            CreatedBy = userId,
            LastUpdatedAt = now,
            Versions = []
        };

        var uploadsDir = Path.Combine("wwwroot", "mods", mod.Id.ToString());
        Directory.CreateDirectory(uploadsDir);

        var fileExt = Path.GetExtension(dto.File.FileName);
        var fileName = $"v{dto.Version}{fileExt}";
        var filePath = Path.Combine(uploadsDir, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        var publicUrl = $"/mods/{mod.Id}/{fileName}";

        var version = new ModVersion
        {
            Id = Guid.NewGuid(),
            ModId = mod.Id,
            Version = dto.Version,
            Changelog = dto.Changelog,
            FileUrl = publicUrl,
            CreatedAt = now,
            CreatedBy = userId,
            LastUpdatedAt = now
        };



        mod.Versions.Add(version);
        await modService.AddModAsync(mod);

        return Ok(new ModUploadResponseDto()
        {
            ModId = mod.Id,
            ModVersionId = version.Id, 
            DownloadUrl = publicUrl
        });
    }


    [HttpPost("UploadNewModVersion")]
    [RequestSizeLimit(500_000_000)] // 500 MB max TODO: adjust as needed. make endpoint for bigger mods
    public async Task<IActionResult> UploadNewModVersion(
        [FromForm] ModVersionUploadForm dto)
    {
        if (dto.File.Length == 0)
            return BadRequest("No file uploaded.");

        var now = DateTime.UtcNow;
        var userId = /* TODO: get from auth */ Guid.Empty;

        var mod = await modService.GetModByIdAsync(dto.ModId);

        if (mod == null)
        {
            return BadRequest($"Mod with id { dto.ModId } doesn\'t exist.");
        }

        var uploadsDir = Path.Combine("wwwroot", "mods", mod.Id.ToString());
        Directory.CreateDirectory(uploadsDir);

        var fileExt = Path.GetExtension(dto.File.FileName);
        var fileName = $"v{dto.Version}{fileExt}";
        var filePath = Path.Combine(uploadsDir, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        var publicUrl = $"/mods/{mod.Id}/{fileName}";

        var version = new ModVersion
        {
            Id = Guid.NewGuid(),
            ModId = mod.Id,
            Version = dto.Version,
            Changelog = dto.Changelog,
            FileUrl = publicUrl,
            CreatedAt = now,
            CreatedBy = userId,
            LastUpdatedAt = now
        };


        mod.Versions.Add(version);

        return Ok(new ModUploadResponseDto()
        {
            ModId = mod.Id,
            ModVersionId = version.Id,
            DownloadUrl = publicUrl
        });
    }


    [HttpGet("GetModsBySteamAppId")]
    public async Task<IActionResult> GetModsBySteamAppId([FromQuery] int appId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var (mods, totalCount) = await modService.GetModsBySteamAppIdAsync(appId, page, pageSize);

        var result = new PaginatedResponse<ModListItemDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount,
            Items = mods.Select(mod => new ModListItemDto
            {
                Id = mod.Id,
                Name = mod.Name,
                DescriptionSnippet = mod.Description,
                ThumbnailUrl = mod.ThumbnailUrl,
                Author = mod.AuthorUsername,
                AuthorId = mod.CreatedBy,
                LastUpdatedDate = mod.LastUpdatedAt
            }).ToList()
        };

        return Ok(result);
    }

    [HttpGet("GetModDetailsById")]
    public async Task<IActionResult> GetModDetailsById([FromQuery] Guid modId)
    {
        var mod = await modService.GetModDetailsById(modId);

        if (mod == null)
        {
            return BadRequest($"Mod with id {modId} doesn\'t exist.");
        }

        var result = new ModDetailsResponseDto()
        {
            Id = mod.Id,
            GameId = mod.GameId,
            AuthorUsername = mod.AuthorUsername,
            Description = mod.Description,
            Name = mod.Name,
            GameName = mod.Game.SteamGameDetails != null ? mod.Game.SteamGameDetails.Name : "",
            Versions = mod.Versions.Select(v => new ModVersionDto()
            {
                Id = v.Id,
                ModId = v.ModId, 
                FileUrl = v.FileUrl, 
                Changelog = v.Changelog, 
                Version = v.Version,
                CreatedAt = v.CreatedAt,
                CreatedBy = v.CreatedBy,
                LastUpdatedAt = v.LastUpdatedAt
            }).ToList(),
            ThumbnailUrl = mod.ThumbnailUrl,
            CreatedAt = mod.CreatedAt,
            CreatedBy = mod.CreatedBy,
            LastUpdatedAt = mod.LastUpdatedAt,
        };

        return Ok(result);
    }
}