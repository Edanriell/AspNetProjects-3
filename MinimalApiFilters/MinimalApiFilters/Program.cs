using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var _fruit = new ConcurrentDictionary<string, Fruit>();

app.MapGet("/fruit", () => _fruit);

//app.MapGet("/fruit/{id}", (string id) =>
//{
//	if (string.IsNullOrEmpty(id) || !id.StartsWith('f'))
//		return Results.ValidationProblem(new Dictionary<string, string[]>
//										 {
//											 { "id", new[] { "Invalid format. Id must start with 'f'" } }
//										 });
//	// Adds extra validation that the provided id has the required format
//	return _fruit.TryGetValue(id, out var fruit)
//			   ? TypedResults.Ok(fruit)
//			   : Results.Problem(statusCode: 404);
//});

//app.MapGet("/fruit/{id}", (string id) =>
//		_fruit.TryGetValue(id, out var fruit)
//			? TypedResults.Ok(fruit)
//			: Results.Problem(statusCode: 404))
//   .AddEndpointFilter(ValidationHelper.ValidateId)
//	// Adds the filter to the endpoint using AddEndpointFilter
//	// Adds a new filter using a lambda function
//   .AddEndpointFilter(async (context, next) =>
//	{
//		app.Logger.LogInformation("Executing filter...");
//		// Logs a message before executing the rest of the pipeline
//		var result = await next(context);
//		// Executes the remainder of the pipeline and the endpoint handler
//		app.Logger.LogInformation($"Handler result: {result}");
//		// Logs the result returned by the rest of the pipeline
//		return result;
//		// Returns the result unmodified
//	});

//app.MapGet("/fruit/{id}", (string id) =>
//		_fruit.TryGetValue(id, out var fruit)
//			? TypedResults.Ok(fruit)
//			: Results.Problem(statusCode: 404))
//   .AddEndpointFilterFactory(ValidationHelper.ValidateIdFactory);
//// The filter factory can handle endpoints with different method signatures

app.MapGet("/fruit/{id}", (string id) =>
		_fruit.TryGetValue(id, out var fruit)
			? TypedResults.Ok(fruit)
			: Results.Problem(statusCode: 404))
   .AddEndpointFilter<IdValidationFilter>();
// Adds the filter using the generic AddEndpointFilter method

app.MapPost("/fruit/{id}", (Fruit fruit, string id) =>
		_fruit.TryAdd(id, fruit)
			? TypedResults.Created($"/fruit/{id}", fruit)
			: Results.ValidationProblem(new Dictionary<string, string[]>
										{
											{ "id", new[] { "A fruit with this id already exists" } }
										}))
   .AddEndpointFilterFactory(ValidationHelper.ValidateIdFactory);
// The filter factory can handle endpoints with different method signatures

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

//internal class ValidationHelper
//{
//	// The method must return a ValueTask
//	internal static async ValueTask<object?> ValidateId(
//			EndpointFilterInvocationContext context,
//			// context exposes the endpoint method arguments and the HttpContext.
//			EndpointFilterDelegate next)
//		// next represents the filter method (or endpoint) that will be called next.
//	{
//		var id = context.GetArgument<string>(0);
//		// You can retrieve the method arguments from the context.
//		if (string.IsNullOrEmpty(id) || !id.StartsWith('f'))
//			return Results.ValidationProblem(
//				new Dictionary<string, string[]>
//				{
//					{ "id", new[] { "Invalid format. Id must start with 'f'" } }
//				});
//		return await next(context);
//		// Calling next executes the remaining filters in the pipeline.
//	}
//}

internal class ValidationHelper
{
	internal static EndpointFilterDelegate ValidateIdFactory(
		EndpointFilterFactoryContext context,
		// The context parameter provides details about the endpoint handler method.
		EndpointFilterDelegate next)
	{
		var parameters =
			context.MethodInfo.GetParameters();
		// GetParameters() provides details about the parameters of the handler being called.
		int? idPosition = null;
		for (var i = 0; i < parameters.Length; i++)
			if (parameters[i].Name == "id" &&
				parameters[i].ParameterType == typeof(string))
			{
				idPosition = i;
				break;
			}
		// Loops through the parameters to find the string id parameter and record its position

		if (!idPosition.HasValue) return next;
		// If the id parameter isn’t not found, doesn’t add a filter, but returns the remainder of the pipeline
		return async invocationContext =>
			// If the id parameter exists, returns a filter function (the filter executed for the endpoint)
		{
			var id = invocationContext
			   .GetArgument<string>(idPosition.Value);
			if (string.IsNullOrEmpty(id) || !id.StartsWith('f'))
				// If the id isn’t valid, returns a Problem Details result
				return Results.ValidationProblem(
					new Dictionary<string, string[]>
					{ { "id", new[] { "Id must start with 'f'" } } });
			// If the id isn’t valid, returns a Problem Details result

			return await next(invocationContext);
			// If the id is valid, executes the next filter in the pipeline
		};
	}
}

internal class IdValidationFilter : IEndpointFilter
// The filter must implement IEndpointFilter
{
	public async ValueTask<object?> InvokeAsync(
		EndpointFilterInvocationContext context,
		EndpointFilterDelegate next)
	{
		// which requires implementing a single method
		var id = context.GetArgument<string>(0);
		if (string.IsNullOrEmpty(id) || !id.StartsWith('f'))
			return Results.ValidationProblem(
				new Dictionary<string, string[]>
				{
					{ "id", new[] { "Invalid format. Id must start with 'f'" } }
				});
		return await next(context);
	}
}

internal record Fruit(string Name, int Stock);