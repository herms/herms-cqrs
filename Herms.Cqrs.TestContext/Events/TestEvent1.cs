using System;
using Herms.Cqrs.Event;

namespace Herms.Cqrs.TestContext.Events
{
    public class TestEvent1 : VersionedEvent, IEvent {}
}