using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Herms.Cqrs.Azure {
    public class EventEntity : TableEntity
    {
        public EventEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey) {}

        public EventEntity() {}
        public string AggregateType { get; set; }
        public string EventName { get; set; }
        public int Version { get; set; }
        public string Payload { get; set; }
    }
}