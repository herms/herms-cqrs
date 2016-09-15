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
            var initializeQueueTask = Task.Run(async () => await this.InitializeQueue(queueClient, queueName));
            _queue = initializeQueueTask.Result;
        }

        public void Publish(IEvent @event)
        {
            var publishTask = this.PublishAsync(@event);
            publishTask.Wait();
        }

        private async Task PublishAsync(IEvent @event)
        {
            var message = CloudQueueMessageSerializer.SerializeEventToMessage(@event);
            await _queue.AddMessageAsync(message);
            _log.Trace("Event published.");
        }

        private async Task<CloudQueue> InitializeQueue(CloudQueueClient cloudQueueClient, string queueName)
        {
            var queue = cloudQueueClient.GetQueueReference(queueName);
            if (!await queue.ExistsAsync())
            {
                _log.Info($"Creating queue {queueName}.");
                await queue.CreateIfNotExistsAsync();
            }
            else
                _log.Debug($"Found queue {queueName}.");
            return queue;
        }
    }
}