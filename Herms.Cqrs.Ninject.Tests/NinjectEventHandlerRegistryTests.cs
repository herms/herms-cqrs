using System;
using Herms.Cqrs.Event;
using Herms.Cqrs.TestContext.EventHandlers;
using Herms.Cqrs.TestContext.Events;
using Ninject;
using Xunit;

namespace Herms.Cqrs.Ninject.Tests
{
    public class NinjectEventHandlerRegistryTests
    {
        [Fact]
        public void GivenEventHandlersInRegistry_WhenRequestingHandlersForAnEventType_ThenTheCorrespondingHandlersShouldBeReturned()
        {
            var kernel = new StandardKernel();
            var eventHandlerRegistry = new NinjectEventHandlerRegistry(kernel);

            eventHandlerRegistry.RegisterImplementation(typeof (TestEventHandler1));
            eventHandlerRegistry.RegisterImplementation(typeof (TestEventHandler2));

            var event1 = new TestEvent1();
            var handlersForEvent1 = eventHandlerRegistry.ResolveHandlers(event1);
            Assert.NotNull(handlersForEvent1);
            Assert.Equal(1, handlersForEvent1.Count);

            var event2 = new TestEvent2();
            var handlersForEvent2 = eventHandlerRegistry.ResolveHandlers(event2);
            Assert.NotNull(handlersForEvent2);
            Assert.Equal(2, handlersForEvent2.Count);

            var event3 = new TestEvent3();
            var handlersForEvent3 = eventHandlerRegistry.ResolveHandlers(event3);
            Assert.NotNull(handlersForEvent3);
            Assert.Equal(1, handlersForEvent3.Count);
        }
    }
}