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
        private readonly bool _clean;
        private readonly IEventMappingRegistry _eventMappingRegistry;
        private readonly ILog _log;
        private readonly string _tableName;
        private readonly string GuidStringFormat = "D";
        private readonly CloudStorageAccount _storageAccount;
        private CloudTable _table;
        private bool _tableCreated;

        public AzureEventRepository(string connectionString, string tableName, IEventMappingRegistry eventMappingRegistry,
            bool clean = false)
        {
            _tableName = tableName;
            _eventMappingRegistry = eventMappingRegistry;
            _clean = clean;
            _log = LogManager.GetLogger(typeof(AzureEventRepository<>));
            _storageAccount = CloudStorageAccount.Parse(connectionString);

            /*var createTableTask = this.CreateTableReference(tableName, storageAccount, clean);
            createTableTask.Wait();*/
        }

        public async Task SaveAsync(TAggregate aggregate)
        {
            await this.CreateTableReferenceAsync();
            var batch = new TableBatchOperation();
            foreach (var @event in aggregate.GetChanges())
            {
                var eventName = this.GetEventName(@event);
                var eventEntity = new EventEntity(@event.AggregateId.ToString(GuidStringFormat), @event.Id.ToString(GuidStringFormat))
                {
                    AggregateType = typeof(TAggregate).Name,
                    EventName = eventName,
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
                    _log.Error($"HTTP {r.HttpStatusCode} for {r.Result}.");
            }
        }

        public void Save(TAggregate aggregate)
        {
            var saveTask = this.SaveAsync(aggregate);
            saveTask.Wait();
        }

        public TAggregate Get(Guid id)
        {
            var task = this.CreateTableReferenceAsync();
            task.Wait();
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
                    throw new AggregateLoadingException(id, typeof(TAggregate), errorMessage);
                }
                aggregate.Load(eventList);
                return aggregate;
            }
            return default(TAggregate);
        }

        private string GetEventName(IEvent @event)
        {
            if (_eventMappingRegistry != null)
                return _eventMappingRegistry.ResolveEventName(@event.GetType());
            return @event.GetType().FullName;
        }

        private static bool IsNotSuccessStatus(TableResult r)
        {
            return (r.HttpStatusCode < 200) || (r.HttpStatusCode >= 300);
        }

        private Type GetTypeFromEntity(EventEntity eventEntity)
        {
            return _eventMappingRegistry.ResolveEventType(eventEntity.EventName);
        }

        private async Task CreateTableReferenceAsync()
        {
            if (!_tableCreated)
            {
                var tableClient = _storageAccount.CreateCloudTableClient();
                _table = tableClient.GetTableReference(_tableName);
                if (_clean)
                    await _table.DeleteIfExistsAsync();
                var created = await _table.CreateIfNotExistsAsync();
                if (created)
                    _log.Info($"Created table {_tableName}.");
                _tableCreated = true;
            }
        }
    }

    public class EventEntity : TableEntity
    {
        public EventEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey) {}

        public EventEntity() {}
        public string AggregateType { get; set; }
        public string EventName { get; set; }
        public int Version { get; set; }
        public string Payload { get; set; }
    }
}