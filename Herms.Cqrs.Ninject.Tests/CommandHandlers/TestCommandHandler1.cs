using Herms.Cqrs.Ninject.Tests.Commands;

namespace Herms.Cqrs.Ninject.Tests.CommandHandlers
{
    public class TestCommandHandler1 : ICommandHandler<TestCommand1>
    {
        public void Handle(TestCommand1 command)
        {
            throw new System.NotImplementedException();
        }
    }
}
