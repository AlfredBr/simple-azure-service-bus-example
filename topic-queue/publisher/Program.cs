using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Azure.Messaging.ServiceBus;
using shared;

namespace publisher;

// https://learn.microsoft.com/en-us/samples/azure/azure-sdk-for-net/azuremessagingservicebus-samples/
// https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/servicebus/Azure.Messaging.ServiceBus/samples/Sample01_SendReceive.md

internal static class Program
{
    static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Publisher>();
        builder.Build().Run();
    }
}

internal class Publisher : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var client = new ServiceBusClient(Config.ConnectionString);
        var sender = client.CreateSender(Config.TopicName);
        int num = 0;

        while(!stoppingToken.IsCancellationRequested)
        {
            var messageTopic = Config.SubscriptionName[num % 2];
            var timestamp = DateTime.Now.ToString();
            var randomstring = RandomStringGenerator.GenerateRandomString();
            var body = $"{timestamp} : {Config.TopicName} : {messageTopic,4} : {num:00000} : {randomstring}";
            var message = new ServiceBusMessage(body);
            message.ApplicationProperties.Add("MessageNumber", num);
            message.ApplicationProperties.Add("MessageTopic", messageTopic);
            await sender.SendMessageAsync(message, stoppingToken);
            Console.WriteLine($"{message.Body}");
            num++;
            await Task.Delay(1000, stoppingToken);
        }
    }
}
