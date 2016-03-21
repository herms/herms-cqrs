using System;
using System.Collections.Generic;
using Herms.Cqrs.Event;
using Ninject;

namespace Herms.Cqrs.Ninject
{
    public class NinjectEventHandlerRegistry : IEventHandlerRegistry
    {
        private readonly IKernel _kernel;

        public NinjectEventHandlerRegistry(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IEnumerable<IEventHandler<T>> ResolveHandlers<T>(T eventType) where T : IEvent
        {
            return null;
        }
    }
}