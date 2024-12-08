var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWelcomePage("/");
// WelcomePageMiddleware handles all requests to the “/ ” path and returns a sample HTML response
app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseRouting();

app.MapGet("/", () => "Hello World!");
// Requests to “/ ” will never reach the endpoint middleware, so this endpoint won’t be called.

app.Run();