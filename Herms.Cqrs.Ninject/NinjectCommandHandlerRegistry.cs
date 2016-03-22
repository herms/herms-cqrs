using System;
using System.Linq;
using Common.Logging;
using Ninject;

namespace Herms.Cqrs.Ninject
{
    public class NinjectCommandHandlerRegistry : ICommandHandlerRegistry
    {
        private readonly IKernel _kernel;
        private readonly ILog _logger;

        public NinjectCommandHandlerRegistry(IKernel kernel)
        {
            _logger = LogManager.GetLogger(GetType());
            _kernel = kernel;
        }

        public void Register(Type handlerType, Type implementationType)
        {
            if (!handlerType.IsGenericType && handlerType.GetGenericTypeDefinition() == typeof (ICommandHandler<>))
            {
                var errorMsg = $"Type {handlerType.Name} is not of type {typeof (ICommandHandler<>).Name}.";
                _logger.Error(errorMsg);
                throw new ArgumentException(errorMsg);
            }
            var genericArguments = handlerType.GetGenericArguments();
            if (genericArguments.Length != 1 || !typeof (Command).IsAssignableFrom(genericArguments[0]))
            {
                var errorMsg = $"{implementationType.Name} contains a command handler which does not comply with signature.";
                _logger.Warn(errorMsg);
                throw new ArgumentException(errorMsg);
            }
            var commandType = genericArguments[0];
            _logger.Debug(
                $"Handling for command {commandType.Name} found in type {implementationType.Name}.");
            if (_kernel.TryGet(handlerType) != null)
            {
                var errorMsg = $"A command handler for command {commandType.Name} is already registered.";
                _logger.Warn(errorMsg);
                throw new ArgumentException(errorMsg);
            }
            _kernel.Bind(handlerType)
                .To(implementationType)
                .Named(CreateCommandHandlerName(implementationType, commandType));
        }

        public ICommandHandler<T> ResolveHandler<T>(T commandType) where T : Command
        {
            return (ICommandHandler<T>) _kernel.Get(typeof (ICommandHandler<T>));
        }

        public void RegisterImplementation(Type implementationType)
        {
            var commandHandlers =
                implementationType.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (ICommandHandler<>));
            foreach (var commandHandler in commandHandlers)
            {
                Register(commandHandler, implementationType);
            }
        }

        private string CreateCommandHandlerName(Type handlerType, Type commandType)
        {
            return $"{handlerType.Name}_{commandType.Name}";
        }
    }
}