using Azure;
using Azure.Data.Tables;
using StorageOperations.Interface;

namespace StorageOperations.Implementation
{
    public class TableStorageOperations : ITableStorageOperations
    {
        public async Task CreateAsync<T>(T entity, TableClient _tableClient) where T : class, ITableEntity, new()
        {
            await _tableClient.CreateIfNotExistsAsync();
            await _tableClient.AddEntityAsync<T>(entity);
        }
        public async Task UpdateAsync<T>(T entity, TableClient _tableClient) where T : class, ITableEntity, new()
        {
            _tableClient.CreateIfNotExists();
            await _tableClient.UpdateEntityAsync<T>(entity, Azure.ETag.All);
        }
        public async Task<T> GetAsync<T>(string partitionKey, string rowkey, TableClient _tableClient) where T : class, ITableEntity, new()
        {
            var response = await _tableClient.GetEntityAsync<T>(partitionKey, rowkey);
            return response.Value;
        }
        public async IAsyncEnumerable<T> QueryAsync<T>(string filterText, CancellationToken cancellationToken, TableClient _tableClient) where T : class, ITableEntity, new()
        {
            AsyncPageable<T> queryResultsMaxPerPage = _tableClient.QueryAsync<T>(filter: filterText, cancellationToken: cancellationToken);

            await foreach (Page<T> page in queryResultsMaxPerPage.AsPages())
            {
                foreach (T qEntity in page.Values)
                {
                    yield return qEntity;
                }
            }
        }
    }
}
