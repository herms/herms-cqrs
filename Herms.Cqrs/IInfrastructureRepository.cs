using System;
using System.Threading.Tasks;
using Herms.Cqrs.Saga;

namespace Herms.Cqrs
{
    public interface IInfrastructureRepository
    {
        Task SaveCommandAsync(Command command);
        Task UpdateCommandStatusAsync(Command command);
        Task SaveSagaAsync(ISaga testSaga);
        ISaga GetSaga(Guid id);
    }

    public interface ICommandRepository
    {
        Task SaveCommandAsync(Command command);
        Command GetCommand(Guid id);
        Command[] GetCorrelatedCommands(Guid id);
    }
}