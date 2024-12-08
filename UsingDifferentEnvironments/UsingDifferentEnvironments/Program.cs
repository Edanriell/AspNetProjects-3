using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// The current IHostEnvironment is available on WebApplicationBuilder.
IHostEnvironment env = builder.Environment;

builder.Configuration.Sources.Clear();
builder.Configuration.AddJsonFile("appsettings.json", false, true)
	// It’s common to make the base appsettings.json compulsory.
   .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);
// Adds an optional environment specific JSON file where the filename varies with the environment

// Extension methods make checking the environment simple and explicit.
if (env.IsDevelopment()) builder.Configuration.AddUserSecrets<Program>();
// In Staging and Production, the User Secrets provider won’t be used.

builder.Services.AddProblemDetails();
// Adds the problem details service to the DI container for use by the ExceptionHandlerMiddleware

builder.Services.Configure<MyValues>(
	builder.Configuration.GetSection(nameof(MyValues)));

builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.WriteIndented = true);

var app = builder.Build();

if (!builder.Environment.IsDevelopment()) app.UseExceptionHandler();
// When not in development, the pipeline uses ExceptionHandlerMiddleware.

app.MapGet("/", () => "Hello World!");
app.MapGet("/", (IOptions<MyValues> opts) => opts.Value);

app.Run();

public class MyValues
{
	public string SingleValue { get; set; }
	public List<string> List { get; set; }
}