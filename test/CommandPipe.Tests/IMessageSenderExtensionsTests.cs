using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CommandPipe;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace CommandPipe.Tests
{
    public class IMessageSenderExtensionsTests
    {
        [Fact]
        public async Task SendAsync_Without_CommandContext_PassingCommnadAndResult()
        {
            var services = new ServiceCollection()
                .AddCommandPipe(commands =>
                {
                    commands.AddCommandHandler<TheCommand, TheResult>((context, ct) =>
                    {
                        context.Result = new TheResult(context.Command.ExecutionTicket);

                        return Task.CompletedTask;
                    });
                });

            var sender = services.BuildServiceProvider(true)
                .CreateScope()
                .ServiceProvider
                .GetService<ICommandSender>();

            var executionTicket = Guid.NewGuid();
            var result = await sender.SendAsync<TheCommand, TheResult>(new TheCommand(executionTicket));

            Assert.Equal(executionTicket, result.ExecutionTicket);
        }

        [Fact]
        public async Task SendAsync_Without_CommandContext_PassingCommnad()
        {
            var services = new ServiceCollection()
                .AddCommandPipe(commands =>
                {
                    commands.AddCommandHandler((TheCommand commnad, CancellationToken ct) =>
                    {
                        return Task.CompletedTask;
                    });
                });

            var sender = services.BuildServiceProvider(true)
                .CreateScope()
                .ServiceProvider
                .GetService<ICommandSender>();

            var result = await sender.SendAsync(new TheCommand());

            Assert.True(result);
        }

        [Fact]
        public async Task SendAsync_Without_CommandContext_ObjectCommandGenericResult()
        {
            var services = new ServiceCollection()
                .AddCommandPipe(commands =>
                {
                    commands.AddCommandHandler<TheCommand, TheResult>((command, ct) =>
                    {
                        return new TheResult(command.ExecutionTicket).AsTask();
                    });
                });

            var sender = services.BuildServiceProvider(true)
                .CreateScope()
                .ServiceProvider
                .GetService<ICommandSender>();

            var executionTicket = Guid.NewGuid();
            var result = await sender.SendAsync<TheResult>(new TheCommand(executionTicket));

            Assert.Equal(executionTicket, result.ExecutionTicket);
        }

        [Fact]
        public async Task SendAsync_Without_CommandContext_ObjectCommand()
        {
            var services = new ServiceCollection()
                .AddCommandPipe(commands =>
                {
                    commands.AddCommandHandler<TheCommand>((context, ct) =>
                    {
                        context.Result = true;

                        return Task.CompletedTask;
                    });
                });

            var sender = services.BuildServiceProvider(true)
                .CreateScope()
                .ServiceProvider
                .GetService<ICommandSender>();

            var command = new TheCommand() as object;
            var result = await sender.SendAsync(command);

            Assert.True(result);
        }

        [Fact]
        public async Task SendAsync_Without_CommandContext_UsingICommandWithResult()
        {
            var services = new ServiceCollection()
                .AddCommandPipe(commands =>
                {
                    commands.AddCommandHandler<TheDataCommand, TheResult>((command, ct) =>
                    {
                        return new TheResult(command.ExecutionTicket).AsTask();
                    });
                });

            var sender = services.BuildServiceProvider(true)
                .CreateScope()
                .ServiceProvider
                .GetService<ICommandSender>();

            var executionTicket = Guid.NewGuid();
            var result = await sender.SendCommandAsync(new TheDataCommand(executionTicket));

            Assert.Equal(executionTicket, result.ExecutionTicket);
        }

        [Fact]
        public async Task SendAsync_Without_CommandContext_UsingICommand()
        {
            var services = new ServiceCollection()
                .AddCommandPipe(commands =>
                {
                    commands.AddCommandHandler<TheVoidCommand>((context, ct) =>
                    {
                        context.Result = true;

                        return Task.CompletedTask;
                    });
                });

            var sender = services.BuildServiceProvider(true)
                .CreateScope()
                .ServiceProvider
                .GetService<ICommandSender>();

            var executionTicket = Guid.NewGuid();
            var result = await sender.SendCommandAsync(new TheVoidCommand(executionTicket));

            Assert.True(result);
        }

        public class TheCommand
        {
            public TheCommand() { }

            public TheCommand(Guid executionTicket) => ExecutionTicket = executionTicket;

            public Guid ExecutionTicket { get; set; }
        }

        public class TheVoidCommand : TheCommand, ICommand
        {
            public TheVoidCommand() { }

            public TheVoidCommand(Guid executionTicket)
                : base(executionTicket)
            { }
        }

        public class TheDataCommand : TheCommand, ICommand<TheResult>
        {
            public TheDataCommand() { }

            public TheDataCommand(Guid executionTicket)
                : base(executionTicket)
            { }
        }

        public class TheResult
        {
            public TheResult() { }

            public TheResult(Guid executionTicket) => ExecutionTicket = executionTicket;

            public Guid ExecutionTicket { get; set; }
        }

        public class TheData { }
    }
}
