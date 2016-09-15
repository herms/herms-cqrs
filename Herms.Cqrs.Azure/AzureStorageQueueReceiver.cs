using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Herms.Cqrs.Azure
{
    public class AzureStorageQueueReceiver : IDisposable
    {
        private readonly CloudQueueMessageSerializer _cloudQueueMessageSerializer;
        private readonly string _connectionString;
        private readonly string _queueName;
        private readonly IEventHandlerRegistry _eventHandlerRegistry;
        private readonly ILog _log;
        private CancellationTokenSource _cancellationTokenSource;

        public AzureStorageQueueReceiver(string connectionString, string queueName, IEventHandlerRegistry eventHandlerRegistry)
        {
            _log = LogManager.GetLogger(this.GetType());
            _connectionString = connectionString;
            _queueName = queueName;
            _eventHandlerRegistry = eventHandlerRegistry;
            _cloudQueueMessageSerializer = new CloudQueueMessageSerializer();
        }

        public void Dispose()
        {
            this.Cancel();
        }

        public async Task Start()
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            _log.Debug("Connected to storage account.");
            var queue = await this.InitializeQueue(queueClient, _queueName);
            _cancellationTokenSource = new CancellationTokenSource();
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var message = await queue.GetMessageAsync(_cancellationTokenSource.Token);
                    this.ProcessMessage(message);
                    await queue.DeleteMessageAsync(message, _cancellationTokenSource.Token);
                }
                catch (TaskCanceledException tce)
                {
                    _log.Debug("Task was canceled.");
                }
                catch (Exception ex)
                {
                    // Chew on it.
                    _log.Error(ex);
                }
            }
        }

        public void Stop()
        {
            _log.Info("Cancelling task...");
            this.Cancel();
            _log.Info("Task cancellation requested.");
        }

        private void Cancel()
        {
            _cancellationTokenSource?.Cancel();
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

        private void ProcessMessage(CloudQueueMessage message)
        {
            var @event = _cloudQueueMessageSerializer.DeserializeMessageToEvent(message);

            _log.Debug($"Message of type {@event.GetType()} received!");
            var eventHandlerCollection = _eventHandlerRegistry.ResolveHandlers(@event);
            var results = eventHandlerCollection.Handle(@event);
            if (results.Status != EventHandlerResultType.Success)
            {
                if (results.Status == EventHandlerResultType.Error)
                {
                    _log.Error($"Event handler collection crashed with message: {results.Message}.");
                }
                else if (results.Status == EventHandlerResultType.HandlerFailed)
                {
                    _log.Error($"Not all event handlers for event {@event.Id} succeeded.");
                    foreach (var result in results.Failed)
                    {
                        _log.Error(result.Message);
                    }
                }
            }
        }
    }
}