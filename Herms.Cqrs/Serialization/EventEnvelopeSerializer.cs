using System;
using Common.Logging;
using Herms.Cqrs.Event;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Herms.Cqrs.Serialization
{
    public static class EventEnvelopeSerializer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(EventEnvelopeSerializer));

        public static IEvent DeserializeEvent(string eventEnvelope)
        {
            var jObject = JsonConvert.DeserializeObject<JObject>(eventEnvelope);
            var eventType = jObject[EventEnvelope.EventTypeField].Value<string>();
            var assemblyName = jObject[EventEnvelope.AssemblyNameField].Value<string>();
            Log.Debug($"Read event of type {eventType}. Trying to deserialize to {assemblyName}.");
            var type = Type.GetType(assemblyName, true);
            var payload = jObject[EventEnvelope.EventDataField].ToString();
            var @event = (IEvent) JsonConvert.DeserializeObject(payload, type);
            return @event;
        }

        public static string SerializeEvent(IEvent @event)
        {
            var envelope = CreateEventEnvelope(@event);
            var contents = JsonConvert.SerializeObject(envelope);
            return contents;
        }

        private static EventEnvelope CreateEventEnvelope(IEvent @event)
        {
            var envelope = new EventEnvelope
            {
                AssemblyName = @event.GetType().AssemblyQualifiedName,
                EventData = @event,
                EventType = @event.GetType().FullName
            };
            return envelope;
        }
    }
}