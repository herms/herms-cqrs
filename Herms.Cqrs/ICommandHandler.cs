using System;
using System.Threading.Tasks;
using Herms.Cqrs.Commands;

namespace Herms.Cqrs
{
    public interface ICommandHandler<in T> where T : CommandBase
    {
        Task HandleAsync(T command);
    }
}