using System;
using Ninject;

namespace Herms.Cqrs.Ninject
{
    public class NinjectCommandHandlerRegistry : ICommandHandlerRegistry
    {
        private readonly IKernel _kernel;

        public NinjectCommandHandlerRegistry(IKernel kernel)
        {
            _kernel = kernel;
        }

        public ICommandHandler<T> ResolveHandler<T>(T commandType) where T : Command
        {
            return (ICommandHandler<T>) _kernel.Get(typeof (ICommandHandler<T>));
        }
    }
}