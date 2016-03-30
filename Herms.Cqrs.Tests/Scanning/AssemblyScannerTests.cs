﻿using System;
using System.Linq;
using Herms.Cqrs.Scanning;
using Herms.Cqrs.TestContext.CommandHandlers;
using Herms.Cqrs.TestContext.Commands;
using Herms.Cqrs.TestContext.Events;
using Xunit;

namespace Herms.Cqrs.Tests.Scanning
{
    public class AssemblyScannerTests
    {
        [Fact]
        public void GivenHandlersInAssembly_WhenScanning_ThenHandlersShouldBeRegistered()
        {
            var assemblyScanner = new AssemblyScanner();
            var results = assemblyScanner.ScanAssembly(typeof (TestEvent1).Assembly);

            var handlersForTestEvent1 = results.EventHandlers.Where(eh => eh.Argument == typeof (TestEvent1));
            var handlersForTestEvent2 = results.EventHandlers.Where(eh => eh.Argument == typeof (TestEvent2));
            var handlersForTestEvent3 = results.EventHandlers.Where(eh => eh.Argument == typeof (TestEvent3));

            Assert.NotNull(handlersForTestEvent1);
            Assert.NotNull(handlersForTestEvent2);
            Assert.NotNull(handlersForTestEvent3);

            Assert.Equal(1, handlersForTestEvent1.Count());
            Assert.Equal(2, handlersForTestEvent2.Count());
            Assert.Equal(1, handlersForTestEvent3.Count());

            var handlerForTestCommand1 = results.CommandHandlers.FirstOrDefault(eh => eh.Argument == typeof (TestCommand1));
            var handlerForTestCommand2 = results.CommandHandlers.FirstOrDefault(eh => eh.Argument == typeof (TestCommand2));
            var handlerForTestCommand3 = results.CommandHandlers.FirstOrDefault(eh => eh.Argument == typeof (TestCommand3));

            Assert.NotNull(handlerForTestCommand1);
            Assert.NotNull(handlerForTestCommand2);
            Assert.NotNull(handlerForTestCommand3);

            Assert.Equal(handlerForTestCommand1.Implementation, typeof (TestCommandHandler1));
            Assert.Equal(handlerForTestCommand2.Implementation, typeof (TestCommandHandler2));
            Assert.Equal(handlerForTestCommand3.Implementation, typeof (TestCommandHandler2));
        }
    }
}