using Candlelight.Core.Entities.Testing;
using Microsoft.AspNetCore.Mvc;

namespace Candlelight.Api.Controllers;

/// <summary>
/// Controller used exclusively for life-check and testing purposes.
/// </summary>
[ApiController]
[Route("[controller]")]
public class WeatherForecastController() : ControllerBase
{
    private static readonly string[] _summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    /// <summary>
    /// Endpoint used exclusively for life-check and testing purposes.
    /// </summary>
    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {     
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = _summaries[Random.Shared.Next(_summaries.Length)]
        })
        .ToArray();
    }
}