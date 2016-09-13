using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Herms.Cqrs.Azure
{
    public class AzureStorageQueueReceiver
    {
        private readonly CloudQueueMessageSerializer _cloudQueueMessageSerializer;
        private readonly IEventHandlerRegistry _eventHandlerRegistry;
        // Make event handler registry.
        private readonly ILog _log;
        private readonly CloudQueueClient _queueClient;
        private CancellationTokenSource _cancellationTokenSource;
        private CloudQueue _queue;
        private Task _readTask;

        public AzureStorageQueueReceiver(string connectionString, string queueName, IEventHandlerRegistry eventHandlerRegistry)
        {
            _log = LogManager.GetLogger(this.GetType());
            _eventHandlerRegistry = eventHandlerRegistry;
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            _queueClient = storageAccount.CreateCloudQueueClient();
            this.InitializeQueue(queueName);
            _cloudQueueMessageSerializer = new CloudQueueMessageSerializer();
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var taskScheduler = TaskScheduler.Default;
            _log.Info("Starting queue listener.");
            _readTask = Task.Factory.StartNew(this.ReadMessageQueue, _cancellationTokenSource.Token,
                TaskCreationOptions.LongRunning,
                taskScheduler);
        }

        public void Stop()
        {
            _log.Info("Cancelling task...");
            this.CancelReadTask();
            _log.Info("Task completed.");
        }

        public void Dispose()
        {
            this.CancelReadTask();
        }

        private void CancelReadTask()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                if (_readTask != null && !this.IsTaskFinished(_readTask))
                {
                    try
                    {
                        _readTask.Wait(_cancellationTokenSource.Token);
                    }
                    catch (Exception exception)
                    {
                        _log.Error($"Caught exception: {exception.Message}");
                    }
                }
                //_cancellationTokenSource?.Dispose();
            }
        }

        private bool IsTaskFinished(Task task)
        {
            return this.IsTaskInStatus(task, new[] { TaskStatus.Canceled, TaskStatus.Faulted, TaskStatus.RanToCompletion });
        }

        private bool IsTaskInStatus(Task task, TaskStatus[] statuses)
        {
            return statuses.Any(s => task.Status == s);
        }

        private void InitializeQueue(string queueName)
        {
            _queue = _queueClient.GetQueueReference(queueName);
            _queue.CreateIfNotExists();
        }

        private async Task ReadMessageQueue()
        {
            _log.Info($"Waiting for message on queue {_queue.Name}...");
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    var message = await _queue.GetMessageAsync(_cancellationTokenSource.Token);
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
                    await _queue.DeleteMessageAsync(message);
                }
                catch (Exception)
                {
                    // Do nothing.
                }
            }
        }
    }
}