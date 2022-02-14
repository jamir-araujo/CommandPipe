
using CommandPipe;
using CommandPipe.Validations;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Represents the builder to configure the commands for the application.
    /// </summary>
    public interface ICommandPipeBuilder
    {
        /// <summary>
        /// The services of the application.
        /// </summary>
        IServiceCollection Services { get; }
    }

    internal class CommandPipeBuilder : ICommandPipeBuilder
    {
        public CommandPipeBuilder(IServiceCollection services)
        {
            Services = services;

            services.AddLogging();

            Services.TryAddScoped(typeof(CommandPipeline<,>));
            Services.TryAddScoped<ICommandSender, CommandSender>();
            Services.TryAddScoped(typeof(ICommandMiddleware<,>), typeof(ValidatorMiddleware<,>));
        }

        public IServiceCollection Services { get; }
    }
}
