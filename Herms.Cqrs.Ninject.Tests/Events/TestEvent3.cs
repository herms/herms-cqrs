using System;
using Herms.Cqrs.Event;

namespace Herms.Cqrs.Ninject.Tests.Events
{
    public class TestEvent3 : VersionedEvent, IEvent {}
}