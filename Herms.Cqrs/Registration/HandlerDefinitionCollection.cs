using System;
using System.Collections.Generic;
using System.Linq;

namespace Herms.Cqrs.Registration
{
    public class HandlerDefinitionCollection
    {
        public static IEnumerable<HandlerDefinition> GetEventHandlerDefinitionsFromImplementation(Type implementation)
        {
            foreach (var handler in implementation.GetInterfaces()
                .Where(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(IEventHandler<>))))
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
        public static IEnumerable<HandlerDefinition> GetCommandHandlerDefinitionsFromImplementation(Type implementation)
        {
            foreach (var handler in implementation.GetInterfaces()
                .Where(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>))))
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