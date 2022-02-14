using System;

using FluentValidation;

namespace CommandPipe.FluentValidation
{
    internal class RelayFluentValidador<TCommand> : CommandFluentValidator<TCommand>
    {
        private readonly Action<CommandFluentValidator<TCommand>> _configure;

        public RelayFluentValidador(Action<CommandFluentValidator<TCommand>> configure) => _configure = configure;

        public override void Configure() => _configure?.Invoke(this);
    }
}
