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

        public StoreRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public async Task StoreCode(Code item)
        {
            var table = await GetCloudTable(_appSettings.TableStorageConnectionString, _appSettings.TableStorageContainerName);

            var rowKey = item.PromoCode;

            TableEntityAdapter<Code> entity = new TableEntityAdapter<Code>(item, _appSettings.TableStoragePartitionKey, rowKey.ToString());

            TableOperation insertOperation = TableOperation.InsertOrReplace(entity);

            await table.ExecuteAsync(insertOperation);
        }

        public async Task DeleteCode(string id)
        {
            //get cloudtable
            var table = await GetCloudTable(_appSettings.TableStorageConnectionString, _appSettings.TableStorageContainerName);

            // Create a retrieve operation that expects a the right entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<TableEntityAdapter<Code>>(_appSettings.CodesTableStoragePartitionKey, id);

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

        public async Task<Code> GetCode(string id)
        {
            //get cloudtable
            var table = await GetCloudTable(_appSettings.TableStorageConnectionString, _appSettings.TableStorageContainerName);

            // Create a retrieve operation that takes an entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<TableEntityAdapter<Code>>(_appSettings.TableStoragePartitionKey, id);

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
            var table = await GetCloudTable(_appSettings.TableStorageConnectionString, _appSettings.TableStorageContainerName);

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
            foreach (var deviceStorageTableEntity in entities)
            {
                codes.Add(deviceStorageTableEntity.OriginalEntity);
            }

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
