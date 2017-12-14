using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Logging;
using Herms.Cqrs.Saga.Exceptions;

namespace Herms.Cqrs.Saga
{
    public abstract class SagaBase : ISaga
    {
        private readonly ICommandHandlerRegistry _commandHandlerRegistry;
        private readonly IInfrastructureRepository _infrastructureRepository;
        private readonly ILog _log;

        public SagaBase(IInfrastructureRepository infrastructureRepository, ICommandHandlerRegistry commandHandlerRegistry, ILog log = null)
        {
            _infrastructureRepository = infrastructureRepository;
            _commandHandlerRegistry = commandHandlerRegistry;
            _log = log ?? LogManager.GetLogger(this.GetType());
        }

        public abstract Guid Id { get; }
        public abstract IEnumerable<Command> GetCommands();

        public async Task ProceedAsync()
        {
            var nextCommand = this.GetNextCommand();
            var commandHandler = _commandHandlerRegistry.ResolveHandler(nextCommand);

            var handleCommandTask = commandHandler.HandleAsync(nextCommand);
            await this.HandleCommand(this, nextCommand, handleCommandTask);
        }

        public Command GetNextCommand()
        {
            foreach (var command in this.GetCommands())
            {
                if(command == null)
                    throw new SagaConsistencyException(this);
                if (command.Status == CommandStatus.Failed)
                    throw new SagaException("This saga has a failed command.");
                if (command.Status == CommandStatus.Dispatched)
                    throw new SagaException("This saga is currently being processed.");
                if (command.Status == CommandStatus.Processed)
                    continue;
                if (command.Status == CommandStatus.Received)
                    return command;
                throw new SagaException(
                    $"{command.GetType().Name} {command.CommandId} in {this.GetType().Name} {Id} has an unsupported status: {command.Status}.");
            }
            throw new SagaException($"{this.GetType().Name} {Id} was not able to find the next unprocessed command.");
        }

        protected async Task HandleCommand(ISaga saga, Command command, Task handleCommandTask)
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
                _log.Error($"Failed to handle {command.GetType().Name} {command.CommandId} for {this.GetType().Name} {saga.Id}", exception);
            }
        }
    }
}