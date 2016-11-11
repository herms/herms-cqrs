using System;
using System.Threading.Tasks;
using Common.Logging;
using Herms.Cqrs.Event;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Herms.Cqrs.Azure
{
    public class AzureStorageQueueDispatcher : IEventDispatcher
    {
        private readonly ILog _log;
        private readonly CloudQueueClient _queueClient;
        private readonly string _queueName;
        private CloudQueue _queue;
        private bool _queueInitialized;

        public AzureStorageQueueDispatcher(AzureStorageQueueConfiguration queueConfiguration)
        {
            if (queueConfiguration == null)
                throw new ArgumentNullException(nameof(queueConfiguration));

            _queueName = queueConfiguration.QueueName;
            _log = LogManager.GetLogger(this.GetType());
            var storageAccount = CloudStorageAccount.Parse(queueConfiguration.ConnectionString);
            _queueClient = storageAccount.CreateCloudQueueClient();
            _log.Debug("Connected to storage account.");
        }

        public async Task PublishAsync(IEvent @event)
        {
            if (!_queueInitialized)
                _queue = await this.InitializeQueueAsync(_queueClient, _queueName);
            var message = CloudQueueMessageSerializer.SerializeEventToMessage(@event);
            await _queue.AddMessageAsync(message);
            _log.Trace($"Event {@event.Id} published.");
        }

        private async Task<CloudQueue> InitializeQueueAsync(CloudQueueClient cloudQueueClient, string queueName)
        {
            var queue = cloudQueueClient.GetQueueReference(queueName);
            if (!queue.Exists())
            {
                _log.Info($"Creating queue {queueName}.");
                await queue.CreateIfNotExistsAsync();
                _queueInitialized = true;
            }
            else
                _log.Debug($"Found queue {queueName}.");
            return queue;
        }
    }
}