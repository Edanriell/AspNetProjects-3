using System.ComponentModel.DataAnnotations;

// Adds this using statement to use the validation attributes

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Complex type, attributes validate first. If they pass, runs IValidatableObject
app.MapPost("/users", (CreateUserModel user) => user.ToString())
   .WithParameterValidation();

// Note that this does NOT work
//app.MapGet("/user/{id}", ([Range(0, 10)] int id) => id.ToString())
//    .WithParameterValidation();

// The API takes a UserModel parameter and binds it to the request body
app.MapPost("/users", (UserModel user) => user.ToString());

app.MapPost("/users", (UserModel user) => user.ToString()).WithParameterValidation();
// Adds the validation filter to the endpoint

// Uses [AsParameters] to create a type than can be validated
// Adds the validation filter to the endpoint
app.MapPost("/user/{id}",
		([AsParameters] GetUserModel model) => $"Received {model.Id}")
   .WithParameterValidation();

// Custom type (struct)
app.MapGet("/user1/{id}", ([AsParameters] GetUserModel model) => model.Id.ToString())
   .WithParameterValidation();

// Custom type (struct record)
app.MapGet("/user2/{id}", ([AsParameters] GetUserModel2 model) => model.Id.ToString())
   .WithParameterValidation();

// TryParse() implementation
app.MapGet("/user3/{id}", (UserId id) => id.ToString())
   .WithParameterValidation();

app.Run();

//struct GetUserModel
//{
//	[Range(1, 10)]
//	Public int Id { get; set; }
//}
//Adds validation attributes to your simple types
public record UserModel
{
	// Values marked Required must be provided.
	[Required]
	// The StringLengthAttribute sets the maximum length for the property.
	[StringLength(100)]
	[Display(Name = "Your name")]
	// Customizes the name used to describe the property
	public string FirstName { get; set; }

	[Required]
	[StringLength(100)]
	[Display(Name = "Last name")]
	public string LastName { get; set; }

	// Validates that the value of Email may be a valid email address
	[Required] [EmailAddress] public string Email { get; set; }

	// Validates that the value of PhoneNumber has a valid telephone number format
	[Phone]
	[Display(Name = "Phone number")]
	public string PhoneNumber { get; set; }
}

public record CreateUserModel : IValidatableObject
{
	[Required]
	[StringLength(100)]
	[Display(Name = "Your name")]
	public string Name { get; set; }

	[Required]
	[StringLength(100)]
	[Display(Name = "Last name")]
	public string LastName { get; set; }

	[EmailAddress] public string Email { get; set; }

	[Phone]
	[Display(Name = "Phone number")]
	public string PhoneNumber { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (string.IsNullOrEmpty(Email)
		 && string.IsNullOrEmpty(PhoneNumber))
			yield return new ValidationResult(
				"You must provide either an Email or a PhoneNumber",
				new[] { nameof(Email), nameof(PhoneNumber) });
	}
}

internal struct GetUserModel
{
	[Range(1, 10)] public int Id { get; set; }
}

internal record struct GetUserModel2([property: Range(1, 10)] int Id);

internal readonly record struct UserId([property: Range(1, 10)] int Id)
{
	public static bool TryParse(string? s, out UserId result)
	{
		if (int.TryParse(s, out var id))
		{
			result = new UserId(id);
			return true;
		}

		result = default;
		return false;
	}
}


// The UserModel defines its validation requirements using DataAnnotations attributes.
//public record UserModel
//{
//	[Required]
//	[StringLength(100)]
//	[Display(Name = "Your name")]
//	public string Name { get; set; }
//
//	[Required] [EmailAddress] public string Email { get; set; }
//}