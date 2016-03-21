using System;
using Herms.Cqrs.Ninject.Tests.Events;

namespace Herms.Cqrs.Ninject.Tests.EventHandlers
{
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
}