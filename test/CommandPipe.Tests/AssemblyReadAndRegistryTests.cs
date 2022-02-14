using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using CommandPipe;
using CommandPipe.Annotations;
using CommandPipe.Validations;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace CommandPipe.Tests
{
    public class AssemblyReadAndRegistryTests
    {
        [Fact]
        public void CountHandlerRegistriesForThisAssembly()
        {
            var services = new ServiceCollection();

            services.AddCommandPipe(commands =>
            {
                commands.AddCommandHandlersFromAssemblyOfType(GetType());
            });

            var handlers = services.Where(serviceDescriptor =>
            {
                if (serviceDescriptor.ServiceType.IsGenericType)
                {
                    var genericType = serviceDescriptor.ServiceType.GetGenericTypeDefinition();

                    var isHandler = genericType == typeof(ICommandHandler<,>);

                    return isHandler;
                }

                return false;
            });

            Assert.Equal(4, handlers.Count());
            Assert.Contains(handlers, h => h.ImplementationType == typeof(PublicInheretedConcreteHandler));
            Assert.Contains(handlers, h => h.ImplementationType == typeof(PublicConcreteHandler));
            Assert.Contains(handlers, h => h.ImplementationType == typeof(NestedInheretedPublicConcreteHandler));
            Assert.Contains(handlers, h => h.ImplementationType == typeof(NestedPublicConcreteHandler));
        }

        [Fact]
        public void CountMiddlewaresRegistriesForThisAssembly()
        {
            var services = new ServiceCollection();

            services.AddCommandPipe(commands =>
            {
                commands.AddCommandHandlersFromAssemblyOfType(GetType());
            });

            var middlewares = services.Where(serviceDescriptor =>
            {
                if (serviceDescriptor.ServiceType.IsGenericType)
                {
                    var genericType = serviceDescriptor.ServiceType.GetGenericTypeDefinition();

                    var isHandler = genericType == typeof(ICommandMiddleware<,>);

                    return isHandler;
                }

                return false;
            });

            Assert.Equal(11, middlewares.Count());
            Assert.Contains(middlewares, h => h.ImplementationType == typeof(NestedInheretFromBaseCommandMiddleware<,>));
            Assert.Contains(middlewares, h => h.ImplementationType == typeof(NestedConcretePublicMiddlware));
            Assert.Contains(middlewares, h => h.ImplementationType == typeof(NestedConcreteInheretedPublicMiddlware));
            Assert.Contains(middlewares, h => h.ImplementationType == typeof(NestedGenericPublicConcreteMiddlewareInterface<,>));
            Assert.Contains(middlewares, h => h.ImplementationType == typeof(NestedGenericPublicConcreteMiddleware<,>));
            Assert.Contains(middlewares, h => h.ImplementationType == typeof(InheretFromBaseCommandMiddleware<,>));
            Assert.Contains(middlewares, h => h.ImplementationType == typeof(ConcretePublicMiddlware));
            Assert.Contains(middlewares, h => h.ImplementationType == typeof(ConcreteInheretedPublicMiddlware));
            Assert.Contains(middlewares, h => h.ImplementationType == typeof(GenericPublicConcreteMiddlewareInterface<,>));
            Assert.Contains(middlewares, h => h.ImplementationType == typeof(GenericPublicConcreteMiddleware<,>));
            Assert.Contains(middlewares, h => h.ImplementationType == typeof(ValidatorMiddleware<,>));
        }

        [Fact]
        public void CountValidatorsRegistriesForThisAssembly()
        {
            var services = new ServiceCollection();

            services.AddCommandPipe(commands =>
            {
                commands.AddCommandHandlersFromAssemblyOfType(GetType());
            });

            var validators = services.Where(serviceDescriptor =>
            {
                if (serviceDescriptor.ServiceType.IsGenericType)
                {
                    var genericType = serviceDescriptor.ServiceType.GetGenericTypeDefinition();

                    var isHandler = genericType == typeof(ICommandValidator<>);

                    return isHandler;
                }

                return false;
            });

            Assert.Equal(4, validators.Count());
            Assert.Contains(validators, h => h.ImplementationType == typeof(NestedConcretePublicValidator));
            Assert.Contains(validators, h => h.ImplementationType == typeof(NestedInheretConcretePublicValidator));
            Assert.Contains(validators, h => h.ImplementationType == typeof(ConcretePublicValidator));
            Assert.Contains(validators, h => h.ImplementationType == typeof(InheretConcretePublicValidator));
        }

        public abstract class NestedBaseCommandMiddleware<TCommand, TResult> : ICommandMiddleware<TCommand, TResult>
        {
            public abstract Task InvokeAsync(ICommandContext<TCommand, TResult> context, Func<Task> next, CancellationToken cancellationToken = default);
        }

        public class NestedInheretFromBaseCommandMiddleware<TCommand, TResult> : NestedBaseCommandMiddleware<TCommand, TResult>
        {
            public override Task InvokeAsync(ICommandContext<TCommand, TResult> context, Func<Task> next, CancellationToken cancellationToken = default)
            {
                return next();
            }
        }

        public class NestedPublicConcreteHandler : ICommandHandler<TheCommand, TheData>
        {
            public Task HandleAsync(ICommandContext<TheCommand, TheData> context, CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }
        }

        public class NestedInheretedPublicConcreteHandler : CommandHandler<TheCommand, TheData>
        {
            public override Task<TheData> ExecuteAsync(TheCommand command, CancellationToken cancellationToken = default)
            {
                return new TheData().AsTask();
            }
        }

        public abstract class NestedPublicAbstractConcreteHandler : CommandHandler<TheCommand, TheData> { }

        internal class NestedInternalConcreteHandler : CommandHandler<TheCommand, TheData>
        {
            public override Task<TheData> ExecuteAsync(TheCommand command, CancellationToken cancellationToken = default)
            {
                return new TheData().AsTask();
            }
        }

        public class NestedGenericPublicConcreteHandler<TCommand, TData> : CommandHandler<TCommand, TData>
        {
            public override Task<TData> ExecuteAsync(TCommand command, CancellationToken cancellationToken = default)
            {
                return default;
            }
        }

        public class NestedPartialyGenericPublicConcreteHandler<TCommand> : CommandHandler<TCommand, TheData>
        {
            public override Task<TheData> ExecuteAsync(TCommand command, CancellationToken cancellationToken = default)
            {
                return new TheData().AsTask();
            }
        }

        public abstract class NestedGenericAbstractConcreteHandler<TCommand, TData> : CommandHandler<TCommand, TData> { }

        public class NestedConcretePublicMiddlware : ICommandMiddleware<TheCommand, TheData>
        {
            public Task<TheData> InvokeAsync(TheCommand command, Func<Task<TheData>> next, CancellationToken cancellationToken = default)
            {
                return next();
            }

            public Task InvokeAsync(ICommandContext<TheCommand, TheData> context, Func<Task> next, CancellationToken cancellationToken = default)
            {
                return next();
            }
        }

        public class NestedConcreteInheretedPublicMiddlware : CommandMiddleware<TheCommand, TheData>
        {
            public override Task<TheData> InvokeAsync(TheCommand command, Func<Task<TheData>> next, CancellationToken cancellationToken = default)
            {
                return next();
            }
        }

        public abstract class NestedAbastractPublicMiddlware : CommandMiddleware<TheCommand, TheData> { }

        internal class NestedConcreteInternalMiddlware : CommandMiddleware<TheCommand, TheData>
        {
            public override Task<TheData> InvokeAsync(TheCommand command, Func<Task<TheData>> next, CancellationToken cancellationToken = default)
            {
                return next();
            }
        }

        public class NestedGenericPublicConcreteMiddlewareInterface<TCommand, TResult> : ICommandMiddleware<TCommand, TResult>
        {
            public Task InvokeAsync(ICommandContext<TCommand, TResult> context, Func<Task> next, CancellationToken cancellationToken = default)
            {
                return next();
            }
        }

        public class NestedGenericPublicConcreteMiddleware<TCommand, TData> : CommandMiddleware<TCommand, TData>
        {
            public override Task<TData> InvokeAsync(TCommand command, Func<Task<TData>> next, CancellationToken cancellationToken = default)
            {
                return next();
            }
        }

        public class NestedGenericCommandPublicConcreteMiddleware<TCommand> : CommandMiddleware<TCommand, TheData>
        {
            public override Task<TheData> InvokeAsync(TCommand command, Func<Task<TheData>> next, CancellationToken cancellationToken = default)
            {
                return next();
            }
        }

        public class NestedGenericDataPublicConcreteMiddleware<TData> : CommandMiddleware<TheCommand, TData>
        {
            public override Task<TData> InvokeAsync(TheCommand command, Func<Task<TData>> next, CancellationToken cancellationToken = default)
            {
                return next();
            }
        }

        public class NestedGenericPublicConcreteDifferentGenericsMiddleware<TCommand, TData> : CommandMiddleware<TCommand, List<TData>>
        {
            public override Task<List<TData>> InvokeAsync(TCommand command, Func<Task<List<TData>>> next, CancellationToken cancellationToken = default)
            {
                return next();
            }
        }

        public abstract class BaseValidator<TCommand> : ICommandValidator<TCommand>
        {
            public abstract Task<ValidationResult> ValidateAsync(TCommand value, CancellationToken cancellation = default);
        }

        public class NestedConcretePublicValidator : ICommandValidator<TheCommand>
        {
            public Task<ValidationResult> ValidateAsync(TheCommand value, CancellationToken cancellation = default)
            {
                return new ValidationResult().AsTask();
            }
        }

        public class NestedInheretConcretePublicValidator : BaseValidator<TheCommand>
        {
            public override Task<ValidationResult> ValidateAsync(TheCommand value, CancellationToken cancellation = default)
            {
                return new ValidationResult().AsTask();
            }
        }

        public abstract class NestedAbstractPublicValidator : ICommandValidator<TheCommand>
        {
            public abstract Task<ValidationResult> ValidateAsync(TheCommand value, CancellationToken cancellation = default);
        }

        internal class NestedConcreteInternalValidador : ICommandValidator<TheCommand>
        {
            public Task<ValidationResult> ValidateAsync(TheCommand value, CancellationToken cancellation = default)
            {
                return new ValidationResult().AsTask();
            }
        }

        internal class NestedConcreteGenericValidador<TCommand> : ICommandValidator<TCommand>
        {
            public Task<ValidationResult> ValidateAsync(TCommand value, CancellationToken cancellation = default)
            {
                return new ValidationResult().AsTask();
            }
        }
    }

    public class TheCommand { }

    public class TheData { }

    public abstract class BaseCommandMiddleware<TCommand, TResult> : ICommandMiddleware<TCommand, TResult>
    {
        public abstract Task InvokeAsync(ICommandContext<TCommand, TResult> context, Func<Task> next, CancellationToken cancellationToken = default);
    }

    public class InheretFromBaseCommandMiddleware<TCommand, TResult> : BaseCommandMiddleware<TCommand, TResult>
    {
        public override Task InvokeAsync(ICommandContext<TCommand, TResult> context, Func<Task> next, CancellationToken cancellationToken = default)
        {
            return next();
        }
    }

    public class PublicConcreteHandler : ICommandHandler<TheCommand, TheData>
    {
        public Task HandleAsync(ICommandContext<TheCommand, TheData> context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    public class PublicInheretedConcreteHandler : CommandHandler<TheCommand, TheData>
    {
        public override Task<TheData> ExecuteAsync(TheCommand command, CancellationToken cancellationToken = default)
        {
            return new TheData().AsTask();
        }
    }

    [IgnoreOnAssemblyRead]
    public class PublicIgnoreConcreteHandler : CommandHandler<TheCommand, TheData>
    {
        public override Task<TheData> ExecuteAsync(TheCommand command, CancellationToken cancellationToken = default)
        {
            return new TheData().AsTask();
        }
    }

    public abstract class PublicAbstractConcreteHandler : CommandHandler<TheCommand, TheData> { }

    internal class InternalConcreteHandler : CommandHandler<TheCommand, TheData>
    {
        public override Task<TheData> ExecuteAsync(TheCommand command, CancellationToken cancellationToken = default)
        {
            return new TheData().AsTask();
        }
    }

    public class GenericPublicConcreteHandler<TCommand, TData> : CommandHandler<TCommand, TData>
    {
        public override Task<TData> ExecuteAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            return default;
        }
    }

    public class PartialyGenericPublicConcreteHandler<TCommand> : CommandHandler<TCommand, string>
    {
        public override Task<string> ExecuteAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            return string.Empty.AsTask();
        }
    }

    public abstract class GenericAbstractConcreteHandler<TCommand, TData> : CommandHandler<TCommand, TData> { }


    public class ConcretePublicMiddlware : ICommandMiddleware<TheCommand, TheData>
    {
        public Task InvokeAsync(ICommandContext<TheCommand, TheData> context, Func<Task> next, CancellationToken cancellationToken = default)
        {
            return next();
        }
    }

    public class ConcreteInheretedPublicMiddlware : CommandMiddleware<TheCommand, TheData>
    {
        public override Task<TheData> InvokeAsync(TheCommand command, Func<Task<TheData>> next, CancellationToken cancellationToken = default)
        {
            return next();
        }
    }

    public abstract class AbastractPublicMiddlware : CommandMiddleware<TheCommand, TheData> { }

    internal class ConcreteInternalMiddlware : CommandMiddleware<TheCommand, TheData>
    {
        public override Task<TheData> InvokeAsync(TheCommand command, Func<Task<TheData>> next, CancellationToken cancellationToken = default)
        {
            return next();
        }
    }

    public class GenericPublicConcreteMiddlewareInterface<TCommand, TResult> : ICommandMiddleware<TCommand, TResult>
    {
        public Task InvokeAsync(ICommandContext<TCommand, TResult> context, Func<Task> next, CancellationToken cancellationToken = default)
        {
            return next();
        }
    }

    public class GenericPublicConcreteMiddleware<TCommand, TResult> : CommandMiddleware<TCommand, TResult>
    {
        public override Task<TResult> InvokeAsync(TCommand command, Func<Task<TResult>> next, CancellationToken cancellationToken = default)
        {
            return next();
        }
    }

    public class GenericCommandPublicConcreteMiddleware<TCommand> : CommandMiddleware<TCommand, TheData>
    {
        public override Task<TheData> InvokeAsync(TCommand command, Func<Task<TheData>> next, CancellationToken cancellationToken = default)
        {
            return next();
        }
    }

    public class GenericDataPublicConcreteMiddleware<TData> : CommandMiddleware<TheCommand, TData>
    {
        public override Task<TData> InvokeAsync(TheCommand command, Func<Task<TData>> next, CancellationToken cancellationToken = default)
        {
            return next();
        }
    }

    public class GenericPublicConcreteDifferentGenericsMiddleware<TCommand, TData> : CommandMiddleware<TCommand, List<TData>>
    {
        public override Task<List<TData>> InvokeAsync(TCommand command, Func<Task<List<TData>>> next, CancellationToken cancellationToken = default)
        {
            return next();
        }
    }

    public abstract class BaseValidator<TCommand> : ICommandValidator<TCommand>
    {
        public abstract Task<ValidationResult> ValidateAsync(TCommand value, CancellationToken cancellation = default);
    }

    public class ConcretePublicValidator : ICommandValidator<TheCommand>
    {
        public Task<ValidationResult> ValidateAsync(TheCommand value, CancellationToken cancellation = default)
        {
            return new ValidationResult().AsTask();
        }
    }

    public class InheretConcretePublicValidator : BaseValidator<TheCommand>
    {
        public override Task<ValidationResult> ValidateAsync(TheCommand value, CancellationToken cancellation = default)
        {
            return new ValidationResult().AsTask();
        }
    }

    public abstract class AbstractPublicValidator : ICommandValidator<TheCommand>
    {
        public abstract Task<ValidationResult> ValidateAsync(TheCommand value, CancellationToken cancellation = default);
    }

    internal class ConcreteInternalValidador : ICommandValidator<TheCommand>
    {
        public Task<ValidationResult> ValidateAsync(TheCommand value, CancellationToken cancellation = default)
        {
            return new ValidationResult().AsTask();
        }
    }

    internal class ConcreteGenericValidador<TCommand> : ICommandValidator<TCommand>
    {
        public Task<ValidationResult> ValidateAsync(TCommand value, CancellationToken cancellation = default)
        {
            return new ValidationResult().AsTask();
        }
    }
}
