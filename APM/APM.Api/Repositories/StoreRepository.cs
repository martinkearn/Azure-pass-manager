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
            // TO DO: This overwrites codes if someone else has inserted the same batch. need to use owner as the partition key
            var table = await GetCloudTable(_appSecretSettings.TableStorageConnectionString, _appSettings.TableStorageContainerName);

            TableEntityAdapter<Code> entity = new TableEntityAdapter<Code>(item, item.Owner, item.PromoCode);

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
                    TableEntityAdapter<Code> entity = new TableEntityAdapter<Code>(item, item.Owner, item.PromoCode);
                    batchOperation.InsertOrReplace(entity);
                }

                // Execute the batch operation.
                await table.ExecuteBatchAsync(batchOperation);
            }

        }

        public async Task DeleteCode(string owner, string id)
        {
            //get cloudtable
            var table = await GetCloudTable(_appSecretSettings.TableStorageConnectionString, _appSettings.TableStorageContainerName);

            // Create a retrieve operation that expects a the right entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<TableEntityAdapter<Code>>(owner, id);

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

        public async Task DeleteCodes(string owner, string codeIds)
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
                    TableOperation retrieveOperation = TableOperation.Retrieve<TableEntityAdapter<Code>>(owner, item);

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

        public async Task<Code> GetCode(string owner, string id)
        {
            //get cloudtable
            var table = await GetCloudTable(_appSecretSettings.TableStorageConnectionString, _appSettings.TableStorageContainerName);

            // Create a retrieve operation that takes an entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<TableEntityAdapter<Code>>(owner, id);

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

        public async Task<List<Code>> GetCodes(string owner)
        {
            var table = await GetCloudTable(_appSecretSettings.TableStorageConnectionString, _appSettings.TableStorageContainerName);

            TableContinuationToken token = null;

            var entities = new List<TableEntityAdapter<Code>>();
            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(new TableQuery<TableEntityAdapter<Code>>(), token);
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
            var codesForOwner = codes.Where(c => c.Owner.ToLower() == owner.ToLower()).ToList();

            return codesForOwner;
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
