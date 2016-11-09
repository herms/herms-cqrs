using System;
using System.Threading.Tasks;

namespace Herms.Cqrs
{
    public interface ICommandHandler<in T> where T : Command
    {
        Task HandleAsync(T command);
    }
}