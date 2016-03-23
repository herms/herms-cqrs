using System;
using System.Collections.Generic;
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
            EventTypes = GenericArgumentExtractor.GetHandledEvents(typeof (T));
        }

        protected EventHandlerBase()
        {
            _log = LogManager.GetLogger(this.GetType());
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