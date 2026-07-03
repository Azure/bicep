// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
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
        public static async Task<int> Main(string[] args)
        {
            var host = CreateHost(args);
            await host.StartAsync();

            var rootCommand = BuildRootCommand(host.Services);
            var exitCode = await rootCommand.Parse(args).InvokeAsync();

            await host.StopAsync();
            return exitCode;
        }

        private static IHost CreateHost(string[] args) => Host.CreateDefaultBuilder()
            .ConfigureServices(services => services.AddBicepCore())
            .UseSerilog((_, logging) => logging
                .MinimumLevel.Is(GetMinimumLogEventLevel(args))
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.Console())
            .UseCommandHandlers()
            .Build();

        private static RootCommand BuildRootCommand(IServiceProvider services)
        {
            var rootCommand = new RootCommand("Bicep registry module tool");
            rootCommand.UseVerboseOption();

            var console = new SystemConsole();

            var generateCmd = new GenerateCommand();
            generateCmd.SetAction(async (ParseResult _, CancellationToken ct) =>
                await services.GetRequiredService<GenerateCommand.CommandHandler>().InvokeAsync(console, ct));

            var validateCmd = new ValidateCommand();
            validateCmd.SetAction(async (ParseResult _, CancellationToken ct) =>
                await services.GetRequiredService<ValidateCommand.CommandHandler>().InvokeAsync(console, ct));

            rootCommand.Add(generateCmd);
            rootCommand.Add(validateCmd);

            return rootCommand;
        }

        private static LogEventLevel GetMinimumLogEventLevel(string[] args) =>
            args.Contains("--verbose") ? LogEventLevel.Debug : LogEventLevel.Fatal;

        private sealed class SystemConsole : IConsole
        {
            public TextWriter Out => Console.Out;
            public TextWriter Error => Console.Error;
        }
    }
}

