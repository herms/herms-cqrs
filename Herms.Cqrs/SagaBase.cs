using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Logging;

namespace Herms.Cqrs
{
    public class SagaBase
    {
        private IInfrastructureRepository _infrastructureRepository;

        public SagaBase(IInfrastructureRepository infrastructureRepository)
        {
            this._infrastructureRepository = infrastructureRepository;
        }

        public Command GetNextCommand(ISaga saga)
        {
            foreach (var command in saga.GetCommands())
            {
                if (command.Status == CommandStatus.Failed)
                    throw new SagaException("This saga has a failed command.");
                if (command.Status == CommandStatus.Dispatched)
                    throw new SagaException("This saga is currently being processesd. Try again later.");
                if (command.Status == CommandStatus.Processed)
                    continue;
                if (command.Status == CommandStatus.Received)
                    return command;
                else
                {
                    throw new SagaException(
                        $"{command.GetType().Name} {command.CommandId} in {saga.GetType().Name} {saga.Id} has an unsupported status: {command.Status}.");
                }
            }
            throw new SagaException($"{saga.GetType().Name} {saga.Id} was not able to find the next unprocessed command.");
        }

        protected async Task HandleCommand(ISaga saga, Command command, Task handleCommandTask, ILog log)
        {
            command.Status = CommandStatus.Dispatched;
            var updateCommandStatusTask = _infrastructureRepository.UpdateCommandStatusAsync(command);
            try
            {
                await Task.WhenAll(updateCommandStatusTask, handleCommandTask);
                command.Status = CommandStatus.Processed;
                await _infrastructureRepository.UpdateCommandStatusAsync(command);
            }
            catch (Exception exception)
            {
                command.Status = CommandStatus.Failed;
                log.Error($"Failed to handle {command.GetType().Name} {command.CommandId} for {this.GetType().Name} {saga.Id}", exception);
            }
        }


    }
}