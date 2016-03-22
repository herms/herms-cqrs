using System;
using System.Reflection;

namespace Herms.Cqrs.Ninject
{
    public interface IAssemblyScanner
    {
        void ScanAssembly(Assembly assembly);
    }
}