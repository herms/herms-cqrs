using System;
using System.Threading.Tasks;
using Common.Logging;
using Herms.Cqrs.Event;
using Herms.Cqrs.TestContext.Events;

namespace Herms.Cqrs.TestContext.EventHandlers
{
    public class TestEventHandler1 : EventHandlerBase<TestEventHandler1>, IEventHandler, IEventHandler<TestEvent1>,
        IEventHandler<TestEvent2>
    {
        private readonly ILog _log;

        public TestEventHandler1()
        {
            _log = LogManager.GetLogger(this.GetType());
        }

        public Task<EventHandlerResult> HandleAsync(IEvent @event)
        {
            if (this.CanHandle(@event))
                return HandleAsync((dynamic) @event);
            throw new ArgumentException($"Can not handle event of type {@event.GetType().Name}");
        }

        public bool CanHandle(IEvent @event)
        {
            return base.CanHandle(@event, this.GetType());
        }

        public Task<EventHandlerResult> HandleAsync(TestEvent1 @event)
        {
            _log.Debug($"{this.GetType().Name} handling {@event.GetType().Name}");
            return Task.FromResult(EventHandlerResult.CreateSuccessResult(this.GetType()));
        }

        public Task<EventHandlerResult> HandleAsync(TestEvent2 @event)
        {
            _log.Debug($"{this.GetType().Name} handling {@event.GetType().Name}");
            return Task.FromResult(EventHandlerResult.CreateSuccessResult(this.GetType()));
        }
    }
}