using RoutingExample;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ProductService>();
builder.Services.AddHealthChecks();
builder.Services.AddRazorPages();
// Adds the services required by the healthcheck middleware and Razor Pages

var app = builder.Build();

app.MapGet("/", (LinkGenerator links, HttpContext context) => GetHomePage(links, context));

app.MapGet("/test", () => "Hello world!").WithName("hello");
//app.MapGet("/test", () => "Hello world!");
// Registers a minimal API endpoint that returns “Hello World!” at the route /test

// The endpoint echoes the name it receives in the route template.
//app.MapGet("/product/{name}", (string name) => $"The product is {name}")
//   .WithName("product");
// Gives the endpoint a name by adding metadata to it
app.MapGet("/{name}", (ProductService service, string name) =>
{
	var product = service.GetProduct(name);
	return product is null
			   ? Results.NotFound()
			   : Results.Ok(product);
}).WithName("product");

// References the LinkGenerator class in the endpoint handler
app.MapGet("/links", (LinkGenerator links) =>
{
	var link = links.GetPathByName("product",
		// Creates a link using the route name “product” and provides a value for the route parameter
		new { name = "big-widget" });
	// Returns the value “View the product at /product/big-widget”
	return $"View the product at {link}";
});

app.MapHealthChecks("/healthz").WithName("healthcheck");
//app.MapHealthChecks("/healthz");
// Registers a health-check endpoint at the route /healthz
app.MapRazorPages();
// Registers all the Razor Pages in your application as endpoints

app.MapGet("/redirect-me", () => Results.RedirectToRoute("hello"))
   .WithName("redirect");

app.Run();

static string GetHomePage(LinkGenerator links, HttpContext context)
{
	var healthcheck = links.GetPathByName("healthcheck");
	var helloWorld = links.GetPathByName("hello");
	var redirect = links.GetPathByName("redirect");
	var bigWidget = links.GetPathByName("product", new { Name = "big-widget" });
	var fancyWidget = links.GetUriByName(context, "product", new { Name = "super-fancy-widget" });

	return $@"Try navigating to one of the following paths:
    {healthcheck} (standard health check)
    {helloWorld} (Hello world! response)
    {redirect} (Redirects to the {helloWorld} endpoint)
    {bigWidget} or {fancyWidget} (returns the Product details)
    /not-a-product (returns a 404)";
}