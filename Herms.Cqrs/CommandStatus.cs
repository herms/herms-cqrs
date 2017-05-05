using System;

namespace Herms.Cqrs
{
    public enum CommandStatus
    {
        Received,
        Dispatched,
        Processed,
        Failed
    }
}