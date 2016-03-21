using System;
using System.Linq;
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

            var bindingsForEvent1 = eventHandlerRegistry.ResolveHandlers(new TestEvent1()).ToList();
            var bindingsForEvent2 = eventHandlerRegistry.ResolveHandlers(new TestEvent2()).ToList();
            var bindingsForEvent3 = eventHandlerRegistry.ResolveHandlers(new TestEvent3()).ToList();

            Console.WriteLine($"Bindings for {nameof(TestEvent1)}");
            bindingsForEvent1.ForEach(b => Console.WriteLine(" - " + b.GetType().Name));
            Console.WriteLine($"Bindings for {nameof(TestEvent2)}");
            bindingsForEvent2.ForEach(b => Console.WriteLine(" - " + b.GetType().Name));
            Console.WriteLine($"Bindings for {nameof(TestEvent3)}");
            bindingsForEvent3.ForEach(b => Console.WriteLine(" - " + b.GetType().Name));

            Assert.Equal(1, bindingsForEvent1.Count);
            Assert.Equal(2, bindingsForEvent2.Count);
            Assert.Equal(1, bindingsForEvent3.Count);
        }
    }
}