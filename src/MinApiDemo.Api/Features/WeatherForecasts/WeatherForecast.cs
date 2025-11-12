namespace MinApiDemo.Api.Features.WeatherForecasts;

/// <summary>
/// 
/// </summary>
/// <param name="Date"></param>
/// <param name="TemperatureC"></param>
/// <param name="Summary"></param>
public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    /// <summary>
    /// 
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
