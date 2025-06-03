using System.Reflection.Metadata.Ecma335;
using System.Security.AccessControl;
using Candlelight.Core.Dtos.Game;
using Candlelight.Core.Entities;
using Candlelight.Core.Entities.Steam;
using Candlelight.Core.Enums;
using Candlelight.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Candlelight.Application.Services;

public class GameService(DataContext dataContext, SteamService steamApiService)
{
    private readonly DataContext _dataContext = dataContext;
    private readonly SteamService _steamApiService = steamApiService;

    public async Task<Game?> GetSteamGameDetailsByIdAsync(Guid id)
    {
        var game = await _dataContext.Games
            .Include(g => g.SteamGameDetails!)
                .ThenInclude(d => d.Genres)
            .Include(g => g.SteamGameDetails!)
                .ThenInclude(d => d.Categories)
            .Include(g => g.SteamGameDetails!)
                .ThenInclude(d => d.Platforms)
            .Include(g => g.CustomGameDetails)
            .FirstOrDefaultAsync(g => g.Id == id);

        return game;
    }

    public async Task<Guid> GetGameIdByModIdAsync(Guid id)
    {
        var mod = await _dataContext.Mods.SingleOrDefaultAsync(m => m.Id == id);

        return mod?.GameId ?? Guid.Empty;
    }

    public async Task<SteamGameDetails?> GetSteamGameDetailsBySteamAppIdAsync(int appId)
    {
        var game = await _dataContext.SteamGameDetails
            .Include(d => d.Genres)
            .Include(d => d.Categories)
            .Include(d => d.Platforms)
            .FirstOrDefaultAsync(g => g.AppId == appId);

        return game;
    }

    public async Task<SteamGameDetails?> GetOrFetchSteamGameDetailsByIdAsync(int appId, Guid currentUserId)
    {
        var steamGameDetails = await _dataContext.SteamGameDetails
            .Include(d => d.Genres)
            .Include(d => d.Categories)
            .Include(d => d.Platforms)
            .FirstOrDefaultAsync(g => g.AppId == appId);

        if (steamGameDetails != null)
        {
            return steamGameDetails;
        }

        steamGameDetails = await _steamApiService.FetchGameDetailsAsync(appId, currentUserId);
        if (steamGameDetails == null)
        {
            return null;
        }

        if (steamGameDetails.ReleaseDate.HasValue)
        {
            steamGameDetails.ReleaseDate = steamGameDetails.ReleaseDate.Value.ToUniversalTime();
        }

        var game = new Game()
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            LastUpdatedAt = DateTime.Now,
            CreatedBy = currentUserId,
            SteamGameDetails = steamGameDetails
        };

        steamGameDetails.Game = game;
        steamGameDetails.GameId = game.Id;

