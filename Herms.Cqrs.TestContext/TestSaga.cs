using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
using Herms.Cqrs.Saga;
using Herms.Cqrs.Saga.Exceptions;
using Herms.Cqrs.TestContext.Commands;

namespace Herms.Cqrs.TestContext
{
    public class TestSaga : SagaBase, ISaga
    {
        private readonly ICommandHandlerRegistry _commandHandlerRegistry;
        private readonly ILog _log;
        private TestCommand1 _testCommand1;
        private TestCommand2 _testCommand2;
        private TestCommand3 _testCommand3;

        public TestSaga(Guid id, ICommandHandlerRegistry commandHandlerRegistry, IInfrastructureRepository infrastructureRepository) : base(
            infrastructureRepository)
        {
            _commandHandlerRegistry = commandHandlerRegistry;
            Id = id;
            _log = LogManager.GetLogger(this.GetType());
        }

        public TestCommand1 TestCommand1
        {
            get => _testCommand1;
            set
            {
                _testCommand1 = value;
                _testCommand1.CorrelationId = Id;
            }
        }

        public TestCommand2 TestCommand2
        {
            get => _testCommand2;
            set
            {
                _testCommand2 = value;
                _testCommand2.CorrelationId = Id;
            }
        }

        public TestCommand3 TestCommand3
        {
            get => _testCommand3;
            set
            {
                _testCommand3 = value;
                _testCommand3.CorrelationId = Id;
            }
        }

        public Guid Id { get; }

        public IEnumerable<Command> GetCommands()
        {
            return new List<Command> { TestCommand1, TestCommand2, TestCommand3 };
        }

        public async Task ProceedAsync()
        {
            this.Validate();
            if (this.GetCommands().Any(c => c.Status == CommandStatus.Failed))
                throw new SagaException("This saga has a failed command.");
            if (this.GetCommands().Any(c => c.Status == CommandStatus.Dispatched))
                throw new SagaException("This saga is currently being processesd. Try again later.");

            var nextCommand = this.GetNextCommand(this);

            Task handleCommandTask = null;
            if (TestCommand1 == nextCommand)
            {
                var commandHandler = _commandHandlerRegistry.ResolveHandler(TestCommand1);
                handleCommandTask = commandHandler.HandleAsync(TestCommand1);
            }
            await this.HandleCommand(this, TestCommand1, handleCommandTask, _log);
        }

        private void Validate()
        {
            if (TestCommand1 == null || TestCommand2 == null || TestCommand3 == null)
                throw new SagaConsistencyException(this);
        }
    }
}