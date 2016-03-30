using System;
using System.Collections.Generic;
using Herms.Cqrs.Aggregate;
using Herms.Cqrs.Event;
using Herms.Cqrs.TestContext.Commands;
using Herms.Cqrs.TestContext.Events;

namespace Herms.Cqrs.TestContext.Models
{
    public class TestAggregate : EventSourcedAggregateBase, IEventSourced
    {
        private static readonly List<Type> EventTypes;

        static TestAggregate()
        {
            EventTypes = new List<Type> { typeof (TestEvent1), typeof (TestEvent2) };
        }

        public TestAggregate()
        {
            Id = Guid.NewGuid();
        }

        public void Load(IEnumerable<IEvent> events)
        {
            foreach (var @event in events)
            {
                if (this.CanHandle(@event))
                    Apply((dynamic) @event);
                else
                    throw new ArgumentException(
                        $"Aggregate of type {this.GetType().Name} can not apply event of type {@event.GetType().Name}.");
            }
        }

        public void InvokeCommand1(TestCommand1 command)
        {
            var testEvent1 = new TestEvent1();
            this.Apply(testEvent1);
        }

        public void InvokeCommand2(TestCommand2 command)
        {
            var testEvent2 = new TestEvent2();
            this.Apply(testEvent2);
        }

        private bool CanHandle(IEvent @event)
        {
            return EventTypes.Contains(@event.GetType());
        }

        private void Apply(TestEvent1 testEvent1, bool replay = false)
        {
            Version++;
            if (!replay)
            {
                Changes.Add(testEvent1);
                this.TagVersionedEvent(testEvent1);
            }
            this.VerfiyVersion(testEvent1);
        }

        private void Apply(TestEvent2 testEvent2, bool replay = false)
        {
            Version++;
            if (!replay)
            {
                Changes.Add(testEvent2);
                this.TagVersionedEvent(testEvent2);
            }
            this.VerfiyVersion(testEvent2);
        }
    }
}