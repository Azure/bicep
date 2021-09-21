// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// <see cref="BuildCommand"/> and that implements <see cref="ICommand"/> will be registered. 
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

        public static IServiceCollection AddInvocationContext(this IServiceCollection services, InvocationContext context)
        {
            // add itself
            services.AddSingleton(context);

            // add contents of the context
            services.AddSingleton(context.NamespaceProvider);
            services.AddSingleton(context.Features);
            services.AddSingleton(context.ClientFactory);
            services.AddSingleton(context.TemplateSpecRepositoryFactory);

            return services;
        }
    }
}
