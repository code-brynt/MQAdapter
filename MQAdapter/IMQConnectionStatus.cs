namespace MQAdapter
{
    public interface IMQConnectionStatus
    {
        string QueueManager { get; set; }
        Status Status { get; set; }
        string TopicQueueName { get; set; }

        string ToString();
    }
}