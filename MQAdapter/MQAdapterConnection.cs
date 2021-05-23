namespace MQAdapter
{
    public class MQAdapterConnection : IMQAdapterConnection
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string Channel { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string QueueManagerName { get; set; }
    }
}
