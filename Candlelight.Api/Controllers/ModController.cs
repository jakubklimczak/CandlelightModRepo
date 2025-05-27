using Candlelight.Api.Attributes;
using Candlelight.Application.Services;
using Candlelight.Core.Dtos.Mod;
using Candlelight.Core.Dtos.Query;
using Candlelight.Core.Entities;
using Candlelight.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    [Authorize]
    [HttpPost("UploadNewMod")]
    [RequestSizeLimit(500_000_000)] // 500 MB max TODO: adjust as needed. make endpoint for bigger mods
    public async Task<IActionResult> UploadNewMod(
        [FromForm] ModUploadForm dto, [CurrentUser] AppUser user)
    {
        if (dto.File.Length == 0)
            return BadRequest("No file uploaded.");

        var now = DateTime.UtcNow;
        var userId = user.Id;

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

    [Authorize]
    [HttpPost("UploadNewModVersion")]
    [RequestSizeLimit(500_000_000)] // 500 MB max TODO: adjust as needed. make endpoint for bigger mods
    public async Task<IActionResult> UploadNewModVersion(
        [FromForm] ModVersionUploadForm dto, [CurrentUser] AppUser user)
    {
        if (dto.File.Length == 0)
            return BadRequest("No file uploaded.");

        var now = DateTime.UtcNow;
        var userId = user.Id;

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
    public async Task<IActionResult> GetModsBySteamAppId([FromQuery] int appId, [FromQuery] PaginatedQuery query, [FromQuery] bool showOnlyFavourites, [FromQuery] ModsSortingOptions sortBy, [FromQuery] string? searchTerm = null)
    {
        //TODO: implement favourite mods
        var (mods, totalCount) = await modService.GetModsBySteamAppIdAsync(appId, query.Page, query.PageSize, sortBy, searchTerm);

        var result = new PaginatedResponse<ModListItemDto>
        {
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalCount,
            Items = mods.Select(mod => new ModListItemDto
            {
                Id = mod.Id,
                Name = mod.Name,
                DescriptionSnippet = mod.DescriptionSnippet,
                ThumbnailUrl = mod.ThumbnailUrl,
                Author = mod.AuthorUsername,
                AuthorId = mod.CreatedBy,
                LastUpdatedDate = mod.LastUpdatedAt,
                TotalDownloads = mod.Versions.Sum(v => v.DownloadCount),
                TotalFavourited = mod.Favourites.Count,
                TotalReviews = mod.Reviews.Count,
                AverageRating = mod.Reviews.Average(r => r.Rating)
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
            AverageRating = ModService.GetRatingsAverage(mod.Reviews),
            ReviewCount = mod.Reviews.Count,
            FavouriteCount = mod.Favourites.Count 
        };

        return Ok(result);
    }

    [HttpPost("{modId}/favourite")]
    [Authorize]
    public async Task<IActionResult> AddFavourite(Guid modId, [CurrentUser] AppUser user)
    {
        var userId = user.Id;

        if (!await modService.MarkModAsFavourite(modId, userId))
            return BadRequest("This mod is already in favourites!");
        return Ok();
    }

    [HttpDelete("{modId}/favourite")]
    [Authorize]
    public async Task<IActionResult> RemoveFavourite(Guid modId, [CurrentUser] AppUser user)
    {
        var userId = user.Id;
        if (!await modService.RemoveModFromFavourites(modId, userId))
            return BadRequest("This mod is not in your favourites!");
        return Ok();
    }

    [HttpGet("{modId}/reviews")]
    public async Task<IActionResult> GetReviewsPaginatedQuery(Guid modId, ReviewsSortingOptions? sortBy = ReviewsSortingOptions.HighestRated, int page = 1, int pageSize = 10)
    {
        var result = await modService.GetModReviewsPaginatedResponse(modId, page, pageSize, sortBy);

        return Ok(result);
    }
}