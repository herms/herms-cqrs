using System;

namespace Herms.Cqrs.Aggregate
{
    public interface IAggregate
    {
        Guid Id { get; }
    }
}