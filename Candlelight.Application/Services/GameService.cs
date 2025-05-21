using System.Reflection.Metadata.Ecma335;
using Candlelight.Core.Entities;
using Candlelight.Core.Entities.Steam;
using Candlelight.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

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

    public async Task<SteamGameDetails?> GetOrFetchSteamGameDetailsByIdAsync(int appId)
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

        steamGameDetails = await _steamApiService.FetchGameDetailsAsync(appId);
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
            CreatedBy = Guid.Empty, // TODO: Set the CreatedBy field to the appropriate user ID
            SteamGameDetails = steamGameDetails
        };

        steamGameDetails.Game = game;
        steamGameDetails.GameId = game.Id;

        _dataContext.SteamGameDetails.Add(steamGameDetails);
        _dataContext.Games.Add(game);
        await _dataContext.SaveChangesAsync();
        return steamGameDetails;
    }

    public async Task<(List<SteamGameDetails> Games, int TotalCount)> GetSteamGameDetailsFromDbAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var totalGames = await _dataContext.SteamGameDetails.CountAsync();
        var games = await _dataContext.SteamGameDetails
            .Include(d => d.Genres)
            .Include(d => d.Categories)
            .Include(d => d.Platforms)
            .OrderBy(g => g.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();


        return (games, totalGames);
    }
}