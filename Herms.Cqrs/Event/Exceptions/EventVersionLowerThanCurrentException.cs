using System;

namespace Herms.Cqrs.Event.Exceptions
{
    public class EventVersionLowerThanCurrentException : Exception
    {
        public EventVersionLowerThanCurrentException(int currentVersion, int eventVersion)
            : base($"Current version is {currentVersion}, event version is {eventVersion}.") {}
    }
}