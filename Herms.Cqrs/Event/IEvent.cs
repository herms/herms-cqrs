namespace Herms.Cqrs.Event
{
    public interface IEvent
    {
        int Version { get; }
    }
}