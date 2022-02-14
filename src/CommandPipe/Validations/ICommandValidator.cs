
using System.Threading;
using System.Threading.Tasks;

namespace CommandPipe.Validations
{
    /// <summary>
    /// Represents a validator to validate an object.
    /// </summary>
    /// <typeparam name="T">The type of the object to validate</typeparam>
    public interface ICommandValidator<T>
    {
        /// <summary>
        /// Validates the given value.
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="cancellation">the token to cancel the operation</param>
        /// <returns>The result of the validation</returns>
        Task<ValidationResult> ValidateAsync(T value, CancellationToken cancellation = default);
    }
}
