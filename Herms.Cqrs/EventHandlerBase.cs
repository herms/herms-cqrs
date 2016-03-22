using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Herms.Cqrs.Event;

namespace Herms.Cqrs
{
    public class EventHandlerBase<T>
    {
        private static readonly List<Type> EventTypes;
        private readonly ILog _log;

        static EventHandlerBase()
        {
            EventTypes = new List<Type>();
            var handlerInterfaces =
                typeof (T).GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IEventHandler<>));
            foreach (var handlerInterface in handlerInterfaces)
            {
                var genericArguments = handlerInterface.GetGenericArguments();
                if (genericArguments.Length == 1 && typeof (IEvent).IsAssignableFrom(genericArguments[0]))
                {
                    EventTypes.Add(genericArguments[0]);
                }
            }
        }

        protected EventHandlerBase()
        {
            _log = LogManager.GetLogger(GetType());
        }

        protected bool CanHandle(IEvent @event, Type handlerType)
        {
            _log.Debug($"Checking whether {handlerType.Name} can handle {@event.GetType().Name}.");

            if (EventTypes.Contains(@event.GetType()))
            {
                _log.Debug($"{handlerType.Name} can handle {@event.GetType().Name}.");
                return true;
            }
            _log.Trace($"{handlerType.Name} can't handle {@event.GetType().Name}.");
            return false;
        }
    }
}