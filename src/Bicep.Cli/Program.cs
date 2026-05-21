// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.Runtime;
using Azure.Core.Diagnostics;
using Bicep.Cli.Arguments;
using Bicep.Cli.Commands;
using Bicep.Cli.Helpers;
using Bicep.Cli.Helpers.Deploy;
using Bicep.Cli.Helpers.Repl;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Emit;
using Bicep.Core.Features;
using Bicep.Core.Tracing;
using Bicep.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;

using SclRootCommand = System.CommandLine.RootCommand;

namespace Bicep.Cli
{
    public record IOContext(
        InputContext Input,
        OutputContext Output,
        ErrorContext Error);

    public record InputContext(
        TextReader Reader,
        bool IsRedirected);
    public record OutputContext(
        TextWriter Writer,
        bool IsRedirected);
    public record ErrorContext(
        TextWriter Writer,
        bool IsRedirected);

    public class Program
    {
        private readonly IServiceProvider services;
        private readonly IOContext io;

        public Program(IOContext io, Action<IServiceCollection>? registerAction = null)
        {
            var services = ConfigureServices(io);
            registerAction?.Invoke(services);
            this.services = services.BuildServiceProvider();
            this.io = io;
        }

        public static async Task<int> Main(string[] args)
            => await RunWithCancellationAsync(async cancellationToken =>
            {
                StartProfile();

                Console.OutputEncoding = TemplateEmitter.UTF8EncodingWithoutBom;

                if (FeatureProvider.TracingEnabled)
                {
                    Trace.Listeners.Add(new TextWriterTraceListener(Console.Error));
                }

                using var azureEventSourceListener = FeatureProvider.TracingEnabled
                    ? (IDisposable?)AzureEventSourceListenerFactory.Create(FeatureProvider.TracingVerbosity)
                    : null;

                var program = new Program(new(
                    Input: new(Console.In, Console.IsInputRedirected),
                    Output: new(Console.Out, Console.IsOutputRedirected),
                    Error: new(Console.Error, Console.IsErrorRedirected)));

                return await program.RunAsync(args, cancellationToken);
            });

        public async Task<int> RunAsync(string[] args, CancellationToken cancellationToken)
        {
            var environment = services.GetRequiredService<IEnvironment>();
            Trace.WriteLine($"Bicep version: {environment.GetVersionString()}, OS: {environment.CurrentPlatform?.ToString() ?? "unknown"}, Architecture: {environment.CurrentArchitecture}, CLI arguments: \"{string.Join(' ', args)}\"");

            var rootCommand = BuildCommandLine();
            var invocationConfig = new InvocationConfiguration
            {
                Output = io.Output.Writer,
                Error = io.Error.Writer,
            };

            return await rootCommand.Parse(args).InvokeAsync(invocationConfig, cancellationToken);
        }

