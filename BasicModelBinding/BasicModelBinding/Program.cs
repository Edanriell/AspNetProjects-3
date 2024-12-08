using Microsoft.AspNetCore.Mvc;

// All the [From*] attributes are in this namespace.

var builder = WebApplication.CreateBuilder(args);

// Optional customization of serialization
// Make sure to update your JSON posts if you change the naming policy
//builder.Services.ConfigureHttpJsonOptions(o =>
//{
//    o.SerializerOptions.AllowTrailingCommas = true;
//    o.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
//    o.SerializerOptions.PropertyNameCaseInsensitive = true;
//});
//OR
//builder.Services.Configure<JsonOptions>(o =>
//{
//    o.JsonSerializerOptions.AllowTrailingCommas = true;
//    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
//    o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
//});

var app = builder.Build();

// id will be bound to the route parameter value.
// The following requests will both have id = 123
// /products/123
// /products/123?id=456
app.MapGet("/products/{id?}", (int? id) => $"Received {id}");

// id will be bound to the querystring
// The following request will both have id = 456
// /products?id=456
app.MapGet("/products", (int id) => $"Received {id}");

// Specifically define where parameters will be bound from
// The following request will have id = 123, page = 2
// page size must be sent as a header value using e.g. PostMan
// /products/123/page?paged=2
app.MapGet("/products/{id}/paged",
	([FromRoute] int id,
	 // [FromRoute] forces the argument to bind to the route value.
	 [FromQuery] int page,
	 // [FromQuery] forces the argument to bind to the query string.
	 [FromHeader(Name = "PageSize")] int pageSize)
		=> $"Received id {id}, page {page}, pageSize {pageSize}");
// [FromHeader] binds the argument to the specified header.

// ProductId contains a TryParse method, so it is treated as a simple type
// and is bound to the route parameter
// /product/p123
// /product/p0
// /product/p953
app.MapGet("/product/{id}", (ProductId id) => $"Received {id}");
// ProductId automatically binds to route values as it implements TryParse.

// Product is not a simple type, so it is bound from the body
// Use postman to send a POST request containing the
// following JSON to /product:
// { "id": 1, "Name": "Shoes", "Stock": 12 }
app.MapPost("/product", (Product product) => $"Received {product}");
// Product is a complex type, so it’s bound to the JSON body of the request.

app.MapGet("/products/search",
	(int[] id) => $"Received {id.Length} ids");
// The array will bind to multiple instances of id in the query string.

// Simple types will be bound to multiple querystring values for GET requests
// /products/search?id=123&id=456
app.MapGet("/products/search",
	([FromQuery(Name = "id")] int[] ids) => $"Received {ids.Length} ids");

// Simple types will be bound from the request body for POST request
// Use postman to send a POST request containing the
// following JSON to /products/search:
// [123, 456]
app.MapPost("/products/search", (int[] ids) => $"Received {ids.Length} ids: {string.Join(", ", ids)}");

// Complex types will be bound from the request body for POST request
// Use postman to send a POST request containing the
// following JSON to /products:
// [{ "id": 1, "Name": "Shoes", "Stock": 12 },  { "id": 2, "Name": "Socks", "Stock": 3 }]
app.MapPost("/products",
	(Product[] products) => $"Received {products.Length} items: {string.Join(", ", products.Select(x => x))}");

// Use ? for optional parameters.
// id binds to the optional route value. If the value is not provided, id will be null
// /stock
// /stock/123
app.MapGet("/stock/{id?}", (int? id) => $"Received {id}");
// Uses a nullable simple type to indicate that the value is optional, so id is null when calling /stock

// Use ? for optional parameters.
// id binds to the optional query string value. If the value is not provided, id will be null
// /stock2
// /stock2?id=123
app.MapGet("/stock2", (int? id) => $"Received {id}");
//This example binds to the query string. Id will be null for the request /stock2.

