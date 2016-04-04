using System;
using System.Linq;
using Common.Logging;
using Herms.Cqrs.Aggregate;
using Herms.Cqrs.Event;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Herms.Cqrs.Azure
{
    public class AzureEventRepository<TAggregate> : IAggregateRepository<TAggregate> where TAggregate : IAggregate, IEventSourced, new()
    {
        private readonly string _tableName;
        private readonly string GuidStringFormat = "D";
        private readonly ILog _log;
        private CloudTable _table;

        public AzureEventRepository(string connectionString, string tableName)
        {
            _log = LogManager.GetLogger(this.GetType());
            _tableName = tableName;
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            this.CreateTableReference(tableName, storageAccount);
        }

        public AzureEventRepository(bool clean = false)
        {
            _log = LogManager.GetLogger(this.GetType());
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var tableName = typeof (TAggregate).Name;
            this.CreateTableReference(tableName, storageAccount, clean);
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
                    EventType = @event.GetType().FullName,
                    AssemblyName = @event.GetType().Assembly.GetName().Name,
                    Version = @event.Version,
                    Payload = JsonConvert.SerializeObject(@event)
                };
                var operation = TableOperation.Insert(eventEntity);
                batch.Add(operation);
            }
            var result = _table.ExecuteBatch(batch);
            if (result.Any(r => r.HttpStatusCode != 200)) {}
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
                aggregate.Load(eventEntities.Select(e => (IEvent)JsonConvert.DeserializeObject(e.Payload, this.GetTypeFromEntity(e))));
                return aggregate;
            }
            return default(TAggregate);
        }

        private Type GetTypeFromEntity(EventEntity eventEntity)
        {
            var fullyQualifiedTypeName = $"{eventEntity.EventType}, {eventEntity.AssemblyName}";
            if(_log.IsTraceEnabled)
                _log.Trace($"Trying to get type by name: {fullyQualifiedTypeName}");
            var typeFromEntity = Type.GetType(fullyQualifiedTypeName);
            if(typeFromEntity == null)
                _log.Warn($"Could not find type {fullyQualifiedTypeName}"); 
            return typeFromEntity;
        }

        private void CreateTableReference(string tableName, CloudStorageAccount storageAccount, bool clean = false)
        {
            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference(tableName);
            if (clean)
                _table.DeleteIfExists();
            if (_table.CreateIfNotExists())
                _log.Info($"Created table {tableName}.");
        }
    }

    public class EventEntity : TableEntity
    {
        public string AggregateType { get; set; }
        public string EventType { get; set; }
        public string AssemblyName { get; set; }
        public int Version { get; set; }
        public string Payload { get; set; }

        public EventEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey) {}

        public EventEntity() {}
    }
}