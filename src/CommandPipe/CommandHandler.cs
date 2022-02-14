
using System.Threading;
using System.Threading.Tasks;

namespace CommandPipe
{
    /// <summary>
    /// Reprensents a base class to implement a command handler
    /// </summary>
    /// <typeparam name="TCommand">The command type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    public abstract class CommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    {
        /// <summary>
        /// Executes the command and returns the result.
        /// </summary>
        /// <param name="command">The command to be executed</param>
        /// <param name="cancellationToken">The token to cancel the operation</param>
        /// <returns>Returns the result of the command.</returns>
        public abstract Task<TResult> ExecuteAsync(TCommand command, CancellationToken cancellationToken = default);

        public async Task HandleAsync(ICommandContext<TCommand, TResult> context, CancellationToken cancellationToken = default)
        {
            context.Result = await ExecuteAsync(context.Command, cancellationToken);
        }
    }

    /// <summary>
    /// Reprensents a base class to implement a command handler
    /// </summary>
    /// <typeparam name="TCommand">The command type</typeparam>
    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand, bool>
    {
        /// <summary>
        /// Executes the command and returns the result.
        /// </summary>
        /// <param name="command">The command to be executed</param>
        /// <param name="cancellationToken">The token to cancel the operation</param>
        public abstract Task ExecuteAsync(TCommand command, CancellationToken cancellationToken = default);

        public async Task HandleAsync(ICommandContext<TCommand, bool> context, CancellationToken cancellationToken = default)
        {
            await ExecuteAsync(context.Command, cancellationToken);

            context.Result = true;
        }
    }

}
