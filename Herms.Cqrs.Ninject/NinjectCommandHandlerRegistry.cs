using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Herms.Cqrs.Commands;
using Herms.Cqrs.Registration;
using Ninject;

namespace Herms.Cqrs.Ninject
{
    public class NinjectCommandHandlerRegistry : ICommandHandlerRegistry
    {
        private readonly IKernel _kernel;
        private readonly ILog _log;

        public NinjectCommandHandlerRegistry(IKernel kernel)
        {
            _log = LogManager.GetLogger(this.GetType());
            _kernel = kernel;
        }

        public void Register(Type handlerType, Type implementationType)
        {
            if (!handlerType.IsGenericType && handlerType.GetGenericTypeDefinition() == typeof (ICommandHandler<>))
            {
                var errorMsg = $"Type {handlerType.Name} is not of type {typeof (ICommandHandler<>).Name}.";
                _log.Error(errorMsg);
                throw new ArgumentException(errorMsg);
            }
            var genericArguments = handlerType.GetGenericArguments();
            if (genericArguments.Length != 1 || !typeof (CommandBase).IsAssignableFrom(genericArguments[0]))
            {
                var errorMsg = $"{implementationType.Name} contains a command handler which does not comply with signature.";
                _log.Warn(errorMsg);
                throw new ArgumentException(errorMsg);
            }
            var commandType = genericArguments[0];
            _log.Debug(
                $"Handling for command {commandType.Name} found in type {implementationType.Name}.");
            if (_kernel.TryGet(handlerType) != null)
            {
                var errorMsg = $"A command handler for command {commandType.Name} is already registered.";
                throw new ArgumentException(errorMsg);
            }
            _kernel.Bind(handlerType)
                .To(implementationType)
                .Named(this.CreateCommandHandlerName(implementationType, commandType));
        }

        public void Register(IEnumerable<HandlerDefinition> definitions)
        {
            foreach (var handlerDefinition in definitions)
            {
                this.Register(handlerDefinition.Handler, handlerDefinition.Implementation);
            }
        }

        public ICommandHandler<T> ResolveHandler<T>(T commandType) where T : CommandBase
        {
            return (ICommandHandler<T>) _kernel.Get(typeof (ICommandHandler<T>));
        }

        public void RegisterImplementation(Type implementationType)
        {
            var commandHandlers = HandlerDefinitionCollection.GetCommandHandlerDefinitionsFromImplementation(implementationType);
            this.Register(commandHandlers);
        }

        private string CreateCommandHandlerName(Type handlerType, Type commandType)
        {
            var commandHandlerName = $"{handlerType.Name}_{commandType.Name}";
            if (_log.IsTraceEnabled)
                _log.Trace($"Handler name for type {handlerType.Name} handling command {commandType.Name}: {commandHandlerName}.");
            return $"{handlerType.Name}_{commandType.Name}";
        }
    }
}