// Use ? for optional parameters.
// product binds to the body. If there is no request body, or it contains the value "null"
// then product will be null
// Use postman to send a POST request to /stock:
app.MapPost("/stock", (Product? product) => $"Received {product}");
// A nullable complex type binds to the body if it’s available; otherwise, it’s null.

// Alternatively to optional values, you can use a default values.
// Note that you can't use default values with Lambdas, so using a local function instead
// id binds to the query string value. If the value is not provided, id will be 0
// /stock3
// /stock3?id=123
app.MapGet("/stock", StockWithDefaultValue);
// The local function StockWithDefaultValue is the endpoint handler.

// Accessing well-known types
app.MapGet("/well-known", httpContext => httpContext.Response.WriteAsync("Hello World!"));


// Using services
// The LinkGenerator is registered in the DI container so it can be used as a parameter
app.MapGet("/links",
		([FromServices] LinkGenerator links) => $"The Links API can be found at {links.GetPathByName("LinksApi")}")
   .WithName("LinksApi");

// The LinkGenerator can be used as a parameter because it’s available in the DI container
app.MapGet("/links", (LinkGenerator links) =>
{
	var link = links.GetPathByName("products");
	return $"View the product at {link}";
});

// Using custom binding with BindAsync
// Send a post request to PostMan at /sizes with a body like:
// 1.234
// 2.3455
app.MapPost("/sizes", (SizeDetails size) => $"Received {size}");
// No extra attributes are needed for the SizeDetails parameter, as it has a BindAsync method.

app.MapGet("/category/{id}",
	([AsParameters] SearchModel model) => $"Received {model}");
// [AsParameters] indicates that the constructor or properties of the type should be bound, not the type itself.

app.Run();

string StockWithDefaultValue(int id = 0)
{
	return $"Received {id}";
}
// The id parameter binds to the query string value if it’s available; otherwise, it has the value 0.

internal record struct SearchModel(
	int id,
	int page,
	[FromHeader(Name = "sort")] bool? sortAsc,
	[FromQuery(Name = "q")] string search);
// Each parameter is bound as though it were written in the endpoint handler.

// SizeDetails implements the static Creates a BindAsync method.
public record SizeDetails(double height, double width)
{
	public static async ValueTask<SizeDetails?> BindAsync(
		HttpContext context)
	{
		// Creates a BindAsync method. StreamReader to read the request body
		using var sr = new StreamReader(context.Request.Body);

		var line1 = await sr.ReadLineAsync(context.RequestAborted);
		if (line1 is null) return null;
		var line2 = await sr.ReadLineAsync(context.RequestAborted);
		// Reads a line of text from the body
		// If either line is null, indicating no content, stops processing
		if (line2 is null) return null;
		// Tries to parse the two lines as doubles
		return double.TryParse(line1, out var height)
			&& double.TryParse(line2, out var width)
				   ? new SizeDetails(height, width)
				   : null;
		// If the parsing is successful, creates the SizeDetails model and returns it
		// otherwise, returns null
	}
}

// ProductId is a C# 10 record struct.
internal readonly record struct ProductId(int Id)
{
	// It implements TryParse, so it’s treated as a simple type by minimal APIs.
	public static bool TryParse(string? s, out ProductId result)
	{
		if (s is not null
		 && s.StartsWith('p')
		 && int.TryParse(s.AsSpan().Slice(1),
				out var id))
			// Checks that the string is not null and that the first character in the string is ‘p’
			// and if it is, tries to parse the remaining characters as an integer
			// Efficiently skips the first character by treating the string as a ReadOnlySpan
			// If the string was parsed successfully, id contains the parsed value.
		{
			result = new ProductId(id);
			// Everything parsed successfully, so creates a new ProductId and returns true
			return true;
		}

		result = default;
		return false;
		// Something went wrong, so returns false and assigns a default value to the (unused) resultSomething went wrong, so returns false and assigns a default value to the (unused) result
	}
}

internal record Product(int Id, string Name, int Stock);