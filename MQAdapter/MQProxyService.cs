using IBM.WMQ;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace MQAdapter
{
    public class MQProxyService : IMQProxyService
    {
        private Hashtable _queueManagerProperties;
        private readonly IMQProxyConnection _configuration;
        private readonly ILogger<MQProxyService> _logger;

        public MQProxyService(IMQProxyConnection configuration, ILogger<MQProxyService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _queueManagerProperties = new Hashtable
            {
                { MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_MANAGED },
                { MQC.HOST_NAME_PROPERTY, _configuration.HostName },
                { MQC.PORT_PROPERTY, _configuration.Port },
                { MQC.CHANNEL_PROPERTY, _configuration.Channel },
                { MQC.USER_ID_PROPERTY, _configuration.UserId },
                { MQC.PASSWORD_PROPERTY, _configuration.Password }
            };
        }

        public bool SendMessageToQueue(string queueName, object objToSend)
        {
            try
            {
                var hearBeatConfirmationText = JsonConvert.SerializeObject(objToSend);

                using (var queueManager = new MQQueueManager(_configuration.QueueManagerName, _queueManagerProperties))
                {
                    using (var outboundTopic = queueManager.AccessQueue(queueName, MQC.MQOO_INPUT_AS_Q_DEF + MQC.MQOO_OUTPUT + MQC.MQOO_FAIL_IF_QUIESCING))
                    {
                        var msg = new MQMessage { Persistence = MQC.MQPER_PERSISTENCE_AS_TOPIC_DEF };
                        msg.WriteString(hearBeatConfirmationText);
                        outboundTopic.Put(msg);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                _logger?.LogError(e, $"Error in sending message to the queue {queueName}.");
                return false;
            }
        }

        public bool SendMessageToTopic(string topicName, object objToSend)
        {
            try
            {
                var hearBeatConfirmationText = JsonConvert.SerializeObject(objToSend);

                using (var queueManager = new MQQueueManager(_configuration.QueueManagerName, _queueManagerProperties))
                {
                    using (var outboundTopic = queueManager.AccessTopic(topicName, null, MQC.MQTOPIC_OPEN_AS_PUBLICATION, MQC.MQOO_OUTPUT))
                    {
                        var msg = new MQMessage { Persistence = MQC.MQPER_PERSISTENCE_AS_TOPIC_DEF };
                        msg.WriteString(hearBeatConfirmationText);
                        outboundTopic.Put(msg);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                _logger?.LogError(e, $"Error in sending message to the topic {topicName}.");
                return false;
            }
        }

        public Task ListenToTopic(string topicString, Action<MQMessage> messageHandler, Action<IMQConnectionStatus> connectionStatusChangedHandler)
        {
            return ListenToMq(_configuration.QueueManagerName, topicString, false, messageHandler, connectionStatusChangedHandler);
        }

        public Task ListenToQueue(string queueName, Action<MQMessage> messageHandler, Action<IMQConnectionStatus> connectionStatusChangedHandler)
        {
            return ListenToMq(_configuration.QueueManagerName, queueName, true, messageHandler, connectionStatusChangedHandler);
        }

        private Task ListenToMq(string queueManagerName, string topicOrQueueString, bool isQueue, Action<MQMessage> messageHandler, Action<IMQConnectionStatus> connectionStatusChangedHandler)
        {
            var openOptionsForGet = MQC.MQSO_CREATE | MQC.MQSO_FAIL_IF_QUIESCING | MQC.MQSO_MANAGED | MQC.MQSO_NON_DURABLE;
            MQDestination inboundDestination = null;

            var gmo = new MQGetMessageOptions();
            gmo.Options |= MQC.MQGMO_NO_WAIT | MQC.MQGMO_FAIL_IF_QUIESCING;

            try
            {
                using (var queueManager = new MQQueueManager(queueManagerName, _queueManagerProperties))
                {
                    if (isQueue)
                    {
                        using (inboundDestination = queueManager.AccessQueue(topicOrQueueString, MQC.MQTOPIC_OPEN_AS_SUBSCRIPTION))
                        {
                            connectionStatusChangedHandler(new MQConnectionStatus(queueManagerName, topicOrQueueString, Status.Connected));
                            GetMessageLoop(messageHandler, inboundDestination, gmo);
                        }
                    }
                    else
                    {
                        using (inboundDestination = queueManager.AccessTopic(topicOrQueueString, null, MQC.MQTOPIC_OPEN_AS_SUBSCRIPTION, openOptionsForGet))
                        { 
                            connectionStatusChangedHandler(new MQConnectionStatus(queueManagerName, topicOrQueueString, Status.Connected));
                            GetMessageLoop(messageHandler, inboundDestination, gmo);
                        }
                    }
                }
            }
            catch (MQException mqException)
            {
                _logger?.LogError(mqException, "Error accessing the queue.");
                connectionStatusChangedHandler(new MQConnectionStatus(queueManagerName, topicOrQueueString, Status.Disconnected));
            }

            return Task.CompletedTask;
        }

        private void GetMessageLoop(Action<MQMessage> messageHandler, MQDestination inboundDestination, MQGetMessageOptions gmo)
        {
            while (true)
            {
                try
                {
                    var msg = new MQMessage();
                    inboundDestination.Get(msg, gmo);

                    messageHandler(msg);
                }
                catch (MQException mqException)
                {
                    if (mqException.Reason == MQC.MQRC_NO_MSG_AVAILABLE)
                    {
                        Thread.Sleep(200);
                    }
                    else
                    {
                        _logger?.LogError(mqException, "Error getting the messages.");
                        throw;
                    }
                }
            }
        }
    }
}