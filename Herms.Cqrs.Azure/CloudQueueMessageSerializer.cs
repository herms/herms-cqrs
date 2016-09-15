using System;
using Common.Logging;
using Herms.Cqrs.Event;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Herms.Cqrs.Azure
{
    public class CloudQueueMessageSerializer
    {
        private readonly ILog _log;

        public CloudQueueMessageSerializer()
        {
            _log = LogManager.GetLogger(this.GetType());
        }

        public IEvent DeserializeMessageToEvent(CloudQueueMessage message)
        {
            var contents = message.AsString;
            var jObject = JsonConvert.DeserializeObject<JObject>(contents);
            var eventType = jObject[EventEnvelope.EventTypeField].Value<string>();
            var assemblyName = jObject[EventEnvelope.AssemblyNameField].Value<string>();
            _log.Debug($"Read event of type {eventType}. Trying to deserialize to {assemblyName}.");
            var type = Type.GetType(assemblyName, true);
            var payload = jObject[EventEnvelope.EventDataField].ToString();
            var @event = (IEvent) JsonConvert.DeserializeObject(payload, type);
            return @event;
        }

        public static CloudQueueMessage SerializeEventToMessage(IEvent @event)
        {
            var envelope = new EventEnvelope
            {
                AssemblyName = @event.GetType().AssemblyQualifiedName,
                EventData = @event,
                EventType = @event.GetType().FullName
            };
            var contents = JsonConvert.SerializeObject(envelope);
            var message = new CloudQueueMessage(contents);
            return message;
        }
    }
}