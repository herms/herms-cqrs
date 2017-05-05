using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Herms.Cqrs
{
    public interface ISaga
    {
        Guid Id { get; }
        IEnumerable<Command> GetCommands();
        Task Proceed();
    }
}