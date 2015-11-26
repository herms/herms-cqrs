using System;
using Herms.Cqrs.Event;

namespace Herms.Cqrs
{
    public interface IEventBus
    {
        void Publish(IEvent @event);
    }
}