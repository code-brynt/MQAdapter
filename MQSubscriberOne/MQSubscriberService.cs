using IBM.WMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQAdapter;
using MQSubscriberOne.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MQSubscriberOne
{
    public class MQSubscriberService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MQSubscriberService> _logger;
        private readonly IMQAdapterService _mqAdapterService;

        public MQSubscriberService(IServiceProvider sp, ILogger<MQSubscriberService> logger, IMQAdapterService mqAdapterService)
        {
            _serviceProvider = sp;
            _logger = logger;
            _mqAdapterService = mqAdapterService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ListenForMessages(cancellationToken);
                await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
            }
        }

        private async Task ListenForMessages(CancellationToken cancellationToken)
        {
            await Task.Run(() => _mqAdapterService.ListenToTopic("BroadCast", MessageHandler, ConnectionStatusChangedHandler), cancellationToken);
        }

        private void ConnectionStatusChangedHandler(IMQConnectionStatus connectionStatus)
        {
            _logger.LogInformation(connectionStatus.ToString());
        }

        private void MessageHandler(MQMessage mqMessage)
        {
            var msg = mqMessage.ReadString(mqMessage.DataLength);
            var message = new Message
            {
                Data = msg
            };
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Messages.Add(message);
                dbContext.SaveChanges();
            }

            _logger.LogInformation(msg);
        }
    }
}
