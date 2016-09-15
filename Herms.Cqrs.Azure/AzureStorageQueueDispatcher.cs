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
        private readonly CloudQueueMessageSerializer _cloudQueueMessageSerializer;
        private readonly string _connectionString;
        private readonly ILog _log;
        private readonly CloudQueueClient _queueClient;
        private readonly string _queueName;
        private CloudQueue _queue;

        public AzureStorageQueueDispatcher(string connectionString, string queueName)
        {
            _log = LogManager.GetLogger(this.GetType());
            _connectionString = connectionString;
            _queueName = queueName;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            _queueClient = storageAccount.CreateCloudQueueClient();
            var initializeQueueTask = this.InitializeQueue(queueName);
            _cloudQueueMessageSerializer = new CloudQueueMessageSerializer();
            initializeQueueTask.Wait();
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
        }

        private async Task InitializeQueue(string queueName)
        {
            _queue = _queueClient.GetQueueReference(queueName);
            await _queue.CreateIfNotExistsAsync();
        }
    }
}
