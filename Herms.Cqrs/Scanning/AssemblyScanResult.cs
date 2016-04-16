using System;
using System.Collections.Generic;
using System.Diagnostics;
using Herms.Cqrs.Registration;

namespace Herms.Cqrs.Scanning
{
    [DebuggerDisplay("{Implementations.Count} types, {EventHandlers.Count} event handlers, {CommandHandlers.Count} command handlers.")]
    public class AssemblyScanResult
    {
        public List<Type> Implementations { get; set; } = new List<Type>();
        public List<HandlerDefinition> EventHandlers { get; set; } = new List<HandlerDefinition>();
        public List<HandlerDefinition> CommandHandlers { get; set; } = new List<HandlerDefinition>();
        public Dictionary<string, Type> EventMap { get; set; } = new Dictionary<string, Type>();
    }
}