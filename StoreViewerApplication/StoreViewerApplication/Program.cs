using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MapSettings>(builder.Configuration.GetSection("MapSettings"));
builder.Services.Configure<AppDisplaySettings>(builder.Configuration.GetSection("AppDisplaySettings"));
builder.Services.Configure<List<Store>>(builder.Configuration.GetSection("Stores"));

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/stores", (IOptions<List<Store>> opts) => opts.Value);
app.MapGet("/map-settings", (IOptions<MapSettings> opts) => opts.Value);
app.MapGet("/display-settings", (IOptions<AppDisplaySettings> opts) => opts.Value);

//// Binds the MapSettings section to the POCO options class MapSettings
//builder.Services.Configure<MapSettings>(
//	builder.Configuration.GetSection("MapSettings"));
//// Binds the AppDisplaySettings section to the POCO options class AppDisplaySettings
//builder.Services.Configure<AppDisplaySettings>(builder.Configuration.GetSection("AppDisplaySettings"));
//
//// Clears the providers configured by default in WebApplicationBuilder
//builder.Configuration.Sources.Clear();
//
//if (builder.Environment.IsDevelopment()) builder.Configuration.AddUserSecrets<Program>();
//
////  Loads configuration from a different JSON configuration file before the appsettings.json file
//builder.Configuration
//   .AddJsonFile("sharedSettings.json", true);
//
//// Adds a JSON configuration provider, providing the name of the configuration file
//builder.Configuration.AddJsonFile("appsettings.json", true, true);
//
//// Adds the machine’s environment variables as a configuration provider
//builder.Configuration.AddEnvironmentVariables();
//
//var app = builder.Build();
//
//// app.MapGet("/", (IConfiguration config) => config.AsEnumerable());
//app.MapGet("/", () => app.Configuration.AsEnumerable());
//
app.MapGet("/display-settings3",
	(IOptions<AppDisplaySettings> options) =>
		// You can inject a strongly typed options class using the IOptions<> wrapper interface.
	{
		var settings = options.Value;
		// The Value property exposes the POCO settings object.
		var title = settings.Title;
		// The settings object contains properties that are bound to configuration values at runtime.
		var showCopyright = settings.ShowCopyright;
		// The binder can also convert string values directly to built-in types.
		return new { title, showCopyright };
	});

app.MapGet("/display-settings2",
	(IOptionsSnapshot<AppDisplaySettings> options) =>
		// IOptionsSnapshot<T> updates automatically if the underlying configuration values change.
	{
		var settings = options.Value;
		//  The Value property exposes the POCO settings object, the same as for IOptions<T>.

		return new
			   {
				   title = settings.Title,
				   showCopyright = settings.ShowCopyright
			   };
		// The settings match the configuration values at that point in time instead of at first run.
	});
// Returns all the configuration key-value pairs for display purposes

// Don't favour this approach
app.MapGet("/display-settings-alt", (IConfiguration config) => new
															   {
																   title = config["AppDisplaySettings:Title"],
																   showCopyright = bool.Parse(
																	   config["AppDisplaySettings:ShowCopyright"]!)
															   });

app.Run();

internal class MapSettings
{
	public string GoogleMapsApiKey { get; set; }
	public int DefaultZoomLevel { get; set; }
	public Location DefaultLocation { get; set; }
}

internal class Location
{
	public decimal Latitude { get; set; }
	public decimal Longitude { get; set; }
}

internal class Store
{
	public string Name { get; set; }
	public Location Location { get; set; }
}

internal class AppDisplaySettings
{
	public string Title { get; set; }
	public bool ShowCopyright { get; set; }
}