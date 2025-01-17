using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

var _fruit = new ConcurrentDictionary<string, Fruit>();

app.MapGet("/fruit/{id}", (string id) =>
		_fruit.TryGetValue(id, out var fruit)
			? TypedResults.Ok(fruit)
			: Results.Problem(statusCode: 404))
   .WithName("GetFruit") // Added name
   .WithTags("fruit")
   .Produces<Fruit>()
   .ProducesProblem(404);

app.MapPost("/fruit/{id}", (string id, Fruit fruit) =>
		_fruit.TryAdd(id, fruit)
			? TypedResults.Created($"/fruit/{id}", fruit)
			: Results.ValidationProblem(new Dictionary<string, string[]>
										{
											{ "id", new[] { "A fruit with this id already exists" } }
										}))
   .WithName("CreateFruit") // Added name
   .WithTags("fruit")
   .Produces<Fruit>(201)
   .ProducesValidationProblem();


app.Run();

internal record Fruit(string Name, int Stock);