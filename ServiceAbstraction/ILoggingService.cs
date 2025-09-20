using System;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface ILoggingService
    {
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(string message, Exception? exception = null);
        void LogDebug(string message, params object[] args);
        Task<T> LogOperationAsync<T>(string operationName, Func<Task<T>> operation);
        T LogOperation<T>(string operationName, Func<T> operation);
    }
}
