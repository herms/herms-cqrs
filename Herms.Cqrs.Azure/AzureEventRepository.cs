using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Herms.Cqrs.Aggregate;
using Herms.Cqrs.Event;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Herms.Cqrs.Azure
{
    public class AzureEventRepository<TAggregate> : IAggregateRepository<TAggregate> where TAggregate : IAggregate, IEventSourced, new()
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudTableClient _tableClient;
        private CloudTable _table;

        public AzureEventRepository(string connectionString, string tableName)
        {
            _storageAccount = CloudStorageAccount.Parse(connectionString);
            _tableClient = _storageAccount.CreateCloudTableClient();
            _table = _tableClient.GetTableReference(tableName);
        }

        public AzureEventRepository(string connectionString) : this(connectionString, typeof (TAggregate).Name) {}

        public void Save(TAggregate aggregate)
        {
            var batch = new TableBatchOperation();
            foreach (var @event in aggregate.GetChanges())
            {
                var eventEntity = new EventEntity(@event.AggregateId.ToString("D"), @event.EventId.ToString("D"))
                {
                    AggregateType = typeof (TAggregate).Name,
                    EventType = @event.GetType().Name,
                    AssemblyName = @event.GetType().Assembly.GetName().Name,
                    Version = @event.Version,
                    Payload = @event
                };
                var operation = TableOperation.Insert(eventEntity);
                batch.Add(operation);
            }
            var result = _table.ExecuteBatch(batch);
            if (result.Any(r => r.HttpStatusCode != 200))
            {
                
            }
        }

        public TAggregate Get(Guid id)
        {
            throw new NotImplementedException();
        }
    }

    public class EventEntity : TableEntity
    {
        public string AggregateType { get; set; }
        public string EventType { get; set; }
        public string AssemblyName { get; set; }
        public int Version { get; set; }
        public IEvent Payload { get; set; }
        public EventEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
        }
    }
}