using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CommandPipe.Annotations;

using Microsoft.Extensions.Logging;

namespace CommandPipe
{
    internal class CommandPipeline<TCommand, TResult>
    {
        private readonly IEnumerable<ICommandMiddleware<TCommand, TResult>> _middlewares;
        private readonly ILogger _logger;

        public CommandPipeline(IEnumerable<ICommandMiddleware<TCommand, TResult>> middlewares, ILogger<CommandPipeline<TCommand, TResult>> logger)
        {
            _middlewares = middlewares;
            _logger = logger;
        }

        public async Task SendAsync(ICommandContext<TCommand, TResult> context, Func<Task> next, CancellationToken cancellationToken)
        {
            ICommandMiddleware<TCommand, TResult> middleware = null;
            if (_middlewares.Any())
            {
                middleware = _middlewares
                    .Aggregate((outer, inner) => new MiddlewareConnector(outer, inner, _logger));
            }
            else
            {
                middleware = new RelayMiddleware<TCommand, TResult>((c, n, ct) => n());
            }

            await middleware.InvokeAsync(context, next, cancellationToken);
        }

        [IgnoreOnAssemblyRead]
        class MiddlewareConnector : ICommandMiddleware<TCommand, TResult>
        {
            private readonly ICommandMiddleware<TCommand, TResult> _outer;
            private readonly ICommandMiddleware<TCommand, TResult> _inner;
            private readonly ILogger _logger;

            public MiddlewareConnector(ICommandMiddleware<TCommand, TResult> outer, ICommandMiddleware<TCommand, TResult> inner, ILogger logger)
            {
                _outer = outer;
                _inner = inner;
                _logger = logger;
            }

            public async Task InvokeAsync(ICommandContext<TCommand, TResult> context, Func<Task> next, CancellationToken cancellationToken = default)
            {
                _logger.ExecutingMiddleware(_outer, context.Command);

                await _outer.InvokeAsync(context, async () =>
                {
                    await _inner.InvokeAsync(context, next, cancellationToken);
                }, cancellationToken);

                _logger.MiddlewareExecuted(_outer, context.Command);
            }
        }
    }
}
