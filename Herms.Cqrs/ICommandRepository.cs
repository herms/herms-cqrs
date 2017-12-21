using System;
using System.Threading.Tasks;

namespace Herms.Cqrs {
    public interface ICommandRepository
    {
        Task SaveCommandAsync(Command command);
        Command GetCommand(Guid id);
        Command[] GetCorrelatedCommands(Guid id);
    }
}