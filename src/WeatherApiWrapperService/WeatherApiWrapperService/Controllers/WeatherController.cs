using Microsoft.AspNetCore.Mvc;
using WeatherApiWrapperService.Contracts;

namespace WeatherApiWrapperService.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherController : ControllerBase
{
    private readonly ILogger<WeatherController> _logger;
    private readonly IWeatherService _weatherService;
    public WeatherController(ILogger<WeatherController> logger , IWeatherService weatherService ) 
    {
        _logger = logger;
        _weatherService = weatherService;
        
    }

}
