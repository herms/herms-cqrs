using System;

namespace Herms.Cqrs
{
    public interface ICommandHandlerRegistry
    {
        //void Add(ICommandHandler handler);
        ICommandHandler<T> ResolveHandler<T>(T commandType) where T : Command;
    }
}