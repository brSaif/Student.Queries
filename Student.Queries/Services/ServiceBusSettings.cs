namespace StudentQueries.Services;

public class ServiceBusSettings
{
    public const string ServiceBus = "ServiceBus";
    public string ConnectionString { get; set; }
    public string TopicName { get; set; }
}