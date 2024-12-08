var builder = WebApplication.CreateBuilder(args);
// The default builder sets ValidateScopes to validate only in development environments.

// You can override the validation check with the UseDefaultServiceProvider extension.
builder.Host.UseDefaultServiceProvider(o =>
{
	// Set the below values to true to always validate scopes,
	// These are only set to false here to demonstrate the (errorenous)
	// behaviour of captured dependencies on the /captured endpoint

	// Setting this to true will validate scopes in all environments, which has performance implications.
	o.ValidateScopes = false;
	o.ValidateOnBuild = false;
	// ValidateOnBuild checks that every registered service has all its dependencies registered.

	// The default definition (commented out below) only
	// validates in dev environments (for performance reasons)
	//o.ValidateScopes = builder.Environment.IsDevelopment();
	//o.ValidateOnBuild = builder.Environment.IsDevelopment();
});


builder.Services.AddTransient<TransientRepository>();
builder.Services.AddTransient<TransientDataContext>();

builder.Services.AddScoped<ScopedRepository>();
builder.Services.AddScoped<ScopedDataContext>();

builder.Services.AddSingleton<SingletonRepository>();
builder.Services.AddSingleton<SingletonDataContext>();
builder.Services.AddSingleton<CapturingRepository>();

// builder.Services.AddScoped<DataContext>();
// DataContext is registered as scoped, so it shouldn’t be resolved directly from app.Services

// var settings = app.Services.GetRequiredService<EmailServerSettings>();
// It’s important, however, that you resolve only singleton services this way.

var app = builder.Build();

// Creates an IServiceScope
await using (var scope = app.Services.CreateAsyncScope())
{
	// Resolves the scoped service from the scoped container
	var dbContext = scope.ServiceProvider.GetRequiredService<ScopedDataContext>();
	Console.WriteLine($"Retrieved scope with RowCount: {dbContext.RowCount}");
}
// When the IServiceScope is disposed, all resolved services are also disposed.

app.MapGet("/", () => @"Visit /singleton, /scoped, /transient, or /captured.

Refresh the page a few times to see the relationship between the 
DataContext values and the Repository values, and how these
change when refreshing the page
");

List<string> _singletons = new();
List<string> _scopeds = new();
List<string> _transients = new();
List<string> _captured = new();

app.MapGet("/singleton", Singleton);
app.MapGet("/scoped", Scoped);
app.MapGet("/transient", Transient);
app.MapGet("/captured", Captured);


string Singleton(SingletonDataContext db, SingletonRepository repo)
{
	return RowCounts(db, repo, _singletons);
}

string Scoped(ScopedDataContext db, ScopedRepository repo)
{
	return RowCounts(db, repo, _scopeds);
}

string Transient(TransientDataContext db, TransientRepository repo)
{
	return RowCounts(db, repo, _transients);
}

string Captured(ScopedDataContext db, CapturingRepository repo)
{
	return RowCounts(db, repo, _captured);
}

// DataContext and Repository are created using DI.
static string RowCounts(DataContext db, Repository repo, List<string> previous)
{
	// When invoked, the page handler retrieves and records RowCount from both dependencies.
	// The counts are returned in the response.
	var counts = $"{db.GetType().Name}: {db.RowCount:000,000,000}, {repo.GetType().Name}: {repo.RowCount:000,000,000}";

	var result = $@"
Current values:
{counts}

Previous values:
{string.Join(Environment.NewLine, previous)}";

	previous.Insert(0, counts);
	return result;
}

app.Run();

internal class DataContext
{
	// The class will return the same random number for its lifetime
	//  The property is read-only, so it always returns the same value.
	public int RowCount { get; } = Random.Shared.Next(1, 1_000_000_000);
	// Generates a random number between 1 and 1,000,000,000
}

internal class TransientDataContext : DataContext
{
}

internal class ScopedDataContext : DataContext
{
}

internal class SingletonDataContext : DataContext
{
}

internal class Repository
{
	private readonly DataContext _dataContext;

	public Repository(DataContext dataContext)
	{
		_dataContext = dataContext;
	}
	// An instance of DataContext is provided using DI.

	public int RowCount => _dataContext.RowCount;
	// RowCount returns the same value as the current instance of DataContext.
}

internal class ScopedRepository : Repository
{
	public ScopedRepository(ScopedDataContext generator) : base(generator)
	{
	}
}

internal class TransientRepository : Repository
{
	public TransientRepository(TransientDataContext generator) : base(generator)
	{
	}
}

internal class SingletonRepository : Repository
{
	public SingletonRepository(SingletonDataContext generator) : base(generator)
	{
	}
}

internal class CapturingRepository : Repository
{
	public CapturingRepository(ScopedDataContext generator) : base(generator)
	{
	}
}