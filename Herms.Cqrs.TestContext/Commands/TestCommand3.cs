using System;
using Herms.Cqrs.Commands;

namespace Herms.Cqrs.TestContext.Commands
{
    public class TestCommand3 : CommandBase
    {
        public TestCommand3() : base(Guid.NewGuid(), Guid.NewGuid()) {}
    }
}