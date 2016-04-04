using System;
using System.Linq;
using Herms.Cqrs.Aggregate;
using Herms.Cqrs.Event;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Herms.Cqrs.Azure
{
    public class AzureEventRepository<TAggregate> : IAggregateRepository<TAggregate> where TAggregate : IAggregate, IEventSourced, new()
    {
        private readonly string _tableName;
        private readonly CloudTable _table;
        private readonly string GuidStringFormat = "D";

        public AzureEventRepository(string connectionString, string tableName)
        {
            _tableName = tableName;
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var cloudTable = _table = tableClient.GetTableReference(tableName);
        }

        public AzureEventRepository(string connectionString) : this(connectionString, typeof (TAggregate).Name) {}

        public void Save(TAggregate aggregate)
        {
            var batch = new TableBatchOperation();
            foreach (var @event in aggregate.GetChanges())
            {
                var eventEntity = new EventEntity(@event.AggregateId.ToString(GuidStringFormat), @event.EventId.ToString(GuidStringFormat))
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
            var query =
                new TableQuery<EventEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
                    id.ToString(GuidStringFormat)));
            var results = _table.ExecuteQuery(query);
            var eventEntities = results.ToList().OrderBy(e => e.Version);
            if (eventEntities.Any())
            {
                var aggregate = new TAggregate();
                aggregate.Load(eventEntities.Select(e => e.Payload));
                return aggregate;
            }
            return default(TAggregate);
        }
    }

    public class EventEntity : TableEntity
    {
        public string AggregateType { get; set; }
        public string EventType { get; set; }
        public string AssemblyName { get; set; }
        public int Version { get; set; }
        public IEvent Payload { get; set; }

        public EventEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey) {}

        public EventEntity() {}
    }
}