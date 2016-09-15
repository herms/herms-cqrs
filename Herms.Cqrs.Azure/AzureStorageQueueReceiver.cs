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
        private readonly CloudQueueClient _queueClient;
        private readonly CloudQueueMessageSerializer _cloudQueueMessageSerializer;
        private readonly IEventHandlerRegistry _eventHandlerRegistry;
        private readonly ILog _log;
        private CancellationTokenSource _cancellationTokenSource;
        private CloudQueue _queue;

        public AzureStorageQueueReceiver(string connectionString, string queueName, IEventHandlerRegistry eventHandlerRegistry)
        {
            _log = LogManager.GetLogger(this.GetType());
            _eventHandlerRegistry = eventHandlerRegistry;
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            _queueClient = storageAccount.CreateCloudQueueClient();
            var initializeQueueTask = this.InitializeQueue(queueName);
            _cloudQueueMessageSerializer = new CloudQueueMessageSerializer();
            initializeQueueTask.Wait();
        }

        public void Dispose()
        {
            this.Cancel();
        }

        public async Task Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var message = await _queue.GetMessageAsync(_cancellationTokenSource.Token);
                    this.ProcessMessage(message);
                    await _queue.DeleteMessageAsync(message, _cancellationTokenSource.Token);
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

        private async Task InitializeQueue(string queueName)
        {
            _queue = _queueClient.GetQueueReference(queueName);
            await _queue.CreateIfNotExistsAsync();
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