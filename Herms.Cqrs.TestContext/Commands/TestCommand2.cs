using System;

namespace Herms.Cqrs.TestContext.Commands
{
    public class TestCommand2 : Command
    {
        public TestCommand2() : base(Guid.NewGuid(), Guid.NewGuid()) {}

        public string Param1 { get; set; }
    }
}