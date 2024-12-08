using ATypicalRazorPage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
// Registers the required services to use the Razor Pages feature
builder.Services.AddSingleton<ToDoService>();

// Conditionally adds middleware depending on the runtime environment
//if (!app.Environment.IsDevelopment())
//{
//	app.UseExceptionHandler("/Error");
//	app.UseHsts()
//} 

var app = builder.Build();

//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();
//app.UseAuthorization();
// Additional middleware can be added to the middleware pipeline.

app.MapRazorPages();
// Registers each Razor Page as an endpoint in your application

// When viewing in the browser, the default page ('/') is requested
// But as that page does not exist, redirect to a page that does!
// You can also try /category/Long 
app.Map("/", () => Results.Redirect("/category/Simple"));
app.Run();