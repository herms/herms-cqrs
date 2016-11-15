using System;
using Herms.Cqrs.TestContext.CommandHandlers;
using Herms.Cqrs.TestContext.Commands;
using SimpleInjector;
using Xunit;

namespace Herms.Cqrs.SimpleInjector.Tests
{
    public class SimpleInjectorCommandHandlerRegistryTests

    {

        [Fact]
        public void GivenCommandHandlersInRegistry_WhenRequestCommandHandler_ThenTheCorrespondingHandlerShouldBeReturned()
        {
            var kernel = new Container();
            var commandHandlerRegistry = new SimpleInjectorCommandHandlerRegistry(kernel);

            commandHandlerRegistry.RegisterImplementation(typeof(TestCommandHandler1));

            var command1 = new TestCommand1();
            var handler = commandHandlerRegistry.ResolveHandler(command1);

            Assert.NotNull(handler);
            Assert.Equal(typeof(TestCommandHandler1), handler.GetType());
        }
    }
}