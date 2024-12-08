using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var _fruit = new ConcurrentDictionary<string, Fruit>();

// WithoutRouteGroups();
UsingRouteGroups();

app.Run();

void WithoutRouteGroups()
{
	// Same /fruit prefix 👇
	app.MapGet("/fruit", () => _fruit);

	// Same /fruit prefix 👇
	app.MapGet("/fruit/{id}", (string id) =>
			_fruit.TryGetValue(id, out var fruit)
				? TypedResults.Ok(fruit)
				: Results.Problem(statusCode: 404))
	   .AddEndpointFilterFactory(ValidationHelper.ValidateIdFactory); // 👈 Duplicated on every endpoint

	// Same /fruit prefix 👇
	app.MapPost("/fruit/{id}", (Fruit fruit, string id) =>
			_fruit.TryAdd(id, fruit)
				? TypedResults.Created($"/fruit/{id}", fruit)
				: Results.ValidationProblem(new Dictionary<string, string[]>
											{
												{ "id", new[] { "A fruit with this id already exists" } }
											}))
	   .AddEndpointFilterFactory(ValidationHelper.ValidateIdFactory); // 👈 Duplicated on every endpoint

	// Same /fruit prefix 👇
	app.MapPut("/fruit/{id}", (string id, Fruit fruit) =>
		{
			_fruit[id] = fruit;
			return Results.NoContent();
		})
	   .AddEndpointFilterFactory(ValidationHelper.ValidateIdFactory); // 👈 Duplicated on every endpoint

	// Same /fruit prefix 👇
	app.MapDelete("/fruit/{id}", (string id) =>
		{
			_fruit.TryRemove(id, out _);
			return Results.NoContent();
		})
	   .AddEndpointFilterFactory(ValidationHelper.ValidateIdFactory); // 👈 Duplicated on every endpoint
}

void UsingRouteGroups()
{
	// Fruit prefix only defined in one place        👇
	var fruitApi = app.MapGroup("/fruit");
	// Creates a route group by calling MapGroup and providing a prefix

	fruitApi.MapGet("/", () => _fruit);
	// Endpoints defined on the route group will have the group prefix prepended to the route.

	// Nested group. "/" prefix means still only using "/fruit" prefix 👇
	var fruitApiWithValidation = fruitApi.MapGroup("/")
	   .AddEndpointFilterFactory(ValidationHelper.ValidateIdFactory); // 👈 Validation filter only declared once
	// You can create nested route groups with multiple prefixes.

	fruitApiWithValidation.MapGet("/{id}", (string id) =>
		_fruit.TryGetValue(id, out var fruit)
			? TypedResults.Ok(fruit)
			: Results.Problem(statusCode: 404));

	fruitApiWithValidation.MapPost("/{id}", (Fruit fruit, string id) =>
		_fruit.TryAdd(id, fruit)
			? TypedResults.Created($"/fruit/{id}", fruit)
			: Results.ValidationProblem(new Dictionary<string, string[]>
										{
											{ "id", new[] { "A fruit with this id already exists" } }
										}));

	fruitApiWithValidation.MapPut("/{id}", (string id, Fruit fruit) =>
	{
		_fruit[id] = fruit;
		return Results.NoContent();
	});

	fruitApiWithValidation.MapDelete("/{id}", (string id) =>
	{
		_fruit.TryRemove(id, out _);
		return Results.NoContent();
	});
	// You can add filters to the route group and the filter will be applied to all the endpoints defined on the route group.
}

internal record Fruit(string Name, int Stock);

internal class ValidationHelper
{
	internal static EndpointFilterDelegate ValidateIdFactory(
		EndpointFilterFactoryContext context, EndpointFilterDelegate next)
	{
		var parameters = context.MethodInfo.GetParameters();
		int? idPosition = null;
		for (var i = 0; i < parameters.Length; i++)
			if (parameters[i].Name == "id" &&
				parameters[i].ParameterType == typeof(string))
			{
				idPosition = i;
				break;
			}

		if (!idPosition.HasValue) return next;

		return async invocationContext =>
		{
			var id = invocationContext.GetArgument<string>(idPosition.Value);
			if (string.IsNullOrEmpty(id) || !id.StartsWith('f'))
				return Results.ValidationProblem(new Dictionary<string, string[]>
												 {
													 { "id", new[] { "Invalid format. Id must start with 'f'" } }
												 });
			return await next(invocationContext);
		};
	}
}