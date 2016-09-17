using System;

namespace Herms.Cqrs
{
    public interface ICommandHandler<in T> where T : Command
    {
        void Handle(T command);
    }
}