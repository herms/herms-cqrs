using System;
using System.Collections.Generic;

namespace Herms.Cqrs
{
    public class EventHandlerResults
    {
        public List<EventHandlerResult> Results { get; set; } = new List<EventHandlerResult>();
        public bool Success => Results.TrueForAll(r => r.Success);
    }
}