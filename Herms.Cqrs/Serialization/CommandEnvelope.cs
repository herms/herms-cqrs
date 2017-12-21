using System;
using Herms.Cqrs.Commands;

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
        public CommandBase CommandData { get; set; }
    }
}