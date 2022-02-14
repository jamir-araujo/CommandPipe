
using CommandPipe.Annotations;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommandPipe
{
    [IgnoreOnAssemblyRead]
    internal class RelayMiddleware<TCommand, TResult> : ICommandMiddleware<TCommand, TResult>
    {
        private readonly Func<ICommandContext<TCommand, TResult>, Func<Task>, CancellationToken, Task> _middleware;

        public RelayMiddleware(Func<ICommandContext<TCommand, TResult>, Func<Task>, CancellationToken, Task> middleware)
            => _middleware = middleware;

        public async Task InvokeAsync(ICommandContext<TCommand, TResult> context, Func<Task> next, CancellationToken cancellationToken = default)
            => await _middleware(context, next, cancellationToken);
    }
}
