var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/register/{username}", RegisterUser);
// The endpoint is called when a new user is created.

app.Run();

// The RegisterUser function is the handler for the endpoint.
//string RegisterUser(string username)
//{
//	var emailSender = new EmailSender();
//	// Creates a new instance of EmailSender
//	emailSender.SendEmail(username);
//	// Uses the new instance to send the email
//	return $"Email sent to {username}!";
//}

//public class EmailSender
//{
//	public void SendEmail(string username)
//	{
//		Console.WriteLine($"Email sent to {username}!");
//	}
//}

string RegisterUser(string username)
{
	var emailSender = new EmailSender(
			// To create EmailSender, you must create all its dependencies.
			new MessageFactory(),
			// You need a new MessageFactory.
			new NetworkClient(
				// The NetworkClient also has dependencies.
				new EmailServerSettings
					(
						"smtp.server.com",
						25
					))
			// You’re already two layers deep, but there could feasibly be more.
		);
	emailSender.SendEmail(username);
	// Finally, you can send the email.
	return $"Email sent to {username}!";
}

public record Email(string Address, string Message);

public record EmailServerSettings(string Host, int Port);

// Now the EmailSender depends on two other classes.
public class EmailSender
{
	private readonly NetworkClient _client;

	private readonly MessageFactory _factory;

	// Instances of the dependencies are provided in the constructor.
	public EmailSender(MessageFactory factory, NetworkClient client)
	{
		_factory = factory;
		_client = client;
	}

	public void SendEmail(string username)
	{
		var email = _factory.Create(username);
		_client.SendEmail(email);
		//  The EmailSender coordinates the dependencies to create and send an email.
		Console.WriteLine($"Email sent to {username}!");
	}
}

//public class NetworkClient
//{
//	private readonly EmailServerSettings _settings;
//	public NetworkClient(EmailServerSettings settings)
//	{
//		_settings = settings;
//	}
//}

public class NetworkClient
{
	private readonly EmailServerSettings _settings;

	public NetworkClient(EmailServerSettings settings)
	{
		_settings = settings;
	}

	public void SendEmail(Email email)
	{
		Console.WriteLine($"Connecting to server {_settings.Host}:{_settings.Port}");
		Console.WriteLine($"Email sent to {email.Address}: {email.Message}");
	}
}

public class MessageFactory
{
	public Email Create(string emailAddress)
	{
		return new Email(emailAddress, "Thanks for signing up!");
	}
}