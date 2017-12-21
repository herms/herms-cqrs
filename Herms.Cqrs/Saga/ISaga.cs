using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Herms.Cqrs.Commands;

namespace Herms.Cqrs.Saga
{
    public interface ISaga
    {
        Guid Id { get; }
        IEnumerable<CommandBase> GetCommands();
        Task ProceedAsync();
    }
}