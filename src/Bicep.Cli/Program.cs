// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.IO.Abstractions;
using System.Runtime;
using Azure.Deployments.Engine.Workers;
using Bicep.Cli.Arguments;
using Bicep.Cli.Commands;
using Bicep.Cli.Helpers;
using Bicep.Cli.Helpers.Deploy;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Emit;
using Bicep.Core.Exceptions;
using Bicep.Core.Features;
using Bicep.Core.Tracing;
using Bicep.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Bicep.Cli
{
    public record IOContext(
        TextWriter Output,
        TextWriter Error);

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

                // this event listener picks up SDK events and writes them to Trace.WriteLine()
                using (FeatureProvider.TracingEnabled ? AzureEventSourceListenerFactory.Create(FeatureProvider.TracingVerbosity) : null)
                {
                    var program = new Program(new(Output: Console.Out, Error: Console.Error));

                    // this must be awaited so dispose of the listener occurs in the continuation
                    // rather than the sync part at the beginning of RunAsync()
                    return await program.RunAsync(args, cancellationToken);
                }
            });

        public async Task<int> RunAsync(string[] args, CancellationToken cancellationToken)
        {
            var environment = services.GetRequiredService<IEnvironment>();
            Trace.WriteLine($"Bicep version: {environment.GetVersionString()}, OS: {environment.CurrentPlatform?.ToString() ?? "unknown"}, Architecture: {environment.CurrentArchitecture}, CLI arguments: \"{string.Join(' ', args)}\"");

            try
            {
                switch (ArgumentParser.TryParse(args, services.GetRequiredService<IFileSystem>()))
                {
                    case BuildArguments buildArguments when buildArguments.CommandName == Constants.Command.Build: // bicep build [options]
                        return await services.GetRequiredService<BuildCommand>().RunAsync(buildArguments);

                    case TestArguments testArguments when testArguments.CommandName == Constants.Command.Test: // bicep test [options]
                        return await services.GetRequiredService<TestCommand>().RunAsync(testArguments);

                    case BuildParamsArguments buildParamsArguments when buildParamsArguments.CommandName == Constants.Command.BuildParams: // bicep build-params [options]
                        return await services.GetRequiredService<BuildParamsCommand>().RunAsync(buildParamsArguments);

                    case FormatArguments formatArguments when formatArguments.CommandName == Constants.Command.Format: // bicep format [options]
                        return services.GetRequiredService<FormatCommand>().Run(formatArguments);

                    case GenerateParametersFileArguments generateParametersFileArguments when generateParametersFileArguments.CommandName == Constants.Command.GenerateParamsFile: // bicep generate-params [options]
                        return await services.GetRequiredService<GenerateParametersFileCommand>().RunAsync(generateParametersFileArguments);

                    case DecompileArguments decompileArguments when decompileArguments.CommandName == Constants.Command.Decompile: // bicep decompile [options]
                        return await services.GetRequiredService<DecompileCommand>().RunAsync(decompileArguments);

                    case DecompileParamsArguments decompileParamsArguments when decompileParamsArguments.CommandName == Constants.Command.DecompileParams:
                        return services.GetRequiredService<DecompileParamsCommand>().Run(decompileParamsArguments);

                    case PublishArguments publishArguments when publishArguments.CommandName == Constants.Command.Publish: // bicep publish [options]
                        return await services.GetRequiredService<PublishCommand>().RunAsync(publishArguments);

                    case PublishExtensionArguments publishProviderArguments when publishProviderArguments.CommandName == Constants.Command.PublishExtension: // bicep publish-extension [options]
                        return await services.GetRequiredService<PublishExtensionCommand>().RunAsync(publishProviderArguments, cancellationToken);

                    case RestoreArguments restoreArguments when restoreArguments.CommandName == Constants.Command.Restore: // bicep restore
                        return await services.GetRequiredService<RestoreCommand>().RunAsync(restoreArguments);

                    case LintArguments lintArguments when lintArguments.CommandName == Constants.Command.Lint: // bicep lint [options]
                        return await services.GetRequiredService<LintCommand>().RunAsync(lintArguments);

                    case JsonRpcArguments jsonRpcArguments when jsonRpcArguments.CommandName == Constants.Command.JsonRpc: // bicep jsonrpc [options]
                        return await services.GetRequiredService<JsonRpcCommand>().RunAsync(jsonRpcArguments, cancellationToken);

                    case LocalDeployArguments localDeployArguments when localDeployArguments.CommandName == Constants.Command.LocalDeploy: // bicep local-deploy [options]
                        return await services.GetRequiredService<LocalDeployCommand>().RunAsync(localDeployArguments, cancellationToken);

                    case SnapshotArguments snapshotArguments when snapshotArguments.CommandName == Constants.Command.Snapshot: // bicep snapshot [options]
                        return await services.GetRequiredService<SnapshotCommand>().RunAsync(snapshotArguments, cancellationToken);

                    case DeployArguments deployArguments when deployArguments.CommandName == Constants.Command.Deploy: // bicep deploy [options]
                        return await services.GetRequiredService<DeployCommand>().RunAsync(deployArguments, cancellationToken);

                    case WhatIfArguments whatIfArguments when whatIfArguments.CommandName == Constants.Command.WhatIf: // bicep what-if [options]
                        return await services.GetRequiredService<WhatIfCommand>().RunAsync(whatIfArguments, cancellationToken);

                    case TeardownArguments teardownArguments when teardownArguments.CommandName == Constants.Command.Teardown: // bicep teardown [options]
                        return await services.GetRequiredService<TeardownCommand>().RunAsync(teardownArguments, cancellationToken);

                    case ConsoleArguments consoleArguments when consoleArguments.CommandName == Constants.Command.Console: // bicep console
                        return await services.GetRequiredService<ConsoleCommand>().RunAsync(consoleArguments);

                    case RootArguments rootArguments when rootArguments.CommandName == Constants.Command.Root: // bicep [options]
                        return services.GetRequiredService<RootCommand>().Run(rootArguments);

                    default:
                        await io.Error.WriteLineAsync(string.Format(CliResources.UnrecognizedArgumentsFormat, string.Join(' ', args), ThisAssembly.AssemblyName)); // should probably print help here??
                        return 1;
                }
            }
            catch (BicepException exception)
            {
                await io.Error.WriteLineAsync(exception.Message);
                return 1;
            }
        }

        private static ILoggerFactory CreateLoggerFactory(IOContext io)
        {
            // apparently logging requires a factory factory 🤦‍
            return LoggerFactory.Create(builder =>
            {
                builder.AddProvider(new BicepLoggerProvider(new BicepLoggerOptions(true, ConsoleColor.Red, ConsoleColor.DarkYellow, io.Error)));
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
                // this is expected - no need to rethrow
                return 1;
            }
        }

        private static IServiceCollection ConfigureServices(IOContext io)
            => new ServiceCollection()
                .AddBicepCore()
                .AddBicepDecompiler()
                .AddLocalDeploy()
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
                    Out = new AnsiConsoleOutput(io.Output),
                }))
                .AddSingleton<IDeploymentProcessor, DeploymentProcessor>()
                .AddSingleton<DeploymentRenderer>();

        // This logic is duplicated in Bicep.Cli. We avoid placing it in Bicep.Core
        // to keep Bicep.Core free of System.IO dependencies. Consider moving this
        // and other components shared between the CLI and Language Server to a
        // separate project, such as Bicep.Hosting.
        private static void StartProfile()
        {
            string profilePath = Path.Combine(Path.GetTempPath(), LanguageConstants.LanguageFileExtension); // bicep extension as a hidden folder name
            Directory.CreateDirectory(profilePath);
            ProfileOptimization.SetProfileRoot(profilePath);
            ProfileOptimization.StartProfile("bicep.profile");
        }
    }
}
