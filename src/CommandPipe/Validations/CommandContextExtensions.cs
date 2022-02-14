using CommandPipe.Validations;

using System.Collections.Generic;

namespace CommandPipe
{
    public static class CommandContextExtensions
    {
        private const string HasErrors = "HasError";
        private const string ValidationFailures = "ValidationFailures";

        /// <summary>
        /// Sets if the context has errors.
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="context">The context</param>
        /// <param name="hasError">the flag informing if the context has errors</param>
        public static void SetHasError<TCommand, TResult>(this ICommandContext<TCommand, TResult> context, bool hasError)
        {
            context.Items[HasErrors] = hasError;
        }

        /// <summary>
        /// Gets if the context has errors.
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="context">The context</param>
        /// <returns>Returns true if the context has erros, otherwise returns false</returns>
        public static bool HasError<TCommand, TResult>(this ICommandContext<TCommand, TResult> context)
        {
            return context.Items.TryGetValue(HasErrors, out var value) && value is bool hasError && hasError;
        }

        public static void SetErrors<TCommand, TResult>(this ICommandContext<TCommand, TResult> context, List<ValidationFailure> failures)
        {
            if (context.Items.TryGetValue(ValidationFailures, out var value) && value is List<ValidationFailure> current)
            {
                current.AddRange(failures);
            }
            else
            {
                context.Items[ValidationFailures] = failures;
            }

            context.SetHasError(true);
        }

        public static List<ValidationFailure> GetErrors<TCommand, TResult>(this ICommandContext<TCommand, TResult> context)
        {
            if (context.Items.TryGetValue(ValidationFailures, out var value) && value is List<ValidationFailure> failures)
            {
                return failures;
            }

            return null;
        }
    }
}
