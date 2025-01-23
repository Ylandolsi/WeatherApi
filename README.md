# WeatherApiWrapperService

WeatherApiWrapperService is a .NET 9.0 web API that provides weather information by interacting with an external weather service. It supports caching using Redis to improve performance and reduce the number of API calls to the external service.

## Features

- Fetch weather data by city and country.
- Fetch weather data by geographic coordinates (latitude and longitude).
- Caching of weather data using Redis.
- Swagger documentation for API endpoints.
- Configurable via `appsettings.json`.

### API Endpoints

- `GET /api/weather/{city}/{country}`: Get weather data for a specific city and country.
- `GET /api/weather/location/{latitude}/{longitude}`: Get weather data for specific geographic coordinates.
- `DELETE /api/weather`: Delete all cached weather data.
- `DELETE /api/weather/{key}`: Delete cached weather data by key.
