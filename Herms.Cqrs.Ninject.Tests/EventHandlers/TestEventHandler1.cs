using System;
using Common.Logging;
using Herms.Cqrs.Ninject.Tests.Events;

namespace Herms.Cqrs.Ninject.Tests.EventHandlers
{
    public class TestEventHandler1 : IEventHandler<TestEvent1>, IEventHandler<TestEvent2>
    {
        private ILog _log;

        public TestEventHandler1()
        {
            _log = LogManager.GetLogger(GetType());
        }

        public EventHandlerResult Handle(TestEvent1 @event)
        {
            _log.Debug($"{GetType().Name} handling {@event.GetType().Name}");
            return new EventHandlerResult();
        }

        public EventHandlerResult Handle(TestEvent2 @event)
        {
            _log.Debug($"{GetType().Name} handling {@event.GetType().Name}");
            return new EventHandlerResult();
        }
    }
}