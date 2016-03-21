using System;
using System.Linq;
using Herms.Cqrs.Event;
using Ninject;
using Xunit;

namespace Herms.Cqrs.Ninject.Tests
{
    public class EventHandlerRegistryTests

    {
        [Fact]
        public void
            GivenEventHandlersInRegistry_WhenRequestingHandlersForAnEventType_ThenTheCorrespondingHandlersShouldBeReturned
            ()
        {
            var kernel = new StandardKernel();
            var eventHandlerRegistry = new NinjectEventHandlerRegistry(kernel);

            kernel.Bind<IEventHandler<TestEvent1>>().To<TestEventHandler1>();
            kernel.Bind<IEventHandler<TestEvent2>>().To<TestEventHandler1>();
            kernel.Bind<IEventHandler<TestEvent2>>().To<TestEventHandler2>();
            kernel.Bind<IEventHandler<TestEvent3>>().To<TestEventHandler2>();

            var bindingsForEvent1 = kernel.GetBindings(typeof(IEventHandler<TestEvent1>));
            var bindingsForEvent2 = kernel.GetBindings(typeof(IEventHandler<TestEvent2>));
            var bindingsForEvent3 = kernel.GetBindings(typeof(IEventHandler<TestEvent3>));

            Assert.Equal(1, bindingsForEvent1.ToList().Count);
            Assert.Equal(2, bindingsForEvent2.ToList().Count);
            Assert.Equal(1, bindingsForEvent3.ToList().Count);
        }
    }

    public class TestEventHandler1 : IEventHandler<TestEvent1>, IEventHandler<TestEvent2>
    {
        public void Handle(TestEvent1 @event)
        {
            throw new NotImplementedException();
        }

        public void Handle(TestEvent2 @event)
        {
            throw new NotImplementedException();
        }
    }

    public class TestEventHandler2 : IEventHandler<TestEvent2>, IEventHandler<TestEvent3>
    {
        public void Handle(TestEvent2 @event)
        {
            throw new NotImplementedException();
        }

        public void Handle(TestEvent3 @event)
        {
            throw new NotImplementedException();
        }
    }

    public class TestEvent3 : VersionedEvent {}

    public class TestEvent2 : VersionedEvent {}

    public class TestEvent1 : VersionedEvent {}
}