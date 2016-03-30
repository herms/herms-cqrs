using System;
using Herms.Cqrs.Scanning;
using Herms.Cqrs.TestContext.CommandHandlers;
using Herms.Cqrs.TestContext.Commands;
using Herms.Cqrs.TestContext.Events;
using Ninject;
using Xunit;

namespace Herms.Cqrs.Ninject.Tests
{
    public class AssemblyScannerWithNinjectRegistriesTests
    {
        [Fact]
        public void GivenHandlersInAssembly_WhenScanning_ThenHandlersShouldBeRegistered()
        {
            var kernel = new StandardKernel();

            var commandHandlerRegistry = new NinjectCommandHandlerRegistry(kernel);
            var eventHandlerRegistry = new NinjectEventHandlerRegistry(kernel);

            var assemblyScanner = new AssemblyScanner();
            var results = assemblyScanner.ScanAssembly(typeof (TestEvent1).Assembly);
            commandHandlerRegistry.Register(results.CommandHandlers);
            eventHandlerRegistry.Register(results.EventHandlers);

            var handlersForTestEvent1 = eventHandlerRegistry.ResolveHandlers(new TestEvent1());
            var handlersForTestEvent2 = eventHandlerRegistry.ResolveHandlers(new TestEvent2());
            var handlersForTestEvent3 = eventHandlerRegistry.ResolveHandlers(new TestEvent3());

            Assert.NotNull(handlersForTestEvent1);
            Assert.NotNull(handlersForTestEvent2);
            Assert.NotNull(handlersForTestEvent3);

            Assert.Equal(1, handlersForTestEvent1.Count);
            Assert.Equal(2, handlersForTestEvent2.Count);
            Assert.Equal(1, handlersForTestEvent3.Count);

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