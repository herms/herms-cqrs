using System;
using System.Threading.Tasks;

namespace Herms.Cqrs.Saga
{
    public interface ISagaTriggerDispatcher
    {
        Task TriggerSagaAsync(Guid id);
    }
}