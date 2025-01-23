using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using WeatherApiWrapperService.Contracts;

namespace WeatherApiWrapperService.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;

    // private readonly IDistributedCache _cache;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly ILogger<WeatherService> _logger;

    // Dependency for interacting with Redis via IDistributedCache
    private readonly IDistributedCache _distributedCache;

    // Dependency for interacting directly with Redis using StackExchange.Redis
    private readonly IConnectionMultiplexer _redisConnection;

    private readonly IConfiguration _configuration;


    public WeatherService(HttpClient httpClient,
        ILogger<WeatherService> logger,
        IDistributedCache distributedCache,
        IConnectionMultiplexer redisConnection,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration.GetSection("WeatherApi:ApiKey").Value;
        _baseUrl = configuration.GetSection("WeatherApi:BaseUrl").Value;
        _logger = logger;
        _distributedCache = distributedCache;
        _redisConnection = redisConnection;
        _configuration = configuration;
    }

    public async Task<string?> GetWeatherAsync(string city, string country)
    {
        var cacheKey = $"{city}-{country}";
        var cacheValue = await _distributedCache.GetStringAsync(cacheKey);
        if ( cacheValue != null)
        {
            _logger.LogInformation($"Cache hit for {city}-{country}");
            var cacheDataJson = JsonSerializer.Deserialize<string>(cacheValue);
            return cacheDataJson;
        }
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
        
        
        // cache 
        var serializedData = JsonSerializer.Serialize(content);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(10)
        };

        await _distributedCache.SetStringAsync(cacheKey, serializedData,cacheOptions);        
        return content;
    }

    public async Task<string?> GetWeatherByLocationAsync(double latitude, double longitude)
    {
        var cacheKey = $"{latitude}-{longitude}";
        var cacheValue = await _distributedCache.GetStringAsync(cacheKey);
        if ( cacheValue != null)
        {
            _logger.LogInformation($"Cache hit for {latitude}-{longitude}");
            var cacheData = JsonSerializer.Deserialize<string>(cacheValue);
            return cacheValue;
        }
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
        // cache 
        var serializedData = JsonSerializer.Serialize(content);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            // AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10) 
             // SlidingExpiration= TimeSpan.FromMinutes(10)
            //
        };

        await _distributedCache.SetStringAsync(cacheKey, content,cacheOptions);   
        return content;
    }
    public Task DeleteAllCaches()
    {
        // Get the Redis server instance from the connection multiplexer
        var server = _redisConnection.GetServer(_redisConnection.GetEndPoints().First());

        // Iterate over all keys in the Redis database
        foreach (var key in server.Keys())
        {
            // Get the InstanceName from the configuration (as specified in appsettings.json)
            string instanceName = _configuration["RedisCacheOptions:InstanceName"] ?? string.Empty;

            // Remove the instance name prefix if it exists
            var keyWithoutPrefix = key.ToString().Replace($"{instanceName}", "");

            // Remove each key-value pair from the distributed cache
            _distributedCache.Remove(keyWithoutPrefix);
        }
        return Task.CompletedTask;

    }
    public async Task  DeleteCache(string key)
    {
        var cacheValue = await _distributedCache.GetStringAsync(key);
        if (cacheValue is null )
        {
            _logger.LogInformation($"Deletion failed : Cache key {key} not found");
            
            return;
        }
        // Remove the specified key-value pair from the distributed cache
        _distributedCache.Remove(key);
        _logger.LogInformation($"Cache key {key} deleted");


    }
}
