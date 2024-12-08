var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
// In development, WebApplication automatically adds the developer exception page middleware.

if (!app.Environment.IsDevelopment())
	// Configures a different pipeline when not running in development
	app.UseExceptionHandler("/error");
// The ExceptionHandlerMiddleware won’t leak sensitive details when running in production.
// additional middleware configuration
app.MapGet("/error", () => "Sorry, an error occurred");
// This error endpoint will be executed when an exception is handled.

app.Run();