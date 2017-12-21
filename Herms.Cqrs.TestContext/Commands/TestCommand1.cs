using System;
using Herms.Cqrs.Commands;

namespace Herms.Cqrs.TestContext.Commands
{
    public class TestCommand1 : CommandBase
    {
        public TestCommand1() : base(Guid.NewGuid(), Guid.NewGuid())
        {
            
        }
    }
}