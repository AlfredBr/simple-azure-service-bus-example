using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Azure.Messaging.ServiceBus;
using System;
using System.Diagnostics;

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

		var randomstring = RandomStringGenerator.GenerateRandomString();

		while (!stoppingToken.IsCancellationRequested)
		{
			var timestamp = $"[{DateTime.Now.ToString("HH:mm:ss.fff")}]";
			var message = new ServiceBusMessage($"{timestamp} {{ msg: '{randomstring}' }}");
			message.TimeToLive = TimeSpan.FromSeconds(5);
			message.ApplicationProperties.Add("MessageNumber", num++);
			var sw = Stopwatch.StartNew();
			await sender.SendMessageAsync(message, stoppingToken);
			Console.WriteLine($"{message.Body} {sw.ElapsedMilliseconds}ms");
			await Task.Delay(10, stoppingToken);
		}
	}
}
