// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using Bicep.RegistryModuleTool.Commands;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Bicep.RegistryModuleTool
{
    public class Program
    {
        public static Task<int> Main(string[] args) => CreateParser().InvokeAsync(args);

        private static Parser CreateParser()
        {
            var rootCommand = new RootCommand("Bicep registry module tool")
                .AddSubcommand(new ValidateCommand())
                .AddSubcommand(new GenerateCommand());

            var parser = new CommandLineBuilder(rootCommand)
                .UseHost(Host.CreateDefaultBuilder, ConfigureHost)
                .UseDefaults()
                .UseVerboseOption()
                .Build();

            // Have to use parser.Invoke instead of rootCommand.Invoke due to the
            // System.CommandLine bug: https://github.com/dotnet/command-line-api/issues/1691.
            rootCommand.Handler = CommandHandler.Create(() => parser.Invoke("-h"));

            return parser;
        }

        private static void ConfigureHost(IHostBuilder builder) => builder
            .ConfigureServices(services => services.AddBicepCore())
            .UseSerilog((context, logging) => logging
                .MinimumLevel.Is(GetMinimumLogEventLevel(context))
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.Console())
            .UseCommandHandlers();

        private static LogEventLevel GetMinimumLogEventLevel(HostBuilderContext context)
        {
            var verboseSpecified =
                context.Properties.TryGetValue(typeof(InvocationContext), out var value) &&
                value is InvocationContext invocationContext &&
                invocationContext.ParseResult.FindResultFor(GlobalOptions.Verbose) is not null;

            return verboseSpecified ? LogEventLevel.Debug : LogEventLevel.Fatal;
        }
    }
}

