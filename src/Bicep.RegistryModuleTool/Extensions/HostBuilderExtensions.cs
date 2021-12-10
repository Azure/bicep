// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Reflection;

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

        // The built-in UseCommandHandler extension method does not work for sub-class (https://github.com/dotnet/command-line-api/issues/1209). 
        // This is a workaround.
        private static void UseCommandHandler(this IHostBuilder builder, Type commandType, Type handlerType)
        {
            if (!typeof(Command).IsAssignableFrom(commandType))
            {
                throw new ArgumentException($"{nameof(commandType)} must be a type of {nameof(Command)}", nameof(handlerType));
            }

            if (!typeof(ICommandHandler).IsAssignableFrom(handlerType))
            {
                throw new ArgumentException($"{nameof(handlerType)} must implement {nameof(ICommandHandler)}", nameof(handlerType));
            }

            if (builder.Properties[typeof(InvocationContext)] is InvocationContext invocation &&
                invocation.ParseResult.CommandResult.Command is Command command &&
                command.GetType() == commandType)
            {
                invocation.BindingContext.AddService(handlerType, serviceProvider =>
                    serviceProvider.GetService<IHost>()?.Services.GetService(handlerType) ??
                    throw new InvalidOperationException($"Cannot get the service object for \"{handlerType.Name}\"."));

                var baseType = handlerType.BaseType;

                while (baseType is not null && baseType != typeof(object))
                {
                    invocation.BindingContext.AddService(baseType, serviceProvider =>
                        serviceProvider.GetService<IHost>()?.Services.GetService(handlerType) ??
                        throw new InvalidOperationException($"Cannot get the service object for \"{handlerType.Name}\"."));

                    baseType = baseType.BaseType;
                }

                builder.ConfigureServices(services =>
                {
                    services.AddTransient(handlerType);
                });

                command.Handler = CommandHandler.Create(
                    handlerType.GetMethod(nameof(ICommandHandler.InvokeAsync)) ??
                    throw new InvalidOperationException($"Cannot get the method info for \"{handlerType.Name}.{nameof(ICommandHandler.InvokeAsync)}\""));
            }
        }
    }
}
