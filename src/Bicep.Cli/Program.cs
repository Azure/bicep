// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
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
using Bicep.Core.Emit.Options;
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
            rootCommand.Add(CreateBuildCommand());

            rootCommand.Add(CreateTestCommand());

            rootCommand.Add(CreateBuildParamsCommand());

            rootCommand.Add(LegacyCommand(
                Constants.Command.Format,
                "Formats a .bicep file.",
                args => Task.FromResult(services.GetRequiredService<FormatCommand>().Run(new FormatArguments(args)))));

            rootCommand.Add(CreateGenerateParamsFileCommand());

            rootCommand.Add(CreateDecompileCommand());

            rootCommand.Add(CreateDecompileParamsCommand());

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

            rootCommand.Add(CreateLocalDeployCommand());

            rootCommand.Add(CreateSnapshotCommand());

            rootCommand.Add(CreateDeployCommand());

            rootCommand.Add(CreateWhatIfCommand());

            rootCommand.Add(CreateTeardownCommand());

            rootCommand.Add(CreateConsoleCommand());

            return rootCommand;
        }

        private Command CreateBuildCommand()
        {
            var command = new Command(Constants.Command.Build, "Builds a .bicep file.")
            {
                TreatUnmatchedTokensAsErrors = true,
            };

            var inputFileArgument = new Argument<string?>("input-file")
            {
                Description = "The path to the input .bicep file.",
                Arity = ArgumentArity.ZeroOrOne,
            };
            var stdoutOption = new Option<bool>("--stdout")
            {
                Description = "Print output to stdout.",
            };
            var noRestoreOption = new Option<bool>("--no-restore")
            {
                Description = "Do not restore modules prior to building.",
            };
            var outDirOption = new Option<string?>("--outdir")
            {
                Description = "Save output to the specified directory.",
            };
            var outFileOption = new Option<string?>("--outfile")
            {
                Description = "Save output to the specified file path.",
            };
            var filePatternOption = new Option<string?>("--pattern")
            {
                Description = "Build all files matching the specified pattern.",
            };
            var diagnosticsFormatOption = new Option<DiagnosticsFormat?>("--diagnostics-format")
            {
                Description = "Set the format of diagnostics (Default, SARIF).",
            };

            command.Add(inputFileArgument);
            command.Add(stdoutOption);
            command.Add(noRestoreOption);
            command.Add(outDirOption);
            command.Add(outFileOption);
            command.Add(filePatternOption);
            command.Add(diagnosticsFormatOption);

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var args = new BuildArguments(
                    result.GetValue(inputFileArgument),
                    result.GetValue(stdoutOption),
                    result.GetValue(noRestoreOption),
                    result.GetValue(outDirOption),
                    result.GetValue(outFileOption),
                    result.GetValue(filePatternOption),
                    result.GetValue(diagnosticsFormatOption));

                return await services.GetRequiredService<BuildCommand>().RunAsync(args);
            }));

            return command;
        }

        private Command CreateTestCommand()
        {
            var command = new Command(Constants.Command.Test, "Runs tests in a .bicep file.")
            {
                TreatUnmatchedTokensAsErrors = true,
            };

            var inputFileArgument = new Argument<string>("input-file")
            {
                Description = "The path to the input .bicep file.",
            };
            var noRestoreOption = new Option<bool>("--no-restore")
            {
                Description = "Do not restore modules prior to running tests.",
            };
            var diagnosticsFormatOption = new Option<DiagnosticsFormat?>("--diagnostics-format")
            {
                Description = "Set the format of diagnostics (Default, SARIF).",
            };

            command.Add(inputFileArgument);
            command.Add(noRestoreOption);
            command.Add(diagnosticsFormatOption);

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var args = new TestArguments(
                    result.GetRequiredValue(inputFileArgument),
                    result.GetValue(noRestoreOption),
                    result.GetValue(diagnosticsFormatOption));

                return await services.GetRequiredService<TestCommand>().RunAsync(args);
            }));

            return command;
        }

        private Command CreateBuildParamsCommand()
        {
            var command = new Command(Constants.Command.BuildParams, "Builds a .bicepparam file.")
            {
                TreatUnmatchedTokensAsErrors = true,
            };

            var inputFileArgument = new Argument<string?>("input-file")
            {
                Description = "The path to the input .bicepparam file.",
                Arity = ArgumentArity.ZeroOrOne,
            };
            var stdoutOption = new Option<bool>("--stdout")
            {
                Description = "Print output to stdout.",
            };
            var noRestoreOption = new Option<bool>("--no-restore")
            {
                Description = "Do not restore modules prior to building.",
            };
            var outDirOption = new Option<string?>("--outdir")
            {
                Description = "Save output to the specified directory.",
            };
            var outFileOption = new Option<string?>("--outfile")
            {
                Description = "Save output to the specified file path.",
            };
            var filePatternOption = new Option<string?>("--pattern")
            {
                Description = "Build all files matching the specified pattern.",
            };
            var bicepFileOption = new Option<string?>("--bicep-file")
            {
                Description = "Path to the .bicep template file that will be used to validate the .bicepparam file.",
            };
            var diagnosticsFormatOption = new Option<DiagnosticsFormat?>("--diagnostics-format")
            {
                Description = "Set the format of diagnostics (Default, SARIF).",
            };

            command.Add(inputFileArgument);
            command.Add(stdoutOption);
            command.Add(noRestoreOption);
            command.Add(outDirOption);
            command.Add(outFileOption);
            command.Add(filePatternOption);
            command.Add(bicepFileOption);
            command.Add(diagnosticsFormatOption);

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var args = new BuildParamsArguments(
                    result.GetValue(inputFileArgument),
                    result.GetValue(stdoutOption),
                    result.GetValue(noRestoreOption),
                    result.GetValue(outDirOption),
                    result.GetValue(outFileOption),
                    result.GetValue(filePatternOption),
                    result.GetValue(bicepFileOption),
                    result.GetValue(diagnosticsFormatOption));

                return await services.GetRequiredService<BuildParamsCommand>().RunAsync(args);
            }));

            return command;
        }

        private Command CreateGenerateParamsFileCommand()
        {
            var command = new Command(Constants.Command.GenerateParamsFile, "Generates a parameters file for a .bicep file.")
            {
                TreatUnmatchedTokensAsErrors = true,
            };

            var inputFileArgument = new Argument<string>("input-file")
            {
                Description = "The path to the input .bicep file.",
            };
            var stdoutOption = new Option<bool>("--stdout")
            {
                Description = "Print output to stdout.",
            };
            var noRestoreOption = new Option<bool>("--no-restore")
            {
                Description = "Do not restore modules prior to generating.",
            };
            var outDirOption = new Option<string?>("--outdir")
            {
                Description = "Save output to the specified directory.",
            };
            var outFileOption = new Option<string?>("--outfile")
            {
                Description = "Save output to the specified file path.",
            };
            var outputFormatOption = new Option<OutputFormatOption>("--output-format")
            {
                Description = "Output format (Json, BicepParam).",
            };
            var includeParamsOption = new Option<IncludeParamsOption>("--include-params")
            {
                Description = "Which parameters to include (RequiredOnly, All).",
            };

            command.Add(inputFileArgument);
            command.Add(stdoutOption);
            command.Add(noRestoreOption);
            command.Add(outDirOption);
            command.Add(outFileOption);
            command.Add(outputFormatOption);
            command.Add(includeParamsOption);

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var args = new GenerateParametersFileArguments(
                    result.GetRequiredValue(inputFileArgument),
                    result.GetValue(stdoutOption),
                    result.GetValue(noRestoreOption),
                    result.GetValue(outDirOption),
                    result.GetValue(outFileOption),
                    result.GetValue(outputFormatOption),
                    result.GetValue(includeParamsOption));

                return await services.GetRequiredService<GenerateParametersFileCommand>().RunAsync(args);
            }));

            return command;
        }

        private Command CreateDecompileCommand()
        {
            var command = new Command(Constants.Command.Decompile, "Attempts to decompile a template .json file to .bicep.")
            {
                TreatUnmatchedTokensAsErrors = true,
            };

            var inputFileArgument = new Argument<string>("input-file")
            {
                Description = "The path to the ARM template .json file.",
            };
            var stdoutOption = new Option<bool>("--stdout")
            {
                Description = "Print output to stdout.",
            };
            var forceOption = new Option<bool>("--force")
            {
                Description = "Allow overwriting existing files.",
            };
            var outDirOption = new Option<string?>("--outdir")
            {
                Description = "Save output to the specified directory.",
            };
            var outFileOption = new Option<string?>("--outfile")
            {
                Description = "Save output to the specified file path.",
            };

            command.Add(inputFileArgument);
            command.Add(stdoutOption);
            command.Add(forceOption);
            command.Add(outDirOption);
            command.Add(outFileOption);

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var args = new DecompileArguments(
                    result.GetRequiredValue(inputFileArgument),
                    result.GetValue(stdoutOption),
                    result.GetValue(forceOption),
                    result.GetValue(outDirOption),
                    result.GetValue(outFileOption));

                return await services.GetRequiredService<DecompileCommand>().RunAsync(args);
            }));

            return command;
        }

        private Command CreateDecompileParamsCommand()
        {
            var command = new Command(Constants.Command.DecompileParams, "Attempts to decompile a parameters .json file to .bicepparam.")
            {
                TreatUnmatchedTokensAsErrors = true,
            };

            var inputFileArgument = new Argument<string>("input-file")
            {
                Description = "The path to the parameters .json file.",
            };
            var stdoutOption = new Option<bool>("--stdout")
            {
                Description = "Print output to stdout.",
            };
            var forceOption = new Option<bool>("--force")
            {
                Description = "Allow overwriting existing files.",
            };
            var outDirOption = new Option<string?>("--outdir")
            {
                Description = "Save output to the specified directory.",
            };
            var outFileOption = new Option<string?>("--outfile")
            {
                Description = "Save output to the specified file path.",
            };
            var bicepFileOption = new Option<string?>("--bicep-file")
            {
                Description = "Path to the .bicep template file associated with the parameters file.",
            };

            command.Add(inputFileArgument);
            command.Add(stdoutOption);
            command.Add(forceOption);
            command.Add(outDirOption);
            command.Add(outFileOption);
            command.Add(bicepFileOption);

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var args = new DecompileParamsArguments(
                    result.GetRequiredValue(inputFileArgument),
                    result.GetValue(stdoutOption),
                    result.GetValue(forceOption),
                    result.GetValue(outDirOption),
                    result.GetValue(outFileOption),
                    result.GetValue(bicepFileOption));

                return await Task.FromResult(services.GetRequiredService<DecompileParamsCommand>().Run(args));
            }));

            return command;
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
                JsonRpcArguments args = new(
                    Pipe: result.GetValue(pipeOption),
                    Socket: result.GetValue(socketOption),
                    Stdio: result.GetValue(stdioOption));

                return await services.GetRequiredService<JsonRpcCommand>().RunAsync(args, cancellationToken);
            });

            return command;
        }

        private Command CreateDeployCommand()
        {
            var command = new Command(Constants.Command.Deploy, "Deploys infrastructure using a .bicepparam file.")
            {
                TreatUnmatchedTokensAsErrors = false,
            };

            var parametersFileArgument = new Argument<string>("parameters-file")
            {
                Description = "The path to the .bicepparam file.",
            };
            var noRestoreOption = new Option<bool>("--no-restore")
            {
                Description = "Do not restore modules prior to deploying.",
            };
            var formatOption = new Option<DeploymentOutputFormat?>("--format")
            {
                Description = "Output format for deployment results (Default, Json).",
            };

            command.Add(parametersFileArgument);
            command.Add(noRestoreOption);
            command.Add(formatOption);

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var additionalArguments = ParseAdditionalArguments(result.UnmatchedTokens);
                var args = new DeployArguments(
                    result.GetRequiredValue(parametersFileArgument),
                    result.GetValue(noRestoreOption),
                    additionalArguments,
                    result.GetValue(formatOption));

                return await services.GetRequiredService<DeployCommand>().RunAsync(args, ct);
            }));

            return command;
        }

        private Command CreateWhatIfCommand()
        {
            var command = new Command(Constants.Command.WhatIf, "Previews the changes a deployment would make.")
            {
                TreatUnmatchedTokensAsErrors = false,
            };

            var parametersFileArgument = new Argument<string>("parameters-file")
            {
                Description = "The path to the .bicepparam file.",
            };
            var noRestoreOption = new Option<bool>("--no-restore")
            {
                Description = "Do not restore modules prior to running what-if.",
            };

            command.Add(parametersFileArgument);
            command.Add(noRestoreOption);

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var additionalArguments = ParseAdditionalArguments(result.UnmatchedTokens);
                var args = new WhatIfArguments(
                    result.GetRequiredValue(parametersFileArgument),
                    result.GetValue(noRestoreOption),
                    additionalArguments);

                return await services.GetRequiredService<WhatIfCommand>().RunAsync(args, ct);
            }));

            return command;
        }

        private Command CreateTeardownCommand()
        {
            var command = new Command(Constants.Command.Teardown, "Tears down resources deployed by a .bicepparam file.")
            {
                TreatUnmatchedTokensAsErrors = false,
            };

            var parametersFileArgument = new Argument<string>("parameters-file")
            {
                Description = "The path to the .bicepparam file.",
            };
            var noRestoreOption = new Option<bool>("--no-restore")
            {
                Description = "Do not restore modules prior to tearing down.",
            };

            command.Add(parametersFileArgument);
            command.Add(noRestoreOption);

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var additionalArguments = ParseAdditionalArguments(result.UnmatchedTokens);
                var args = new TeardownArguments(
                    result.GetRequiredValue(parametersFileArgument),
                    result.GetValue(noRestoreOption),
                    additionalArguments);

                return await services.GetRequiredService<TeardownCommand>().RunAsync(args, ct);
            }));

            return command;
        }

        private Command CreateLocalDeployCommand()
        {
            var command = new Command(Constants.Command.LocalDeploy, "Performs a local deployment.");

            var paramsFileArgument = new Argument<string>("parameters-file")
            {
                Description = "The path to the .bicepparam file.",
            };
            var noRestoreOption = new Option<bool>("--no-restore")
            {
                Description = "Do not restore modules prior to deploying.",
            };
            var formatOption = new Option<DeploymentOutputFormat?>("--format")
            {
                Description = "Output format for deployment results (Default, Json).",
            };

            command.Add(paramsFileArgument);
            command.Add(noRestoreOption);
            command.Add(formatOption);

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var args = new LocalDeployArguments(
                    result.GetRequiredValue(paramsFileArgument),
                    result.GetValue(noRestoreOption),
                    result.GetValue(formatOption));

                return await services.GetRequiredService<LocalDeployCommand>().RunAsync(args, ct);
            }));

            return command;
        }

        private Command CreateSnapshotCommand()
        {
            var command = new Command(Constants.Command.Snapshot, "Creates an extension snapshot.");

            var inputFileArgument = new Argument<string>("input-file")
            {
                Description = "The path to the .bicepparam file.",
            };
            var modeOption = new Option<SnapshotArguments.SnapshotMode?>("--mode")
            {
                Description = "Snapshot mode (Overwrite, Validate).",
            };
            var tenantIdOption = new Option<string?>("--tenant-id")
            {
                Description = "The tenant ID.",
            };
            var subscriptionIdOption = new Option<string?>("--subscription-id")
            {
                Description = "The subscription ID.",
            };
            var locationOption = new Option<string?>("--location")
            {
                Description = "The location.",
            };
            var resourceGroupOption = new Option<string?>("--resource-group")
            {
                Description = "The resource group.",
            };
            var deploymentNameOption = new Option<string?>("--deployment-name")
            {
                Description = "The deployment name.",
            };

            command.Add(inputFileArgument);
            command.Add(modeOption);
            command.Add(tenantIdOption);
            command.Add(subscriptionIdOption);
            command.Add(locationOption);
            command.Add(resourceGroupOption);
            command.Add(deploymentNameOption);

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var args = new SnapshotArguments(
                    result.GetRequiredValue(inputFileArgument),
                    result.GetValue(modeOption),
                    result.GetValue(tenantIdOption),
                    result.GetValue(subscriptionIdOption),
                    result.GetValue(locationOption),
                    result.GetValue(resourceGroupOption),
                    result.GetValue(deploymentNameOption));

                return await services.GetRequiredService<SnapshotCommand>().RunAsync(args, ct);
            }));

            return command;
        }

        private Command CreateConsoleCommand()
        {
            var command = new Command(Constants.Command.Console, "Opens an interactive Bicep console.");

            command.SetAction((result, ct) => RunCommandAsync(
                () => services.GetRequiredService<ConsoleCommand>().RunAsync(new ConsoleArguments())));

            return command;
        }

        private async Task<int> RunCommandAsync(Func<Task<int>> action)
        {
            try
            {
                return await action();
            }
            catch (BicepException exception)
            {
                await io.Error.Writer.WriteLineAsync(exception.Message);
                return 1;
            }
        }

        private static ImmutableDictionary<string, string> ParseAdditionalArguments(IReadOnlyList<string> unmatchedTokens)
        {
            var additionalArguments = new Dictionary<string, string>();
            for (var i = 0; i < unmatchedTokens.Count; i++)
            {
                var token = unmatchedTokens[i];
                if (token.StartsWith(ArgumentConstants.CliArgPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    var key = token[ArgumentConstants.CliArgPrefix.Length..];
                    if (additionalArguments.ContainsKey(key))
                    {
                        throw new CommandLineException($"Parameter \"{token}\" cannot be specified multiple times.");
                    }
                    if (i + 1 >= unmatchedTokens.Count)
                    {
                        throw new CommandLineException($"Parameter \"{token}\" requires a value.");
                    }
                    additionalArguments[key] = unmatchedTokens[++i];
                }
                else
                {
                    throw new CommandLineException($"Unrecognized parameter \"{token}\"");
                }
            }

            return additionalArguments.ToImmutableDictionary();
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

            command.SetAction((ParseResult pr, CancellationToken ct) =>
                RunCommandAsync(() => handler(pr.UnmatchedTokens.ToArray())));

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
