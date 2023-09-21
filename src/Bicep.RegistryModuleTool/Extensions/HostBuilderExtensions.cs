// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using System.CommandLine.Hosting;
using System.Linq;
using System.Reflection;
using Bicep.RegistryModuleTool.Commands;
using Microsoft.Extensions.Hosting;

namespace Bicep.RegistryModuleTool.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseCommandHandlers(this IHostBuilder builder)
        {
            var baseCommandType = typeof(Command);
            var baseCommandHandlerType = typeof(BaseCommandHandler);

            var commandTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.Namespace == baseCommandHandlerType.Namespace && baseCommandType.IsAssignableFrom(t));

            foreach (var commandType in commandTypes)
            {
                var commandHandlerType = commandType.GetNestedTypes().First(t => baseCommandHandlerType.IsAssignableFrom(t));

                builder.UseCommandHandler(commandType, commandHandlerType);
            }

            return builder;
        }
    }
}
