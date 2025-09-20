using Microsoft.Extensions.Logging;
using ServiceAbstraction;
using System;
using System.Threading.Tasks;

namespace Service.CoreServices
{
	public class LoggingService : ILoggingService
	{
		private readonly ILogger<LoggingService> _logger;

		public LoggingService(ILogger<LoggingService> logger)
		{
			_logger = logger;
		}

		public void LogInformation(string message, params object[] args)
		{
			_logger.LogInformation(message, args);
		}

		public void LogWarning(string message, params object[] args)
		{
			_logger.LogWarning(message, args);
		}

		public void LogError(string message, Exception? exception = null)
		{
			if (exception != null)
			{
				_logger.LogError(exception, message);
			}
			else
			{
				_logger.LogError(message);
			}
		}

		public void LogDebug(string message, params object[] args)
		{
			_logger.LogDebug(message, args);
		}

		public async Task<T> LogOperationAsync<T>(string operationName, Func<Task<T>> operation)
		{
			try
			{
				_logger.LogInformation("Starting operation: {OperationName}", operationName);
				var startTime = DateTime.UtcNow;

				var result = await operation();

				var duration = DateTime.UtcNow - startTime;
				_logger.LogInformation("Completed operation: {OperationName} in {Duration}ms", operationName, duration.TotalMilliseconds);

				return result;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Operation failed: {OperationName}", operationName);
				throw;
			}
		}

		public T LogOperation<T>(string operationName, Func<T> operation)
		{
			try
			{
				_logger.LogInformation("Starting operation: {OperationName}", operationName);
				var startTime = DateTime.UtcNow;

				var result = operation();

				var duration = DateTime.UtcNow - startTime;
				_logger.LogInformation("Completed operation: {OperationName} in {Duration}ms", operationName, duration.TotalMilliseconds);

				return result;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Operation failed: {OperationName}", operationName);
				throw;
			}
		}

		public void LogError(Exception ex, string message, Exception? exception = null)
		{
			try
			{
				_logger?.LogError(ex, message, exception);
			}
			catch (Exception ex2)
			{
				_logger?.LogError(ex2, ex2.Message);

			}

		}


	}
}
