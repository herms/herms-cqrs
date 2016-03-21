using System;
using Herms.Cqrs.Ninject.Tests.Commands;

namespace Herms.Cqrs.Ninject.Tests.CommandHandlers
{
    public class TestCommandHandler2 : ICommandHandler<TestCommand2>, ICommandHandler<TestCommand3>
    {
        public void Handle(TestCommand2 command)
        {
            throw new NotImplementedException();
        }

        public void Handle(TestCommand3 command)
        {
            throw new NotImplementedException();
        }
    }
}