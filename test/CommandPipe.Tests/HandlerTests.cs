using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CommandPipe;
using CommandPipe.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Xunit;

namespace CommandPipe.Tests
{
    public class CommandHandlerTests
    {
        [Fact]
        public async Task HandlerShouldBeCalled()
        {
            var services = new ServiceCollection();

            services.AddCommandPipe(commands =>
            {
                commands.AddCommandHandler<TheCommand, TheData, TheCommandHandler>();
            });

            var provider = services.BuildServiceProvider();

            var sender = provider.GetService<ICommandSender>();

            var command = new TheCommand();
            var result = await sender.SendAsync<TheData>(command);

            Assert.NotNull(result);
            var handler = Assert.Single(command.PassedBy);
            Assert.Equal(typeof(TheCommandHandler), handler);
        }

        [Fact]
        public async Task VoidHandlerShouldBeCalled()
        {
            var services = new ServiceCollection();

            services.AddCommandPipe(commands =>
            {
                commands.AddCommandHandler<TheCommand, TheVoidCommandHandler>();
            });

            var provider = services.BuildServiceProvider();

            var sender = provider.GetService<ICommandSender>();

            var command = new TheCommand();
            var result = await sender.SendAsync(command);

            Assert.True(result);
            var handler = Assert.Single(command.PassedBy);
            Assert.Equal(typeof(TheVoidCommandHandler), handler);
        }

        [Fact]
        public async Task InlineHandlerShouldBeCalled()
        {
            var services = new ServiceCollection();

            var called = false;
            services.AddCommandPipe(commands =>
            {
                commands.AddCommandHandler<TheCommand, TheData>((c, ct) =>
                {
                    called = true;
                    return new TheData().AsTask();
                });
            });

            var provider = services.BuildServiceProvider();

            var sender = provider.GetService<ICommandSender>();

            var command = new TheCommand();
            var result = await sender.SendAsync<TheData>(command);

            Assert.True(called);
        }

        [Fact]
        public async Task Send_Should_Trown_When_CommandAndResultHadNoHandler()
        {
            var services = new ServiceCollection();

            services.AddCommandPipe(commands =>
            {
            });

            var provider = services.BuildServiceProvider();

            var sender = provider.GetService<ICommandSender>();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sender.SendAsync<TheData>(new TheCommand()));
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
                command.AddPassedBy(this);

                return new TheData().AsTask();
            }
        }

        [IgnoreOnAssemblyRead]
        public class TheVoidCommandHandler : CommandHandler<TheCommand>
        {
            public override Task ExecuteAsync(TheCommand command, CancellationToken cancellationToken = default)
            {
                command.AddPassedBy(this);

                return Task.CompletedTask;
            }
        }
    }
}
