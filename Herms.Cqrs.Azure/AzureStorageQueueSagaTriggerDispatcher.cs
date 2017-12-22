using System;
using System.Threading.Tasks;
using Common.Logging;
using Herms.Cqrs.Saga;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Herms.Cqrs.Azure {
    public class AzureStorageQueueSagaTriggerDispatcher : AzureStorageQueueDispatcher, ISagaTriggerDispatcher
    {
        private readonly ILog _log;
        public AzureStorageQueueSagaTriggerDispatcher(AzureStorageQueueConfiguration queueConfiguration):base(queueConfiguration)
        {
            if (queueConfiguration == null)
                throw new ArgumentNullException(nameof(queueConfiguration));
            _log = LogManager.GetLogger(this.GetType());
        }

        public async Task TriggerSagaAsync(Guid id)
        {
            var message = new CloudQueueMessage(id.ToString("D"));
            await this.PublishAsync(message);
            _log.Trace($"Trigger signal for saga {id} published.");
        }
    }
}