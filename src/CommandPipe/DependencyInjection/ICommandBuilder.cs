
using System;
using System.Threading;
using System.Threading.Tasks;

using CommandPipe;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Represents the configuration object for the command.
    /// </summary>
    /// <typeparam name="TCommand">The command type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    public interface ICommandBuilder<TCommand, TResult>
    {
        /// <summary>
        /// Get the application services.
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// Adds a middleware to the command and result combination.
        /// </summary>
        /// <typeparam name="TMiddleware">The middleware type</typeparam>
        ICommandBuilder<TCommand, TResult> AddMiddleware<TMiddleware>() where TMiddleware : class, ICommandMiddleware<TCommand, TResult>;

        /// <summary>
        /// Adds a middleware to the command and result combination.
        /// </summary>
        /// <typeparam name="TMiddleware">The middleware type</typeparam>
        /// <param name="middleware">The middleware instance</param>
        ICommandBuilder<TCommand, TResult> AddMiddleware<TMiddleware>(TMiddleware middleware) where TMiddleware : class, ICommandMiddleware<TCommand, TResult>;

        /// <summary>
        /// Adds an inline middleware to the command and result combination.
        /// </summary>
        /// <param name="middleware">the inline middleware</param>
        ICommandBuilder<TCommand, TResult> AddMiddleware(Func<ICommandContext<TCommand, TResult>, Func<Task>, CancellationToken, Task> middleware);
    }

    internal class CommandBuilder<TCommand, TResult> : ICommandBuilder<TCommand, TResult>
    {
        public IServiceCollection Services { get; }

        public CommandBuilder(IServiceCollection services)
        {
            Services = services;

            Services.TryAddScoped(typeof(CommandPipeline<,>));
            Services.TryAddScoped<ICommandSender, CommandSender>();
        }

        public CommandBuilder<TCommand, TResult> AddHandler<TCommandHandler>(TCommandHandler handler)
            where TCommandHandler : class, ICommandHandler<TCommand, TResult>
        {
            Services.AddSingleton<ICommandHandler<TCommand, TResult>>(handler);

            return this;
        }

        public ICommandBuilder<TCommand, TResult> AddMiddleware<TMiddleware>()
             where TMiddleware : class, ICommandMiddleware<TCommand, TResult>
        {
            Services.AddScoped<ICommandMiddleware<TCommand, TResult>, TMiddleware>();

            return this;
        }

        public ICommandBuilder<TCommand, TResult> AddMiddleware<TMiddleware>(TMiddleware middleware)
            where TMiddleware : class, ICommandMiddleware<TCommand, TResult>
        {
            Services.AddSingleton<ICommandMiddleware<TCommand, TResult>>(middleware);

            return this;
        }

        public ICommandBuilder<TCommand, TResult> AddMiddleware(Func<ICommandContext<TCommand, TResult>, Func<Task>, CancellationToken, Task> middleware)
        {
            return AddMiddleware(new RelayMiddleware<TCommand, TResult>(middleware));
        }
    }

    internal class CommandBuilder<TCommand, TResult, TCommandHandler> : CommandBuilder<TCommand, TResult>
        where TCommandHandler : class, ICommandHandler<TCommand, TResult>
    {
        public CommandBuilder(IServiceCollection services)
            : base(services)
        { }

        public CommandBuilder<TCommand, TResult, TCommandHandler> AddHandler()
        {
            Services.AddScoped<ICommandHandler<TCommand, TResult>, TCommandHandler>();

            return this;
        }
    }
}
