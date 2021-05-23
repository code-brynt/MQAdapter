using IBM.WMQ;
using System;
using System.Threading.Tasks;

namespace MQAdapter
{
    public interface IMQAdapterService
    {
        bool SendMessageToQueue(string queueName, object objToSend);
        bool SendMessageToTopic(string topicName, object objToSend);

        Task ListenToTopic(string topicString, Action<MQMessage> messageHandler, Action<IMQConnectionStatus> connectionStatusChangedHandler);
        Task ListenToQueue(string queueName, Action<MQMessage> messageHandler, Action<IMQConnectionStatus> connectionStatusChangedHandler);
    }
}