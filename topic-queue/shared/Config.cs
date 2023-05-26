namespace shared;

public static class Config
{
    public static string ConnectionString => "Endpoint=sb://meritagedevtest.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=E109aTJR7Q+AA4M17L7mUHhxWNuJGHn/x+ASbDKkaH8=";
    public static string QueueName => "test-queue";
    public static string TopicName => "test-topic";
    public static string[] SubscriptionName => new string[] { "even", "odd" };
}