using Microsoft.AspNetCore.Mvc;

namespace ZymLabs.NSwag.FluentValidation.WebApi.Tests.Controllers;
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly List<WeatherForecast> _forecasts = new();

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
        _forecasts.Add(new WeatherForecast()
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
            TemperatureC = 20,
            Summary = "Mild"
        });
        _forecasts.Add(new WeatherForecast()
        {
            Date = DateOnly.FromDateTime(DateTime.Now),
            TemperatureC = 30,
            Summary = "Hot"
        });
        _forecasts.Add(new WeatherForecast()
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(+1)),
            TemperatureC = 10,
            Summary = "Cold"
        });
    }

    [HttpPost(Name = "CreateWeatherForecast")]
    public IEnumerable<WeatherForecast> Post([FromBody] WeatherForecast forecast)
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
