using System;
using System.Collections.Generic;
using Common.Logging;
using Herms.Cqrs.Commands;
using Herms.Cqrs.Registration;
using SimpleInjector;

namespace Herms.Cqrs.SimpleInjector
{
    public class SimpleInjectorCommandHandlerRegistry : ICommandHandlerRegistry

    {
        private readonly Container _container;
        private readonly ILog _log;

        /// <summary>
        /// Simple Injector locks the container after querying it, so some state is kept here instead.
        /// </summary>
        private List<Type> _registeredHandlers;

        public SimpleInjectorCommandHandlerRegistry(Container container)
        {
            _log = LogManager.GetLogger(this.GetType());
            _container = container;
            _registeredHandlers = new List<Type>();
        }

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
            if (_registeredHandlers.Contains(handlerType))
            {
                var errorMsg = $"A command handler for command {commandType.Name} is already registered.";
                throw new ArgumentException(errorMsg);
            }
            _registeredHandlers.Add(handlerType);
            _container.Register(handlerType, implementationType);
        }

        public void Register(IEnumerable<HandlerDefinition> handlerDefinitions)
        {
            foreach (var handlerDefinition in handlerDefinitions)
            {
                this.Register(handlerDefinition.Handler, handlerDefinition.Implementation);
            }
        }

        public void RegisterImplementation(Type implementationType)
        {
            var commandHandlers = HandlerDefinitionCollection.GetCommandHandlerDefinitionsFromImplementation(implementationType);
            this.Register(commandHandlers);
        }

        public ICommandHandler<T> ResolveHandler<T>(T commandType) where T : CommandBase
        {
            return _container.GetInstance<ICommandHandler<T>>();
        }
    }
}