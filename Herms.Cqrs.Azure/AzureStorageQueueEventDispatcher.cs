using System;
using System.Threading.Tasks;
using Common.Logging;
using Herms.Cqrs.Event;

namespace Herms.Cqrs.Azure
{
    public class AzureStorageQueueEventDispatcher : AzureStorageQueueDispatcher, IEventDispatcher
    {
        private readonly ILog _log;

        public AzureStorageQueueEventDispatcher(AzureStorageQueueConfiguration queueConfiguration) : base(queueConfiguration)
        {
            if (queueConfiguration == null)
                throw new ArgumentNullException(nameof(queueConfiguration));
            _log = LogManager.GetLogger(this.GetType());
        }

        public async Task PublishAsync(IEvent @event)
        {
            var message = CloudQueueMessageSerializer.SerializeEventToMessage(@event);
            await this.PublishAsync(message);
            _log.Trace($"Event {@event.Id} published.");
        }
    }
}