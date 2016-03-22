using System;
using Common.Logging;
using Herms.Cqrs.Ninject.Tests.Commands;

namespace Herms.Cqrs.Ninject.Tests.CommandHandlers
{
    public class TestCommandHandler1 : ICommandHandler<TestCommand1>
    {
        private readonly ILog _log;

        public TestCommandHandler1()
        {
            _log = LogManager.GetLogger(GetType());
        }

        public void Handle(TestCommand1 command)
        {
            _log.Debug($"{GetType().Name} handling {command.GetType().Name}");
        }
    }
}