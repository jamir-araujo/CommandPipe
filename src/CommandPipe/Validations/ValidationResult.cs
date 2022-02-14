
using System.Collections.Generic;
using System.Linq;

namespace CommandPipe.Validations
{
    /// <summary>
    /// Represents the result of a validation
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Instantiates an empty result that will be valid.
        /// </summary>
        public ValidationResult() => Errors = new List<ValidationFailure>();

        /// <summary>
        /// Instantiates a result with a collection of failures.
        /// </summary>
        /// <param name="errors">The validation errors</param>
        public ValidationResult(IEnumerable<ValidationFailure> errors)
        {
            Errors = Check.NotNull(errors, nameof(errors)).ToList();
        }

        /// <summary>
        /// Gets the boolean informing if the object validated was valid or not.
        /// </summary>
        public bool IsValid => Errors.Count == 0;

        /// <summary>
        /// Gets the list of validations failures.
        /// </summary>
        public List<ValidationFailure> Errors { get; }
    }
}
