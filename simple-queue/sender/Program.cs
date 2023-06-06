using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Azure.Messaging.ServiceBus;

namespace publisher;

// https://learn.microsoft.com/en-us/samples/azure/azure-sdk-for-net/azuremessagingservicebus-samples/
// https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/servicebus/Azure.Messaging.ServiceBus/samples/Sample01_SendReceive.md

internal static class Program
{
	static void Main(string[] args)
	{
		var builder = Host.CreateApplicationBuilder(args);
		builder.Services.AddHostedService<Sender>();
		builder.Build().Run();
	}
}

internal class Sender : BackgroundService
{
	private IConfiguration _configuration;

	public Sender(IConfiguration configuration)
	{
		_configuration = configuration;
	}
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var connectionString = _configuration["connectionString"];
		var queueName = _configuration["queueName"];
		await using var client = new ServiceBusClient(connectionString);
		var sender = client.CreateSender(queueName);
		int num = 0;

		while (!stoppingToken.IsCancellationRequested)
		{
			var timestamp = DateTime.Now.ToString();
			var randomstring = RandomStringGenerator.GenerateRandomString();
			var message = new ServiceBusMessage($"{timestamp} {randomstring}");
			message.ApplicationProperties.Add("MessageNumber", num++);
			await sender.SendMessageAsync(message, stoppingToken);
			Console.WriteLine($"{message.Body}");
			await Task.Delay(1000, stoppingToken);
		}
	}
}
