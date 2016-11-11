using System;
using System.Threading.Tasks;
using Herms.Cqrs.Event;
using Herms.Cqrs.TestContext.Events;
using Xunit;

namespace Herms.Cqrs.Tests
{
    public class EventHandlerResultsTests
    {
        [Fact]
        public async Task GivenMultipleHandlersOneOfWhichThrowsException_WhenHandlingEvent_ThenResultStatusShouldBeHandlerFailure()
        {
            var eventHandlerRegistry = new PoorMansEventHandlerRegistry();
            eventHandlerRegistry.Register(typeof(IEventHandler<TestEvent1>), typeof(NegativeEventHandler));
            eventHandlerRegistry.Register(typeof(IEventHandler<TestEvent1>), typeof(PositiveEventHandler));

            var testEvent1 = new TestEvent1();
            var eventHandlers = eventHandlerRegistry.ResolveHandlers(testEvent1);

            Assert.Equal(2, eventHandlers.Count);
            var results = await eventHandlers.HandleAsync(testEvent1);
            Assert.Equal(EventHandlerResultType.HandlerFailed, results.Status);
            Assert.Equal(2, results.Items.Count);
            Assert.Equal(1, results.Failed.Count);
        }
    }

    public class NegativeEventHandler : IEventHandler, IEventHandler<TestEvent1>, IEventHandler<TestEvent2>
    {
        public async Task<EventHandlerResult> HandleAsync(IEvent @event)
        {
            if (this.CanHandle(@event))
                return await HandleAsync((dynamic) @event);
            throw new ArgumentException($"Can not handle events of type {@event.GetType().Name}.");
        }

        public bool CanHandle(IEvent @event)
        {
            if (@event is TestEvent1)
                return true;
            if (@event is TestEvent2)
                return true;
            return false;
        }

        public Task<EventHandlerResult> HandleAsync(TestEvent1 @event)
        {
            throw new NotImplementedException("How do you like that?");
        }

        public Task<EventHandlerResult> HandleAsync(TestEvent2 @event)
        {
            return Task.FromResult(EventHandlerResult.CreateSuccessResult(this.GetType()));
        }
    }

    public class PositiveEventHandler : IEventHandler, IEventHandler<TestEvent1>
    {
        public async Task<EventHandlerResult> HandleAsync(IEvent @event)
        {
            if (this.CanHandle(@event))
                return await HandleAsync((dynamic) @event);
            throw new ArgumentException($"Can not handle events of type {@event.GetType().Name}.");
        }

        public bool CanHandle(IEvent @event)
        {
            if (@event is TestEvent1)
                return true;
            return false;
        }

        public Task<EventHandlerResult> HandleAsync(TestEvent1 @event)
        {
            return Task.FromResult(EventHandlerResult.CreateSuccessResult(this.GetType()));
        }
    }
}