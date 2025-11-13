using Carter;

namespace MinApiDemo.Api.Features.WeatherForecasts;

/// <summary>
/// 
/// </summary>
public class WeatherForecastEndpoint : ICarterModule
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder routeGroupBuilder =
            app.MapGroup("/api/weatherforcasts");

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        routeGroupBuilder.MapGet("/current", () =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();

            return TypedResults.Ok(forecast);
        })        
        .WithTags("Weather Forecast")
        .WithName("GetWeatherForecast")
        .WithSummary("Gets the Current Weather Forecasts")
        .Produces<WeatherForecast[]>(StatusCodes.Status200OK);
    }    
}