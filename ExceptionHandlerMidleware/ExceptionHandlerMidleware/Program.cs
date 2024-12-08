var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment()) app.UseExceptionHandler("/error");

app.MapGet("/", () => BadService.GetValues());
app.MapGet("/error", () => "Sorry, there was a problem processing your request");

app.Run();

internal class BadService
{
	public static string? GetValues()
	{
		throw new Exception("Oops, something bad happened!");
	}
}