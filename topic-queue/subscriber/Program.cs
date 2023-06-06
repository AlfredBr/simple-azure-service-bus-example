using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Azure.Messaging.ServiceBus;
using shared;

namespace subscriber;

// https://learn.microsoft.com/en-us/samples/azure/azure-sdk-for-net/azuremessagingservicebus-samples/
// https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/servicebus/Azure.Messaging.ServiceBus/samples/Sample01_SendReceive.md

internal static class Program
{
	static void Main(string[] args)
	{
		var builder = Host.CreateApplicationBuilder(args);
		builder.Services.AddHostedService<Subscriber>();
		builder.Build().Run();
	}
}

internal class Subscriber : BackgroundService
{
	private IConfiguration _configuration;
	private int _subscriptionIndex = 0;

	public Subscriber(IConfiguration configuration)
	{
		_configuration = configuration;

		if (_configuration["subscribe"] is not null)
		{
			_subscriptionIndex = configuration["subscribe"]?.Equals(Config.SubscriptionName[0]) ?? true ? 0 : 1;
		}
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var connectionString = _configuration["connectionString"];
		var topicName = _configuration["topicName"];
		await using var client = new ServiceBusClient(connectionString);
		var options = new ServiceBusProcessorOptions
		{
			ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
		};
		var processor = client.CreateProcessor(topicName, Config.SubscriptionName[_subscriptionIndex], options);
		processor.ProcessMessageAsync += (args) =>
		{
			var message = args.Message;
			Console.WriteLine($"{message.Body} [{message.MessageId}]");
			return Task.CompletedTask;
		};
		processor.ProcessErrorAsync += (args) =>
		{
			var exception = args.Exception.ToString();
			Console.WriteLine(exception);
			return Task.CompletedTask;
		};
		await processor.StartProcessingAsync();

		try
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay(1000, stoppingToken);
			}
		}
		catch (TaskCanceledException)
		{
			Console.WriteLine("Task Canceled");
		}
		finally
		{
			await processor.StopProcessingAsync();
		}
	}
}
