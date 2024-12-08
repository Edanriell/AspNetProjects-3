var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWelcomePage();
// The only custom middleware in the pipeline

app.Run();
// Runs the application to handle requests