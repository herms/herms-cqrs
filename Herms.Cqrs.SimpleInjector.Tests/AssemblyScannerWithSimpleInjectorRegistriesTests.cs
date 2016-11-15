using System;
using System.Diagnostics;
using Herms.Cqrs.TestCommon;
using SimpleInjector;
using Xunit;

namespace Herms.Cqrs.SimpleInjector.Tests
{
    public class AssemblyScannerWithSimpleInjectorRegistriesTests
    {
        [Fact]
        public void GivenHandlersInAssembly_WhenScanning_ThenHandlersShouldBeRegistered()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var kernel = new Container();

            var commandHandlerRegistry = new SimpleInjectorCommandHandlerRegistry(kernel);
            var eventHandlerRegistry = new SimpleInjectorEventHandlerRegistry(kernel);

            AssemblyScannerCommonTests.ScanAssembliesAndRegisterHandlers(eventHandlerRegistry, commandHandlerRegistry);
            eventHandlerRegistry.Build();

            AssemblyScannerCommonTests.GivenAssembliesAreScanned_WhenResolvingHandlers_ThenInstancesShouldBeReturned(eventHandlerRegistry,
                commandHandlerRegistry);
            stopwatch.Stop();
            Console.WriteLine("Resolving took " + stopwatch.Elapsed + "ms.");
        }
    }
}