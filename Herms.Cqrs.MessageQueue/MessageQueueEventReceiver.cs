using System;
using System.Linq;
using System.Messaging;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Herms.Cqrs.Event;

namespace Herms.Cqrs.Msmq
{
    public class MessageQueueEventReceiver
    {
        private readonly IEventHandlerRegistry _eventHandlerRegistry;
        // Make event handler registry.
        private readonly ILog _log;
        private CancellationTokenSource _cancellationTokenSource;
        private MessageQueue _queue;
        private Task _readTask;

        public MessageQueueEventReceiver(IEventHandlerRegistry eventHandlerRegistry, string queuePath)
        {
            _log = LogManager.GetLogger(this.GetType());
            _eventHandlerRegistry = eventHandlerRegistry;
            this.InitializeQueue(queuePath);
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
            if (MessageQueue.Exists(queueName))
            {
                _queue = new MessageQueue(queueName, false);
                _log.Debug($"Found queue {queueName}.");
            }
            else
            {
                _queue = MessageQueue.Create(queueName);
                _log.Info($"Created queue {queueName}.");
            }
            _queue.Formatter = new BinaryMessageFormatter();
        }

        private async Task ReadMessageQueue()
        {
            _log.Info($"Waiting for message on queue {_queue.QueueName}...");
            while (_queue.CanRead && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    var message = _queue.Receive(TimeSpan.FromSeconds(5));
                    if (message?.Body is IEvent)
                    {
                        var payload = (IEvent) message.Body;
                        _log.Debug($"Message of type {payload.GetType()} received!");
                        var eventHandlerCollection = _eventHandlerRegistry.ResolveHandlers(payload);
                        var results = await eventHandlerCollection.HandleAsync(payload);
                        if (results.Status != EventHandlerResultType.Success)
                        {
                            if (results.Status == EventHandlerResultType.Error)
                            {
                                _log.Error($"Event handler collection crashed with message: {results.Message}.");
                            }
                            else if (results.Status == EventHandlerResultType.HandlerFailed)
                            {
                                _log.Error($"Not all event handlers for event {payload.Id} succeeded.");
                                foreach (var result in results.Failed)
                                {
                                    _log.Error(result.Message);
                                }
                            }
                        }
                    }
                }
                catch (MessageQueueException)
                {
                    // Do nothing.
                }
            }
        }
    }
}