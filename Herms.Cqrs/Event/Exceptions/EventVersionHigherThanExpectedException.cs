using System;

namespace Herms.Cqrs.Event.Exceptions
{
    public class EventVersionHigherThanExpectedException : Exception
    {
        public EventVersionHigherThanExpectedException(int currentVersion, int eventVersion)
            : base(
                $"Current aggregate version is {currentVersion}, event version is {eventVersion}. Event version should be {currentVersion + 1}."
                ) {}
    }
}