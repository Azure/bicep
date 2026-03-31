// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Runtime;
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

// Avoid naming conflict with Bicep.Cli.Commands.RootCommand
using SclRootCommand = System.CommandLine.RootCommand;

public class HelpExamplesAction : SynchronousCommandLineAction
{
    public override int Invoke(ParseResult parseResult)
    {
        throw new NotImplementedException();
    }
}

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

                // this event listener picks up SDK events and writes them to Trace.WriteLine()
                using (FeatureProvider.TracingEnabled ? AzureEventSourceListenerFactory.Create(FeatureProvider.TracingVerbosity) : null)
                {
                    var program = new Program(new(
                        Input: new(Console.In, Console.IsInputRedirected),
                        Output: new(Console.Out, Console.IsOutputRedirected),
                        Error: new(Console.Error, Console.IsErrorRedirected)));

                    // this must be awaited so dispose of the listener occurs in the continuation
                    // rather than the sync part at the beginning of RunAsync()
                    return await program.RunAsync(args, cancellationToken);
                }
            });

        public async Task<int> RunAsync(string[] args, CancellationToken cancellationToken)
        {
            var environment = services.GetRequiredService<IEnvironment>();
            Trace.WriteLine($"Bicep version: {environment.GetVersionString()}, OS: {environment.CurrentPlatform?.ToString() ?? "unknown"}, Architecture: {environment.CurrentArchitecture}, CLI arguments: \"{string.Join(' ', args)}\"");

            var rootCommand = BuildCommandLine(cancellationToken);
            return await rootCommand.Parse(args).InvokeAsync(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Builds the System.CommandLine command hierarchy. Each existing subcommand is currently
        /// registered as a legacy pass-through stub via <see cref="LegacyCommand"/>. To migrate a
        /// command, replace its <c>LegacyCommand</c> call with a <see cref="Command"/> that
        /// declares its own <see cref="Option{T}"/> and <see cref="Argument{T}"/> members and
        /// invokes the command handler directly.
        /// </summary>
        private SclRootCommand BuildCommandLine(CancellationToken cancellationToken)
        {
            var rootCommand = new SclRootCommand("Bicep CLI")
            {
                TreatUnmatchedTokensAsErrors = true,
            };

            // System.CommandLine adds --version and --help to RootCommand by default.
            // Replace both with custom options so their output goes through io.Output.Writer
            // (not Console.Out) and --version prints the Bicep-specific version string.
            var builtInVersion = rootCommand.Options.FirstOrDefault(o => o.Name == "--version");
            if (builtInVersion is not null)
            {
                rootCommand.Options.Remove(builtInVersion);
            }

            var versionOption = new Option<bool>("--version", "-v") { Description = "Show version information." };
            var licenseOption = new Option<bool>("--license") { Description = "Print license information." };
            var thirdPartyNoticesOption = new Option<bool>("--third-party-notices") { Description = "Print third-party notice information." };

            // rootCommand.Add(helpOption);
            rootCommand.Add(versionOption);
            rootCommand.Add(licenseOption);
            rootCommand.Add(thirdPartyNoticesOption);

            rootCommand.SetAction(async (ParseResult pr, CancellationToken ct) =>
            {
                var bicepRootCommand = services.GetRequiredService<Commands.RootCommand>();

                var unmatched = pr.UnmatchedTokens;

                if (pr.GetValue(versionOption))
                {
                    return bicepRootCommand.Run(new RootArguments("--version", Constants.Command.Root));
                }

                if (pr.GetValue(licenseOption))
                {
                    return bicepRootCommand.Run(new RootArguments("--license", Constants.Command.Root));
                }

                if (pr.GetValue(thirdPartyNoticesOption))
                {
                    return bicepRootCommand.Run(new RootArguments("--third-party-notices", Constants.Command.Root));
                }

                await io.Error.Writer.WriteLineAsync(
                    string.Format(CliResources.UnrecognizedArgumentsFormat, string.Join(' ', unmatched), ThisAssembly.AssemblyName));
                return 1;
            });

            // Each subcommand below is a legacy pass-through stub. Arguments not recognized by
            // System.CommandLine are collected in ParseResult.UnmatchedTokens and forwarded to
            // the existing argument class for parsing. Once a command is fully migrated, replace
            // the LegacyCommand call with a Command that has explicit Option<T>/Argument<T>
            // members and calls the command handler with the bound values directly.
            rootCommand.Add(LegacyCommand(
                Constants.Command.Build,
                "Builds a .bicep file.",
                args => services.GetRequiredService<BuildCommand>().RunAsync(new BuildArguments(args))));

            rootCommand.Add(LegacyCommand(
                Constants.Command.Test,
                "Runs tests in a .bicep file.",
                args => services.GetRequiredService<TestCommand>().RunAsync(new TestArguments(args))));

            rootCommand.Add(LegacyCommand(
                Constants.Command.BuildParams,
                "Builds a .bicepparam file.",
                args => services.GetRequiredService<BuildParamsCommand>().RunAsync(new BuildParamsArguments(args))));

            rootCommand.Add(LegacyCommand(
                Constants.Command.Format,
                "Formats a .bicep file.",
                args => Task.FromResult(services.GetRequiredService<FormatCommand>().Run(new FormatArguments(args)))));

            rootCommand.Add(LegacyCommand(
                Constants.Command.GenerateParamsFile,
                "Generates a parameters file for a .bicep file.",
                args => services.GetRequiredService<GenerateParametersFileCommand>().RunAsync(new GenerateParametersFileArguments(args))));

            rootCommand.Add(LegacyCommand(
                Constants.Command.Decompile,
                "Attempts to decompile a template .json file to .bicep.",
                args => services.GetRequiredService<DecompileCommand>().RunAsync(new DecompileArguments(args))));

            rootCommand.Add(LegacyCommand(
                Constants.Command.DecompileParams,
                "Attempts to decompile a parameters .json file to .bicepparam.",
                args => Task.FromResult(services.GetRequiredService<DecompileParamsCommand>().Run(new DecompileParamsArguments(args)))));

            rootCommand.Add(LegacyCommand(
                Constants.Command.Publish,
                "Publishes a .bicep file to a registry.",
                args => services.GetRequiredService<PublishCommand>().RunAsync(new PublishArguments(args))));

            rootCommand.Add(LegacyCommand(
                Constants.Command.PublishExtension,
                "Publishes a Bicep extension to a registry.",
                args => services.GetRequiredService<PublishExtensionCommand>().RunAsync(new PublishExtensionArguments(args), cancellationToken)));

            rootCommand.Add(LegacyCommand(
                Constants.Command.Restore,
                "Restores external modules for a .bicep file.",
                args => services.GetRequiredService<RestoreCommand>().RunAsync(new RestoreArguments(args))));

            rootCommand.Add(LegacyCommand(
                Constants.Command.Lint,
                "Lints a .bicep file.",
                args => services.GetRequiredService<LintCommand>().RunAsync(new LintArguments(args))));

            rootCommand.Add(CreateJsonRpcCommand());

            rootCommand.Add(LegacyCommand(
                Constants.Command.LocalDeploy,
                "Performs a local deployment.",
                args => services.GetRequiredService<LocalDeployCommand>().RunAsync(new LocalDeployArguments(args), cancellationToken)));

            rootCommand.Add(LegacyCommand(
                Constants.Command.Snapshot,
                "Creates an extension snapshot.",
                args => services.GetRequiredService<SnapshotCommand>().RunAsync(new SnapshotArguments(args), cancellationToken)));

            rootCommand.Add(LegacyCommand(
                Constants.Command.Deploy,
                "Deploys infrastructure using a .bicepparam file.",
                args => services.GetRequiredService<DeployCommand>().RunAsync(new DeployArguments(args), cancellationToken)));

            rootCommand.Add(LegacyCommand(
                Constants.Command.WhatIf,
                "Previews the changes a deployment would make.",
                args => services.GetRequiredService<WhatIfCommand>().RunAsync(new WhatIfArguments(args), cancellationToken)));

            rootCommand.Add(LegacyCommand(
                Constants.Command.Teardown,
                "Tears down resources deployed by a .bicepparam file.",
                args => services.GetRequiredService<TeardownCommand>().RunAsync(new TeardownArguments(args), cancellationToken)));

            rootCommand.Add(LegacyCommand(
                Constants.Command.Console,
                "Opens an interactive Bicep console.",
                args => services.GetRequiredService<ConsoleCommand>().RunAsync(new ConsoleArguments(args))));

            return rootCommand;
        }

        private Command CreateJsonRpcCommand()
        {
            var command = new Command(Constants.Command.JsonRpc, "Starts the Bicep JSON-RPC server.");

            var pipeOption = new Option<string?>("--pipe")
            {
                Description = "Connect via a named pipe with the given name.",
            };
            var socketOption = new Option<int?>("--socket")
            {
                Description = "Connect via a TCP socket on the specified port.",
            };
            var stdioOption = new Option<bool>("--stdio")
            {
                Description = "Use standard input/output for communication (default when no transport is specified).",
            };

            command.Add(pipeOption);
            command.Add(socketOption);
            command.Add(stdioOption);

            command.Validators.Add(result =>
            {
                var hasPipe = result.GetResult(pipeOption) is { Implicit: false };
                var hasSocket = result.GetResult(socketOption) is { Implicit: false };
                var hasStdio = result.GetResult(stdioOption)  is { Implicit: false };

                if ((hasPipe ? 1 : 0) + (hasSocket ? 1 : 0) + (hasStdio ? 1 : 0) > 1)
                {
                    result.AddError("Only one of --pipe, --socket, or --stdio may be specified.");
                }
            });

            command.SetAction(async (result, cancellationToken) =>
            {
                JsonRpcArguments args = new()
                {
                    Pipe = result.GetValue(pipeOption),
                    Socket = result.GetValue(socketOption),
                    Stdio = result.GetValue(stdioOption)
                };

                return await services.GetRequiredService<JsonRpcCommand>().RunAsync(args, cancellationToken);
            });

            return command;
        }

        /// <summary>
        /// Creates a pass-through command stub that forwards all unrecognized tokens to an
        /// existing argument class and command handler. System.CommandLine's built-in options
        /// (e.g. <c>--help</c>) are still handled; everything else lands in
        /// <see cref="ParseResult.UnmatchedTokens"/> and is passed to <paramref name="handler"/>
        /// as a plain <c>string[]</c>.
        /// <para>
        /// Once a command is fully migrated, replace this stub with a <see cref="Command"/> that
        /// declares its own <see cref="Option{T}"/> and <see cref="Argument{T}"/> members.
        /// </para>
        /// </summary>
        private Command LegacyCommand(string name, string description, Func<string[], Task<int>> handler)
        {
            var command = new Command(name, description)
            {
                TreatUnmatchedTokensAsErrors = false,
            };

            command.SetAction(async (ParseResult pr, CancellationToken ct) =>
            {
                try
                {
                    return await handler(pr.UnmatchedTokens.ToArray());
                }
                catch (BicepException exception)
                {
                    await io.Error.Writer.WriteLineAsync(exception.Message);
                    return 1;
                }
            });

            return command;
        }

        private static ILoggerFactory CreateLoggerFactory(IOContext io)
        {
            // apparently logging requires a factory factory 🤦‍
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
                // this is expected - no need to rethrow
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
