using System;
using Herms.Cqrs.Ninject.Tests.Events;

namespace Herms.Cqrs.Ninject.Tests.EventHandlers
{
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
}