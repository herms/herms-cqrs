using System;
using System.Collections.Generic;
using Herms.Cqrs.Event;
using Herms.Cqrs.TestContext.Commands;
using Herms.Cqrs.TestContext.Events;

namespace Herms.Cqrs.TestContext.Models
{
    public class TestAggregate : Aggregate, IAggregate, IApplyEvent<TestEvent1>, IApplyEvent<TestEvent2>
    {
        private static readonly List<Type> EventTypes;

        static TestAggregate()
        {
            EventTypes = GenericArgumentExtractor.GetApplicableEvents(typeof (TestAggregate));
        }

        public TestAggregate()
        {
            Id = Guid.NewGuid();
        }

        public void Apply(IEvent @event, bool replay)
        {
            if (EventTypes.Contains(@event.GetType()))
                Apply((dynamic) @event, replay);
        }

        public void Apply(TestEvent1 testEvent1, bool replay = false)
        {
            Version++;
            if (!replay)
            {
                Changes.Add(testEvent1);
                this.TagVersionedEvent(testEvent1);
            }
            this.VerfiyVersion(testEvent1);
        }

        public void Apply(TestEvent2 testEvent2, bool replay = false)
        {
            Version++;
            if (!replay)
            {
                Changes.Add(testEvent2);
                this.TagVersionedEvent(testEvent2);
            }
            this.VerfiyVersion(testEvent2);
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
    }
}