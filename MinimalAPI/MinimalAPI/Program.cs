using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);
// Creates a WebApplicationBuilder using the CreateBuilder method

builder.Services.AddHttpLogging(opts =>
	opts.LoggingFields = HttpLoggingFields.RequestProperties);
// You can customize features by adding or customizing the services of the application

builder.Logging.AddFilter(
	"Microsoft.AspNetCore.HttpLogging", LogLevel.Information);
// Ensures that logs added by the HTTP logging middleware are visible in the log output

var app = builder.Build();
// Builds and returns an instance of WebApplication from the WebApplicationBuilder

// You can add middleware conditionally, depending on the runtime environment.
if (app.Environment.IsDevelopment()) app.UseHttpLogging();
// The HTTP logging middleware logs each request to your application in the log output.
app.MapGet("/", () => "Hello World!");
app.MapGet("/person", () => new Person("Lauris", "Lock"));
// Creates a new endpoint that returns the C# object serialized as JSON

app.Run();
// Runs the WebApplication to start listening for requests and generating responses

public record Person(string FirstName, string LastName);
// Creates a record type