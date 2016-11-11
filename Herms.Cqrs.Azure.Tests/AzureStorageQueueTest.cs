﻿using System;
using System.Configuration;
using System.Threading.Tasks;
using Herms.Cqrs.TestContext.EventHandlers;
using Herms.Cqrs.TestContext.Events;
using Xunit;

namespace Herms.Cqrs.Azure.Tests
{
    public class AzureStorageQueueTest
    {
        [Fact]
        public async Task GivenTheWorld_WhenSendingMessageToQueue_ThenMessageShouldBeReceived()
        {
            var storageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            var eventHandlerRegistry = new PoorMansEventHandlerRegistry();
            eventHandlerRegistry.Register(typeof(IEventHandler<TestEvent1>), typeof(TestEventHandler1));
            var queueName = "test-messages-001";

            var queueConfiguration = new AzureStorageQueueConfiguration
            {
                ConnectionString = storageConnectionString,
                QueueName = queueName
            };

            var queueReceiver = new AzureStorageQueueReceiver(queueConfiguration, eventHandlerRegistry);

            var queueDispatcher = new AzureStorageQueueDispatcher(storageConnectionString, queueName);

            queueReceiver.Start();

            await queueDispatcher.PublishAsync(new TestEvent1 {AggregateId = Guid.NewGuid(), Id =  Guid.NewGuid(), Timestamp = DateTime.UtcNow, Version = 1});

            await Task.Delay(100);

            queueReceiver.Stop();
        }
    }
}