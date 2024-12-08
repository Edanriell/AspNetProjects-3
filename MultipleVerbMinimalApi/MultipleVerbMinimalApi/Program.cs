var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/fruit", () => Fruit.All);
// Lambda expressions are the simplest but least descriptive way to create a handler.

var getFruit = (string id) => Fruit.All[id];
app.MapGet("/fruit/{id}", getFruit);
// Storing the lambda expression as a variable means you can name it—getFruit in this case.

app.MapPost("/fruit/{id}", Handlers.AddFruit);
// Handlers can be static methods in any class

Handlers handlers = new();
app.MapPut("/fruit/{id}", handlers.ReplaceFruit);
// Handlers can also be instance methods.

app.MapDelete("/fruit/{id}", DeleteFruit);
// You can also use local functions, introduced in C# 7.0, as handler methods.

app.Run();

void DeleteFruit(string id)
{
	Fruit.All.Remove(id);
}

internal record Fruit(string Name, int Stock)
{
	public static readonly Dictionary<string, Fruit> All = new();
}

internal class Handlers
{
	public void ReplaceFruit(string id, Fruit fruit)
	{
		Fruit.All[id] = fruit;
	}
	// Handlers can also be instance methods.

	public static void AddFruit(string id, Fruit fruit)
	{
		// Converts the response to a JsonObject
		Fruit.All.Add(id, fruit);
	}
}