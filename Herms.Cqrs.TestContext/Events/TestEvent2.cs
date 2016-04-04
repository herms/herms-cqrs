using System;
using Herms.Cqrs.Event;

namespace Herms.Cqrs.TestContext.Events
{
    public class TestEvent2 : VersionedEvent, IEvent
    {
        public string Param1 { get; set; }
    }
}