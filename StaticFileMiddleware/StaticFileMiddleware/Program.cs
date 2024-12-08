var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles();
// Adds the StaticFileMiddleware to the pipeline

app.Run();