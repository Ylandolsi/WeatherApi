using Microsoft.AspNetCore.Mvc;
using WeatherApiWrapperService.Contracts;
using WeatherApiWrapperService.Extensions;
using WeatherApiWrapperService.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.ConfigureCors();

builder.Services.ConfigureSwagger();
builder.Services.AddProblemDetails(); 

builder.Services.ConfigureWeatherService();
builder.Services.AddHttpClient<IWeatherService, WeatherService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetSection("WeatherApi:BaseUrl").Value);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});


// to enable custoum response from action
// exp : return BadRequest("some message")
// cuz [apiController] return a default response ( 400 - badRequest ) 
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();
app.UseExceptionHandler();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blog API ");
});


app.UseStaticFiles(); // Enable static files ( html , css , js , images .. )  to be served
app.UseRouting(); // maps incoming requests to route handlers

app.UseCors("CorsPolicy"); // allowing or blocking  requests from different origins ( cross-origin requests )

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
