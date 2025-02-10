using Candlelight.Core.Entities.Steam;
using Candlelight.Infrastructure.Persistence.Data;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Candlelight.Application.Services;

public class SteamService(IOptions<SteamSettings> options, HttpClient httpClient, DataContext dataContext)
{
    private readonly string _steamApiKey = options.Value.SteamApiKey ??
                                           throw new InvalidOperationException(
                                               "Steam API key is missing! Add it as a dotnet secret with key: SteamApiKey");

    private readonly HttpClient _httpClient =
        httpClient ?? throw new InvalidOperationException("Couldn't assign http client.");

    private readonly DataContext _dataContext = dataContext;
    private const string SteamStoreApiUrl = "https://store.steampowered.com/api/appdetails?appids=";
    private const string SteamChartsApiUrl = "https://api.steampowered.com/ISteamChartsService/GetMostPlayedGames/v1/";

    public string GetSteamApiKey()
    {
        return _steamApiKey;
    }

    public async Task<GameDetails?> FetchGameDetailsAsync(int appId)
    {
        var response = await _httpClient.GetStringAsync($"{SteamStoreApiUrl}{appId}");
        using JsonDocument json = JsonDocument.Parse(response);

        var root = json.RootElement.GetProperty(appId.ToString());
        if (!root.GetProperty("success").GetBoolean()) return null;

        var data = root.GetProperty("data");

        return new GameDetails
        {
            AppId = appId,
            Name = data.GetProperty("name").GetString() ?? "Unknown",
            ShortDescription = data.GetProperty("short_description").GetString(),
            DetailedDescription = data.GetProperty("detailed_description").GetString(),
            HeaderImage = data.GetProperty("header_image").GetString(),
            Website = data.TryGetProperty("website", out var website) ? website.GetString() : null,
            IsFree = data.GetProperty("is_free").GetBoolean(),
            Price = data.TryGetProperty("price_overview", out var price)
                ? price.GetProperty("final").GetDecimal() / 100
                : (decimal?)null,
            Currency = data.TryGetProperty("price_overview", out var currency)
                ? currency.GetProperty("currency").GetString()
                : null,
            Developer = data.GetProperty("developers")[0].GetString(),
            Publisher = data.GetProperty("publishers")[0].GetString(),
            MetacriticScore = data.TryGetProperty("metacritic", out var metacritic)
                ? metacritic.GetProperty("score").GetInt32()
                : 0,
            ReleaseDate = data.GetProperty("release_date").GetProperty("coming_soon").GetBoolean()
                ? (DateTime?)null
                : DateTime.Parse(data.GetProperty("release_date").GetProperty("date").GetString()!),
            Genres = data.GetProperty("genres").EnumerateArray()
                .Select(g => new Genre { Name = g.GetProperty("description").GetString()! }).ToList(),

            Categories = data.GetProperty("categories").EnumerateArray()
                .Select(c => new Category { Name = c.GetProperty("description").GetString()! }).ToList(),

            Platforms = data.GetProperty("platforms").EnumerateObject()
                .Where(p => p.Value.GetBoolean())
                .Select(p => new Platform { Name = p.Name }).ToList()
        };
    }

    public async Task FetchAndSaveTopGamesAsync(int maxGames = 100)
    {
        try
        {
            var response = await httpClient.GetStringAsync(SteamChartsApiUrl);
            using var json = JsonDocument.Parse(response);

            if (!json.RootElement.TryGetProperty("response", out var responseElement) ||
                !responseElement.TryGetProperty("ranks", out var ranks))
            {
                return;
            }

            var topAppIds = new List<int>();

            foreach (var game in ranks.EnumerateArray())
            {
                if (game.TryGetProperty("appid", out var appIdProperty))
                {
                    topAppIds.Add(appIdProperty.GetInt32());
                }
            }

            int savedCount = 0;
            foreach (var appId in topAppIds.Take(maxGames))
            {
                var existingGame = await _dataContext.Games.FindAsync(appId);
                if (existingGame != null) continue;

                var gameDetails = await FetchGameDetailsAsync(appId);
                if (gameDetails == null) continue;

                _dataContext.Games.Add(gameDetails);
                savedCount++;
            }

            if (savedCount > 0)
            {
                await _dataContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching top games: {ex.Message}");
        }
    }
}

