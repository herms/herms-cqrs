using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Logging;
using Herms.Cqrs.Aggregate;
using Herms.Cqrs.Event;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Herms.Cqrs.File
{
    public class FileSystemEventRepository<TAggregate> : IAggregateRepository<TAggregate>
        where TAggregate : IAggregate, IEventSourced, new()
    {
        private readonly ILog _log;
        private readonly string _aggregateTypePath;

        public FileSystemEventRepository()
        {
            // GetLogger(this.GetType()) produces a very long logger name. Using generic type instead.
            _log = LogManager.GetLogger(typeof(FileSystemEventRepository<>));
            _aggregateTypePath = $"{Directory.GetCurrentDirectory()}\\{typeof (TAggregate).Name}";
            _log.Debug($"Event store path is set to {_aggregateTypePath}.");
        }

        public void Save(TAggregate aggregate)
        {
            var events = aggregate.GetChanges().ToList();
            _log.Info($"Saving {events.Count} new events for aggregate {aggregate.Id} of type {typeof (TAggregate).Name}.");
            foreach (var @event in events)
            {
                var envelope = new EventEnvelope
                {
                    EventData = @event,
                    EventType = @event.GetType().FullName,
                    AssemblyName = @event.GetType().Assembly.FullName
                };
                var json = JsonConvert.SerializeObject(envelope);
                if (!Directory.Exists(_aggregateTypePath))
                    Directory.CreateDirectory(_aggregateTypePath);
                var aggregatePath = this.GetAggregatePath(@event.AggregateId);
                if (!Directory.Exists(aggregatePath))
                    Directory.CreateDirectory(aggregatePath);
                var path = $"{aggregatePath}\\{GetFileNameFromEventVersion(@event)}.json";
                _log.Trace("Write to path: " + path);
                try
                {
                    System.IO.File.WriteAllText(path, json);
                }
                catch (Exception exception)
                {
                    _log.Error(exception.Message);
                }
            }
        }

        public TAggregate Get(Guid id)
        {
            if (!Directory.Exists(_aggregateTypePath))
                throw new FileNotFoundException($"Could not find folder for aggregate type {typeof (TAggregate).Name}");
            if (!Directory.Exists(this.GetAggregatePath(id)))
                throw new FileNotFoundException($"Could not find folder for aggregate {id} of type {typeof (TAggregate).Name}");
            var eventFiles = Directory.GetFiles(this.GetAggregatePath(id), "*.json").OrderBy(s => s);
            var events = new List<IEvent>();
            foreach (var eventFile in eventFiles)
            {
                var contents = System.IO.File.ReadAllText(eventFile);
                var jObject = JsonConvert.DeserializeObject<JObject>(contents);
                var eventType = jObject["EventType"].Value<string>();
                var assemblyName = jObject["AssemblyName"].Value<string>();
                _log.Debug($"Read event of type {eventType}. Trying to deserialize...");
                var type = Type.GetType(eventType + ", " + assemblyName, true);
                _log.Trace(type.Name);
                var payload = jObject["EventData"].ToString();
                _log.Trace(payload);
                var @event = (IEvent) JsonConvert.DeserializeObject(payload, type);
                events.Add(@event);
            }
            try
            {
                var testAggregate = new TAggregate();
                testAggregate.Load(events);
                return testAggregate;
            }
            catch (Exception exception)
            {
                _log.Error("Could not materialize aggregate from event stream. " + exception.Message);
            }

            return default(TAggregate);
        }

        private static string GetFileNameFromEventVersion(IEvent @event)
        {
            return @event.Version.ToString("00000000");
        }

        private string GetAggregatePath(Guid id)
        {
            return _aggregateTypePath + "\\" + id.ToString("N");
        }
    }
}