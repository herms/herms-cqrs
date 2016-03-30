using System;
using Herms.Cqrs.Aggregate;

namespace Herms.Cqrs
{
    public interface IAggregateRepository<TAggregate> where TAggregate : IAggregate
    {
        void Save(TAggregate vehicle);
        TAggregate Get(Guid id);
    }
}