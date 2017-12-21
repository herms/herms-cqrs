using System;
using System.Collections.Generic;
using Common.Logging;
using Herms.Cqrs.Commands;
using Herms.Cqrs.Saga;
using Herms.Cqrs.Saga.Exceptions;
using Herms.Cqrs.TestContext.Commands;

namespace Herms.Cqrs.TestContext
{
    public class TestSaga : SagaBase
    {
        private readonly ILog _log;
        private TestCommand1 _testCommand1;
        private TestCommand2 _testCommand2;
        private TestCommand3 _testCommand3;

        public TestSaga(Guid id, ICommandHandlerRegistry commandHandlerRegistry, ICommandLogRepository commandLogRepository) : base(
            commandLogRepository, commandHandlerRegistry)
        {
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

        public override Guid Id { get; }

        public override IEnumerable<CommandBase> GetCommands()
        {
            return new List<CommandBase> { TestCommand1, TestCommand2, TestCommand3 };
        }

        private void Validate()
        {
            if (TestCommand1 == null || TestCommand2 == null || TestCommand3 == null)
                throw new SagaConsistencyException(this);
        }
    }
}