using System;
using Common.Logging;

namespace Herms.Cqrs.TestContext.Commands
{
    public class TestCommand2 : Command
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