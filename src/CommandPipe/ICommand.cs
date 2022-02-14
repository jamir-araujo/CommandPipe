namespace CommandPipe
{
    /// <summary>
    /// Represents an command with a result
    /// </summary>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <remarks>
    /// This interface is just a helper to enable you to call SendAsync on the <see cref="ICommandSender"/>
    /// without having to pass the generic parameter for the result.
    /// </remarks>
    public interface ICommand<TResult> { }

    /// <summary>
    /// Represents an command with a boolean result
    /// </summary>
    public interface ICommand : ICommand<bool> { }
}
