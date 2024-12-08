var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var people = new List<Person>
			 {
				 new("Tom", "Hanks"),
				 new("Denzel", "Washington"),
				 new("Leondardo", "DiCaprio"),
				 new("Al", "Pacino"),
				 new("Morgan", "Freeman")
			 };
// Creates a list of people as the data for the API

app.MapGet("/person/{name}", (string name) =>
	people.Where(p => p.FirstName.StartsWith(name)));
// The route is parameterized to extract the name from the URL.
// The extracted value can be injected into the lambda handler

app.Run();

internal record Person(string FirstName, string LastName);