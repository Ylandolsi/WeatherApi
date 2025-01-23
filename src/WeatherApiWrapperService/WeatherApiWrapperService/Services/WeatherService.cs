using WeatherApiWrapperService.Contracts;

namespace WeatherApiWrapperService.Services;

public class WeatherService : IWeatherService 
{
    private readonly HttpClient _httpClient;
    // private readonly IDistributedCache _cache;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly ILogger<WeatherService> _logger; 
    
    public WeatherService(HttpClient httpClient, IConfiguration configuration , ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _apiKey = configuration.GetSection("WeatherApi:ApiKey").Value;
        _baseUrl = configuration.GetSection("WeatherApi:BaseUrl").Value ;
        _logger = logger;
        
    }


    
        
    
}
