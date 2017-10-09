using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Herms.Cqrs.Saga
{
    public interface ISaga
    {
        Guid Id { get; }
        IEnumerable<Command> GetCommands();
        Task ProceedAsync();
    }
}