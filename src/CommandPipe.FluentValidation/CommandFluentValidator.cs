
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using FluentValidationResult = FluentValidation.Results.ValidationResult;
using FluentValidationFailure = FluentValidation.Results.ValidationFailure;
using CommandPipe.Validations;

namespace FluentValidation
{
    /// <summary>
    /// Represents a validation class for commands.
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    public abstract class CommandFluentValidator<TCommand> : AbstractValidator<TCommand>, ICommandValidator<TCommand>
    {
        private bool _configured;

        /// <summary>
        /// Configures the validations for the command.
        /// </summary>
        public abstract void Configure();

        protected override bool PreValidate(ValidationContext<TCommand> context, FluentValidationResult result)
        {
            if (!_configured)
            {
                Configure();

                _configured = true;
            }

            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new FluentValidationFailure("this", "command cannot be null")
                {
                    ErrorCode = "CommandCannotBeNull"
                });

                return false;
            }
            else
            {
                return base.PreValidate(context, result);
            }
        }

        async Task<ValidationResult> ICommandValidator<TCommand>.ValidateAsync(TCommand commnad, CancellationToken cancellation)
        {
            var validationResult = await ValidateAsync(commnad, cancellation);

            var errors = new List<ValidationFailure>();

            foreach (var failure in validationResult.Errors)
            {
                errors.Add(new ValidationFailure(failure.ErrorCode, failure.ErrorMessage, failure.PropertyName));
            }

            return new ValidationResult(errors);
        }
    }
}
