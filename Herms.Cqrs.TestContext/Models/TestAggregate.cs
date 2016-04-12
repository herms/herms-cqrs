using System;
using System.Collections.Generic;
using Common.Logging;
using Herms.Cqrs.Aggregate;
using Herms.Cqrs.Event;
using Herms.Cqrs.TestContext.Commands;
using Herms.Cqrs.TestContext.Events;

namespace Herms.Cqrs.TestContext.Models
{
    public class TestAggregate : EventSourcedAggregateBase
    {
        private readonly ILog _log;

        public string Prop1 { get; private set; }
        public List<string> Prop1History { get; private set; } = new List<string>();

        static TestAggregate()
        {
            EventTypes = new List<Type> { typeof (TestEvent1), typeof (TestEvent2) };
        }

        public TestAggregate()
        {
            _log = LogManager.GetLogger(this.GetType());
            Id = Guid.NewGuid();
        }

        public override void Load(IEnumerable<IEvent> events)
        {
            foreach (var @event in events)
            {
                if (@event == null)
                    throw new ArgumentNullException(nameof(@event));
                if (!this.CanHandle(@event))
                    throw new ArgumentException(
                        $"Aggregate of type {this.GetType().Name} can not apply event of type {@event.GetType().Name}.");
                Apply((dynamic) @event);
            }
        }

        public void InvokeCommand1(TestCommand1 command)
        {
            if (_log.IsTraceEnabled)
                _log.Trace($"Executing command {command.GetType()}.");
            var testEvent1 = new TestEvent1 { AggregateId = Id };
            this.Apply(testEvent1);
        }

        public void InvokeCommand2(TestCommand2 command)
        {
            if (_log.IsTraceEnabled)
                _log.Trace($"Executing command {command.GetType()}.");
            var testEvent2 = new TestEvent2 { Param1 = command.Param1 };
            this.Apply(testEvent2);
        }

        private void Apply(TestEvent1 testEvent1, bool replay = false)
        {
            Id = testEvent1.AggregateId;
            Version++;
            if (!replay)
            {
                Changes.Add(testEvent1);
                this.TagVersionedEvent(testEvent1);
            }
            this.VerfiyEventVersion(testEvent1);
            if (_log.IsTraceEnabled)
                _log.Trace($"Applied event {testEvent1.GetType()}.");
        }

        private void Apply(TestEvent2 testEvent2, bool replay = false)
        {
            Prop1 = testEvent2.Param1;
            Prop1History.Add(Prop1);
            Version++;
            if (!replay)
            {
                Changes.Add(testEvent2);
                this.TagVersionedEvent(testEvent2);
            }
            this.VerfiyEventVersion(testEvent2);
            if (_log.IsTraceEnabled)
                _log.Trace($"Applied event {testEvent2.GetType()}.");
        }
    }
}