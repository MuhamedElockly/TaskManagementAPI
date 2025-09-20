using System;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface ILoggingRepository
    {
        void LogDatabaseOperation(string operation, string entity, object? id = null);
        void LogDatabaseError(string operation, string entity, Exception ex, object? id = null);
        Task<T> LogDatabaseOperationAsync<T>(string operation, string entity, Func<Task<T>> dbOperation, object? id = null);
    }
}
