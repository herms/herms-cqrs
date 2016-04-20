using System;
using System.Collections.Generic;
using System.Diagnostics;
using Herms.Cqrs.Registration;

namespace Herms.Cqrs.Scanning
{
    [DebuggerDisplay("{Implementations.Count} types, {EventHandlers.Count} event handlers, {CommandHandlers.Count} command handlers, {EventMap.Count} events.")]
    public class AssemblyScanResult
    {
        public List<Type> Implementations { get; set; } = new List<Type>();
        public List<HandlerDefinition> EventHandlers { get; set; } = new List<HandlerDefinition>();
        public List<HandlerDefinition> CommandHandlers { get; set; } = new List<HandlerDefinition>();
        public List<EventMapping> EventMap { get; set; } = new List<EventMapping>();
    }
}