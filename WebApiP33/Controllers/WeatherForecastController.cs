using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApiP33.Controllers;

[ApiController]
[Route("[controller]")]
[SwaggerTag("Weather Forecast API")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    [SwaggerOperation(
        Summary = "Get the weather forecast for the next 5 days",
        Description = "Returns an array of WeatherForecast objects, each containing the date, temperature in Celsius and Fahrenheit, and a summary of the weather conditions."
    )]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<WeatherForecast>))]
    [SwaggerResponse(500, "Internal Server Error")]
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
