using System;

namespace Herms.Cqrs.Event.Exceptions
{
    public class UnexpectedEventVersionException : Exception
    {
        public UnexpectedEventVersionException(int currentVersion, int expectedVersion, int eventVersion)
            : base(
                $"Current aggregate version is {currentVersion}, event version is {eventVersion}. Event version should be {expectedVersion}."
                ) {}
    }
}