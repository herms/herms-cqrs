using System;
using System.Threading.Tasks;
using Common.Logging;
using Herms.Cqrs.TestContext.Commands;

namespace Herms.Cqrs.TestContext.CommandHandlers
{
    public class TestCommandHandler2 : ICommandHandler<TestCommand2>, ICommandHandler<TestCommand3>
    {
        private readonly ILog _log;

        public TestCommandHandler2()
        {
            _log = LogManager.GetLogger(this.GetType());
        }

        public Task HandleAsync(TestCommand2 command)
        {
            _log.Debug($"{this.GetType().Name} handling {command.GetType().Name}");
            return Task.CompletedTask;
        }

        public Task HandleAsync(TestCommand3 command)
        {
            _log.Debug($"{this.GetType().Name} handling {command.GetType().Name}");
            return Task.CompletedTask;
        }
    }
}