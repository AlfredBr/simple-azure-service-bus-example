namespace shared;

public static class Config
{
    public static string ConnectionString => "<your-azure-connection-string>";
    public static string QueueName => "demo-asb-queuename";
    public static string TopicName => "demo-asb-topicname";
    public static string[] SubscriptionName => new string[] { "even", "odd" };
}