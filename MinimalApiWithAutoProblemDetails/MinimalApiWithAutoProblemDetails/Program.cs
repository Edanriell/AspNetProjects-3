using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
// Adds the IProblemDetailsService implementation
var app = builder.Build();

if (!app.Environment.IsDevelopment()) app.UseExceptionHandler();
// Configures the ExceptionHandlerMiddleware without a path so that it uses the IProblemDetailsService
var _fruit = new ConcurrentDictionary<string, Fruit>();

app.UseStatusCodePages();
// Adds the StatusCodePagesMiddleware

app.MapGet("/", () => Results.NotFound());
// The StatusCodePagesMiddleware automatically adds a Problem Details body to the 404 response.

//app.MapGet("/", void () => throw new Exception());
// Throws an exception to demonstrate the behavior

app.MapGet("/fruit", () => _fruit);

app.MapGet("/fruit/{id}", (string id) =>
	_fruit.TryGetValue(id, out var fruit)
		? TypedResults.Ok(fruit)
		: Results.Problem(statusCode: 404));
// Returns a Problem Details object with a 404 status code

app.MapPost("/fruit/{id}", (string id, Fruit fruit) =>
	_fruit.TryAdd(id, fruit)
		? TypedResults.Created($"/fruit/{id}", fruit)
		: Results.ValidationProblem(new Dictionary<string, string[]>
									{
										{ "id", new[] { "A fruit with this id already exists" } }
									}));
// Returns a Problem Details object with a 400 status code and includes the validation errors

app.MapPut("/fruit/{id}", (string id, Fruit fruit) =>
{
	_fruit[id] = fruit;
	return Results.NoContent();
});

app.MapDelete("/fruit/{id}", (string id) =>
{
	_fruit.TryRemove(id, out _);
	return Results.NoContent();
});

app.Run();

internal record Fruit(string Name, int Stock);