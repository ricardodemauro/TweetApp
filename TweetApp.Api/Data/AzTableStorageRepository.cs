using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetApp.Api.Logging;
using TweetApp.Api.Models;

namespace TweetApp.Api.Data
{
    public class AzTableStorageRepository : ITweetRepository
    {
        private readonly ILogger<AzTableStorageRepository> _logger;

        private readonly string _connectionString;

        private CloudTable _tableRefence;

        public AzTableStorageRepository(ILogger<AzTableStorageRepository> logger, string connectionString)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        #region Table Api
        private CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            if (string.IsNullOrEmpty(storageConnectionString))
                throw new ArgumentNullException(nameof(storageConnectionString));

            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException fe)
            {
                _logger.LogError(fe, "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }
            catch (ArgumentException ae)
            {
                _logger.LogError(ae, "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                throw;
            }

            return storageAccount;
        }

        private async ValueTask<CloudTable> CreateTableAsync(string tableName = "TweetsTable")
        {
            if (_tableRefence != null)
                return _tableRefence;

            // Retrieve storage account information from connection string.
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(_connectionString);

            // Create a table client for interacting with the table service
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            _logger.LogInformation("Creating table {tablename}", tableName);

            // Create a table client for interacting with the table service 
            CloudTable table = tableClient.GetTableReference(tableName);

            if (await table.CreateIfNotExistsAsync())
                _logger.LogInformation("Created table {tablename}", tableName);
            else
                _logger.LogInformation("Table {tablename} already", tableName);

            _tableRefence = table;
            return table;
        }

        private async Task<T> InsertOrMergeEntityAsync<T>(CloudTable table, T entity)
            where T : TableEntity
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                T record = result.Result as T;

                // Get the request units consumed by the current operation. RequestCharge of a TableResult is only applied to Azure Cosmos DB
                if (result.RequestCharge.HasValue)
                    _logger.LogInformation("Request Charge of InsertOrMerge {operation}", result.RequestCharge);

                return record;
            }
            catch (StorageException e)
            {
                _logger.LogError(e);
                throw;
            }
        }

        private async Task<T> RetrieveEntityUsingPointQueryAsync<T>(CloudTable table, string partitionKey, string rowKey)
            where T : TableEntity
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
                TableResult result = await table.ExecuteAsync(retrieveOperation);
                T record = result.Result as T;

                // Get the request units consumed by the current operation. RequestCharge of a TableResult is only applied to Azure CosmoS DB 
                if (result.RequestCharge.HasValue)
                    _logger.LogInformation("Request Charge of Retrieve {operation}", result.RequestCharge);

                return record;
            }
            catch (StorageException e)
            {
                _logger.LogError(e);
                throw;
            }
        }

        private IEnumerable<T> RetriveQuery<T>(CloudTable table)
            where T : TableEntity, new()
        {
            try
            {
                var entities = table.ExecuteQuery<T>(new TableQuery<T>()).ToList();
                return entities;
            }
            catch (StorageException e)
            {
                _logger.LogError(e);
                throw;
            }
        }

        private async Task<IReadOnlyList<T>> RetriveQueryAsync<T>(CloudTable table)
            where T : TableEntity, new()
        {
            try
            {
                List<T> result = new List<T>();

                var emptyQuery = table.CreateQuery<T>()
                    .OrderByDesc("Timestamp");
                var token = new TableContinuationToken();

                TableQuerySegment<T> segment = default;
                do
                {
                    segment = await table.ExecuteQuerySegmentedAsync<T>(emptyQuery, token);
                    result.AddRange(segment);

                    token = segment.ContinuationToken;

                } while (token != null);

                return result;
            }
            catch (StorageException e)
            {
                _logger.LogError(e);
                throw;
            }
        }

        #endregion Table Api

        public async Task<Tweet> InsertOneAsync(Tweet data)
        {
            return await InsertOrMergeEntityAsync(await CreateTableAsync(), data);
        }

        public async Task<IReadOnlyList<Tweet>> List()
        {
            return await RetriveQueryAsync<Tweet>(await CreateTableAsync());
        }
    }
}
