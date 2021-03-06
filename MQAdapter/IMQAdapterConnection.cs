namespace MQAdapter
{
    public interface IMQAdapterConnection
    {
        string QueueManagerName { get; set; }
        string Channel { get; set; }
        string HostName { get; set; }
        string Password { get; set; }
        int Port { get; set; }
        string UserId { get; set; }
    }
}