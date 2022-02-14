
using CommandPipe.Annotations;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommandPipe
{
    [IgnoreOnAssemblyRead]
    internal class RelayCommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    {
        private readonly Func<ICommandContext<TCommand, TResult>, CancellationToken, Task> _handler;

        public RelayCommandHandler(Func<ICommandContext<TCommand, TResult>, CancellationToken, Task> handler) => _handler = handler;

        public async Task HandleAsync(ICommandContext<TCommand, TResult> context, CancellationToken cancellationToken = default) => await _handler(context, cancellationToken);
    }
}
