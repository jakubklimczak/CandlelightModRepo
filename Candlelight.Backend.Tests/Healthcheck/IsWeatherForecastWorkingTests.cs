using Candlelight.Backend.Entities;
using Candlelight.Server.Controllers;

namespace Candlelight.Backend.Tests.Healthcheck;

[TestFixture]
public class IsWeatherForecastWorkingTests
{
    private WeatherForecastController _controller;

    [SetUp]
    public void Setup() => _controller = new WeatherForecastController();

    [Test]
    public void Get_ReturnsWeatherForecasts()
    {
        // Act
        var result = _controller.Get();

        // Assert
        Assert.That(result, Is.Not.EqualTo(null));
        Assert.That(result, Is.InstanceOf<WeatherForecast[]>());
        Assert.That(result.Count(), Is.EqualTo(5));
    }

    [Test]
    public void Get_ReturnsValidWeatherForecasts()
    {
        // Act
        var result = _controller.Get().ToList();

        // Assert
        foreach (var forecast in result)
        {
            Assert.Multiple(() =>
            {
                Assert.That(forecast.Date, Is.InstanceOf<DateOnly>());
                Assert.That(forecast.TemperatureC, Is.InstanceOf<int>());
                Assert.That(forecast.Summary, Is.Not.EqualTo(null));
            });
        }
    }
}
