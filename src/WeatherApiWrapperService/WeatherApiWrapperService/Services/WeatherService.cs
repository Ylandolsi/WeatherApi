using WeatherApiWrapperService.Contracts;

namespace WeatherApiWrapperService.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;

    // private readonly IDistributedCache _cache;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _apiKey = configuration.GetSection("WeatherApi:ApiKey").Value;
        _baseUrl = configuration.GetSection("WeatherApi:BaseUrl").Value;
        _logger = logger;
    }

    public async Task<string?> GetWeatherAsync(string city, string country)
    {
        DateTime now = DateTime.Now;
        string formattedDate = now.ToString("yyyy-MM-dd");
        _logger.LogInformation($"Http Get request for weather in {city} ,{country} at {formattedDate}");
        // include days => doesnt show hours 
        var url =
            $"{_baseUrl}{city}%2C{country}/{formattedDate}/{formattedDate}?unitGroup=metric&key={_apiKey}&include=days&elements=tempmax,tempmin";

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Input location is invalid");
            return null;
        }

        // response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        // _logger.LogInformation(content);
        return content;
    }

    public async Task<string?> GetWeatherByLocationAsync(double latitude, double longitude)
    {
        DateTime now = DateTime.Now;
        string formattedDate = now.ToString("yyyy-MM-dd");
        _logger.LogInformation(
            $"Http Get request for weather at latitude: {latitude}, longitude: {longitude} at {formattedDate}");
        var url =
            $"{_baseUrl}{latitude},{longitude}/{formattedDate}/{formattedDate}?unitGroup=metric&key={_apiKey}&include=days&elements=tempmax,tempmin";

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Input location is invalid");
            return null;
        }

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
}
