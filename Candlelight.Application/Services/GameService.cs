using System.Reflection.Metadata.Ecma335;
using Candlelight.Core.Entities;
using Candlelight.Core.Entities.Steam;
using Candlelight.Core.Enums;
using Candlelight.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Candlelight.Application.Services;

public class GameService(DataContext dataContext, SteamService steamApiService)
{
    private readonly DataContext _dataContext = dataContext;
    private readonly SteamService _steamApiService = steamApiService;

    public async Task<SteamGameDetails?> GetSteamGameDetailsByIdAsync(int appId)
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
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(g => g.Name.Contains(searchTerm));
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
            var fav = await _dataContext.GameFavourites.FindAsync(gameId, userId);
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
}