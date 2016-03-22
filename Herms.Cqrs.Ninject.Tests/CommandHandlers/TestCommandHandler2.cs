using System;
using Common.Logging;
using Herms.Cqrs.Ninject.Tests.Commands;

namespace Herms.Cqrs.Ninject.Tests.CommandHandlers
{
    public class TestCommandHandler2 : ICommandHandler<TestCommand2>, ICommandHandler<TestCommand3>
    {
        private readonly ILog _log;

        public TestCommandHandler2()
        {
            _log = LogManager.GetLogger(GetType());
        }

        public void Handle(TestCommand2 command)
        {
            _log.Debug($"{GetType().Name} handling {command.GetType().Name}");
        }

        public void Handle(TestCommand3 command)
        {
            _log.Debug($"{GetType().Name} handling {command.GetType().Name}");
        }
    }
}