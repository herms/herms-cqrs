using System;
using Herms.Cqrs.Event;
using Herms.Cqrs.TestContext.EventHandlers;
using Herms.Cqrs.TestContext.Events;
using Ninject;
using Xunit;

namespace Herms.Cqrs.Ninject.Tests
{
    public class EventHandlerRegistryTests
    {
        [Fact]
        public void GivenEventHandlersInRegistry_WhenRequestingHandlersForAnEventType_ThenTheCorrespondingHandlersShouldBeReturned()
        {
            var kernel = new StandardKernel();
            var eventHandlerRegistry = new NinjectEventHandlerRegistry(kernel);

            eventHandlerRegistry.RegisterImplementation(typeof (TestEventHandler1));
            eventHandlerRegistry.RegisterImplementation(typeof (TestEventHandler2));

            var event1 = (IEvent) new TestEvent1();
            var result1 = event1.Handle(eventHandlerRegistry);
            Assert.Equal(EventHandlerResultType.Success, result1.Status);
            Assert.Equal(1, result1.Items.Count);

            var event2 = (IEvent) new TestEvent2();
            var result2 = event2.Handle(eventHandlerRegistry);
            Assert.Equal(EventHandlerResultType.Success, result2.Status);
            Assert.Equal(2, result2.Items.Count);

            var event3 = (IEvent) new TestEvent3();
            var result3 = event3.Handle(eventHandlerRegistry);
            Assert.Equal(EventHandlerResultType.Success, result3.Status);
            Assert.Equal(1, result3.Items.Count);
        }
    }
}