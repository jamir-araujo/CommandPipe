
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace CommandPipe
{
    public static class CommandSenderExtensions
    {
        /// <summary>
        /// Sends a command to be executed on a handler.
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="sender">The command sender</param>
        /// <param name="command">The command the be send</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>The result of the command</returns>
        public static async Task<TResult> SendAsync<TCommand, TResult>(this ICommandSender sender, TCommand command, CancellationToken cancellationToken = default)
        {
            var context = new CommandContext<TCommand, TResult>(command);

            return await sender.SendAsync(context, cancellationToken);
        }

        /// <summary>
        /// Sends a command to be executed on a handler.
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <param name="sender">The command sender</param>
        /// <param name="command">The command the be send</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>A boolean indication weather the handler was executed or not</returns>
        public static async Task<bool> SendAsync<TCommand>(this ICommandSender sender, TCommand command, CancellationToken cancellationToken = default)
        {
            var context = new CommandContext<TCommand, bool>(command);

            return await sender.SendAsync(context, cancellationToken);
        }

        /// <summary>
        /// Sends a command to be executed on a handler.
        /// </summary>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="sender">The command sender</param>
        /// <param name="command">The command the be send</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>The result of the command</returns>
        public static async Task<TResult> SendAsync<TResult>(this ICommandSender sender, object command, CancellationToken cancellationToken = default)
        {
            Check.NotNull(command, nameof(command));

            var proxy = CommandGenericProxy.GetProxy(command.GetType());

            return await proxy.SendAsync<TResult>(sender, command, cancellationToken);
        }

        /// <summary>
        /// Sends a command to be executed on a handler.
        /// </summary>
        /// <param name="sender">The command sender</param>
        /// <param name="command">The command the be send</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>A boolean indication weather the handler was executed or not</returns>
        public static async Task<bool> SendAsync(this ICommandSender sender, object command, CancellationToken cancellationToken = default)
        {
            Check.NotNull(command, nameof(command));

            var proxy = CommandGenericProxy.GetProxy(command.GetType());

            return await proxy.SendAsync<bool>(sender, command, cancellationToken);
        }

        /// <summary>
        /// Sends a command that implements the <see cref="ICommand{TResult}"/> interface to be executed on a handler.
        /// </summary>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="sender">The command sender</param>
        /// <param name="command">The command the be send</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>The result of the command</returns>
        public static async Task<TResult> SendCommandAsync<TResult>(this ICommandSender sender, ICommand<TResult> command, CancellationToken cancellationToken = default)
        {
            Check.NotNull(command, nameof(command));

            var proxy = CommandGenericProxy.GetProxy(command.GetType());

            return await proxy.SendAsync<TResult>(sender, command, cancellationToken);
        }

        abstract class CommandGenericProxy
        {
            private static readonly ConcurrentDictionary<Type, CommandGenericProxy> _commnad = new ConcurrentDictionary<Type, CommandGenericProxy>();

            public static CommandGenericProxy GetProxy(Type commnadType)
            {
                return _commnad.GetOrAdd(commnadType, type =>
                {
                    var proxyType = typeof(CommandGenericProxy<>).MakeGenericType(type);
                    return (CommandGenericProxy)Activator.CreateInstance(proxyType);
                });
            }

            public abstract Task<TResult> SendAsync<TResult>(ICommandSender sender, object command, CancellationToken cancellationToken);
        }

        class CommandGenericProxy<TCommand> : CommandGenericProxy
        {
            public override async Task<TResult> SendAsync<TResult>(ICommandSender sender, object command, CancellationToken cancellationToken)
                => await sender.SendAsync<TCommand, TResult>((TCommand)command, cancellationToken);
        }
    }
}
