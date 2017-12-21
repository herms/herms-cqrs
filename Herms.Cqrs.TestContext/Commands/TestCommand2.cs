using System;
using Common.Logging;
using Herms.Cqrs.Commands;

namespace Herms.Cqrs.TestContext.Commands
{
    public class TestCommand2 : CommandBase
    {
        private ILog _log;
        public TestCommand2() : base(Guid.NewGuid())
        {
            _log = LogManager.GetLogger(this.GetType());
            _log.Debug("In constructor." + this.CommandId);
        }

        public string Param1 { get; set; }
    }
}