using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CommandPipe;
using CommandPipe.Annotations;
using CommandPipe.Validations;

using Xunit;

namespace CommandPipe.Tests
{
    public class ValidatorMiddlewareTests
    {
        private readonly List<ICommandValidator<TheCommand>> _validators;
        private readonly ValidatorMiddleware<TheCommand, bool> _middleware;

        public ValidatorMiddlewareTests()
        {
            _validators = new List<ICommandValidator<TheCommand>>();
            _middleware = new ValidatorMiddleware<TheCommand, bool>(_validators);
        }

        [Fact]
        public async Task HasErrors_Should_ReturnFalse_When_MiddlewareAddNoErrors()
        {
            var context = new CommandContext<TheCommand, bool>(new TheCommand());
            await _middleware.InvokeAsync(context, () => Task.CompletedTask);

            Assert.False(context.HasError());
        }

        [Fact]
        public async Task TryGetErrors_Should_ReturnFalse_When_MiddlewareAddNoErrors()
        {
            var context = new CommandContext<TheCommand, bool>(new TheCommand());
            await _middleware.InvokeAsync(context, () => Task.CompletedTask);

            Assert.Null(context.GetErrors());
        }

        [Fact]
        public async Task Should_CallNext_When_MiddlewareAddNoErrors()
        {
            var context = new CommandContext<TheCommand, bool>(new TheCommand());
            var called = false;
            await _middleware.InvokeAsync(context, () =>
            {
                called = true;
                return Task.CompletedTask;
            });

            Assert.True(called);
        }

        [Fact]
        public async Task HasErrors_ShouldReturnTrue_When_MiddlewareAddsErrors()
        {
            _validators.Add(new RelayValidator(command =>
            {
                return new ValidationResult(new List<ValidationFailure>
                {
                    new ValidationFailure("1293-ae-d", "Something went wrong", "MyProperty")
                });
            }));

            var context = new CommandContext<TheCommand, bool>(new TheCommand());
            await _middleware.InvokeAsync(context, () => Task.CompletedTask);

            Assert.True(context.HasError());
        }

        [Fact]
        public async Task TryGetErrors_Should_ReturnTrue_When_MiddlewareAddsErrors()
        {
            _validators.Add(new RelayValidator(command =>
            {
                return new ValidationResult(new List<ValidationFailure>
                {
                    new ValidationFailure("1293-ae-d", "Something went wrong", "MyProperty")
                });
            }));

            var context = new CommandContext<TheCommand, bool>(new TheCommand());
            await _middleware.InvokeAsync(context, () => Task.CompletedTask);

            var errors = context.GetErrors();
            var error = Assert.Single(errors);
            Assert.Equal("1293-ae-d", error.ErrorCode);
            Assert.Equal("Something went wrong", error.ErrorMessage);
            Assert.Equal("MyProperty", error.PropertyName);
        }

        [IgnoreOnAssemblyRead]
        public class RelayValidator : ICommandValidator<TheCommand>
        {
            private readonly Func<TheCommand, ValidationResult> _validation;

            public RelayValidator(Func<TheCommand, ValidationResult> validation)
            {
                _validation = validation;
            }

            public Task<ValidationResult> ValidateAsync(TheCommand value, CancellationToken cancellation = default)
            {
                return _validation(value).AsTask();
            }
        }

        public enum Errors
        {
            MyPropertyIsRequired
        }
    }
}
