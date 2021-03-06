using System;
using Common.Logging;
using Herms.Cqrs.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Herms.Cqrs.Serialization {
    public static class CommandEnvelopeSerilizer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CommandEnvelopeSerilizer));

        public static CommandBase DeserializeCommand(string contents)
        {
            var jObject = JsonConvert.DeserializeObject<JObject>(contents);
            var commandType = jObject[CommandEnvelope.CommandTypeField].Value<string>();
            var assemblyName = jObject[CommandEnvelope.AssemblyNameField].Value<string>();
            Log.Debug($"Read command of type {commandType}. Trying to deserialize to {assemblyName}.");
            var type = Type.GetType(assemblyName, true);
            var payload = jObject[CommandEnvelope.CommandDataField].ToString();
            var command = (CommandBase)JsonConvert.DeserializeObject(payload, type);
            return command;
        }

        public static string SerializeCommand(CommandBase command)
        {
            var envelope = CreateCommandEnvelope(command);
            var contents = JsonConvert.SerializeObject(envelope);
            return contents;
        }

        public static CommandEnvelope CreateCommandEnvelope(CommandBase command)
        {
            var envelope = new CommandEnvelope()
            {
                AssemblyName = command.GetType().AssemblyQualifiedName,
                CommandData = command,
                CommandType = command.GetType().FullName
            };
            return envelope;
        }
    }
}