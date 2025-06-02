using Candlelight.Api.Attributes;
using Candlelight.Application.Services;
using Candlelight.Core.Dtos.Mod;
using Candlelight.Core.Dtos.Query;
using Candlelight.Core.Entities;
using Candlelight.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Candlelight.Api.Controllers;


/// <summary>
/// Controller which handles modifications.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ModController(
    ModService modService) : ControllerBase
{
    private readonly ModService _modService = modService;

    /// <summary>
    /// Returns paginated modifications for the specified game.
    /// </summary>
    [HttpGet("GetModsByGameId")]
    public async Task<IActionResult> GetModsByGameId(
        [FromQuery] PaginatedQuery query,
        [FromQuery] bool showOnlyFavourites, 
        [FromQuery] Guid gameId,
        [FromQuery] ModsSortingOptions sortBy,
        [FromQuery] string? searchTerm = null
        )
    {
        var (mods, totalCount) = await _modService.GetModsByGameIdAsync(gameId, query.Page, query.PageSize, sortBy, searchTerm);

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
                Author = mod.CreatedByUser.UserName!,
                AuthorId = mod.CreatedBy,
                LastUpdatedDate = mod.LastUpdatedAt,
                TotalDownloads = mod.Versions?.Sum(v => v.DownloadCount) ?? 0,
                TotalFavourited = mod.Favourites?.Count ?? 0,
                TotalReviews = mod.Reviews?.Count ?? 0,
                AverageRating = mod.Reviews?.Any() == true ? mod.Reviews.Average(r => r.Rating) : 0
            }).ToList()
        };

        return Ok(result);
    }

    /// <summary>
    /// Endpoint for uploading a new modification.
    /// </summary>
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
            CreatedAt = now,
            CreatedBy = userId,
            LastUpdatedAt = now,
            Versions = []
        };

        if (dto.Images?.Count > 10)
            return BadRequest("You can upload up to 10 images.");

        var imagesDir = Path.Combine("wwwroot", "mod-images", mod.Id.ToString());
        Directory.CreateDirectory(imagesDir);

        string? savedThumbnailPath = null;

        if (dto.Images is not null)
        {
            for (var i = 0; i < dto.Images.Count; i++)
            {
                var img = dto.Images[i];
                var ext = Path.GetExtension(img.FileName);
                var name = $"{i + 1}{ext}";
                var path = Path.Combine(imagesDir, name);

                await using var stream = new FileStream(path, FileMode.Create);
                await img.CopyToAsync(stream);

                if (dto.SelectedThumbnail == name)
                    savedThumbnailPath = $"/mod-images/{mod.Id}/{name}";
            }

            if (savedThumbnailPath == null && dto.Images.Count > 0)
            {
                var first = dto.Images[0];
                var ext = Path.GetExtension(first.FileName);
                savedThumbnailPath = $"/mod-images/{mod.Id}/1{ext}";
            }

            mod.ThumbnailUrl = savedThumbnailPath!;
        }
        else
        {
            mod.ThumbnailUrl = "";
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
            FileSizeBytes = dto.File.Length,
            FileType = fileExt.TrimStart('.').ToLowerInvariant(),
            SupportedVersions = dto.SupportedVersions,
            Dependencies = dto.Dependencies,
            DownloadCount = 0,
            CreatedAt = now,
            CreatedBy = userId,
            LastUpdatedAt = now
        };

        await _modService.AddModAsync(mod);
        await _modService.AddModVersionAsync(version);

        return Ok(new ModUploadResponseDto
        {
            ModId = mod.Id,
            ModVersionId = version.Id,
            DownloadUrl = publicUrl
        });
    }

    /// <summary>
    /// Endpoint for uploading a new version of a modification.
    /// </summary>
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

        var mod = await _modService.GetModWithVersionsByIdAsync(dto.ModId);

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
            Mod = mod,
            Version = dto.Version,
            Changelog = dto.Changelog,
            FileUrl = publicUrl,
            CreatedAt = now,
            CreatedBy = userId,
            LastUpdatedAt = now,
            FileSizeBytes = dto.File.Length,
            FileType = fileExt.TrimStart('.').ToLowerInvariant(),
            SupportedVersions = dto.SupportedVersions,
            Dependencies = dto.Dependencies,
            DownloadCount = 0,
        };

        await _modService.AddModVersionAsync(version);
        return Ok(new ModUploadResponseDto()
        {
            ModId = mod.Id,
            ModVersionId = version.Id,
            DownloadUrl = publicUrl
        });
    }


    /// <summary>
    /// Returns all modifications for a Steam game.
    /// </summary>
    [HttpGet("GetModsBySteamAppId")]
    public async Task<IActionResult> GetModsBySteamAppId([FromQuery] int appId, [FromQuery] PaginatedQuery query, [FromQuery] bool showOnlyFavourites, [FromQuery] ModsSortingOptions sortBy, [FromQuery] string? searchTerm = null)
    {
        //TODO: implement favourite mods
        var (mods, totalCount) = await _modService.GetModsBySteamAppIdAsync(appId, query.Page, query.PageSize, sortBy, searchTerm);

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
                Author = mod.CreatedByUser.UserName!,
                AuthorId = mod.CreatedBy,
                LastUpdatedDate = mod.LastUpdatedAt,
                TotalDownloads = mod.Versions?.Sum(v => v.DownloadCount) ?? 0,
                TotalFavourited = mod.Favourites?.Count ?? 0,
                TotalReviews = mod.Reviews?.Count ?? 0,
                AverageRating = mod.Reviews?.Any() == true ? mod.Reviews.Average(r => r.Rating) : 0
            }).ToList()
        };

        return Ok(result);
    }

    //TODO: update to new structure
    /// <summary>
    /// Returns details for a specified modification.
    /// </summary>
    [HttpGet("GetModDetailsById")]
    public async Task<IActionResult> GetModDetailsById([FromQuery] Guid modId)
    {
        var mod = await _modService.GetModDetailsById(modId);

        if (mod == null)
        {
            return BadRequest($"Mod with id {modId} doesn\'t exist.");
        }

        var result = new ModDetailsResponseDto()
        {
            Id = mod.Id,
            GameId = mod.GameId,
            AuthorUsername = mod.CreatedByUser.UserName!,
            Description = mod.Description,
            Name = mod.Name,
            GameName = mod.Game.SteamGameDetails != null ? mod.Game.SteamGameDetails.Name : "",
            Versions = mod.Versions.Select(v => new ModVersionDto()
            {
                Id = v.Id,
                ModId = v.ModId, 
                ModName = mod.Name,
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
            AverageRating = mod.Reviews?.Any() == true ? mod.Reviews.Average(r => r.Rating) : 0,
            ReviewCount = mod.Reviews?.Count ?? 0,
            FavouriteCount = mod.Favourites.Count 
        };

        return Ok(result);
    }

    /// <summary>
    /// Endpoint that adds a mod to current user's favourites.
    /// </summary>
    [HttpPost("{modId}/Favourite")]
    [Authorize]
    public async Task<IActionResult> AddFavourite(Guid modId, [CurrentUser] AppUser user)
    {
        var userId = user.Id;

        if (!await _modService.MarkModAsFavourite(modId, userId))
            return BadRequest("This mod is already in favourites!");
        return Ok();
    }

    /// <summary>
    /// Endpoint that removes a mod from current user's favourites.
    /// </summary>
    [HttpDelete("{modId}/Favourite")]
    [Authorize]
    public async Task<IActionResult> RemoveFavourite(Guid modId, [CurrentUser] AppUser user)
    {
        var userId = user.Id;
        if (!await _modService.RemoveModFromFavourites(modId, userId))
            return BadRequest("This mod is not in your favourites!");
        return Ok();
    }

    /// <summary>
    /// Returns paginated list of reviews for a specified modification.
    /// </summary>
    [HttpGet("{modId}/Reviews")]
    public async Task<IActionResult> GetReviewsPaginatedQuery(Guid modId, ReviewsSortingOptions? sortBy = ReviewsSortingOptions.HighestRated, int page = 1, int pageSize = 10)
    {
        var result = await _modService.GetModReviewsPaginatedResponse(modId, page, pageSize, sortBy);

        return Ok(result);
    }

    /// <summary>
    /// Returns all mods created by the current user
    /// </summary>
    [HttpGet("CurrentUserCreatedMods")]
    [Authorize(Policy = "JwtOnly")]
    public async Task<IActionResult> GetCurrentUserCreatedMods([CurrentUser] AppUser user)
    {
        var mods = await _modService.GetModsByUserIdAsync(user.Id);
        var result = mods.Select(mod => new ModListItemDto
        {
            Id = mod.Id,
            Name = mod.Name,
            DescriptionSnippet = mod.DescriptionSnippet,
            ThumbnailUrl = mod.ThumbnailUrl,
            Author = mod.CreatedByUser.UserName!,
            AuthorId = mod.CreatedBy,
            LastUpdatedDate = mod.LastUpdatedAt,
            TotalDownloads = mod.Versions?.Sum(v => v.DownloadCount) ?? 0,
            TotalFavourited = mod.Favourites?.Count ?? 0,
            TotalReviews = mod.Reviews?.Count ?? 0,
            AverageRating = mod.Reviews?.Any() == true ? mod.Reviews.Average(r => r.Rating) : 0
        });

        return Ok(result);
    }


    /// <summary>
    /// Returns all mods created by the user with specified ID
    /// </summary>
    [HttpGet("UserCreatedMods/{userId}")]
    public async Task<IActionResult> GetUserCreatedMods(Guid userId)
    {
        var mods = await _modService.GetModsByUserIdAsync(userId);
        var result = mods.Select(mod => new ModListItemDto
        {
            Id = mod.Id,
            Name = mod.Name,
            DescriptionSnippet = mod.DescriptionSnippet,
            ThumbnailUrl = mod.ThumbnailUrl,
            Author = mod.CreatedByUser.UserName!,
            AuthorId = mod.CreatedBy,
            LastUpdatedDate = mod.LastUpdatedAt,
            TotalDownloads = mod.Versions?.Sum(v => v.DownloadCount) ?? 0,
            TotalFavourited = mod.Favourites?.Count ?? 0,
            TotalReviews = mod.Reviews?.Count ?? 0,
            AverageRating = mod.Reviews?.Any() == true ? mod.Reviews.Average(r => r.Rating) : 0
        });

        return Ok(result);
    }

    /// <summary>
    /// Returns all versions of a mod with given id.
    /// </summary>
    [HttpGet("GetVersionsOfMod/{modId}")]
    public async Task<IActionResult> GetVersionsOfMod(Guid modId)
    {
        var versions = await _modService.GetModVersionsOfModAsync(modId);
        return Ok(versions);
    }
}