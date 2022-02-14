using System;

using CommandPipe.FluentValidation;

using FluentValidation;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ICommandPipeBuilderExtensions
    {
        public static ICommandPipeBuilder AddValidator<TCommand>(this ICommandPipeBuilder builder, Action<CommandFluentValidator<TCommand>> configure)
        {
            return builder.AddValidator(new RelayFluentValidador<TCommand>(configure));
        }
    }
}
