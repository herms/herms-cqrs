using System;

namespace Herms.Cqrs.TestContext.Commands
{
    public class TestCommand3 : Command
    {
        public TestCommand3() : base(Guid.NewGuid(), Guid.NewGuid()) {}
    }
}