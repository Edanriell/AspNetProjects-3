var builder = WebApplication.CreateBuilder(args);

// Can also remove the explicit configurations below and use extension method
// builder.Services.AddEmailSender();

//  Whenever you require an IEmailSender, use EmailSender.
//builder.Services.AddScoped<IEmailSender, EmailSender>();
//builder.Services.AddSingleton<NetworkClient>();
// Whenever you require a NetworkClient, use NetworkClient.
//builder.Services.AddScoped<MessageFactory>();
// Whenever you require a MessageFactory, use MessageFactory
//builder.Services.AddSingleton(provider =>
//	new EmailServerSettings
//		(
//			"smtp.server.com",
//			25
//		));
// This instance of EmailServerSettings will be used whenever an instance is required

//  EmailSender is registered with the container.
//builder.Services.AddScoped<IMessageSender, EmailSender>();

//builder.Services.TryAddScoped<IMessageSender, SmsSender>();
// There’s already an IMessageSender implementation, so SmsSender isn’t registered.

//builder.Services.Replace(new ServiceDescriptor(
//		typeof(IMessageSender), typeof(SmsSender), ServiceLifetime.Scoped
//	));

builder.Services.AddEmailSender();
// The extension method registers all the services associated with the EmailSender

// Because you’re providing a function to create the object, you aren’t restricted to a singleton.
//builder.Services.AddScoped(
//	// The lambda is provided an instance of IServiceProvider.
//	provider =>
//		new EmailServerSettings(
//				"smtp.server.com",
//				25
//			));
// The constructor is called every time an EmailServerSettings object is required instead of only once.

var app = builder.Build();

// The endpoint is called when a new user is created.
app.MapGet("/register/{username}", RegisterUser);
app.MapGet("/", () => "Navigate to register/{username} to test the Mock email sending");

app.Run();

//string RegisterUser(string username)
//{
//	// To create EmailSender, you must create all its dependencies.
//	IEmailSender emailSender = new EmailSender(
//			new MessageFactory(),
//			// You need a new MessageFactory.
//			new NetworkClient(
//				// The NetworkClient also has dependencies.
//				new EmailServerSettings
//					(
//						Host: "smtp.server.com",
//						Port: 25
//					))
//			// You’re already two layers deep, but there could feasibly be more.
//		);
//	emailSender.SendEmail(username);
//	// Finally, you can send the email.
//	return $"Email sent to {username}!";
//} 

// The IEmailSender is injected into the handler using DI.
string RegisterUser(string username, IEmailSender emailSender)
{
	emailSender.SendEmail(username);
	// The handler uses the IEmailSender instance.
	return $"Email sent to {username}!";
}

public record Email(string Address, string Message);

public record EmailServerSettings(string Host, int Port);

public interface IEmailSender
{
	void SendEmail(string username);
}

public class EmailSender : IEmailSender
{
	private readonly NetworkClient _client;
	private readonly MessageFactory _factory;

	public EmailSender(MessageFactory factory, NetworkClient client)
	{
		_factory = factory;
		_client = client;
	}

	public void SendEmail(string username)
	{
		var email = _factory.Create(username);
		_client.SendEmail(email);
		Console.WriteLine($"Email sent to {username}!");
	}
}

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

public static class EmailSenderServiceCollectionExtensions
{
	public static IServiceCollection AddEmailSender(this IServiceCollection services)
	{
		// Creates an extension method on IServiceCollection by using the “this” keyword
		services.AddScoped<IEmailSender, EmailSender>();
		services.AddSingleton<NetworkClient>();
		services.AddScoped<MessageFactory>();
		services.AddSingleton(
			new EmailServerSettings
				(
					"smtp.server.com",
					25
				));
		return services;

		// Cuts and pastes your registration code from Program.cs
		// By convention, returns the IServiceCollection to allow method chaining
	}
}

//string RegisterUser(
//	string username,
//	IEnumerable<IMessageSender> senders)
//// Requests an IEnumerable injects an array of IMessageSender
//{
//	foreach (var sender in senders)
//	{
//		Sender.SendMessage($”Hello {username}!”);
//	}
//	// Each IMessageSender in the IEnumerable is a different implementation.
//
//	return $"Welcome message sent to {username}";
//}