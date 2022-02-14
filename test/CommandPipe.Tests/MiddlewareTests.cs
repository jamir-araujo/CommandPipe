using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CommandPipe;
using CommandPipe.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace CommandPipe.Tests
{
    public class MiddlewareTests
    {
        [Fact]
        public async Task MiddlewareShouldBeCalled()
        {
            var services = new ServiceCollection();

            services.AddCommandPipe(commands =>
            {
                commands.AddCommandHandler<TheCommand, TheData, TheCommandHandler>(command =>
                {
                    command.AddMiddleware<TheCommandMiddleware>();
                });
            });

            var provider = services.BuildServiceProvider();

            var sender = provider.GetService<ICommandSender>();

            var command = new TheCommand();
            var result = await sender.SendAsync<TheData>(command);

            Assert.NotNull(result);
            var middlware = Assert.Single(command.PassedBy);
            Assert.Equal(typeof(TheCommandMiddleware), middlware);
        }

        [Fact]
        public async Task VoidMiddlewareShouldBeCalled()
        {
            var services = new ServiceCollection();

            services.AddCommandPipe(commands =>
            {
                commands.AddCommandHandler<TheCommand, TheVoidCommandHandler>(command =>
                {
                    command.AddMiddleware<TheVoidCommandMiddleware>();
                });
            });

            var provider = services.BuildServiceProvider();

            var sender = provider.GetService<ICommandSender>();

            var command = new TheCommand();
            var result = await sender.SendAsync(command);

            Assert.True(result);
            var middlware = Assert.Single(command.PassedBy);
            Assert.Equal(typeof(TheVoidCommandMiddleware), middlware);
        }

        [Fact]
        public async Task InlineMiddlewareShouldBeCalled()
        {
            var services = new ServiceCollection();

            var called = false;
            services.AddCommandPipe(commands =>
            {
                commands.AddCommandHandler<TheCommand, TheData, TheCommandHandler>(commnad =>
                {
                    commnad.AddMiddleware((c, n, ct) =>
                    {
                        called = true;
                        return n();
                    });
                });
            });

            var provider = services.BuildServiceProvider();

            var sender = provider.GetService<ICommandSender>();

            var command = new TheCommand();
            var result = await sender.SendAsync<TheData>(command);

            Assert.True(called);
        }

        public class TheCommand
        {
            public List<Type> PassedBy { get; } = new List<Type>();

            public void AddPassedBy<T>() => PassedBy.Add(typeof(T));

            public void AddPassedBy<T>(T instance) => PassedBy.Add(instance.GetType());
        }

        public class TheData { }

        [IgnoreOnAssemblyRead]
        public class TheCommandHandler : CommandHandler<TheCommand, TheData>
        {
            public override Task<TheData> ExecuteAsync(TheCommand command, CancellationToken cancellationToken = default)
            {
                return new TheData().AsTask();
            }
        }

        [IgnoreOnAssemblyRead]
        private class TheCommandMiddleware : CommandMiddleware<TheCommand, TheData>
        {
            public override Task<TheData> InvokeAsync(TheCommand command, Func<Task<TheData>> next, CancellationToken cancellationToken = default)
            {
                command.AddPassedBy(this);

                return next();
            }
        }

        [IgnoreOnAssemblyRead]
        private class TheVoidCommandHandler : CommandHandler<TheCommand>
        {
            public override Task ExecuteAsync(TheCommand command, CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }
        }

        [IgnoreOnAssemblyRead]
        private class TheVoidCommandMiddleware : CommandMiddleware<TheCommand>
        {
            public override Task InvokeAsync(TheCommand command, Func<Task> next, CancellationToken cancellationToken = default)
            {
                command.AddPassedBy(this);

                return next();
            }
        }
    }
}
