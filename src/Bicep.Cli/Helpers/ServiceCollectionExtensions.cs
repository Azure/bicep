// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Bicep.Cli.Commands;
using Bicep.Cli.Arguments;

namespace Bicep.Cli.Helpers
{

    /// <summary>
    /// Contains the collection extensions for adding the CLI commands and configuration of those commands.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the CLI commands to the DI container. These are resolved when the commands are registered with the
        /// <c>CommandLineBuilder</c>.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <returns>The service collection, for chaining.</returns>
        /// <remarks>
        /// We are using convention to register the commands; essentially everything in the same namespace as the
        /// <see cref="BuildCommand"/> and that implements <see cref="CommandBase"/> will be registered. 
        ///
        /// See https://endjin.com/blog/2020/09/simple-pattern-for-using-system-commandline-with-dependency-injection for reference.
        /// </remarks>
        public static IServiceCollection AddCommands(this IServiceCollection services)
        {
            Type grabCommandType = typeof(BuildCommand);
            Type commandType = typeof(ICommand);

            IEnumerable<Type> commands = grabCommandType
                .Assembly
                .GetExportedTypes()
                 .Where(x => x.Namespace == grabCommandType.Namespace && x.GetInterfaces().Contains(commandType));

            foreach (Type command in commands)
            {
                services.AddSingleton(command);
            }

            return services;
        }

        /// <summary>
        /// Adds the corresponding configuration, used by the commands, to the DI container. These are resolved when the commands are registered with the
        /// <c>CommandLineBuilder</c>.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <returns>The service collection, for chaining.</returns>
        /// <remarks>
        /// We are using convention to register these configurations; essentially everything in the same namespace as the
        /// <see cref="BuildArguments"/> and that implements <see cref="IArguments"/> will be registered. 
        ///
        /// See https://endjin.com/blog/2020/09/simple-pattern-for-using-system-commandline-with-dependency-injection for reference.
        /// </remarks>
        public static IServiceCollection AddArguments(this IServiceCollection services)
        {
            Type grabConfigType = typeof(BuildArguments);
            Type configType = typeof(IArguments);

            IEnumerable<Type> configs = grabConfigType
                .Assembly
                .GetExportedTypes()
                .Where(x => x.Namespace == grabConfigType.Namespace && x.GetInterfaces().Contains(configType));

            foreach (Type config in configs)
            {
                services.AddSingleton(config);
            }

            return services;
        }
    }
}