using System;
using System.Collections.Generic;
using System.Linq;

namespace Herms.Cqrs.Registration
{
    public static class HandlerDefinitionCollection
    {
        public static IEnumerable<HandlerDefinition> GetEventHandlerDefinitionsFromImplementation(Type implementation)
        {
            return GetHandlerDefinitionsForTypeFromImplementation(typeof(IEventHandler<>), implementation);
        }
        public static IEnumerable<HandlerDefinition> GetCommandHandlerDefinitionsFromImplementation(Type implementation)
        {
            return GetHandlerDefinitionsForTypeFromImplementation(typeof(ICommandHandler<>), implementation);
        }

        private static IEnumerable<HandlerDefinition> GetHandlerDefinitionsForTypeFromImplementation(Type definition, Type implementation)
        {
            foreach (var handler in implementation.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == definition))
            {
                var genericArgument = handler.GetGenericArguments()[0];
                yield return new HandlerDefinition
                {
                    Handler = handler,
                    Argument = genericArgument,
                    Implementation = implementation
                };
            }

        }
    }
}