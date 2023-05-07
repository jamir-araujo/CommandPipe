
using System.Collections.Generic;

namespace CommandPipe.Validations
{
    /// <summary>
    /// Represents a validation failure.
    /// </summary>
    public class ValidationFailure
    {
        /// <summary>
        /// Instantiates an empty <see cref="ValidationFailure"/>;
        /// </summary>
        public ValidationFailure() { }

        /// <summary>
        /// Instantiates an <see cref="ValidationFailure"/> with a propertyName, errorMessage and errorCode;
        /// </summary>
        /// <param name="errorCode">The error code</param>
        /// <param name="errorMessage">The error message</param>
        /// <param name="propertyName">The name of the property that failed the validation</param>
        public ValidationFailure(string errorCode, string errorMessage, string propertyName)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            PropertyName = propertyName;
        }

        /// <summary>
        /// Gets or sets the error code
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets the extension items for this validation
        /// </summary>
        public IDictionary<string, object> Items { get; } = new Dictionary<string, object>();
    }
}
