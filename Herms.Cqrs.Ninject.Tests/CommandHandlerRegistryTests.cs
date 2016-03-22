using System;
using Herms.Cqrs.TestContext.CommandHandlers;
using Herms.Cqrs.TestContext.Commands;
using Ninject;
using Xunit;

namespace Herms.Cqrs.Ninject.Tests
{
    public class CommandHandlerRegistryTests
    {
        [Fact]
        public void GivenCommandHandlersInRegistry_WhenRequestCommandHandler_ThenTheCorrespondingHandlerShouldBeReturned()
        {
            var kernel = new StandardKernel();
            var commandHandlerRegistry = new NinjectCommandHandlerRegistry(kernel);

            commandHandlerRegistry.RegisterImplementation(typeof (TestCommandHandler1));

            var command1 = new TestCommand1();
            var handler = commandHandlerRegistry.ResolveHandler(command1);

            Assert.NotNull(handler);
            Assert.Equal(typeof (TestCommandHandler1), handler.GetType());
        }
    }
}