using System;
using Common.Logging;
using Herms.Cqrs.Event;
using Herms.Cqrs.TestContext.Events;

namespace Herms.Cqrs.TestContext.EventHandlers
{
    public class TestEventHandler2 : EventHandlerBase<TestEventHandler2>, IEventHandler, IEventHandler<TestEvent2>,
        IEventHandler<TestEvent3>
    {
        private readonly ILog _log;

        public TestEventHandler2()
        {
            _log = LogManager.GetLogger(this.GetType());
        }

        public EventHandlerResult Handle(IEvent @event)
        {
            if (this.CanHandle(@event))
                return Handle((dynamic) @event);
            throw new ArgumentException($"Can not handle event of type {@event.GetType().Name}.");
        }

        public bool CanHandle(IEvent @event)
        {
            return base.CanHandle(@event, this.GetType());
        }

        public EventHandlerResult Handle(TestEvent2 @event)
        {
            _log.Debug($"{this.GetType().Name} handling {@event.GetType().Name}");
            return new EventHandlerResult();
        }

        public EventHandlerResult Handle(TestEvent3 @event)
        {
            _log.Debug($"{this.GetType().Name} handling {@event.GetType().Name}");
            return new EventHandlerResult();
        }
    }
}