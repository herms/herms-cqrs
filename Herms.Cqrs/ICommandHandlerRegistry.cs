using System;
using System.Collections.Generic;
using Herms.Cqrs.Commands;
using Herms.Cqrs.Registration;

namespace Herms.Cqrs
{
    public interface ICommandHandlerRegistry
    {
        void Register(Type handlerType, Type implementationType);
        void Register(IEnumerable<HandlerDefinition> handlerDefinitions);
        ICommandHandler<T> ResolveHandler<T>(T commandType) where T : CommandBase;
    }
}