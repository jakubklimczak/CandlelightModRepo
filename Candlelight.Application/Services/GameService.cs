using Candlelight.Core.Entities.Steam;
using Candlelight.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Candlelight.Application.Services;

public class GameService(DataContext dataContext, SteamService steamApiService)
{
    private readonly DataContext _dataContext = dataContext;
    private readonly SteamService _steamApiService = steamApiService;

    public async Task<GameDetails?> GetGameByIdAsync(int appId)
    {
        var game = await _dataContext.Games
            .Include(g => g.Genres)
            .Include(g => g.Categories)
            .Include(g => g.Platforms)
            .FirstOrDefaultAsync(g => g.AppId == appId);
        return game;
    }

    public async Task<GameDetails?> GetOrFetchGameByIdAsync(int appId)
    {
        var game = await _dataContext.Games
            .Include(g => g.Genres)
            .Include(g => g.Categories)
            .Include(g => g.Platforms)
            .FirstOrDefaultAsync(g => g.AppId == appId);

        if (game != null)
        {
            return game;
        }

        game = await _steamApiService.FetchGameDetailsAsync(appId);
        if (game == null)
        {
            return null;
        }

        if (game.ReleaseDate.HasValue)
        {
            game.ReleaseDate = game.ReleaseDate.Value.ToUniversalTime();
        }

        _dataContext.Games.Add(game);
        await _dataContext.SaveChangesAsync();
        return game;
    }

    public async Task<(List<GameDetails> Games, int TotalCount)> GetGamesFromDbAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var totalGames = await _dataContext.Games.CountAsync();
        var games = await _dataContext.Games
            .Include(g => g.Genres)
            .Include(g => g.Categories)
            .Include(g => g.Platforms)
            .OrderBy(g => g.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();


        return (games, totalGames);
    }
}