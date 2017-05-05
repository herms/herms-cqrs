using System;
using System.Threading.Tasks;

namespace Herms.Cqrs
{
    public interface IInfrastructureRepository
    {
        Task UpdateCommandStatusAsync(Command command);
        Task SaveSaga(ISaga testSaga);
        ISaga GetSaga(Guid id);
    }
}