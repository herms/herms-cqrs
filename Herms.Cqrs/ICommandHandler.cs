using System;

namespace Herms.Cqrs
{
    public interface ICommandHandler {}

    public interface ICommandHandler<in T> : ICommandHandler where T : Command
    {
        void Handle(T command);
    }
}