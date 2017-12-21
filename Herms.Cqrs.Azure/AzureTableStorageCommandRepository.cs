using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
using Herms.Cqrs.Commands;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Herms.Cqrs.Azure
{
    public class AzureTableStorageCommandRepository : ICommandRepository
    {
        private readonly bool _clean;
        private readonly ILog _log;
        private readonly CloudStorageAccount _storageAccount;
        private readonly string _tableName;
        private readonly ITypeMappingRegistry _commandMappingRegistry;
        private readonly string GuidStringFormat = "D";
        private bool _initialized;
        private CloudTable _table;

        public AzureTableStorageCommandRepository(string connectionString, string tableName, ITypeMappingRegistry commandMappingRegistry,
            bool clean = false)
        {
            _tableName = tableName;
            _commandMappingRegistry = commandMappingRegistry;
            _clean = clean;
            _log = LogManager.GetLogger(typeof(AzureEventRepository<>));
            _storageAccount = CloudStorageAccount.Parse(connectionString);
        }

        public async Task SaveCommandAsync(CommandBase command)
        {
            await this.Initialize();
            var commandName = GetCommandName(command);

            var commandEntity = new CommandEntity
            {
                CommandName = commandName,
                AggregateId = command.AggregateId.ToString(GuidStringFormat),
                CorrelationId = command.CorrelationId?.ToString(GuidStringFormat),
                RowKey = command.CommandId.ToString(GuidStringFormat),
                PartitionKey = command.CorrelationId?.ToString(GuidStringFormat) ?? Guid.Empty.ToString(GuidStringFormat),
                UserId = command.Issuer.ToString(GuidStringFormat),
                Payload = JsonConvert.SerializeObject(command)
            };
            var operation = TableOperation.Insert(commandEntity);

            var result = await _table.ExecuteAsync(operation);
            if (IsNotSuccessStatus(result))
            {
                _log.Fatal($"Error while saving command!");
                _log.Error($"HTTP {result.HttpStatusCode} for {result.Result}.");
            }
        }

        public CommandBase GetCommand(Guid id)
        {
            var task = this.Initialize();
            task.Wait();
            //_table.
            var query =
                new TableQuery<CommandEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal,
                    id.ToString(GuidStringFormat)));
            var results = _table.ExecuteQuery(query);
            var commandEntities = results.ToList();
            if (commandEntities == null || commandEntities.Count == 0)
            {
                throw new ArgumentException($"Could not find command {id.ToString(GuidStringFormat)}.");
            }
            if (commandEntities.Count > 1)
            {
                throw new Exception($"More than one command found for id {id.ToString(GuidStringFormat)}.");
            }
            var commandEntity = commandEntities.First();
            var typeFromEntity = this.GetTypeFromEntity(commandEntity);
            var command = (CommandBase) JsonConvert.DeserializeObject(commandEntity.Payload, typeFromEntity);
            _log.Info($"Deserialized {command.GetType().Name} command...");
            return command;
        }

        public CommandBase[] GetCorrelatedCommands(Guid id)
        {
            throw new NotImplementedException();
        }

        private async Task Initialize()
        {
            if (!_initialized)
            {
                var tableClient = _storageAccount.CreateCloudTableClient();
                _table = tableClient.GetTableReference(_tableName);
                if (_clean)
                    await _table.DeleteIfExistsAsync();
                var created = await _table.CreateIfNotExistsAsync();
                if (created)
                    _log.Info($"Created storage table \"{_tableName}\".");
                _initialized = true;
            }
        }

        private static bool IsNotSuccessStatus(TableResult r)
        {
            return r.HttpStatusCode < 200 || r.HttpStatusCode >= 300;
        }

        private string GetCommandName(CommandBase command)
        {
            var commandMapping = _commandMappingRegistry.ResolveName(command.GetType());
            if (commandMapping != null)
                return commandMapping;
            return command.GetType().FullName;
        }

        private Type GetTypeFromEntity(CommandEntity commandEntity)
        {
            var commandType = _commandMappingRegistry.ResolveType(commandEntity.CommandName);
            if (commandType != null)
                return commandType;
            //return _commandMappingRegistry.ResolveCommandType(commandEntity.CommandName);
            return Type.GetType(commandEntity.CommandName, true);
        }
    }

    public class CommandEntity : TableEntity
    {
        public CommandEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }

        public CommandEntity() { }
        public string CommandName { get; set; }
        public string AggregateId { get; set; }
        public string CorrelationId { get; set; }
        public string UserId { get; set; }
        public string Payload { get; set; }
    }
}