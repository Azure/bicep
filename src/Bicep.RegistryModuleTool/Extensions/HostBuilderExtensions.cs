// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;
using Bicep.RegistryModuleTool.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bicep.RegistryModuleTool.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseCommandHandlers(this IHostBuilder builder)
        {
            var baseCommandHandlerType = typeof(BaseCommandHandler);

            var commandHandlerTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => baseCommandHandlerType.IsAssignableFrom(t) && !t.IsAbstract);

            return builder.ConfigureServices(services =>
            {
                foreach (var handlerType in commandHandlerTypes)
                {
                    services.AddScoped(handlerType);
                }
            });
        }
    }
}
