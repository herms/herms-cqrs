using System;
using System.Linq;
using Herms.Cqrs.Ninject.Tests.CommandHandlers;
using Herms.Cqrs.Ninject.Tests.Commands;
using Herms.Cqrs.Ninject.Tests.Events;
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
            var scanner = new CommandHandlerScanner(kernel);

            scanner.ScanAndRegisterCommandHandlers(GetType().Assembly);

            var commandHandlerRegistry = new NinjectCommandHandlerRegistry(kernel);

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

    public class EventHandlerScannerTest
    {
        [Fact]
        public void GivenEventHandlersInAssembly_WhenScanning_ThenHandlersShouldBeRegistered()
        {
            var kernel = new StandardKernel();
            var scanner = new EventHandlerScanner(kernel);

            scanner.ScanAndRegisterEventHandlers(GetType().Assembly);

            var eventHandlerRegistry = new NinjectEventHandlerRegistry(kernel);

            var handlersForTestEvent1 = eventHandlerRegistry.ResolveHandlers(new TestEvent1());
            var handlersForTestEvent2 = eventHandlerRegistry.ResolveHandlers(new TestEvent2());
            var handlersForTestEvent3 = eventHandlerRegistry.ResolveHandlers(new TestEvent3());

            Assert.NotNull(handlersForTestEvent1);
            Assert.NotNull(handlersForTestEvent2);
            Assert.NotNull(handlersForTestEvent3);

            Assert.Equal(1, handlersForTestEvent1.Count());
            Assert.Equal(2, handlersForTestEvent2.Count());
            Assert.Equal(1, handlersForTestEvent3.Count());
        }
    }
}