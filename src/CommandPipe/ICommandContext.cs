
using System.Collections.Generic;

namespace CommandPipe
{
    /// <summary>
    /// Represents a context of a command.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command</typeparam>
    /// <typeparam name="TResult">The type of the result</typeparam>
    public interface ICommandContext<out TCommand, TResult>
    {
        /// <summary>
        /// Gets the command.
        /// </summary>
        TCommand Command { get; }

        /// <summary>
        /// Gets or sets whether the handler has been executed successfully.
        /// </summary>
        bool HandlerExecuted { get; set; }

        /// <summary>
        /// Gets or sets the result of the command.
        /// </summary>
        TResult Result { get; set; }

        /// <summary>
        /// Gets a dictionary of extension items for the context.
        /// </summary>
        IDictionary<string, object> Items { get; }
    }
}
