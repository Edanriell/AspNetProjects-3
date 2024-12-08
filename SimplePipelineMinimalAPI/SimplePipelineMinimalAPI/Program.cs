var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDeveloperExceptionPage();
// This call isn’t strictly necessary, as it’s already added by WebApplication by default.
app.UseStaticFiles();
app.UseRouting();
// Adds the RoutingMiddleware to the pipeline

app.MapGet("/", () => "Hello World!");
// Defines an endpoint for the application

app.Run();