namespace StudentQueries.Services;

public class ServiceBusSettings
{
    public const string ServiceBus = "ServiceBus";
    public string ConnectionString { get; set; } = default!;
    public string TopicName { get; set; } = default!;
}