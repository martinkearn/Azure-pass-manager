using APM.Api.Extensions;
using APM.Api.Interfaces;
using APM.Api.Models;
using APM.Domain;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APM.Api.Repositories
{
    public class StoreRepository : IStoreRepository
    {
        private readonly AppSettings _appSettings;
        private readonly AppSecretSettings _appSecretSettings;

        public StoreRepository(IOptions<AppSettings> appSettings, IOptions<AppSecretSettings> appSecretSettings)
        {
            _appSettings = appSettings.Value;
            _appSecretSettings = appSecretSettings.Value;
        }

        public async Task StoreCode(Code item)
        {
            var table = await GetCloudTable(_appSecretSettings.TableStorageConnectionString, _appSettings.TableStorageContainerName);

            TableEntityAdapter<Code> entity = new TableEntityAdapter<Code>(item, item.EventName, item.PromoCode);

            TableOperation insertOperation = TableOperation.InsertOrReplace(entity);

            await table.ExecuteAsync(insertOperation);
        }

        public async Task StoreCodes(List<Code> items)
        {
            var table = await GetCloudTable(_appSecretSettings.TableStorageConnectionString, _appSettings.TableStorageContainerName);

            // Need to split list into block of 100 as 100 is the maximum amount of item permitted in a batch operation https://docs.microsoft.com/en-us/azure/cosmos-db/table-storage-how-to-use-dotnet#insert-a-batch-of-entities
            var listOfListOfItems = items.ChunkBy(100);

            foreach (var listOfItems in listOfListOfItems)
            {
                // Create the batch operation.
                TableBatchOperation batchOperation = new TableBatchOperation();

                // Add each item to the batch
                foreach (var item in listOfItems)
                {
                    var rowKey = item.PromoCode;
                    TableEntityAdapter<Code> entity = new TableEntityAdapter<Code>(item, item.EventName, item.PromoCode);
                    batchOperation.InsertOrReplace(entity);
                }

                // Execute the batch operation.
                await table.ExecuteBatchAsync(batchOperation);
            }

        }

        public async Task DeleteCode(string eventName, string id)
        {
            //get cloudtable
            var table = await GetCloudTable(_appSecretSettings.TableStorageConnectionString, _appSettings.TableStorageContainerName);

            // Create a retrieve operation that expects a the right entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<TableEntityAdapter<Code>>(eventName, id);

            // Execute the operation.
            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);

            // Assign the result to an Entity.
            var deleteEntity = (TableEntityAdapter<Code>)retrievedResult.Result;

            // Create the Delete TableOperation.
            if (deleteEntity != null)
            {
                // Create the Delete TableOperation.
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                // Execute the operation.
                await table.ExecuteAsync(deleteOperation);
            }
        }

        public async Task DeleteCodes(string eventName, string codeIds)
        {
            //get cloudtable
            var table = await GetCloudTable(_appSecretSettings.TableStorageConnectionString, _appSettings.TableStorageContainerName);

            // Cast to list
            var items = codeIds.Split(',').ToList();

            // Need to split list into block of 100 as 100 is the maximum amount of item permitted in a batch operation https://docs.microsoft.com/en-us/azure/cosmos-db/table-storage-how-to-use-dotnet#insert-a-batch-of-entities
            var listOfListOfItems = items.ChunkBy(100);

            foreach (var listOfItems in listOfListOfItems)
            {
                // Create the batch operation.
                TableBatchOperation batchOperation = new TableBatchOperation();

                // add codes to the batch operation
                foreach (var item in listOfItems)
                {
                    // Create a retrieve operation that takes an entity.
                    TableOperation retrieveOperation = TableOperation.Retrieve<TableEntityAdapter<Code>>(eventName, item);

                    // Execute the retrieve operation.
                    TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);

                    // get the result and create a new object from the data
                    var result = (TableEntityAdapter<Code>)retrievedResult.Result;

                    // Add to delete operation
                    batchOperation.Delete(result);
                }

                // Execute the batch operation.
                await table.ExecuteBatchAsync(batchOperation);
            }
        }

        public async Task<Code> GetCode(string eventName, string id)
        {
            //get cloudtable
            var table = await GetCloudTable(_appSecretSettings.TableStorageConnectionString, _appSettings.TableStorageContainerName);

            // Create a retrieve operation that takes an entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<TableEntityAdapter<Code>>(eventName, id);

            // Execute the retrieve operation.
            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);

            if (retrievedResult.Result != null)
            {
                // get the result and create a new object from the data
                var deviceResult = (TableEntityAdapter<Code>)retrievedResult.Result;

                return deviceResult.OriginalEntity;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<Code>> GetCodes()
        {
            var table = await GetCloudTable(_appSecretSettings.TableStorageConnectionString, _appSettings.TableStorageContainerName);

            TableContinuationToken token = null;

            var entities = new List<TableEntityAdapter<Code>>();
            //TableQuery<TableEntityAdapter<Code>> query = new TableQuery<TableEntityAdapter<Code>>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, eventName));
            TableQuery<TableEntityAdapter<Code>> query = new TableQuery<TableEntityAdapter<Code>>();
            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            // create list of objects from the storage entities
            var codes = new List<Code>();
            foreach (var entity in entities)
            {
                codes.Add(entity.OriginalEntity);
            }

            //filter by owner
            //TO DO, is there a better way to do this as part of the query?
            //var codesForOwner = codes.Where(c => c.EventName.ToLower() == eventName.ToLower()).ToList();

            return codes;
        }

        private async Task<CloudTable> GetCloudTable(string tableConnectionString, string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(tableConnectionString);

            var blobClient = storageAccount.CreateCloudTableClient();

            var table = blobClient.GetTableReference(containerName);

            await table.CreateIfNotExistsAsync();

            return table;
        }
    }
}
