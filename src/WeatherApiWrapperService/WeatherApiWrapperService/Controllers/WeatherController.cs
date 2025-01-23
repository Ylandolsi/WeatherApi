using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WeatherApiWrapperService.Contracts;

namespace WeatherApiWrapperService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly ILogger<WeatherController> _logger;
    private readonly IWeatherService _weatherService;

    public WeatherController(ILogger<WeatherController> logger, IWeatherService weatherService)
    {
        _logger = logger;
        _weatherService = weatherService;
    }

    [HttpGet("{city}/{country}")]
    public async Task<IActionResult> GetWeatherToday(string country, string city)
    {
        if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(country))
        {
            return BadRequest("City and country are required.");
        }

        _logger.LogInformation($"Http Get request for weather in {city} ,{country}");
        var result = await _weatherService.GetWeatherAsync(city, country);
        if (result == null) return BadRequest();

        return Ok(result);
    }

    [HttpGet("location/{latitude}/{longitude}")]
    public async Task<IActionResult> GetWeatherByLocation(double latitude, double longitude)
    {
        _logger.LogInformation($"Http Get request for weather at latitude: {latitude}, longitude: {longitude}");
        var result = await _weatherService.GetWeatherByLocationAsync(latitude, longitude);
        if (result == null) return NotFound("Weather data not found.");

        return Ok(result);
    }
}
