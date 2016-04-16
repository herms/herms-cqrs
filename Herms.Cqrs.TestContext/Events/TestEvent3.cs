using System;
using Herms.Cqrs.Event;

namespace Herms.Cqrs.TestContext.Events
{
    [EventName("NewNameForTestEvent3")]
    public class TestEvent3 : VersionedEvent, IEvent {}
}