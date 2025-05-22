using Candlelight.Core.Dtos.Mod;
using Candlelight.Core.Dtos.Query;
using Candlelight.Core.Entities;
using Candlelight.Core.Enums;
using Candlelight.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Candlelight.Application.Services;

public class ModService(DataContext context)
{
    private readonly DataContext _context = context;

    public async Task<Mod?> GetModByIdAsync(Guid modId)
    {
        return await _context.Mods
            .Include(m => m.Game)
            .FirstOrDefaultAsync(m => m.Id == modId);
    }

    public async Task<List<Mod>> GetModsByUserIdAsync(Guid userId)
    {
        return await _context.Mods
            .Include(m => m.Game)
            .Where(m => m.CreatedBy == userId)
            .ToListAsync();
    }

    public async Task<Mod> AddModAsync(Mod mod)
    {
        await _context.Mods.AddAsync(mod);
        if (mod.Versions.Count > 0)
        {
            foreach (var version in mod.Versions)
            {
                await AddModVersionAsync(version);
            }
            
        }
        await _context.SaveChangesAsync();
        return mod;
    }

    public async Task<ModVersion> AddModVersionAsync(ModVersion modVersion)
    {
        await _context.ModVersions.AddAsync(modVersion);
        await _context.SaveChangesAsync();
        return modVersion;
    }

    public async Task<(List<Mod> Mods, int TotalCount)> GetModsByGameIdAsync(Guid gameId, int page, int pageSize)
    {
        var query = _context.Mods
            .Where(m => m.GameId == gameId)
            .OrderByDescending(m => m.CreatedAt);

        var totalCount = await query.CountAsync();

        var mods = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (mods, totalCount);
    }

    public async Task<(List<Mod> Mods, int TotalCount)> GetModsBySteamAppIdAsync(int appId, int page, int pageSize, ModsSortingOptions sortBy, string? searchTerm)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _context.Mods
            .Include(m => m.Game)
                .ThenInclude(g => g.SteamGameDetails)
            .Include(m => m.Versions)
            .Where(m => m.Game.SteamGameDetails != null && m.Game.SteamGameDetails.AppId == appId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(m => m.Name.Contains(searchTerm));
        }

        IQueryable<Mod> sortedQuery = sortBy switch
        {
            ModsSortingOptions.Newest => query.OrderByDescending(m => m.CreatedAt),
            ModsSortingOptions.Oldest => query.OrderBy(m => m.CreatedAt),
            ModsSortingOptions.HighestRated => query.OrderByDescending(m => m.Reviews.Average(r => (double?)r.Rating) ?? 0),
            ModsSortingOptions.LowestRated => query.OrderBy(m => m.Reviews.Average(r => (double?)r.Rating) ?? 0),
            ModsSortingOptions.MostDownloaded => query.OrderByDescending(m => m.Versions.Sum(v => v.DownloadCount)),
            ModsSortingOptions.MostFavourited => query.OrderByDescending(m => m.Favourites.Count),
            ModsSortingOptions.Alphabetical => query.OrderBy(m => m.Name),
            ModsSortingOptions.ReverseAlphabetical => query.OrderByDescending(m => m.Name),
            _ => query.OrderByDescending(m => m.Reviews.Average(r => (double?)r.Rating) ?? 0)
        };

        var totalCount = await sortedQuery.CountAsync();
        var mods = await sortedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (mods, totalCount);
    }

    public async Task<Mod?> GetModDetailsById(Guid modId)
    {
        var query = _context.Mods
            .Include(m => m.Versions)
            .Include(m => m.Game)
                .ThenInclude(g => g.SteamGameDetails)
            .Include(m => m.Reviews)
            .Include(m => m.Favourites)
            .Where(m => m.Id == modId);

        var mod = await query.FirstOrDefaultAsync();

        return mod;
    }

    public static double GetRatingsAverage(List<ModReview> reviews)
    {
        return reviews.Count > 0 ? reviews.Average(r => r.Rating) : 0;
    }

    public async Task<bool> MarkModAsFavourite(Guid modId, Guid userId)
    {
        try
        {
            if (await _context.ModFavourites.AnyAsync(f => f.ModId == modId && f.UserId == userId))
                return false;

            _context.ModFavourites.Add(new ModFavourite
            {
                Id = Guid.NewGuid(),
                ModId = modId,
                UserId = userId,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }
        catch
        {
            return false;
        }

        return true;
    }

    public async Task<bool> RemoveModFromFavourites(Guid modId, Guid userId)
    {
        try
        {
            var fav = await _context.ModFavourites.FindAsync(modId, userId);
            if (fav == null) return false;

            _context.ModFavourites.Remove(fav);
            await _context.SaveChangesAsync();
        }
        catch
        {
            return false;
        }

        return true;
    }

    public async Task<PaginatedResponse<ModReviewDto>> GetModReviewsPaginatedResponse(Guid modId, int page, int pageSize, ReviewsSortingOptions? sortBy)
    {
        IQueryable<ModReview> query = _context.ModReviews
            .Where(r => r.ModId == modId)
            .Include(r => r.User);

        query = sortBy switch
        {
            ReviewsSortingOptions.HighestRated => query.OrderByDescending(r => r.Rating),
            ReviewsSortingOptions.LowestRated => query.OrderBy(r => r.Rating),
            ReviewsSortingOptions.Newest => query.OrderByDescending(r => r.CreatedAt),
            ReviewsSortingOptions.Oldest => query.OrderBy(r => r.CreatedAt),
            ReviewsSortingOptions.WithCommentsOnly => query.Where(r => !string.IsNullOrEmpty(r.Comment)),
            ReviewsSortingOptions.Longest => query.OrderByDescending(r => r.Comment!.Length),
            ReviewsSortingOptions.Shortest => query.OrderBy(r => r.Comment!.Length),
            _ => query.OrderByDescending(r => r.Rating)
        };


        var totalCount = await query.CountAsync();
        var reviews = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ModReviewDto
            {
                UserId = r.UserId,
                Username = r.User.UserName ?? "Anonymous user",
                Rating = r.Rating,
                ReviewText = r.Comment,
                Id = r.Id,
                ModId = r.ModId,
                IsDeleted = r.IsSoftDeleted,
                CreatedAt = r.CreatedAt,
                LastUpdatedAt = r.LastUpdatedAt,
            })
            .ToListAsync();

        return new PaginatedResponse<ModReviewDto>()
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount, 
            Items =  reviews
        };
    }
}
