using CommandPipe;
using CommandPipe.Annotations;
using CommandPipe.Validations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ICommandPipeBuilderExtensions
    {
        /// <summary>
        /// Configures a handler for the command <typeparamref name="TCommand"/>.
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <typeparam name="THandler">The handler Type</typeparam>
        /// <param name="commands">The command builder</param>
        public static ICommandPipeBuilder AddCommandHandler<TCommand, THandler>(this ICommandPipeBuilder commands)
            where THandler : class, ICommandHandler<TCommand, bool>
        {
            return commands.AddCommandHandler<TCommand, THandler>(_ => { });
        }

        /// <summary>
        /// Configures a handler for the command <typeparamref name="TCommand"/>.
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <typeparam name="THandler">The handler Type</typeparam>
        /// <param name="commands">The command builder</param>
        /// <param name="configure">The action to configure the command handler</param>
        public static ICommandPipeBuilder AddCommandHandler<TCommand, THandler>(this ICommandPipeBuilder commands, Action<ICommandBuilder<TCommand, bool>> configure)
            where THandler : class, ICommandHandler<TCommand, bool>
        {
            return commands.AddCommandHandler<TCommand, bool, THandler>(configure);
        }

        /// <summary>
        /// Configures an inline handler a the given command and result combination.
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="commands">The command builder</param>
        /// <param name="handler">The func with the handler implementation</param>
        public static ICommandPipeBuilder AddCommandHandler<TCommand, TResult>(this ICommandPipeBuilder commands, Func<ICommandContext<TCommand, TResult>, CancellationToken, Task> handler)
        {
            var builder = new CommandBuilder<TCommand, TResult>(commands.Services);

            builder.AddHandler(new RelayCommandHandler<TCommand, TResult>(handler));

            return commands;
        }

        /// <summary>
        /// Configures an inline handler for the given command and result combination.
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="commands">The command builder</param>
        /// <param name="handler">The func with the handler implementation</param>
        public static ICommandPipeBuilder AddCommandHandler<TCommand, TResult>(this ICommandPipeBuilder commands, Func<TCommand, CancellationToken, Task<TResult>> handler)
        {
            return commands.AddCommandHandler<TCommand, TResult>(async (context, ct) =>
            {
                context.Result = await handler(context.Command, ct);
            });
        }

        /// <summary>
        /// Configures an inline handler for the given command.
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <param name="commands">The command builder</param>
        /// <param name="handler">The func with the handler implementation</param>
        public static ICommandPipeBuilder AddCommandHandler<TCommand>(this ICommandPipeBuilder commands, Func<ICommandContext<TCommand, bool>, CancellationToken, Task> handler)
        {
            return commands.AddCommandHandler<TCommand, bool>(handler);
        }

        /// <summary>
        /// Configures an inline handler for the given command.
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <param name="commands">The command builder</param>
        /// <param name="handler">The func with the handler implementation</param>
        public static ICommandPipeBuilder AddCommandHandler<TCommand>(this ICommandPipeBuilder commands, Func<TCommand, CancellationToken, Task> handler)
        {
            return commands.AddCommandHandler<TCommand, bool>(async (context, ct) =>
            {
                await handler(context.Command, ct);

                context.Result = true;
            });
        }

        /// <summary>
        /// Configures a handler for the given command and result combination.
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <typeparam name="THandler">The handler type</typeparam>
        /// <param name="commands">The command builder</param>
        public static ICommandPipeBuilder AddCommandHandler<TCommand, TResult, THandler>(this ICommandPipeBuilder commands)
            where THandler : class, ICommandHandler<TCommand, TResult>
        {
            return commands.AddCommandHandler<TCommand, TResult, THandler>(command => { });
        }

        /// <summary>
        /// Configures a handler for the given command and result combination.
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <typeparam name="THandler">The handler type</typeparam>
        /// <param name="commands">The command builder</param>
        /// <param name="configure">The action to configure the command handler</param>
        public static ICommandPipeBuilder AddCommandHandler<TCommand, TResult, THandler>(this ICommandPipeBuilder commands, Action<ICommandBuilder<TCommand, TResult>> configure)
            where THandler : class, ICommandHandler<TCommand, TResult>
        {
            var builder = new CommandBuilder<TCommand, TResult, THandler>(commands.Services);

            builder.AddHandler();

            configure?.Invoke(builder);

            return commands;
        }

        /// <summary>
        /// Adds a validator to the given command.
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <param name="builder">The builder</param>
        /// <param name="validator">The validator instance</param>
        public static ICommandPipeBuilder AddValidator<TCommand>(this ICommandPipeBuilder builder, ICommandValidator<TCommand> validator)
        {
            builder.Services.AddSingleton(validator);

            return builder;
        }

        /// <summary>
        /// Adds a command validator to the given command.
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <typeparam name="TValidador">The validator type</typeparam>
        /// <param name="builder">The builder</param>
        public static ICommandPipeBuilder AddValidator<TCommand, TValidador>(this ICommandPipeBuilder builder)
            where TValidador : class, ICommandValidator<TCommand>
        {
            builder.Services.AddScoped<ICommandValidator<TCommand>, TValidador>();

            return builder;
        }

        /// <summary>
        /// Adds classes that implement the interfaces <see cref="ICommandHandler{TCommand, TResult}"/>, <see cref="ICommandMiddleware{TCommand, TResult}"/>
        /// and <see cref="ICommandValidator{T}"/> from the given assemblies as services on the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>
        /// The rules for adding command handlers, middlewares and validators are the following
        /// <list type="bullet">
        ///     <item>Handlers, middlewares and validators must be concrete public classes;</item>
        ///     <item>Handlers, middlewares and validators can be nested public classes;</item>
        ///     <item>Handlers and validators cannot be generic;</item>
        ///     <item>You can have multiples validators for the same command;</item>
        ///     <item>Middlewares can be generic if both command and result are generics;</item>
        ///     <item>Middlewares must have the same number and types of generic arguments as the interface <see cref="ICommandMiddleware{TCommand, TResult}"/>;</item>
        ///     <item>The last handler found for a given command and result combination will be the one called by the <see cref="ICommandSender"/>.</item>
        /// </list>
        /// </remarks>
        /// <param name="builder"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static ICommandPipeBuilder AddCommandHandlersFromAssemblies(this ICommandPipeBuilder builder, params Assembly[] assemblies)
            => builder.AddCommandHandlersFromAssemblies(assemblies.AsEnumerable());

        /// <summary>
        /// Adds classes that implement the interfaces <see cref="ICommandHandler{TCommand, TResult}"/>, <see cref="ICommandMiddleware{TCommand, TResult}"/>
        /// and <see cref="ICommandValidator{T}"/> from the given assemblies as services on the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>
        /// The rules for adding command handlers, middlewares and validators are the following
        /// <list type="bullet">
        ///     <item>Handlers, middlewares and validators must be concrete public classes;</item>
        ///     <item>Handlers, middlewares and validators can be nested public classes;</item>
        ///     <item>Handlers and validators cannot be generic;</item>
        ///     <item>You can have multiples validators for the same command;</item>
        ///     <item>Middlewares can be generic if both command and result are generics;</item>
        ///     <item>Middlewares must have the same number and types of generic arguments as the interface <see cref="ICommandMiddleware{TCommand, TResult}"/>;</item>
        ///     <item>The last handler found for a given command and result combination will be the one called by the <see cref="ICommandSender"/>.</item>
        /// </list>
        /// </remarks>
        /// <param name="builder"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static ICommandPipeBuilder AddCommandHandlersFromAssemblies(this ICommandPipeBuilder builder, IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                builder.AddCommandHandlersFromAssembly(assembly);
            }

            return builder;
        }

        /// <summary>
        /// Adds classes that implement the interfaces <see cref="ICommandHandler{TCommand, TResult}"/>, <see cref="ICommandMiddleware{TCommand, TResult}"/>
        /// and <see cref="ICommandValidator{T}"/> from the assembly of the given type as services on the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>
        /// The rules for adding command handlers, middlewares and validators are the following
        /// <list type="bullet">
        ///     <item>Handlers, middlewares and validators must be concrete public classes;</item>
        ///     <item>Handlers, middlewares and validators can be nested public classes;</item>
        ///     <item>Handlers and validators cannot be generic;</item>
        ///     <item>You can have multiples validators for the same command;</item>
        ///     <item>Middlewares can be generic if both command and result are generics;</item>
        ///     <item>Middlewares must have the same number and types of generic arguments as the interface <see cref="ICommandMiddleware{TCommand, TResult}"/>;</item>
        ///     <item>The last handler found for a given command and result combination will be the one called by the <see cref="ICommandSender"/>.</item>
        /// </list>
        /// </remarks>
        /// <param name="builder">The builder for the commands</param>
        /// <param name="type">A type declared in the target assembly</param>
        public static ICommandPipeBuilder AddCommandHandlersFromAssemblyOfType(this ICommandPipeBuilder builder, Type type)
        {
            return builder.AddCommandHandlersFromAssembly(type.Assembly);
        }

        /// <summary>
        /// Adds classes that implement the interfaces <see cref="ICommandHandler{TCommand, TResult}"/>, <see cref="ICommandMiddleware{TCommand, TResult}"/>
        /// and <see cref="ICommandValidator{T}"/> from the given assembly as services on the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>
        /// The rules for adding command handlers, middlewares and validators are the following
        /// <list type="bullet">
        ///     <item>Handlers, middlewares and validators must be concrete public classes;</item>
        ///     <item>Handlers, middlewares and validators can be nested public classes;</item>
        ///     <item>Handlers and validators cannot be generic;</item>
        ///     <item>You can have multiples validators for the same command;</item>
        ///     <item>Middlewares can be generic if both command and result are generics;</item>
        ///     <item>Middlewares must have the same number and types of generic arguments as the interface <see cref="ICommandMiddleware{TCommand, TResult}"/>;</item>
        ///     <item>The last handler found for a given command and result combination will be the one called by the <see cref="ICommandSender"/>.</item>
        /// </list>
        /// </remarks>
        /// <param name="builder"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static ICommandPipeBuilder AddCommandHandlersFromAssembly(this ICommandPipeBuilder builder, Assembly assembly)
        {
            foreach (var type in assembly.DefinedTypes)
            {
                TryAddHandlersAndMiddlewares(builder, type);
            }

            return builder;
        }

        private static void TryAddHandlersAndMiddlewares(ICommandPipeBuilder builder, Type type)
        {
            if (!type.IsPublicConcreteClass() || type.IsDefined(typeof(IgnoreOnAssemblyReadAttribute)))
            {
                return;
            }

            foreach (var interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType)
                {
                    var interfaceGenericTypeDefinition = interfaceType.GetGenericTypeDefinition();

                    if (type.IsGenericType)
                    {
                        if (interfaceGenericTypeDefinition == typeof(ICommandMiddleware<,>) &&
                            HaveTheSameGenericParameters(interfaceType, type))
                        {
                            builder.Services.AddScoped(typeof(ICommandMiddleware<,>), type.GetGenericTypeDefinition());
                        }
                    }
                    else if (interfaceGenericTypeDefinition == typeof(ICommandHandler<,>) ||
                            interfaceGenericTypeDefinition == typeof(ICommandMiddleware<,>) ||
                            interfaceGenericTypeDefinition == typeof(ICommandValidator<>))
                    {
                        builder.Services.AddScoped(interfaceType, type);
                    }
                }
            }
        }

        private static bool HaveTheSameGenericParameters(Type interfaceType, Type implementationType)
        {
            var interfaceGenericArguments = interfaceType.GetGenericArguments();
            var implementationGenericArguments = implementationType.GetGenericArguments();

            if (interfaceGenericArguments.Length != implementationGenericArguments.Length)
            {
                return false;
            }

            return interfaceGenericArguments.All(argument => implementationGenericArguments.Contains(argument));
        }
    }
}
