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
        private readonly CloudQueue _queue;

        public AzureStorageQueueDispatcher(string connectionString, string queueName)
        {
            _log = LogManager.GetLogger(this.GetType());
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            _log.Debug("Connected to storage account.");
            _queue = this.InitializeQueue(queueClient, queueName);
        }

        public void Publish(IEvent @event)
        {
            var message = CloudQueueMessageSerializer.SerializeEventToMessage(@event);
            _queue.AddMessage(message);
            _log.Trace($"Event {@event.Id} published.");
        }

        private async Task PublishAsync(IEvent @event)
        {
            var message = CloudQueueMessageSerializer.SerializeEventToMessage(@event);
            await _queue.AddMessageAsync(message);
            _log.Trace($"Event {@event.Id} published.");
        }

        private CloudQueue InitializeQueue(CloudQueueClient cloudQueueClient, string queueName)
        {
            var queue = cloudQueueClient.GetQueueReference(queueName);
            if (!queue.Exists())
            {
                _log.Info($"Creating queue {queueName}.");
                queue.CreateIfNotExists();
            }
            else
                _log.Debug($"Found queue {queueName}.");
            return queue;
        }
    }
}