using System;

namespace Herms.Cqrs.Event
{
    public interface IApplyEvent<in T>
    {
        void Apply(T @event, bool replay);
    }
}