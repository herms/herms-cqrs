using System;
using Herms.Cqrs.Ninject.Tests.CommandHandlers;
using Herms.Cqrs.Ninject.Tests.Commands;
using Ninject;
using Xunit;

namespace Herms.Cqrs.Ninject.Tests
{
    public class CommandHandlerScannerTest
    {
        [Fact]
        public void GivenCommandHandlersInAssembly_WhenScanning_ThenHandlersShouldBeRegistered()
        {
            var kernel = new StandardKernel();

            var commandHandlerRegistry = new NinjectCommandHandlerRegistry(kernel);
            commandHandlerRegistry.ScanAssembly(GetType().Assembly);

            var handlerForTestCommand1 = commandHandlerRegistry.ResolveHandler(new TestCommand1());
            var handlerForTestCommand2 = commandHandlerRegistry.ResolveHandler(new TestCommand2());
            var handlerForTestCommand3 = commandHandlerRegistry.ResolveHandler(new TestCommand3());

            Assert.NotNull(handlerForTestCommand1);
            Assert.NotNull(handlerForTestCommand2);
            Assert.NotNull(handlerForTestCommand3);

            Assert.Equal(handlerForTestCommand1.GetType(), typeof (TestCommandHandler1));
            Assert.Equal(handlerForTestCommand2.GetType(), typeof (TestCommandHandler2));
            Assert.Equal(handlerForTestCommand3.GetType(), typeof (TestCommandHandler2));
        }
    }
}