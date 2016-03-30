using System;
using Common.Logging;
using Herms.Cqrs.TestContext.Commands;

namespace Herms.Cqrs.TestContext.CommandHandlers
{
    public class TestCommandHandler1 : ICommandHandler<TestCommand1>
    {
        private readonly ILog _log;

        public TestCommandHandler1()
        {
            _log = LogManager.GetLogger(this.GetType());
        }

        public void Handle(TestCommand1 command)
        {
            _log.Debug($"{this.GetType().Name} handling {command.GetType().Name}");
        }
    }
}