using System;
using System.Collections.Generic;

namespace Herms.Cqrs
{
    public interface ISaga
    {
        IEnumerable<Command> GetCommands();
        void Proceed();
    }
}