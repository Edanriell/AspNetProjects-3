using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument();

var app = builder.Build();

app.UseOpenApi();
app.UseSwaggerUi();

var _fruit = new ConcurrentDictionary<string, Fruit>();

app.MapGet("/fruit/{id}", (string id) =>
	_fruit.TryGetValue(id, out var fruit)
		? TypedResults.Ok(fruit)
		: Results.Problem(statusCode: 404));

app.MapPost("/fruit/{id}", (string id, Fruit fruit) =>
	_fruit.TryAdd(id, fruit)
		? TypedResults.Created($"/fruit/{id}", fruit)
		: Results.ValidationProblem(new Dictionary<string, string[]>
									{
										{ "id", new[] { "A fruit with this id already exists" } }
									}));

app.Run();

internal record Fruit(string Name, int Stock);