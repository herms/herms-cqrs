using System;

namespace Herms.Cqrs.Serialization
{
    [Serializable]
    public class CommandEnvelope
    {
        public const string AssemblyNameField = "AssemblyName";
        public const string CommandDataField = "CommandData";
        public const string CommandTypeField = "CommandType";
        public string AssemblyName { get; set; }
        public string CommandType { get; set; }
        public Command CommandData { get; set; }
    }
}