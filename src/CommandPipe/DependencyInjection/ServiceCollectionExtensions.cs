
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the required services to use the CommandPipe library.
        /// </summary>
        /// <param name="services">the services of the application</param>
        /// <param name="configure">the action to configure to the commands</param>
        /// <returns></returns>
        public static IServiceCollection AddCommandPipe(this IServiceCollection services, Action<ICommandPipeBuilder> configure)
        {
            var builder = new CommandPipeBuilder(services);

            configure(builder);

            return services;
        }
    }
}
