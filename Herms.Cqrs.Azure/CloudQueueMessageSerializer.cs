using System;
using Common.Logging;
using Herms.Cqrs.Commands;
using Herms.Cqrs.Event;
using Herms.Cqrs.Serialization;
using Microsoft.WindowsAzure.Storage.Queue;

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
            return EventEnvelopeSerializer.DeserializeEvent(contents);
        }

        public CommandBase DeserializeMessageToCommand(CloudQueueMessage message)
        {
            var contents = message.AsString;
            return CommandEnvelopeSerilizer.DeserializeCommand(contents);
        }

        public static CloudQueueMessage SerializeEventToMessage(IEvent @event)
        {
            var contents = EventEnvelopeSerializer.SerializeEvent(@event);
            return new CloudQueueMessage(contents);
        }
    }
}