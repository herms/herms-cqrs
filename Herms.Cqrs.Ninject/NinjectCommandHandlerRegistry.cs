using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public ICommandHandler<T> ResolveHandler<T>(T commandType) where T : Command
        {
            return (ICommandHandler<T>) _kernel.Get(typeof (ICommandHandler<T>));
        }

        public void ScanAssembly(Assembly assembly)
        {
            _logger.Info($"Scanning assembly {assembly.FullName} for command handlers.");
            var handlersFoundInAssembly = 0;
            var typesWithHandlers = 0;
            foreach (var assemblyType in assembly.GetTypes())
            {
                var handlersFoundInType = 0;
                _logger.Trace($"Scanning type {assemblyType.FullName} for command handlers.");
                var commandHandlers =
                    assemblyType.GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (ICommandHandler<>));
                var commandHandlerList = commandHandlers as IList<Type> ?? commandHandlers.ToList();
                if (commandHandlerList.Any())
                {
                    if (assemblyType.IsPublic)
                    {
                        foreach (var commandHandler in commandHandlerList)
                        {
                            handlersFoundInType++;
                            var typeArgument = commandHandler.GetGenericArguments()[0];
                            if (typeof (Command).IsAssignableFrom(typeArgument))
                            {
                                var commandType = typeArgument;
                                _logger.Debug(
                                    $"Handling for command {typeArgument.Name} found in type {assemblyType.Name}.");
                                if (_kernel.TryGet(commandHandler) != null)
                                    _logger.Warn(
                                        $"A command handler for command {typeArgument.Name} is already registered. Ignoring subsequent registrations.");
                                else
                                    _kernel.Bind(commandHandler)
                                        .To(assemblyType)
                                        .Named(CreateCommandHandlerName(assemblyType, commandType));
                            }
                            else
                            {
                                _logger.Warn($"{assemblyType.Name} contains a command handler which does not comply with signature.");
                            }
                        }
                        if (handlersFoundInType > 0)
                            _logger.Info($"Found {handlersFoundInType} handlers in type {assemblyType.Name}.");
                    }
                    else
                    {
                        _logger.Warn($"{assemblyType.Name} contains command handlers, but not marked public.");
                    }
                }
                handlersFoundInAssembly += handlersFoundInType;
                if (handlersFoundInType > 0)
                    typesWithHandlers++;
            }
            _logger.Info(
                $"{handlersFoundInAssembly} command handlers registered from {typesWithHandlers} types in assembly {assembly.FullName}.");
        }

        private string CreateCommandHandlerName(Type handlerType, Type commandType)
        {
            return $"{handlerType.Name}_{commandType.Name}";
        }

    }
}