using System;

namespace Microsoft.Extensions.Logging
{
    internal static class LoggerExtensions
    {
        private static Action<ILogger, Type, Type, Exception> _executionMiddleware = LoggerMessage
            .Define<Type, Type>(LogLevel.Debug, 1, "Execution middleware {middleware} for command type {type}...");

        private static Action<ILogger, Type, Type, Exception> _middlewareExecuted = LoggerMessage
            .Define<Type, Type>(LogLevel.Debug, 2, "Middleware {middleware} for command type {type} executed successfully.");

        public static void ExecutingMiddleware(this ILogger logger, object middleware, object command)
        {
            _executionMiddleware(logger, middleware.GetType(), command.GetType(), null);
        }

        public static void MiddlewareExecuted(this ILogger logger, object middleware, object command)
        {
            _middlewareExecuted(logger, middleware.GetType(), command.GetType(), null);
        }
    }
}
