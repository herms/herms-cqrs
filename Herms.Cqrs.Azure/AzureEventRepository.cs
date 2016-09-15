using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
using Herms.Cqrs.Aggregate;
using Herms.Cqrs.Aggregate.Exceptions;
using Herms.Cqrs.Event;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Herms.Cqrs.Azure
{
    public class AzureEventRepository<TAggregate> : IAggregateRepository<TAggregate> where TAggregate : IAggregate, IEventSourced, new()
    {
        private readonly string GuidStringFormat = "D";
        private readonly ILog _log;
        private CloudTable _table;

        public AzureEventRepository(string connectionString, string tableName, bool clean)
        {
            _log = LogManager.GetLogger(typeof (AzureEventRepository<>));
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var createTableTask = this.CreateTableReference(tableName, storageAccount, clean);
            createTableTask.Wait();
        }

        public AzureEventRepository(string connectionString) : this(connectionString, typeof (TAggregate).Name, false) {}

        public async Task SaveAsync(TAggregate aggregate)
        {
            var batch = new TableBatchOperation();
            foreach (var @event in aggregate.GetChanges())
            {
                var eventEntity = new EventEntity(@event.AggregateId.ToString(GuidStringFormat), @event.Id.ToString(GuidStringFormat))
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
            var result = await _table.ExecuteBatchAsync(batch);
            if (result.Any(IsNotSuccessStatus))
            {
                _log.Fatal($"Error while saving events!");
                foreach (var r in result.Where(IsNotSuccessStatus))
                {
                    _log.Error($"HTTP {r.HttpStatusCode} for {r.Result}.");
                }
            }
        }


        public void Save(TAggregate aggregate)
        {
            var saveTask = this.SaveAsync(aggregate);
            saveTask.Wait();
        }

        public TAggregate Get(Guid id)
        {
            var query =
                new TableQuery<EventEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
                    id.ToString(GuidStringFormat)));
            var results = _table.ExecuteQuery(query);
            var eventEntities = results.OrderBy(e => e.Version).ToList();
            if (eventEntities.Any())
            {
                _log.Debug($"Found {eventEntities.Count} events.");
                var aggregate = new TAggregate();
                var events = eventEntities.Select(e => (IEvent) JsonConvert.DeserializeObject(e.Payload, this.GetTypeFromEntity(e)));
                var eventList = events as IList<IEvent> ?? events.ToList();
                _log.Info($"Deserialized {eventList.Count} events...");
                if (eventEntities.Count != eventList.Count)
                {
                    var errorMessage = "Number of events found did not match number of events deserialized.";
                    _log.Error(errorMessage);
                    throw new AggregateLoadingException(errorMessage);
                }
                aggregate.Load(eventList);
                return aggregate;
            }
            return default(TAggregate);
        }

        private static bool IsNotSuccessStatus(TableResult r)
        {
            return r.HttpStatusCode < 200 || r.HttpStatusCode >= 300;
        }

        private Type GetTypeFromEntity(EventEntity eventEntity)
        {
            var fullyQualifiedTypeName = $"{eventEntity.EventType}, {eventEntity.AssemblyName}";
            if (_log.IsTraceEnabled)
                _log.Trace($"Trying to get type by name: {fullyQualifiedTypeName}");
            var typeFromEntity = Type.GetType(fullyQualifiedTypeName);
            if (typeFromEntity == null)
                _log.Warn($"Could not find type {fullyQualifiedTypeName}");
            return typeFromEntity;
        }

        private async Task CreateTableReference(string tableName, CloudStorageAccount storageAccount, bool clean = false)
        {
            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference(tableName);
            if (clean)
                await _table.DeleteIfExistsAsync();
            var created = await _table.CreateIfNotExistsAsync();
            if (created)
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