using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Herms.Cqrs
{
    public interface ICommandLogRepository
    {
        Task UpdateCommandStatusAsync(Command command);
        List<Tuple<DateTime, CommandStatus>> GetCommandHistory(Guid commandId);
    }
}