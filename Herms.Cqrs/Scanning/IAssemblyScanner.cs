using System;
using System.Reflection;

namespace Herms.Cqrs.Scanning
{
    public interface IAssemblyScanner
    {
        AssemblyScanResult ScanAssemblyForEventHandlers(Assembly assembly);
        AssemblyScanResult ScanAssemblyForCommandHandlers(Assembly assembly);
        AssemblyScanResult ScanAssemblyForHandlers(Assembly assembly);
        AssemblyScanResult ScanAssembly(Assembly assembly);
    }
}