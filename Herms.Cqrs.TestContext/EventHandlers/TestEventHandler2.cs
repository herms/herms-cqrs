using System;
using Common.Logging;
using Herms.Cqrs.Event;
using Herms.Cqrs.TestContext.Events;

namespace Herms.Cqrs.TestContext.EventHandlers
{
    public class TestEventHandler2 : IEventHandler, IEventHandler<TestEvent2>, IEventHandler<TestEvent3>
    {
        private readonly ILog _log;

        public TestEventHandler2()
        {
            _log = LogManager.GetLogger(GetType());
        }

        public EventHandlerResult Handle(IEvent @event)
        {
            if (CanHandle(@event))
                return Handle((dynamic) @event);
            throw new ArgumentException($"Can not handle event of type {@event.GetType().Name}.");
        }

        public bool CanHandle(IEvent @event)
        {
            if (@event is TestEvent2)
                return true;
            if (@event is TestEvent3)
                return true;
            return false;
        }

        public EventHandlerResult Handle(TestEvent2 @event)
        {
            _log.Debug($"{GetType().Name} handling {@event.GetType().Name}");
            return new EventHandlerResult();
        }

        public EventHandlerResult Handle(TestEvent3 @event)
        {
            _log.Debug($"{GetType().Name} handling {@event.GetType().Name}");
            return new EventHandlerResult();
        }
    }
}