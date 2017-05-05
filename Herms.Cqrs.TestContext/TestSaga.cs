using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
using Herms.Cqrs.TestContext.Commands;

namespace Herms.Cqrs.TestContext
{
    public class TestSaga : SagaBase, ISaga
    {
        private readonly ICommandHandlerRegistry _commandHandlerRegistry;
        private readonly ILog _log;

        public TestSaga(Guid id, ICommandHandlerRegistry commandHandlerRegistry, IInfrastructureRepository infrastructureRepository) : base(infrastructureRepository)
        {
            _commandHandlerRegistry = commandHandlerRegistry;
            Id = id;
            _log = LogManager.GetLogger(this.GetType());
        }

        public TestCommand1 TestCommand1 { get; set; }
        public TestCommand2 TestCommand2 { get; set; }
        public TestCommand3 TestCommand3 { get; set; }

        public Guid Id { get; }

        public IEnumerable<Command> GetCommands()
        {
            return new List<Command> { TestCommand1, TestCommand2, TestCommand3 };
        }

        public async Task Proceed()
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