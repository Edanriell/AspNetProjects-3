using System.Collections.Concurrent;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Adds the endpoint-discovery features of ASP.NET Core that Swashbuckle requires
builder.Services.AddEndpointsApiExplorer();
// Adds the Swashbuckle services required for creating OpenApi Documents
// builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(x =>
	x.SwaggerDoc("v1", new OpenApiInfo
					   {
						   Title = "Fruitify",
						   Description = "An API for interacting with fruit stock",
						   Version = "1.0"
					   }));

var app = builder.Build();

// Adds middleware to expose the OpenAPI document for your app
app.UseSwagger();
app.UseSwaggerUI();
// Adds middleware that serves the Swagger UI

var _fruit = new ConcurrentDictionary<string, Fruit>();

app.MapGet("/fruit/{id}", (string id) =>
		_fruit.TryGetValue(id, out var fruit)
			? TypedResults.Ok(fruit)
			: Results.Problem(statusCode: 404))
   .WithTags("fruit")
	// Adding a tag groups the endpoints in Swagger UI. Each endpoint can have multiple tags.
   .Produces<Fruit>()
	// The endpoint can return a Fruit object. When not specified, a 200 response is assumed.
   .ProducesProblem(404);
// If the id isn’t found, the endpoint returns a 404 Problem Details response.

app.MapPost("/fruit/{id}", (string id, Fruit fruit) =>
		_fruit.TryAdd(id, fruit)
			? TypedResults.Created($"/fruit/{id}", fruit)
			: Results.ValidationProblem(new Dictionary<string, string[]>
										{
											{ "id", new[] { "A fruit with this id already exists" } }
										}))
   .WithTags("fruit")
	// Adding a tag groups the endpoints in Swagger UI. Each endpoint can have multiple tags.
   .Produces<Fruit>(201)
	// This endpoint also returns a Fruit object but uses a 201 response instead of 200.
   .ProducesValidationProblem();
// If the ID already exists, it returns a 400 Problem Details response with validation errors.

app.Run();

internal record Fruit(string Name, int Stock);