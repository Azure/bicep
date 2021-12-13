// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Commands;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.Options;
using Bicep.RegistryModuleTool.Proxies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Bicep.RegistryModuleTool;

public class Program
{
    public static Task<int> Main(string[] args) => CreateCommandLineBuilder()
        .UseHost(Host.CreateDefaultBuilder, ConfigureHost)
        .UseDefaults()
        .UseVerbose()
        .UseVersionOption()
        .Build()
        .InvokeAsync(args);

    private static CommandLineBuilder CreateCommandLineBuilder()
    {
        var rootCommand = new RootCommand("Bicep registry module tool")
            .AddSubcommand(new NewCommand("new", "Create files for a new Bicep Registry module"))
            .AddSubcommand(new ValidateCommand("validate", "Validate files for the Bicep registry module"))
            .AddSubcommand(new GenerateCommand("generate", "Generate files for the Bicep registry module"));

        rootCommand.Handler = CommandHandler.Create(() => rootCommand.Invoke("-h"));

        return new(rootCommand);
    }

    private static void ConfigureHost(IHostBuilder builder) => builder
        .ConfigureLogging((context, logging) =>
        {
            logging
                .ClearProviders()
                .AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Warning)
                .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                .AddConsole()
                .AddDebug();

            if (context.Properties.TryGetValue(typeof(InvocationContext), out var value) &&
                value is InvocationContext invocationContext &&
                invocationContext.ParseResult.FindResultFor(GlobalOptions.Verbose) is not null)
            {
                logging.SetMinimumLevel(LogLevel.Debug);
            }
        })
        .ConfigureServices(services => services
            .AddSingleton<IEnvironmentProxy, EnvironmentProxy>()
            .AddSingleton<IProcessProxy, ProcessProxy>()
            .AddSingleton<IFileSystem, FileSystem>())
        .UseCommandHandlers();
}
