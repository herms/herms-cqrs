using System;

namespace Herms.Cqrs.TestContext.Commands
{
    public class TestCommand1 : Command
    {
        public TestCommand1() : base(Guid.NewGuid(), Guid.NewGuid())
        {
            
        }
    }
}