using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IMessageSender, EmailSender>();
// EmailSender is registered with the container.
builder.Services.AddScoped<IMessageSender, SmsSender>();
builder.Services.AddScoped<IMessageSender, FacebookSender>();
builder.Services.TryAddScoped<IMessageSender, UnregisteredSender>();
// There’s already an IMessageSender implementation, so UnregisteredSender isn’t registered.

//builder.Services.Replace(new ServiceDescriptor( typeof(IMessageSender), typeof(SmsSender), ServiceLifetime.Scoped ));
// When using Replace, you must provide the same lifetime that was used to register the service that’s being replaced.

var app = builder.Build();

app.MapGet("/", () => "Try calling /single-message/{username} or /multi-message/{username} and check the logs");
app.MapGet("/single-message/{username}", SendSingleMessage);
app.MapGet("/multi-message/{username}", SendMultiMessage);

app.Run();

string SendSingleMessage(string username, IMessageSender sender)
{
	sender.SendMessage($"Hello {username}!");
	return "Check the application logs to see what was called";
	//  Of the three implementations available, the container needs to pick a single IMessageSender to inject into this service. It does this by using the last registered implementation: FacebookSender
}

// Requests an IEnumerable injects an array of IMessageSender
string SendMultiMessage(string username, IEnumerable<IMessageSender> senders)
{
	// Each IMessageSender in the IEnumerable is a different implementation.
	foreach (var sender in senders) sender.SendMessage($"Hello {username}!");

	return "Check the application logs to see what was called";
}

internal interface IMessageSender
{
	void SendMessage(string message);
}

internal class EmailSender : IMessageSender
{
	public void SendMessage(string message)
	{
		Console.WriteLine($"Sending Email message: {message}");
	}
}

internal class FacebookSender : IMessageSender
{
	public void SendMessage(string message)
	{
		Console.WriteLine($"Sending Facebook message: {message}");
	}
}

internal class SmsSender : IMessageSender
{
	public void SendMessage(string message)
	{
		Console.WriteLine($"Sending SMS: {message}");
	}
}

internal class UnregisteredSender : IMessageSender
{
	public void SendMessage(string message)
	{
		throw new Exception("I'm never registered so shouldn't be called");
	}
}