using System;
using System.Threading.Tasks;
using Herms.Cqrs.Aggregate;

namespace Herms.Cqrs
{
    public interface IAggregateRepository<TAggregate> where TAggregate : IAggregate
    {
        Task SaveAsync(TAggregate vehicle);
        void Save(TAggregate vehicle);
        TAggregate Get(Guid id);
    }
}