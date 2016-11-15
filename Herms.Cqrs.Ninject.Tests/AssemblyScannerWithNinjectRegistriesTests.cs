using System;
using System.Diagnostics;
using Herms.Cqrs.TestCommon;
using Ninject;
using Xunit;

namespace Herms.Cqrs.Ninject.Tests
{
    public class AssemblyScannerWithNinjectRegistriesTests
    {
        [Fact]
        public void GivenHandlersInAssembly_WhenScanning_ThenHandlersShouldBeRegistered()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var kernel = new StandardKernel();

            var commandHandlerRegistry = new NinjectCommandHandlerRegistry(kernel);
            var eventHandlerRegistry = new NinjectEventHandlerRegistry(kernel);

            AssemblyScannerCommonTests.ScanAssembliesAndRegisterHandlers(eventHandlerRegistry, commandHandlerRegistry);

            AssemblyScannerCommonTests.GivenAssembliesAreScanned_WhenResolvingHandlers_ThenInstancesShouldBeReturned(eventHandlerRegistry,
                commandHandlerRegistry);
            stopwatch.Stop();
            Console.WriteLine("Resolving took " + stopwatch.Elapsed + "ms.");
        }
    }
}