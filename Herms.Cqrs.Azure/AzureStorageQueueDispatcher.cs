using System;
using Herms.Cqrs.Event;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Herms.Cqrs.Azure
{
    public class AzureStorageQueueDispatcher : IEventDispatcher
    {
        private readonly string _connectionString;
        private readonly string _queueName;

        public AzureStorageQueueDispatcher(string connectionString, string queueName)
        {
            _connectionString = connectionString;
            _queueName = queueName;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
        }
        public void Publish(IEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
