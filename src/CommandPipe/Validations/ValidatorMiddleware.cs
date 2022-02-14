using CommandPipe.Annotations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CommandPipe.Validations
{
    [IgnoreOnAssemblyRead]
    internal class ValidatorMiddleware<TCommand, TResult> : ICommandMiddleware<TCommand, TResult>
    {
        private readonly IEnumerable<ICommandValidator<TCommand>> _validators;

        public ValidatorMiddleware(
            IEnumerable<ICommandValidator<TCommand>> validators)
        {
            _validators = validators;
        }

        public async Task InvokeAsync(ICommandContext<TCommand, TResult> context, Func<Task> next, CancellationToken cancellationToken = default)
        {
            var failures = new List<ValidationFailure>();

            foreach (var validator in _validators)
            {
                var result = await validator.ValidateAsync(context.Command, cancellationToken);

                if (!result.IsValid)
                {
                    failures.AddRange(result.Errors);
                }
            }

            if (failures.Any())
            {
                context.SetErrors(failures);
            }
            else
            {
                await next();
            }
        }
    }
}
