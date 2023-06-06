using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Azure.Messaging.ServiceBus;
using shared;
using Microsoft.Extensions.Configuration;

namespace subscriber;

// https://learn.microsoft.com/en-us/samples/azure/azure-sdk-for-net/azuremessagingservicebus-samples/
// https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/servicebus/Azure.Messaging.ServiceBus/samples/Sample01_SendReceive.md

internal static class Program
{
	static void Main(string[] args)
	{
		var builder = Host.CreateApplicationBuilder(args);
		builder.Services.AddHostedService<Receiver>();
		builder.Build().Run();
	}
}

internal class Receiver : BackgroundService
{
	private IConfiguration _configuration;

	public Receiver(IConfiguration configuration)
	{
		_configuration = configuration;
	}
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var connectionString = _configuration["connectionString"];
		var queueName = _configuration["queueName"];
		await using var client = new ServiceBusClient(connectionString);
		var options = new ServiceBusReceiverOptions
		{
			ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
		};
		var receiver = client.CreateReceiver(queueName, options);

		while (!stoppingToken.IsCancellationRequested)
		{
			var message = await receiver.ReceiveMessageAsync(null, stoppingToken);
			if (message is not null)
			{
				var num = message.ApplicationProperties["MessageNumber"];
				Console.WriteLine($"{message.Body} {num:00000} [{message.MessageId}]");
			}
		}
	}
}