        private SclRootCommand BuildCommandLine()
        {
            var rootCommand = new SclRootCommand("Bicep CLI");
            var context = new CommandLineBuilderContext(services, io);

            var builtInVersion = rootCommand.Options.FirstOrDefault(o => o.Name == Constants.Option.Version);
            if (builtInVersion is not null)
            {
                rootCommand.Options.Remove(builtInVersion);
            }

            var versionOption = new Option<bool>(Constants.Option.Version, Constants.Option.VersionShort) { Description = "Shows bicep version information." };
            var licenseOption = new Option<bool>(Constants.Option.License) { Description = "Prints license information." };
            var thirdPartyNoticesOption = new Option<bool>(Constants.Option.ThirdPartyNotices) { Description = "Prints third-party notices." };

            rootCommand.Add(versionOption);
            rootCommand.Add(licenseOption);
            rootCommand.Add(thirdPartyNoticesOption);

            rootCommand.SetAction(async (ParseResult pr, CancellationToken ct) =>
            {
                var environment = services.GetRequiredService<IEnvironment>();
                var unmatched = pr.UnmatchedTokens;

                if (pr.GetValue(versionOption))
                {
                    CliInfoPrinter.PrintVersion(io, environment);
                    return 0;
                }

                if (pr.GetValue(licenseOption))
                {
                    CliInfoPrinter.PrintLicense(io);
                    return 0;
                }

                if (pr.GetValue(thirdPartyNoticesOption))
                {
                    CliInfoPrinter.PrintThirdPartyNotices(io);
                    return 0;
                }

                await io.Error.Writer.WriteLineAsync(string.Format(CliResources.UnrecognizedArgumentsFormat, string.Join(' ', unmatched), ThisAssembly.AssemblyName));
                return 1;
            });

            rootCommand.Add(BuildCommand.CreateCommand(context));
            rootCommand.Add(TestCommand.CreateCommand(context));
            rootCommand.Add(BuildParamsCommand.CreateCommand(context));
            rootCommand.Add(FormatCommand.CreateCommand(context));
            rootCommand.Add(GenerateParametersFileCommand.CreateCommand(context));
            rootCommand.Add(DecompileCommand.CreateCommand(context));
            rootCommand.Add(DecompileParamsCommand.CreateCommand(context));
            rootCommand.Add(PublishCommand.CreateCommand(context));
            rootCommand.Add(PublishExtensionCommand.CreateCommand(context));
            rootCommand.Add(RestoreCommand.CreateCommand(context));
            rootCommand.Add(LintCommand.CreateCommand(context));
            rootCommand.Add(JsonRpcCommand.CreateCommand(context));
            rootCommand.Add(LocalDeployCommand.CreateCommand(context));
            rootCommand.Add(SnapshotCommand.CreateCommand(context));
            rootCommand.Add(DeployCommand.CreateCommand(context));
            rootCommand.Add(WhatIfCommand.CreateCommand(context));
            rootCommand.Add(TeardownCommand.CreateCommand(context));
            rootCommand.Add(ConsoleCommand.CreateCommand(context));

            return rootCommand;
        }

        private static ILoggerFactory CreateLoggerFactory(IOContext io)
        {
            return LoggerFactory.Create(builder =>
            {
                builder.AddProvider(new BicepLoggerProvider(new BicepLoggerOptions(true, ConsoleColor.Red, ConsoleColor.DarkYellow, io.Error.Writer)));
            });
        }

        private static async Task<int> RunWithCancellationAsync(Func<CancellationToken, Task<int>> runFunc)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, e) =>
            {
                cancellationTokenSource.Cancel();
                e.Cancel = true;
            };

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                cancellationTokenSource.Cancel();
            };

            try
            {
                return await runFunc(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException exception) when (exception.CancellationToken == cancellationTokenSource.Token)
            {
                return 1;
            }
        }

        private static IServiceCollection ConfigureServices(IOContext io)
            => new ServiceCollection()
                .AddBicepCore()
                .AddBicepDecompiler()
                .AddBicepLocalDeploy()
                .AddCommands()
                .AddSingleton(CreateLoggerFactory(io).CreateLogger("bicep"))
                .AddSingleton<InputOutputArgumentsResolver>()
                .AddSingleton<DiagnosticLogger>()
                .AddSingleton<OutputWriter>()
                .AddSingleton<PlaceholderParametersWriter>()
                .AddSingleton(io)
                .AddSingleton<ReplEnvironment>()
                .AddSingleton(AnsiConsole.Create(new AnsiConsoleSettings
                {
                    Ansi = AnsiSupport.Detect,
                    ColorSystem = ColorSystemSupport.Detect,
                    Interactive = InteractionSupport.Detect,
                    Out = new AnsiConsoleOutput(io.Output.Writer),
                }))
                .AddSingleton<IDeploymentProcessor, DeploymentProcessor>()
                .AddSingleton<DeploymentRenderer>();

        private static void StartProfile()
        {
            string profilePath = Path.Combine(Path.GetTempPath(), LanguageConstants.LanguageFileExtension);
            Directory.CreateDirectory(profilePath);
            ProfileOptimization.SetProfileRoot(profilePath);
            ProfileOptimization.StartProfile("bicep.profile");
        }
    }
}
