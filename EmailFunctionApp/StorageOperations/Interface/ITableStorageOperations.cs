using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StorageOperations.Interface
{
    public interface ITableStorageOperations
    {
        Task<T> GetAsync<T>(string partitionKey, string rowkey, TableClient _tableClient) where T : class, ITableEntity, new();
        Task CreateAsync<T>(T entity, TableClient _tableClient) where T : class, ITableEntity, new();
        IAsyncEnumerable<T> QueryAsync<T>(string filterText, CancellationToken cancellationToken, TableClient _tableClient) where T : class, ITableEntity, new();
        Task UpdateAsync<T>(T entity, TableClient _tableClient) where T : class, ITableEntity, new();
    }
}
