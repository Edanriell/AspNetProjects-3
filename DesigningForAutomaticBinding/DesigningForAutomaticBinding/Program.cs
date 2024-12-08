using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Creates a new instance of the MapSettings object
var mapSettings = new MapSettings();
// Binds the MapSettings section in IConfiguration to the settings object
builder.Configuration.GetSection("MapSettings").Bind(mapSettings);
// Registers the settings object as a singleton
builder.Services.AddSingleton(mapSettings);

// bind the IOptions options
var section = builder.Configuration.GetSection("AllOptions");
builder.Services.Configure<BindableOptions>(section);
builder.Services.Configure<UnbindableOptions>(section);

// format minimal APIs for legibility
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.WriteIndented = true);

// manually bind and register the settings
// Allows you to inject TestOptions directly into services, 
// instead of using IOptions<TestOptions>
//var settings = new BindableOptions();
//section.Bind(settings);
//builder.Services.AddSingleton(settings);

// Configures the IOptions as normal
builder.Services.Configure<MapSettings>(
	builder.Configuration.GetSection("MapSettings"));
// Registers the MapSettings object in DI by delegating to the IOptions registration
builder.Services.AddSingleton(provider => provider.GetRequiredService<IOptions<MapSettings>>().Value);

var app = builder.Build();

// Injects the MapSettings object directly using DI
app.MapGet("/", (MapSettings mapSettings) => mapSettings);

app.MapGet("/bindable", (IOptions<BindableOptions> options) => options.Value);
app.MapGet("/unbindable", (IOptions<UnbindableOptions> options) => options.Value);

app.MapGet("/", () => @"Try visiting /bindable and /unbindable.
The options in /bindable have successfully bound to the configuration values
The options in /unbindable have their default values as they failed to bind");

app.Run();

public class BindableOptions
{
	//Can Bind
	public string String { get; set; }
	public int Integer { get; set; }
	public SubClass Object { get; set; }
	public SubClass ReadOnly { get; } = new();
	public Dictionary<string, SubClass> Dictionary { get; set; }
	public List<SubClass> List { get; set; }
	public IDictionary<string, SubClass> IDictionary { get; set; }
	public IEnumerable<SubClass> IEnumerable { get; set; }
	public ICollection<SubClass> ReadOnlyCollection { get; } = new List<SubClass>();

	public class SubClass
	{
		public string Value { get; set; }
	}
}

public class UnbindableOptions
{
	private readonly List<SubClass> _indexerList = new();

	public SubClass _setOnly;

	//Can't bind
	internal string NotPublic { get; set; }

	public SubClass SetOnly
	{
		set => _setOnly = value;
	}

	public SubClass NullReadOnly { get; }
	public SubClass NullPrivateSetter { get; private set; }
	public Dictionary<int, SubClass> DictionaryWithNonStringKeys { get; set; }
	public IEnumerable<SubClass> NullIEnumerable { get; }
	public IEnumerable<SubClass> ReadOnlyEnumerable { get; } = new List<SubClass>();
	public Dictionary<int, SubClass> IntegerKeys { get; set; }

	public SubClass this[int i]
	{
		get => _indexerList[i];
		set => _indexerList[i] = value;
	}

	public class SubClass
	{
		public string Value { get; set; }
	}
}

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