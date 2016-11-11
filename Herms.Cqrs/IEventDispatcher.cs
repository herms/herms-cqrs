using System;
using System.Threading.Tasks;
using Herms.Cqrs.Event;

namespace Herms.Cqrs
{
    public interface IEventDispatcher
    {
        Task PublishAsync(IEvent @event);
    }
}