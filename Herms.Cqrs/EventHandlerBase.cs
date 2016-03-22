using System;
using System.Linq;
using Common.Logging;
using Herms.Cqrs.Event;

namespace Herms.Cqrs
{
    public class EventHandlerBase
    {
        private readonly ILog _log;

        protected EventHandlerBase()
        {
            _log = LogManager.GetLogger(GetType());
        }

        protected bool CanHandle(IEvent @event, Type handlerType)
        {
            _log.Debug($"Checking whether {handlerType.Name} can handle {@event.GetType().Name}.");
            var handlerInterfaces =
                handlerType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IEventHandler<>));
            foreach (var handlerInterface in handlerInterfaces)
            {
                _log.Trace($"Found handler interface: {handlerInterface.Name}.");
                var genericArguments = handlerInterface.GetGenericArguments();
                _log.Trace($"Generic arguments: {genericArguments.Length}.");
                if (genericArguments.Length == 1 && genericArguments[0] == @event.GetType())
                {
                    _log.Debug($"{handlerType.Name} can handle {@event.GetType().Name}.");
                    return true;
                }
            }
            _log.Trace($"{handlerType.Name} can't handle {@event.GetType().Name}.");
            return false;
        }
    }
}