using System;

namespace Herms.Cqrs.Registration
{
    public class HandlerDefinition
    {
        public Type Handler { get; set; }
        public Type Argument { get; set; }
        public Type Implementation { get; set; }
    }
}