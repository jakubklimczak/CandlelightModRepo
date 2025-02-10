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
        var url = $"{SteamStoreApiUrl}{appId}";
        try
        {
            var response = await httpClient.GetStringAsync(url);
            if (string.IsNullOrEmpty(response))
            {
                return null;
            }

            using JsonDocument json = JsonDocument.Parse(response);

            if (!json.RootElement.TryGetProperty(appId.ToString(), out var appData) ||
                !appData.TryGetProperty("success", out var success) ||
                !success.GetBoolean() ||
                !appData.TryGetProperty("data", out var data))
            {
                return null;
            }

            return new GameDetails
            {
                AppId = appId,
                Name = data.TryGetProperty("name", out var name) ? name.GetString() ?? "Unknown" : "Unknown",
                ShortDescription = data.TryGetProperty("short_description", out var shortDesc) ? shortDesc.GetString() : null,
                DetailedDescription = data.TryGetProperty("detailed_description", out var detailedDesc) ? detailedDesc.GetString() : null,
                HeaderImage = data.TryGetProperty("header_image", out var headerImage) ? headerImage.GetString() : null,
                Website = data.TryGetProperty("website", out var website) ? website.GetString() : null,
                IsFree = data.TryGetProperty("is_free", out var isFree) && isFree.GetBoolean(),
                Price = data.TryGetProperty("price_overview", out var price) && price.TryGetProperty("final", out var finalPrice)
                ? finalPrice.GetDecimal() / 100
                : (decimal?)null,
                Currency = data.TryGetProperty("price_overview", out var priceCurrency) && priceCurrency.TryGetProperty("currency", out var currency)
                ? currency.GetString()
                : null,
                Developer = data.TryGetProperty("developers", out var devs) && devs.ValueKind == JsonValueKind.Array && devs.GetArrayLength() > 0
                ? devs[0].GetString()
                : null,
                Publisher = data.TryGetProperty("publishers", out var pubs) && pubs.ValueKind == JsonValueKind.Array && pubs.GetArrayLength() > 0
                ? pubs[0].GetString()
                : null,
                MetacriticScore = data.TryGetProperty("metacritic", out var metacritic) && metacritic.TryGetProperty("score", out var score)
                ? score.GetInt32()
                : 0,
                ReleaseDate = data.TryGetProperty("release_date", out var release) &&
                          release.TryGetProperty("coming_soon", out var comingSoon) &&
                          !comingSoon.GetBoolean() &&
                          release.TryGetProperty("date", out var date)
                ? DateTime.TryParse(date.GetString(), out var parsedDate) ? parsedDate.ToUniversalTime() : (DateTime?)null
                : null,
                Genres = data.TryGetProperty("genres", out var genres) && genres.ValueKind == JsonValueKind.Array
                ? genres.EnumerateArray().Select(g => new Genre { Name = g.GetProperty("description").GetString() ?? "Unknown" }).ToList()
                : [],
                Categories = data.TryGetProperty("categories", out var categories) && categories.ValueKind == JsonValueKind.Array
                ? categories.EnumerateArray().Select(c => new Category { Name = c.GetProperty("description").GetString() ?? "Unknown" }).ToList()
                : [],
                Platforms = data.TryGetProperty("platforms", out var platforms) && platforms.ValueKind == JsonValueKind.Object
                ? platforms.EnumerateObject()
                    .Where(p => p.Value.GetBoolean())
                    .Select(p => new Platform { Name = p.Name })
                    .ToList()
                : []
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching details for AppID {appId}: {ex.Message}");
            return null;
        }
    }


    public async Task FetchAndSaveTopGamesAsync()
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

            var savedCount = 0;
            foreach (var appId in topAppIds)
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

