
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommandPipe
{
    /// <summary>
    /// Represents the base class to implement a command middleware.
    /// </summary>
    /// <typeparam name="TCommand">the command type</typeparam>
    /// <typeparam name="TResult">the result type</typeparam>
    public abstract class CommandMiddleware<TCommand, TResult> : ICommandMiddleware<TCommand, TResult>
    {
        public abstract Task<TResult> InvokeAsync(TCommand command, Func<Task<TResult>> next, CancellationToken cancellationToken = default);

        public async Task InvokeAsync(ICommandContext<TCommand, TResult> context, Func<Task> next, CancellationToken cancellationToken = default)
        {
            context.Result = await InvokeAsync(context.Command, async () =>
            {
                await next();

                return context.Result;
            }, cancellationToken);
        }
    }

    /// <summary>
    /// Represents the base class to implement a command middleware for void commands.
    /// </summary>
    /// <typeparam name="TCommand">the command type</typeparam>
    public abstract class CommandMiddleware<TCommand> : ICommandMiddleware<TCommand, bool>
    {
        public abstract Task InvokeAsync(TCommand command, Func<Task> next, CancellationToken cancellationToken = default);

        public async Task InvokeAsync(ICommandContext<TCommand, bool> context, Func<Task> next, CancellationToken cancellationToken = default)
        {
            await InvokeAsync(context.Command, next, cancellationToken);
        }
    }
}
