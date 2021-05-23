namespace MQAdapter
{
    public enum Status
    {
        Connected,
        Disconnected
    }

    public class MQConnectionStatus : IMQConnectionStatus
    {
        public MQConnectionStatus(string queueManager, string topicQueueName, Status status)
        {
            Status = status;
            QueueManager = queueManager;
            TopicQueueName = topicQueueName;
        }

        public Status Status { get; set; }
        public string QueueManager { get; set; }
        public string TopicQueueName { get; set; }

        public override string ToString()
        {
            return $"IBM MQ Adapter Connection Status Changed, Status: {Status}, QueueManager: {QueueManager}, TopicQueueName: {TopicQueueName}.";
        }
    }
}
