using System;
using Microsoft.Extensions.Logging;

namespace Common.Extensions
{
    public static class LoggingExtension
    {
        /// <summary>
        /// Logs an information message with a correlation identifier.
        /// </summary>
        public static void LogWithCorrelation<T>(this ILogger<T> logger, string message, string correlationId)
        {
            logger.LogInformation("[CorrelationId: {CorrelationId}] {Message}", correlationId, message);
        }

        /// <summary>
        /// Logs an error with exception details and a correlation identifier.
        /// </summary>
        public static void LogErrorWithCorrelation<T>(this ILogger<T> logger, Exception ex, string message, string correlationId)
        {
            logger.LogError(ex, "[CorrelationId: {CorrelationId}] {Message}", correlationId, message);
        }

        /// <summary>
        /// Logs a debug message with a correlation identifier.
        /// </summary>
        public static void LogDebugWithCorrelation<T>(this ILogger<T> logger, string message, string correlationId)
        {
            logger.LogDebug("[CorrelationId: {CorrelationId}] {Message}", correlationId, message);
        }
    }
}