namespace shared;

public static class Config
{
    public static string ConnectionString => "<your-azure-connection-string>";
    public static string QueueName => "test-queue";
    public static string TopicName => "test-topic";
    public static string[] SubscriptionName => new string[] { "even", "odd" };
}