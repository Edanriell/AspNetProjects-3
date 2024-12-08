using System.Collections.Concurrent;
using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var _fruit = new ConcurrentDictionary<string, Fruit>();
// Uses a concurrent dictionary to make the API thread-safe

app.MapGet("/fruit", () => _fruit);

app.MapGet("/fruit/{id}", (string id) =>
	_fruit.TryGetValue(id, out var fruit)
		// Tries to get the fruit from the dictionary. If the ID exists in the dictionary, this returns true 
		? TypedResults.Ok(fruit)
		// and we return a 200 OK response, serializing the fruit in the body as JSON.
		: Results.NotFound());
// If the ID doesn’t exist, returns a 404 Not Found response

app.MapPost("/fruit/{id}", (string id, Fruit fruit) =>
	_fruit.TryAdd(id, fruit)
		// Tries to add the fruit to the dictionary. If the ID hasn’t been added yet. this returns true
		? TypedResults.Created($"/fruit/{id}", fruit)
		// and we return a 201 response with a JSON body and set the Location header to the given path.
		: Results.BadRequest(new
							 { id = "A fruit with this id already exists" }));
// If the ID already exists, returns a 400 Bad Request response with an error message

app.MapPut("/fruit/{id}", (string id, Fruit fruit) =>
{
	_fruit[id] = fruit;
	return Results.NoContent();
	// After adding or replacing the fruit, returns a 204 No Content response
});

app.MapDelete("/fruit/{id}", (string id) =>
{
	_fruit.TryRemove(id, out _);
	return Results.NoContent();
	// After deleting the fruit, always returns a 204 No Content response
});

// Accesses the HttpResponse by including it as a parameter in your endpoint handler
app.MapGet("/teapot", (HttpResponse response) =>
{
	response.StatusCode = 418;
	// You can set the status code directly on the response.
	response.ContentType = MediaTypeNames.Text.Plain;
	// Defines the content type that will be sent in the response
	return response.WriteAsync("I'm a teapot!");
	// You can write data to the response stream manually
});

app.Run();

internal record Fruit(string Name, int stock);