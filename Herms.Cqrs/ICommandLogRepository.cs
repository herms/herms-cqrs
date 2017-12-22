using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Herms.Cqrs.Commands;

namespace Herms.Cqrs
{
    public interface ICommandLogRepository
    {
        Task UpdateCommandStatusAsync(CommandBase command);
        List<CommandLogItem> GetCommandHistory(Guid commandId);
        Dictionary<Guid, List<CommandLogItem>> GetCommandHistory(Guid[] commandId);
    }
}