using System;
using System.Collections.Generic;
using System.Linq;
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
            var bindings = _kernel.GetBindings(typeof (IEventHandler<T>));
            return bindings.Select(binding => _kernel.Get<IEventHandler<T>>(binding.BindingConfiguration.Metadata.Name));
        }

        public void RegisterHandler(Type handler)
        {
            var handlers =
                handler.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IEventHandler<>));
            foreach (var eventHandler in handlers)
            {
                var typeArgument = eventHandler.GetGenericArguments()[0];
                if (typeof (IEvent).IsAssignableFrom(typeArgument))
                {
                    var eventType = typeArgument;
                    Console.WriteLine($"Handling for event {typeArgument.Name} found in type {handler.Name}.");
                    _kernel.Bind(eventHandler).To(handler).Named(CreateEventHandlerName(handler, eventType));
                }
            }
        }

        private static string CreateEventHandlerName(Type handler, Type eventType)
        {
            return $"{handler.Name}_{eventType.Name}";
        }
    }
}