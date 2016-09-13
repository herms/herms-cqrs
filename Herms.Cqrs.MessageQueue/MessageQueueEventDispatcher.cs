using System;
using System.Messaging;
using Common.Logging;
using Herms.Cqrs.Event;

namespace Herms.Cqrs.Msmq
{
    public class MessageQueueEventDispatcher : IEventDispatcher
    {
        private readonly ILog _log;
        private MessageQueue _queue;

        public MessageQueueEventDispatcher(string queueName)
        {
            _log = LogManager.GetLogger(this.GetType());
            this.InitializeQueue(queueName);
        }

        public void Publish(IEvent @event)
        {
            _queue.Send(@event);
        }

        private void InitializeQueue(string queueName)
        {
            if (MessageQueue.Exists(queueName))
            {
                _queue = new MessageQueue(queueName, false);
                _log.Debug($"Found queue {queueName}.");
            }
            else
            {
                _queue = MessageQueue.Create(queueName);
                _log.Info($"Created queue {queueName}.");
            }
            _queue.Formatter = new BinaryMessageFormatter();
        }
    }
}