        _dataContext.SteamGameDetails.Add(steamGameDetails);
        _dataContext.Games.Add(game);
        await _dataContext.SaveChangesAsync();
        return steamGameDetails;
    }

    public async Task<(List<SteamGameDetails> Games, int TotalCount)> GetSteamGameDetailsFromDbAsync(int page, int pageSize, GamesSortingOptions sortBy, string? searchTerm)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _dataContext.SteamGameDetails
            .Include(d => d.Genres)
            .Include(d => d.Categories)
            .Include(d => d.Platforms)
            .Include(d => d.Game)
                .ThenInclude(g => g.Favourites)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(g => g.Name.ToLower().Contains(searchTerm.ToLower()));
        }

        var sortedQuery = sortBy switch
        {
            GamesSortingOptions.Alphabetical => query.OrderBy(g => g.Name),
            GamesSortingOptions.ReverseAlphabetical => query.OrderByDescending(g => g.Name),
            GamesSortingOptions.MostMods => query.OrderByDescending(g => g.Game.Mods.Count),
            GamesSortingOptions.LeastMods => query.OrderBy(g => g.Game.Mods.Count),
            GamesSortingOptions.MostFavourited => query.OrderByDescending(g => g.Game.Favourites.Count),
            _ => query.OrderByDescending(g => g.Game.Favourites.Count)
        };

        var totalGames = await sortedQuery.CountAsync();
        var games = await sortedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (games, totalGames);
    }

    public async Task<(List<Game> Games, int TotalCount)> GetAllGamesFromDbAsync(
        int page, int pageSize, GamesSortingOptions sortBy, string? searchTerm)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _dataContext.Games
            .Include(g => g.SteamGameDetails)
            .Include(g => g.CustomGameDetails)
            .Include(g => g.Mods)
            .Include(g => g.Favourites)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(g =>
                (g.SteamGameDetails != null && g.SteamGameDetails.Name.ToLower().Contains(searchTerm.ToLower())) ||
                (g.CustomGameDetails != null && g.CustomGameDetails.Name.ToLower().Contains(searchTerm.ToLower()))
            );
        }

        var sortedQuery = sortBy switch
        {
            GamesSortingOptions.Alphabetical => query.OrderBy(g =>
                g.SteamGameDetails != null ? g.SteamGameDetails.Name :
                g.CustomGameDetails != null ? g.CustomGameDetails.Name : string.Empty),

            GamesSortingOptions.ReverseAlphabetical => query.OrderByDescending(g =>
                g.SteamGameDetails != null ? g.SteamGameDetails.Name :
                g.CustomGameDetails != null ? g.CustomGameDetails.Name : string.Empty),

            GamesSortingOptions.MostMods => query.OrderByDescending(g => g.Mods.Count),
            GamesSortingOptions.LeastMods => query.OrderBy(g => g.Mods.Count),
            GamesSortingOptions.MostFavourited => query.OrderByDescending(g => g.Favourites.Count),
            _ => query.OrderByDescending(g => g.Favourites.Count)
        };

        var totalGames = await sortedQuery.CountAsync();
        var games = await sortedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (games, totalGames);
    }

    public async Task<(List<CustomGameDetails> Games, int TotalCount)> GetCustomGameDetailsFromDbAsync(
        int page, int pageSize, GamesSortingOptions sortBy, string? searchTerm)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _dataContext.CustomGameDetails
            .Include(d => d.Game)
            .ThenInclude(g => g.Mods)
            .Include(d => d.Game)
            .ThenInclude(g => g.Favourites)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(g => g.Name.ToLower().Contains(searchTerm.ToLower()));
        }

        var sortedQuery = sortBy switch
        {
            GamesSortingOptions.Alphabetical => query.OrderBy(g => g.Name),
            GamesSortingOptions.ReverseAlphabetical => query.OrderByDescending(g => g.Name),
            GamesSortingOptions.MostMods => query.OrderByDescending(g => g.Game.Mods.Count),
            GamesSortingOptions.LeastMods => query.OrderBy(g => g.Game.Mods.Count),
            GamesSortingOptions.MostFavourited => query.OrderByDescending(g => g.Game.Favourites.Count),
            _ => query.OrderByDescending(g => g.Game.Favourites.Count)
        };

        var totalGames = await sortedQuery.CountAsync();
        var games = await sortedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (games, totalGames);
    }


    public async Task<bool> MarkGameAsFavourite(Guid gameId, Guid userId)
    {
        try
        {
            if (await _dataContext.GameFavourites.AnyAsync(f => f.GameId == gameId && f.UserId == userId))
                return false;

            _dataContext.GameFavourites.Add(new GameFavourite
            {
                Id = Guid.NewGuid(),
                GameId = gameId,
                UserId = userId,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            });
            await _dataContext.SaveChangesAsync();
        }
        catch
        {
            return false;
        }

        return true;
    }

    public async Task<bool> RemoveGameFromFavourites(Guid gameId, Guid userId)
    {
        try
        {
            var fav = await _dataContext.GameFavourites
                .FirstOrDefaultAsync(f => f.GameId == gameId && f.UserId == userId);

            if (fav == null) return false;

            _dataContext.GameFavourites.Remove(fav);
            await _dataContext.SaveChangesAsync();
        }
        catch
        {
            return false;
        }

        return true;
    }

    public async Task<List<GameFavourite>> GetUserFavouriteGamesAsync(Guid userId)
    {
        return await _dataContext.GameFavourites.Where(f => f.UserId == userId).ToListAsync();
    }

    public async Task<CustomGameDetails> AddCustomGameAsync(CustomGameDto dto, Guid userId, IFormFile? coverImage)
    {
        string? savedImageFilename = null;
        var gameId = Guid.NewGuid();

        if (coverImage is { Length: > 0 })
        {
            var extension = Path.GetExtension(coverImage.FileName);
            if (!_allowedImageExtensions.Contains(extension))
            {
                throw new InvalidDataException("Invalid file type. Only JPG, PNG, WEBP, and GIF are allowed.");
            }

            var filename = $"{gameId}{Path.GetExtension(coverImage.FileName)}";
            var path = Path.Combine("wwwroot/custom-covers", filename);

            await using var stream = System.IO.File.Create(path);
            await coverImage.CopyToAsync(stream);

            savedImageFilename = filename;
        }

        var customGameDetails = new CustomGameDetails
        {
            Id = gameId,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow,
            Name = dto.Name,
            Description = dto.Description,
            CoverImage = savedImageFilename,
            CreatedBy = userId
        };

        var game = new Game()
        {
            Id = Guid.NewGuid(),
            CustomGameDetails = customGameDetails,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            LastUpdatedAt = DateTime.UtcNow
        };

        customGameDetails.GameId = game.Id;

        await _dataContext.Games.AddAsync(game);
        await _dataContext.CustomGameDetails.AddAsync(customGameDetails);
        await _dataContext.SaveChangesAsync();
        return customGameDetails;
    }

    private static readonly HashSet<string> _allowedImageExtensions =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp", ".gif"
        };

    public async Task<GameDetailsDto?> GetGameDetailsAsync(Guid id)
    {
        var game = await _dataContext.Games
            .Include(g => g.SteamGameDetails)
            .Include(g => g.CustomGameDetails)
            .Include(g => g.Mods)
            .Include(g => g.Favourites)
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id);

        if (game == null)
            return null;

        return new GameDetailsDto
        {
            Id = game.Id,
            Name = game.SteamGameDetails?.Name ?? game.CustomGameDetails?.Name ?? "Unknown",
            AppId = game.SteamAppId,
            DescriptionSnippet = game.SteamGameDetails?.ShortDescription ?? game.CustomGameDetails?.Description ?? "",
            Description = game.SteamGameDetails?.DetailedDescription ?? game.CustomGameDetails?.Description ?? "",
            HeaderImage = game.SteamGameDetails?.HeaderImage ?? game.CustomGameDetails?.CoverImage ?? "",
            Developer = game.SteamGameDetails?.Developer ?? game.CustomGameDetails?.Developer ?? "Developer unknown",
            Publisher = game.SteamGameDetails?.Publisher ?? game.CustomGameDetails?.Publisher ?? "Publisher unknown",
            IsFree = game.SteamGameDetails?.IsFree,
            Price = game.SteamGameDetails?.Price,
            Currency = game.SteamGameDetails?.Currency,
            MetacriticScore = game.SteamGameDetails?.MetacriticScore,
            ReleaseDate = game.SteamGameDetails?.ReleaseDate,
            Website = game.SteamGameDetails?.Website,
            ModCount = game.Mods.Count,
            FavouriteCount = game.Favourites.Count,
            Genres = game.SteamGameDetails?.Genres ?? [],
            Categories = game.SteamGameDetails?.Categories ?? [],
            Platforms = game.SteamGameDetails?.Platforms ?? [],
            IsCustom = game.IsCustom,
        };
    }

    public async Task<bool> IsGameFavouritedByUser(Guid gameId, Guid userId)
    {
        return await _dataContext.GameFavourites
            .AnyAsync(f => f.GameId == gameId && f.UserId == userId);
    }
}