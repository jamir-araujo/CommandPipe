using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace CommandPipe
{
    public class CommandSender : ICommandSender
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandSender(IServiceProvider serviceProvider)
        {
            _serviceProvider = Check.NotNull(serviceProvider, nameof(serviceProvider));
        }

        public async Task<TResult> SendAsync<TCommand, TResult>(ICommandContext<TCommand, TResult> context, CancellationToken cancellationToken = default)
        {
            Check.NotNull(context, nameof(context));

            var handler = _serviceProvider.GetService<ICommandHandler<TCommand, TResult>>();
            if (handler is null)
            {
                throw new InvalidOperationException($"No Command Handler found for the command {typeof(TCommand)} with the result {typeof(TResult)}.");
            }

            var pipeline = _serviceProvider.GetService<CommandPipeline<TCommand, TResult>>();

            await pipeline.SendAsync(context, async () =>
            {
                await handler.HandleAsync(context, cancellationToken);

                context.HandlerExecuted = true;
            }, cancellationToken);

            return context.Result;
        }
    }
}
