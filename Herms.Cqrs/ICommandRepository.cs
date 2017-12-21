using System;
using System.Threading.Tasks;
using Herms.Cqrs.Commands;

namespace Herms.Cqrs {
    public interface ICommandRepository
    {
        Task SaveCommandAsync(CommandBase command);
        CommandBase GetCommand(Guid id);
        CommandBase[] GetCorrelatedCommands(Guid id);
    }
}