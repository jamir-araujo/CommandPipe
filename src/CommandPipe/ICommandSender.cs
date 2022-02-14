
using System.Threading;
using System.Threading.Tasks;

namespace CommandPipe
{
    /// <summary>
    /// Represents the sender for commands.
    /// </summary>
    public interface ICommandSender
    {
        /// <summary>
        /// Sends a command context the be executed by a handler
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="context">The command context to be send</param>
        /// <param name="cancellationToken">The token to cancel the operation</param>
        Task<TResult> SendAsync<TCommand, TResult>(ICommandContext<TCommand, TResult> context, CancellationToken cancellationToken = default);
    }
}
