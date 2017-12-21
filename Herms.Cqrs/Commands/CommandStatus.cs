namespace Herms.Cqrs.Commands
{
    public enum CommandStatus
    {
        Received,
        Dispatched,
        Processed,
        Failed
    }
}