
using System.Threading;
using System.Threading.Tasks;

namespace CommandPipe
{
    /// <summary>
    /// Represents the handler to execute the command.
    /// </summary>
    /// <typeparam name="TCommand">The command type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    public interface ICommandHandler<in TCommand, TResult>
    {
        /// <summary>
        /// Handles the execution of the command.
        /// </summary>
        /// <param name="context">The context of the command</param>
        /// <param name="cancellationToken">The token to cancel the operation</param>
        Task HandleAsync(ICommandContext<TCommand, TResult> context, CancellationToken cancellationToken = default);
    }
}
