﻿using System;
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
        private readonly IEventHandlerRegistry _eventHandlerRegistry;
        private readonly ILog _log;
        private readonly string _queueName;
        private readonly QueueWaitState _waitState;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _receiveTask;

        public AzureStorageQueueReceiver(AzureStorageQueueConfiguration queueConfiguration, IEventHandlerRegistry eventHandlerRegistry)
        {
            if (queueConfiguration == null)
                throw new ArgumentNullException(nameof(queueConfiguration));

            _log = LogManager.GetLogger(this.GetType());
            _connectionString = queueConfiguration.ConnectionString;
            _queueName = queueConfiguration.QueueName;
            _eventHandlerRegistry = eventHandlerRegistry;
            _cloudQueueMessageSerializer = new CloudQueueMessageSerializer();
            _waitState = new QueueWaitState(queueConfiguration.MinWaitMs, queueConfiguration.MaxWaitMs, queueConfiguration.WaitMultiplier);
        }

        public void Dispose()
        {
            this.Cancel();
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            _log.Debug("Connected to storage account.");

            _receiveTask = this.ReceiveAsync(queueClient, token);
            // Nothing happens if an exception is thrown... 
        }

        public void Stop()
        {
            _log.Info("Cancelling task...");
            _receiveTask.Wait(5000, _cancellationTokenSource.Token);
            this.Cancel();
            _log.Info("Task cancellation requested.");
        }

        private async Task ReceiveAsync(CloudQueueClient queueClient, CancellationToken cancellationToken)
        {
            var queue = await this.InitializeQueue(queueClient, _queueName);
            while (!cancellationToken.IsCancellationRequested)
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var message = await queue.GetMessageAsync(cancellationToken);
                    if (message == null)
                    {
                        var waitMs = _waitState.GetWait();
                        _log.Trace($"Queue message was null. Chill for {waitMs}ms.");
                        await Task.Delay(waitMs, cancellationToken);
                    }

                    else
                    {
                        _waitState.Reset();
                        await this.ProcessMessageAsync(message);
                        await queue.DeleteMessageAsync(message, cancellationToken);
                    }
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

        private async Task ProcessMessageAsync(CloudQueueMessage message)
        {
            var @event = _cloudQueueMessageSerializer.DeserializeMessageToEvent(message);

            _log.Debug($"Message of type {@event.GetType()} received!");
            var eventHandlerCollection = _eventHandlerRegistry.ResolveHandlers(@event);
            var results = await eventHandlerCollection.HandleAsync(@event);
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
                        _log.Error(result.Message);
                }
            }
        }
    }
}