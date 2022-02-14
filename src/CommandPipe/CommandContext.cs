
using System.Collections.Generic;

namespace CommandPipe
{
    /// <summary>
    /// Represents a context of a command.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command</typeparam>
    /// <typeparam name="TResult">The type of the result</typeparam>
    public class CommandContext<TCommand, TResult> : ICommandContext<TCommand, TResult>
    {
        /// <summary>
        /// Instantiate a command context with a command.
        /// </summary>
        /// <param name="command">The command</param>
        public CommandContext(TCommand command) => Command = command;

        /// <summary>
        /// Instantiate a command context with a command and a result.
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="result">The result</param>
        public CommandContext(TCommand command, TResult result)
            : this(command) => Result = result;

        ///<inheritdoc/>
        public TCommand Command { get; }

        ///<inheritdoc/>
        public bool HandlerExecuted { get; set; }

        ///<inheritdoc/>
        public TResult Result { get; set; }

        ///<inheritdoc/>
        public IDictionary<string, object> Items { get; } = new Dictionary<string, object>();
    }
}
