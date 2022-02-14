
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommandPipe
{
    /// <summary>
    /// Represents the middleware to be placed and execute on the command pipeline.
    /// </summary>
    /// <typeparam name="TCommand">The command type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    public interface ICommandMiddleware<in TCommand, TResult>
    {
        /// <summary>
        /// Invokes the middleware that may or not call the next middlware.
        /// </summary>
        /// <param name="context">The context of the command been executed</param>
        /// <param name="next">The func to invoke the next middleware</param>
        /// <param name="cancellationToken">The token to cancel the operation</param>
        Task InvokeAsync(ICommandContext<TCommand, TResult> context, Func<Task> next, CancellationToken cancellationToken = default);
    }
}
