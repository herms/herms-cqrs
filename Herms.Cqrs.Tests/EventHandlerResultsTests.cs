using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Herms.Cqrs.Event;
using Herms.Cqrs.TestContext.Events;
using Xunit;

namespace Herms.Cqrs.Tests
{
    public class EventHandlerResultsTests
    {
        [Fact]
        public void GivenMultipleHandlersOneOfWhichThrowsException_WhenHandlingEvent_ThenResultStatusShouldBeHandlerFailure()
        {
            var eventHandlerRegistry = new PoorMansEventHandlerRegistry();
            eventHandlerRegistry.Register(typeof (IEventHandler<TestEvent1>), typeof (NegativeEventHandler));
            eventHandlerRegistry.Register(typeof (IEventHandler<TestEvent1>), typeof (PositiveEventHandler));

            var testEvent1 = new TestEvent1();
            var eventHandlers = eventHandlerRegistry.ResolveHandlers(testEvent1);

            Assert.Equal(2, eventHandlers.Count());

            var result = testEvent1.Handle(eventHandlerRegistry);

            Assert.Equal(EventHandlerResultType.HandlerFailed, result.Status);
        }
    }

    public class PoorMansEventHandlerRegistry : IEventHandlerRegistry
    {
        private readonly Dictionary<Type, List<Type>> _registry = new Dictionary<Type, List<Type>>();
        private readonly ILog _log;

        public PoorMansEventHandlerRegistry()
        {
            _log = LogManager.GetLogger(GetType());
        }

        public void Register(Type eventHandler, Type implementationType)
        {
            _log.Debug($"Register event handler {eventHandler.Name} in {implementationType.Name}");
            var genericArgument = eventHandler.GetGenericArguments()[0];
            _log.Debug($"Event type: {genericArgument.Name}.");
            List<Type> eventHandlers;
            if (_registry.ContainsKey(genericArgument))
            {
                eventHandlers = _registry[genericArgument];
            }
            else
            {
                eventHandlers = new List<Type>();
            }
            eventHandlers.Add(implementationType);
            _registry[genericArgument] = eventHandlers;
        }

        public IEnumerable<IEventHandler<T>> ResolveHandlers<T>(T eventType) where T : IEvent
        {
            var eventHandlers = new List<IEventHandler<T>>();
            var handlers = _registry[typeof (T)];
            foreach (var handler in handlers)
            {
                var eventHandler = (IEventHandler<T>) Activator.CreateInstance(handler);
                eventHandlers.Add(eventHandler);
            }
            return eventHandlers;
        }
    }

    public class NegativeEventHandler : IEventHandler<TestEvent1>, IEventHandler<TestEvent2>
    {
        public EventHandlerResult Handle(TestEvent1 @event)
        {
            throw new NotImplementedException("How do you like that?");
        }

        public EventHandlerResult Handle(TestEvent2 @event)
        {
            return new EventHandlerResult { HandlerName = GetType().Name };
        }
    }

    public class PositiveEventHandler : IEventHandler<TestEvent1>
    {
        public EventHandlerResult Handle(TestEvent1 @event)
        {
            return EventHandlerResult.CreateSuccessResult(GetType());
        }
    }
}