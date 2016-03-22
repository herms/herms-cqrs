using System;

namespace Herms.Cqrs
{
    public interface ICommandHandlerRegistry
    {
        void Register(Type handlerType, Type implementationType);
        ICommandHandler<T> ResolveHandler<T>(T commandType) where T : Command;
    }
}