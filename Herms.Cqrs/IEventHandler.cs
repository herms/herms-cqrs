using System;
using System.Threading.Tasks;
using Herms.Cqrs.Event;

namespace Herms.Cqrs
{
    public interface IEventHandler<in T> where T : IEvent
    {
        Task<EventHandlerResult> HandleAsync(T @event);
    }

    public interface IEventHandler
    {
        Task<EventHandlerResult> HandleAsync(IEvent @event);
        bool CanHandle(IEvent @event);
    }
}