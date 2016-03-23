using System;

namespace Herms.Cqrs.Event
{
    public interface IApplyEvent<in T> where T : IEvent
    {
        void Apply(T @event, bool replay);
    }
}