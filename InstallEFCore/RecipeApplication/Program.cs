using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Update User Secrets/appsettings.json with the connection string for your database
//
// e.g. for SQLite:
// Data Source=recipe_app.db;
//
// or for LocalDB:
// (localdb)\\mssqllocaldb;Database=RecipeApplication;Trusted_Connection=True;MultipleActiveResultSets=true

var connString = builder.Configuration.GetConnectionString("DefaultConnection");
// The connection string is taken from configuration, from the ConnectionStrings section.

// Registers your app’s DbContext by using it as the generic parameter
builder.Services.AddDbContext<AppDbContext>(
		// If you're using SQL Server or Local DB, use the following line:
		options => options.UseSqlServer(connString!)
		// If you're using SQLite, use the following line instead:
		//options => options.UseSqlite(connString)
	);
// Specifies the database provider in the customization options for the DbContext.

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();


public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}
	// The constructor options object, containing details such as the connection string

	public DbSet<Recipe> Recipes { get; set; }
	// You’ll use the Recipes property to query the database.
}

public class Recipe
{
	public int RecipeId { get; set; }
	public required string Name { get; set; }
	public TimeSpan TimeToCook { get; set; }
	public bool IsDeleted { get; set; }
	public required string Method { get; set; }

	public required ICollection<Ingredient> Ingredients { get; set; }
	// A Recipe can have many Ingredients, represented by ICollection.
}

public class Ingredient
{
	public int IngredientId { get; set; }
	public int RecipeId { get; set; }
	public required string Name { get; set; }
	public decimal Quantity { get; set; }
	public required string Unit { get; set; }
}