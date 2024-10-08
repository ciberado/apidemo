using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationInsightsTelemetry();

TelemetryConfiguration config = TelemetryConfiguration.CreateDefault();
// config.InstrumentationKey = "INSTRUMENTATION-KEY-HERE";
QuickPulseTelemetryProcessor quickPulseProcessor = null;
config.DefaultTelemetrySink.TelemetryProcessorChainBuilder
    .Use((next) =>
    {
        quickPulseProcessor = new QuickPulseTelemetryProcessor(next);
        return quickPulseProcessor;
    })
    .Build();

var quickPulseModule = new QuickPulseTelemetryModule();
//quickPulseModule.AuthenticationApiKey = "YOUR-API-KEY-HERE";
quickPulseModule.Initialize(config);
quickPulseModule.RegisterTelemetryProcessor(quickPulseProcessor);
TelemetryClient telemetryClient = new TelemetryClient(config);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStatusCodePages(context => {
    var request = context.HttpContext.Request;
    var response = context.HttpContext.Response;

    if (response.StatusCode == 404)
    {
        response.WriteAsync("Resource not found.");
    }

    return Task.CompletedTask;
});

app.Logger.LogInformation("Adding Routes.");


app.MapGet("/", () =>
{
    app.Logger.LogInformation("All systems running.");
})
.WithName("GetHealth")
.WithOpenApi();


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/fail", () =>
{
    app.Logger.LogWarning("The invocation of this route will always generate an error.");

    throw new Exception("buuuuh.");
})
.WithName("ThrowError")
.WithOpenApi();

app.MapGet("/weatherforecast", () =>
{

    app.Logger.LogInformation("Retreiving weather (nah, it is faked).");
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
