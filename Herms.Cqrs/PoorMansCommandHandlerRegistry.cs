using System;
using System.Collections.Generic;
using Common.Logging;
using Herms.Cqrs.Commands;
using Herms.Cqrs.Registration;

namespace Herms.Cqrs {
    public class PoorMansCommandHandlerRegistry:ICommandHandlerRegistry
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(PoorMansCommandHandlerRegistry));
        private readonly Dictionary<Type, Type> _handlerMapping = new Dictionary<Type, Type>();
        public void Register(Type handlerType, Type implementationType)
        {
            if (!handlerType.IsGenericType && handlerType.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
            {
                var errorMsg = $"Type {handlerType.Name} is not of type {typeof(ICommandHandler<>).Name}.";
                _log.Error(errorMsg);
                throw new ArgumentException(errorMsg);
            }
            var genericArguments = handlerType.GetGenericArguments();
            if (genericArguments.Length != 1 || !typeof(CommandBase).IsAssignableFrom(genericArguments[0]))
            {
                var errorMsg = $"{implementationType.Name} contains a command handler which does not comply with signature.";
                _log.Warn(errorMsg);
                throw new ArgumentException(errorMsg);
            }
            var commandType = genericArguments[0];
            _log.Debug(
                $"Handling for command {commandType.Name} found in type {implementationType.Name}.");
            if (_handlerMapping.ContainsKey(commandType))
            {
                var errorMsg = $"A command handler for command {commandType.Name} is already registered.";
                throw new ArgumentException(errorMsg);
            }
            _handlerMapping.Add(handlerType, implementationType);
        }                                           

        public void Register(IEnumerable<HandlerDefinition> handlerDefinitions)
        {
            foreach (var handlerDefinition in handlerDefinitions)
            {
                this.Register(handlerDefinition.Handler, handlerDefinition.Implementation);
            }
        }
        public ICommandHandler<T> ResolveHandler<T>(T commandType) where T : CommandBase
        {
            return null;
        }
    }
}