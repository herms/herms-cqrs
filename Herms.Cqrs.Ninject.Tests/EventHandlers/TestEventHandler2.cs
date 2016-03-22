using System;
using Common.Logging;
using Herms.Cqrs.Ninject.Tests.Events;

namespace Herms.Cqrs.Ninject.Tests.EventHandlers
{
    public class TestEventHandler2 : IEventHandler<TestEvent2>, IEventHandler<TestEvent3>
    {
        private readonly ILog _log;

        public TestEventHandler2()
        {
            _log = LogManager.GetLogger(GetType());
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