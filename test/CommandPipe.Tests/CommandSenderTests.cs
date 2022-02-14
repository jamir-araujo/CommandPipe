using System;
using System.Threading.Tasks;

using CommandPipe;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace CommandPipe.Tests
{
    public class CommandSenderTests
    {
        [Fact]
        public async Task SendAsync_Should_Throw_When_ContextIsNull()
        {
            var provider = new ServiceCollection()
                .BuildServiceProvider();

            var sender = new CommandSender(provider);

            await Assert.ThrowsAsync<ArgumentNullException>("context", async () => await sender.SendAsync<bool, bool>(null));
        }
    }
}
