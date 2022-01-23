// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Commands;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.Options;
using Bicep.RegistryModuleTool.Proxies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Bicep.RegistryModuleTool
{
    public class Program
    {
        public static Task<int> Main(string[] args) => CreateCommandLineBuilder()
            .UseHost(Host.CreateDefaultBuilder, ConfigureHost)
            .UseDefaults()
            .UseVerboseOption()
            .UseVersionOption()
            .Build()
            .InvokeAsync(args);

        private static CommandLineBuilder CreateCommandLineBuilder()
        {
            var rootCommand = new RootCommand("Bicep registry module tool")
                .AddSubcommand(new ValidateCommand())
                .AddSubcommand(new GenerateCommand());

            rootCommand.Handler = CommandHandler.Create(() => rootCommand.Invoke("-h"));

            return new(rootCommand);
        }

        private static void ConfigureHost(IHostBuilder builder) => builder
            .ConfigureServices(services => services
                .AddSingleton<IEnvironmentProxy, EnvironmentProxy>()
                .AddSingleton<IProcessProxy, ProcessProxy>()
                .AddSingleton<IFileSystem, FileSystem>())
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

