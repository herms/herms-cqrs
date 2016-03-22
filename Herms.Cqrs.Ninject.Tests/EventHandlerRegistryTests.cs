using System;
using Herms.Cqrs.Event;
using Herms.Cqrs.Ninject.Tests.EventHandlers;
using Herms.Cqrs.Ninject.Tests.Events;
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

            eventHandlerRegistry.RegisterHandler(typeof (TestEventHandler1));
            eventHandlerRegistry.RegisterHandler(typeof (TestEventHandler2));

            var event1 = (IEvent) new TestEvent1();
            var result1 = event1.Handle(eventHandlerRegistry);
            Assert.True(result1.Success);
            Assert.Equal(1, result1.Results.Count);

            var event2 = (IEvent) new TestEvent2();
            var result2 = event2.Handle(eventHandlerRegistry);
            Assert.True(result2.Success);
            Assert.Equal(2, result2.Results.Count);

            var event3 = (IEvent) new TestEvent3();
            var result3 = event3.Handle(eventHandlerRegistry);
            Assert.True(result3.Success);
            Assert.Equal(1, result3.Results.Count);
        }
    }
}