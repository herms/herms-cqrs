using System;
using System.Collections.Generic;
using System.Linq;
using Herms.Cqrs.Event;

namespace Herms.Cqrs
{
    public static class GenericArgumentExtractor
    {
        public static List<Type> GetApplicableEvents(Type implementationType)
        {
            return GetGenericArgumentsFromInterface(implementationType, typeof (IApplyEvent<>), typeof (IEvent));
        }

        public static List<Type> GetHandledEvents(Type implementationType)
        {
            return GetGenericArgumentsFromInterface(implementationType, typeof (IEventHandler<>), typeof (IEvent));
        }

        private static List<Type> GetGenericArgumentsFromInterface(Type implementation, Type genericInterface, Type genericBaseType)
        {
            var types = new List<Type>();
            var handlerInterfaces =
                implementation.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface);
            foreach (var handlerInterface in handlerInterfaces)
            {
                var genericArguments = handlerInterface.GetGenericArguments();
                if (genericArguments.Length == 1 && genericBaseType.IsAssignableFrom(genericArguments[0]))
                {
                    types.Add(genericArguments[0]);
                }
            }
            return types;
        }
    }
}