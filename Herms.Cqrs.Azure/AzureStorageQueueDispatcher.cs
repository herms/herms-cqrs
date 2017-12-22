using System.Threading.Tasks;
using Common.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Herms.Cqrs.Azure {
    public class AzureStorageQueueDispatcher {
        private readonly ILog _log;
        private readonly CloudQueueClient _queueClient;
        private readonly string _queueName;
        private CloudQueue _queue;
        private bool _queueInitialized;

        public AzureStorageQueueDispatcher(AzureStorageQueueConfiguration queueConfiguration)
        {
            _queueName = queueConfiguration.QueueName;
            _log = LogManager.GetLogger(this.GetType());
            var storageAccount = CloudStorageAccount.Parse(queueConfiguration.ConnectionString);
            _queueClient = storageAccount.CreateCloudQueueClient();
            _log.Debug("Connected to storage account.");
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

        protected async Task PublishAsync(CloudQueueMessage message)
        {
            if (!_queueInitialized) _queue = await this.InitializeQueueAsync(_queueClient, _queueName);
            await _queue.AddMessageAsync(message);
        }
    }
}