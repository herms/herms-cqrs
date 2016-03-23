using System;

namespace Herms.Cqrs
{
    public interface IAggregateRepository<TAggregate> where TAggregate : Aggregate
    {
        void Save(TAggregate vehicle);
        TAggregate Get(Guid id);
    }
}