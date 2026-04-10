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
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Exceptions;
using Bicep.Core.Features;
using Bicep.Core.Tracing;
using Bicep.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;

using SclRootCommand = System.CommandLine.RootCommand;
using System.CommandLine.Parsing;

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
            var invocationConfig = new InvocationConfiguration
            {
                Output = io.Output.Writer,
                Error = io.Error.Writer,
            };
            return await rootCommand.Parse(args).InvokeAsync(invocationConfig, cancellationToken);
        }

        private SclRootCommand BuildCommandLine(CancellationToken cancellationToken)
        {
            var rootCommand = new SclRootCommand("Bicep CLI");

            // System.CommandLine adds --version and --help to RootCommand by default.
            // Replace both with custom options so their output goes through io.Output.Writer
            // (not Console.Out) and --version prints the Bicep-specific version string.
            var builtInVersion = rootCommand.Options.FirstOrDefault(o => o.Name == Constants.Option.Version);
            if (builtInVersion is not null)
            {
                rootCommand.Options.Remove(builtInVersion);
            }

            var versionOption = new Option<bool>(Constants.Option.Version, Constants.Option.VersionShort) { Description = "Shows bicep version information." };
            var licenseOption = new Option<bool>(Constants.Option.License) { Description = "Prints license information." };
            var thirdPartyNoticesOption = new Option<bool>(Constants.Option.ThirdPartyNotices) { Description = "Prints third-party notices." };

            // rootCommand.Add(helpOption);
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

                await io.Error.Writer.WriteLineAsync(
                    string.Format(CliResources.UnrecognizedArgumentsFormat, string.Join(' ', unmatched), ThisAssembly.AssemblyName));
                return 1;
            });

            rootCommand.Add(CreateBuildCommand());

            rootCommand.Add(CreateTestCommand());

            rootCommand.Add(CreateBuildParamsCommand());

            rootCommand.Add(CreateFormatCommand());

            rootCommand.Add(CreateGenerateParamsFileCommand());

            rootCommand.Add(CreateDecompileCommand());

            rootCommand.Add(CreateDecompileParamsCommand());

            rootCommand.Add(CreatePublishCommand());

            rootCommand.Add(CreatePublishExtensionCommand());

            rootCommand.Add(CreateRestoreCommand());

            rootCommand.Add(CreateLintCommand());

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

            var inputFileArgument = new Argument<string?>(Constants.Argument.InputFile)
            {
                Description = "The path to the input .bicep file.",
                Arity = ArgumentArity.ZeroOrOne,
            };
            var stdoutOption = new Option<bool>(Constants.Option.Stdout)
            {
                Description = "Prints the output to stdout.",
            };
            var noRestoreOption = new Option<bool>(Constants.Option.NoRestore)
            {
                Description = "Builds the bicep file without restoring external modules.",
            };
            var outDirOption = new Option<string?>(Constants.Option.OutDir)
            {
                Description = "Saves the output at the specified directory.",
            };
            var outFileOption = new Option<string?>(Constants.Option.OutFile)
            {
                Description = "Saves the output as the specified file path.",
            };
            var filePatternOption = new Option<string?>(Constants.Option.Pattern)
            {
                Description = "Builds all files matching the specified glob pattern.",
            };
            var diagnosticsFormatOption = new Option<DiagnosticsFormat?>(Constants.Option.DiagnosticsFormat)
            {
                Description = "Sets the diagnostics format. Valid values are (Default, SARIF).",
            };

            command.Add(inputFileArgument);
            command.Add(stdoutOption);
            command.Add(noRestoreOption);
            command.Add(outDirOption);
            command.Add(outFileOption);
            command.Add(filePatternOption);
            command.Add(diagnosticsFormatOption);
            command.Validators.Add(result => ValidatePositionalArgument(result, inputFileArgument));

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var outputToStdOut = result.GetValue(stdoutOption);
                var outputDir = result.GetValue(outDirOption);
                var outputFile = result.GetValue(outFileOption);
                var filePattern = result.GetValue(filePatternOption);

                ArgumentHelper.ValidateOutputOptions(outputToStdOut, outputDir, outputFile, filePattern);

                var args = new BuildArguments(
                    result.GetValue(inputFileArgument),
                    outputToStdOut,
                    result.GetValue(noRestoreOption),
                    outputDir,
                    outputFile,
                    filePattern,
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

            var inputFileArgument = new Argument<string?>(Constants.Argument.InputFile)
            {
                Description = "The path to the input .bicep file.",
                Arity = ArgumentArity.ZeroOrOne,
            };
            var noRestoreOption = new Option<bool>(Constants.Option.NoRestore)
            {
                Description = "Do not restore modules prior to running tests.",
            };
            var diagnosticsFormatOption = new Option<DiagnosticsFormat?>(Constants.Option.DiagnosticsFormat)
            {
                Description = "Set the format of diagnostics (Default, SARIF).",
            };

            command.Add(inputFileArgument);
            command.Add(noRestoreOption);
            command.Add(diagnosticsFormatOption);
            command.Validators.Add(result => ValidatePositionalArgument(result, inputFileArgument));

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var inputFile = result.GetValue(inputFileArgument)
                    ?? throw new CommandLineException("The input file path was not specified");
                var args = new TestArguments(
                    inputFile,
                    result.GetValue(noRestoreOption),
                    result.GetValue(diagnosticsFormatOption));

                return await services.GetRequiredService<TestCommand>().RunAsync(args);
            }));

            return command;
        }

        private Command CreateBuildParamsCommand()
        {
            var command = new Command(Constants.Command.BuildParams, "Builds a .json file from a .bicepparam file.")
            {
                TreatUnmatchedTokensAsErrors = true,
            };

            var inputFileArgument = new Argument<string?>(Constants.Argument.InputFile)
            {
                Description = "The path to the input .bicepparam file.",
                Arity = ArgumentArity.ZeroOrOne,
            };
            var stdoutOption = new Option<bool>(Constants.Option.Stdout)
            {
                Description = "Prints the output of building both the parameter file (.bicepparam) and the template it points to (.bicep) as json to stdout.",
            };
            var noRestoreOption = new Option<bool>(Constants.Option.NoRestore)
            {
                Description = "Builds the bicep file (referenced in using declaration) without restoring external modules.",
            };
            var outDirOption = new Option<string?>(Constants.Option.OutDir)
            {
                Description = "Saves the output of building the parameter file only (.bicepparam) as json to the specified directory.",
            };
            var outFileOption = new Option<string?>(Constants.Option.OutFile)
            {
                Description = "Saves the output of building the parameter file only (.bicepparam) as json to the specified file path.",
            };
            var filePatternOption = new Option<string?>(Constants.Option.Pattern)
            {
                Description = "Builds all files matching the specified glob pattern.",
            };
            var bicepFileOption = new Option<string?>(Constants.Option.BicepFile)
            {
                Description = "Verifies if the specified bicep file path matches the one provided in the params file using declaration.",
            };
            var diagnosticsFormatOption = new Option<DiagnosticsFormat?>(Constants.Option.DiagnosticsFormat)
            {
                Description = "Sets the diagnostics format. Valid values are (Default, SARIF).",
            };

            command.Add(inputFileArgument);
            command.Add(stdoutOption);
            command.Add(noRestoreOption);
            command.Add(outDirOption);
            command.Add(outFileOption);
            command.Add(filePatternOption);
            command.Add(bicepFileOption);
            command.Add(diagnosticsFormatOption);
            command.Validators.Add(result => ValidatePositionalArgument(result, inputFileArgument));

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var outputToStdOut = result.GetValue(stdoutOption);
                var outputDir = result.GetValue(outDirOption);
                var outputFile = result.GetValue(outFileOption);
                var filePattern = result.GetValue(filePatternOption);
                var bicepFile = result.GetValue(bicepFileOption);

                if (filePattern is not null && bicepFile is not null)
                {
                    throw new CommandLineException("The --bicep-file parameter cannot be used with the --pattern parameter");
                }
                ArgumentHelper.ValidateOutputOptions(outputToStdOut, outputDir, outputFile, filePattern);

                var args = new BuildParamsArguments(
                    result.GetValue(inputFileArgument),
                    outputToStdOut,
                    result.GetValue(noRestoreOption),
                    outputDir,
                    outputFile,
                    filePattern,
                    bicepFile,
                    result.GetValue(diagnosticsFormatOption));

                return await services.GetRequiredService<BuildParamsCommand>().RunAsync(args);
            }));

            return command;
        }

        private Command CreateGenerateParamsFileCommand()
        {
            var command = new Command(Constants.Command.GenerateParamsFile, "Builds parameters file from the given bicep file, updates if there is an existing parameters file.")
            {
                TreatUnmatchedTokensAsErrors = true,
            };

            var inputFileArgument = new Argument<string?>(Constants.Argument.InputFile)
            {
                Description = "The path to the input .bicep file.",
                Arity = ArgumentArity.ZeroOrOne,
            };
            var stdoutOption = new Option<bool>(Constants.Option.Stdout)
            {
                Description = "Prints the output to stdout.",
            };
            var noRestoreOption = new Option<bool>(Constants.Option.NoRestore)
            {
                Description = "Generates the parameters file without restoring external modules.",
            };
            var outDirOption = new Option<string?>(Constants.Option.OutDir)
            {
                Description = "Saves the output at the specified directory.",
            };
            var outFileOption = new Option<string?>(Constants.Option.OutFile)
            {
                Description = "Saves the output as the specified file path.",
            };
            var outputFormatOption = new Option<OutputFormatOption>(Constants.Option.OutputFormat)
            {
                Description = "Selects the output format (json, bicepparam).",
            };
            var includeParamsOption = new Option<IncludeParamsOption>(Constants.Option.IncludeParams)
            {
                Description = "Selects which parameters to include into output (requiredonly, all).",
            };

            command.Add(inputFileArgument);
            command.Add(stdoutOption);
            command.Add(noRestoreOption);
            command.Add(outDirOption);
            command.Add(outFileOption);
            command.Add(outputFormatOption);
            command.Add(includeParamsOption);
            command.Validators.Add(result => ValidatePositionalArgument(result, inputFileArgument));

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var outputToStdOut = result.GetValue(stdoutOption);
                var outputDir = result.GetValue(outDirOption);
                var outputFile = result.GetValue(outFileOption);

                ArgumentHelper.ValidateOutputOptions(outputToStdOut, outputDir, outputFile);

                var inputFile = result.GetValue(inputFileArgument)
                    ?? throw new CommandLineException("The input file path was not specified");
                var args = new GenerateParametersFileArguments(
                    inputFile,
                    outputToStdOut,
                    result.GetValue(noRestoreOption),
                    outputDir,
                    outputFile,
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

            var inputFileArgument = new Argument<string?>(Constants.Argument.InputFile)
            {
                Description = "The path to the ARM template .json file.",
                Arity = ArgumentArity.ZeroOrOne,
            };
            var stdoutOption = new Option<bool>(Constants.Option.Stdout)
            {
                Description = "Prints the output to stdout.",
            };
            var forceOption = new Option<bool>(Constants.Option.Force)
            {
                Description = "Allows overwriting the output file if it exists (applies only to 'bicep decompile' or 'bicep decompile-params').",
            };
            var outDirOption = new Option<string?>(Constants.Option.OutDir)
            {
                Description = "Saves the output at the specified directory.",
            };
            var outFileOption = new Option<string?>(Constants.Option.OutFile)
            {
                Description = "Saves the output as the specified file path.",
            };

            command.Add(inputFileArgument);
            command.Add(stdoutOption);
            command.Add(forceOption);
            command.Add(outDirOption);
            command.Add(outFileOption);
            command.Validators.Add(result => ValidatePositionalArgument(result, inputFileArgument));

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var outputToStdOut = result.GetValue(stdoutOption);
                var outputDir = result.GetValue(outDirOption);
                var outputFile = result.GetValue(outFileOption);

                ArgumentHelper.ValidateOutputOptions(outputToStdOut, outputDir, outputFile);

                var inputFile = result.GetValue(inputFileArgument)
                    ?? throw new CommandLineException("The input file path was not specified");
                var args = new DecompileArguments(
                    inputFile,
                    outputToStdOut,
                    result.GetValue(forceOption),
                    outputDir,
                    outputFile);

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

            var inputFileArgument = new Argument<string>(Constants.Argument.InputFile)
            {
                Description = "The path to the parameters .json file.",
            };
            var stdoutOption = new Option<bool>(Constants.Option.Stdout)
            {
                Description = "Prints the output to stdout.",
            };
            var forceOption = new Option<bool>(Constants.Option.Force)
            {
                Description = "Allows overwriting the output file if it exists (applies only to 'bicep decompile' or 'bicep decompile-params').",
            };
            var outDirOption = new Option<string?>(Constants.Option.OutDir)
            {
                Description = "Saves the output at the specified directory.",
            };
            var outFileOption = new Option<string?>(Constants.Option.OutFile)
            {
                Description = "Saves the output as the specified file path.",
            };
            var bicepFileOption = new Option<string?>(Constants.Option.BicepFile)
            {
                Description = "Path to the bicep template file that will be referenced in the using declaration.",
            };

            command.Add(inputFileArgument);
            command.Add(stdoutOption);
            command.Add(forceOption);
            command.Add(outDirOption);
            command.Add(outFileOption);
            command.Add(bicepFileOption);
            command.Validators.Add(result => ValidateRequiredPositionalArgument(result, inputFileArgument));

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var outputToStdOut = result.GetValue(stdoutOption);
                var outputDir = result.GetValue(outDirOption);
                var outputFile = result.GetValue(outFileOption);

                ArgumentHelper.ValidateOutputOptions(outputToStdOut, outputDir, outputFile);

                var args = new DecompileParamsArguments(
                    result.GetRequiredValue(inputFileArgument),
                    outputToStdOut,
                    result.GetValue(forceOption),
                    outputDir,
                    outputFile,
                    result.GetValue(bicepFileOption));

                return await Task.FromResult(services.GetRequiredService<DecompileParamsCommand>().Run(args));
            }));

            return command;
        }

        private Command CreateFormatCommand()
        {
            var command = new Command(Constants.Command.Format, "Formats a .bicep file.");

            var inputFileArgument = new Argument<string?>(Constants.Argument.InputFile)
            {
                Description = "The path to the input .bicep file.",
                Arity = ArgumentArity.ZeroOrOne,
            };
            var stdoutOption = new Option<bool>(Constants.Option.Stdout)
            {
                Description = "Prints the output to stdout.",
            };
            var outDirOption = new Option<string?>(Constants.Option.OutDir)
            {
                Description = "Saves the output at the specified directory.",
            };
            var outFileOption = new Option<string?>(Constants.Option.OutFile)
            {
                Description = "Saves the output as the specified file path.",
            };
            var filePatternOption = new Option<string?>(Constants.Option.Pattern)
            {
                Description = "Formats all files matching the specified glob pattern.",
            };
            var newlineKindOption = new Option<NewlineKind?>(Constants.Option.NewlineKind)
            {
                Description = "Set newline char. Valid values are (Auto, LF, CRLF, CR).",
            };
            var indentKindOption = new Option<IndentKind?>(Constants.Option.IndentKind)
            {
                Description = "Set indentation kind. Valid values are (Space, Tab).",
            };
            var indentSizeOption = new Option<int?>(Constants.Option.IndentSize)
            {
                Description = "Number of spaces to indent with (only valid with --indent-kind set to Space).",
            };
            var insertFinalNewlineOption = new Option<bool?>(Constants.Option.InsertFinalNewline)
            {
                Description = "Insert a final newline.",
            };

            command.Add(inputFileArgument);
            command.Add(stdoutOption);
            command.Add(outDirOption);
            command.Add(outFileOption);
            command.Add(filePatternOption);
            command.Add(newlineKindOption);
            command.Add(indentKindOption);
            command.Add(indentSizeOption);
            command.Add(insertFinalNewlineOption);
            command.Validators.Add(result => ValidatePositionalArgument(result, inputFileArgument));

            command.SetAction((result, ct) => RunCommandAsync(() =>
            {
                var args = new FormatArguments(
                    result.GetValue(stdoutOption),
                    result.GetValue(inputFileArgument),
                    result.GetValue(outDirOption),
                    result.GetValue(outFileOption),
                    result.GetValue(filePatternOption),
                    result.GetValue(newlineKindOption),
                    result.GetValue(indentKindOption),
                    result.GetValue(indentSizeOption),
                    result.GetValue(insertFinalNewlineOption));

                return Task.FromResult(services.GetRequiredService<FormatCommand>().Run(args));
            }));

            return command;
        }

        private Command CreatePublishCommand()
        {
            var command = new Command(Constants.Command.Publish, "Publishes the .bicep file to the module registry.");

            var inputFileArgument = new Argument<string?>(Constants.Argument.InputFile)
            {
                Description = "The path to the .bicep file to publish.",
                Arity = ArgumentArity.ZeroOrOne,
            };
            var targetOption = new Option<string>(Constants.Option.Target)
            {
                Description = "The target module reference.",
            };
            var documentationUriOption = new Option<string[]>(Constants.Option.DocumentationUri)
            {
                Description = "Module documentation URI.",
                Arity = ArgumentArity.ZeroOrMore,
            };
            var noRestoreOption = new Option<bool>(Constants.Option.NoRestore)
            {
                Description = "Do not restore modules prior to publishing.",
            };
            var forceOption = new Option<bool>(Constants.Option.Force)
            {
                Description = "Overwrite existing published module or file.",
            };
            var withSourceOption = new Option<bool>(Constants.Option.WithSource)
            {
                Description = "[Experimental] Publish source code with the module.",
            };

            command.Add(inputFileArgument);
            command.Add(targetOption);
            command.Add(documentationUriOption);
            command.Add(noRestoreOption);
            command.Add(forceOption);
            command.Add(withSourceOption);
            command.Validators.Add(result => ValidatePositionalArgument(result, inputFileArgument));

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var inputFile = result.GetValue(inputFileArgument)
                    ?? throw new CommandLineException("The input file path was not specified");
                var target = result.GetValue(targetOption)
                    ?? throw new CommandLineException("The target module was not specified.");
                var docUriResult = result.GetResult(documentationUriOption);
                if (docUriResult is not null)
                {
                    if (docUriResult.Tokens.Count == 0)
                    {
                        throw new CommandLineException("The --documentation-uri parameter expects an argument.");
                    }
                    if (docUriResult.Tokens.Count > 1)
                    {
                        throw new CommandLineException("The --documentation-uri parameter cannot be specified more than once.");
                    }
                }
                var documentationUri = docUriResult?.Tokens.Count == 1 ? docUriResult.Tokens[0].Value : null;
                if (documentationUri is not null && !Uri.IsWellFormedUriString(documentationUri, UriKind.Absolute))
                {
                    throw new CommandLineException("The --documentation-uri should be a well formed uri string.");
                }
                var args = new PublishArguments(
                    inputFile,
                    target,
                    documentationUri,
                    result.GetValue(noRestoreOption),
                    result.GetValue(forceOption),
                    result.GetValue(withSourceOption));

                return await services.GetRequiredService<PublishCommand>().RunAsync(args);
            }));

            return command;
        }

        private Command CreatePublishExtensionCommand()
        {
            var command = new Command(Constants.Command.PublishExtension, "[Experimental] Publishes a Bicep extension to a registry.");

            var indexFileArgument = new Argument<string?>(Constants.Argument.IndexFile)
            {
                Description = "The path to the index file.",
                Arity = ArgumentArity.ZeroOrOne,
            };
            var targetOption = new Option<string?>(Constants.Option.Target)
            {
                Description = "The target extension reference.",
            };
            var forceOption = new Option<bool>(Constants.Option.Force)
            {
                Description = "Force publish even if the extension already exists.",
            };
            // Per-architecture binary options.
            var binLinuxX64Option = new Option<string?>(Constants.Option.BinLinuxX64) { Description = "Path to the linux-x64 binary." };
            var binLinuxArm64Option = new Option<string?>(Constants.Option.BinLinuxArm64) { Description = "Path to the linux-arm64 binary." };
            var binOsxX64Option = new Option<string?>(Constants.Option.BinOsxX64) { Description = "Path to the osx-x64 binary." };
            var binOsxArm64Option = new Option<string?>(Constants.Option.BinOsxArm64) { Description = "Path to the osx-arm64 binary." };
            var binWinX64Option = new Option<string?>(Constants.Option.BinWinX64) { Description = "Path to the win-x64 binary." };
            var binWinArm64Option = new Option<string?>(Constants.Option.BinWinArm64) { Description = "Path to the win-arm64 binary." };

            command.Add(indexFileArgument);
            command.Add(targetOption);
            command.Add(forceOption);
            command.Add(binLinuxX64Option);
            command.Add(binLinuxArm64Option);
            command.Add(binOsxX64Option);
            command.Add(binOsxArm64Option);
            command.Add(binWinX64Option);
            command.Add(binWinArm64Option);
            command.Validators.Add(result => ValidatePositionalArgument(result, indexFileArgument));

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var binaries = new Dictionary<string, string>();
                if (result.GetValue(binLinuxX64Option) is { } p1) { binaries["linux-x64"] = p1; }
                if (result.GetValue(binLinuxArm64Option) is { } p2) { binaries["linux-arm64"] = p2; }
                if (result.GetValue(binOsxX64Option) is { } p3) { binaries["osx-x64"] = p3; }
                if (result.GetValue(binOsxArm64Option) is { } p4) { binaries["osx-arm64"] = p4; }
                if (result.GetValue(binWinX64Option) is { } p5) { binaries["win-x64"] = p5; }
                if (result.GetValue(binWinArm64Option) is { } p6) { binaries["win-arm64"] = p6; }

                var args = new PublishExtensionArguments(
                    result.GetValue(indexFileArgument),
                    result.GetValue(targetOption),
                    binaries,
                    result.GetValue(forceOption));

                return await services.GetRequiredService<PublishExtensionCommand>().RunAsync(args, ct);
            }));

            return command;
        }

        private Command CreateRestoreCommand()
        {
            var command = new Command(Constants.Command.Restore, "Restores external modules from the specified Bicep file to the local module cache.");

            var inputFileArgument = new Argument<string?>(Constants.Argument.InputFile)
            {
                Description = "The path to the .bicep file.",
                Arity = ArgumentArity.ZeroOrOne,
            };
            var filePatternOption = new Option<string?>(Constants.Option.Pattern)
            {
                Description = "Restores all files matching the specified glob pattern.",
            };
            var forceOption = new Option<bool>(Constants.Option.Force)
            {
                Description = "Force restore even if modules are already cached.",
            };

            command.Add(inputFileArgument);
            command.Add(filePatternOption);
            command.Add(forceOption);
            command.Validators.Add(result => ValidatePositionalArgument(result, inputFileArgument));

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var args = new RestoreArguments(
                    result.GetValue(inputFileArgument),
                    result.GetValue(filePatternOption),
                    result.GetValue(forceOption));

                return await services.GetRequiredService<RestoreCommand>().RunAsync(args);
            }));

            return command;
        }

        private Command CreateLintCommand()
        {
            var command = new Command(Constants.Command.Lint, "Lints a .bicep file.");

            var inputFileArgument = new Argument<string?>(Constants.Argument.InputFile)
            {
                Description = "The path to the .bicep file.",
                Arity = ArgumentArity.ZeroOrOne,
            };
            var filePatternOption = new Option<string?>(Constants.Option.Pattern)
            {
                Description = "Lints all files matching the specified glob pattern.",
            };
            var noRestoreOption = new Option<bool>(Constants.Option.NoRestore)
            {
                Description = "Skips restoring external modules.",
            };
            var diagnosticsFormatOption = new Option<DiagnosticsFormat?>(Constants.Option.DiagnosticsFormat)
            {
                Description = "Sets the diagnostics format. Valid values are (Default, SARIF).",
            };

            command.Add(inputFileArgument);
            command.Add(filePatternOption);
            command.Add(noRestoreOption);
            command.Add(diagnosticsFormatOption);
            command.Validators.Add(result => ValidatePositionalArgument(result, inputFileArgument));

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var diagnosticsFormat = result.GetValue(diagnosticsFormatOption) ?? Arguments.DiagnosticsFormat.Default;
                var args = new LintArguments(
                    result.GetValue(inputFileArgument),
                    result.GetValue(filePatternOption),
                    diagnosticsFormat,
                    result.GetValue(noRestoreOption));

                return await services.GetRequiredService<LintCommand>().RunAsync(args);
            }));

            return command;
        }

        private Command CreateJsonRpcCommand()
        {
            var command = new Command(Constants.Command.JsonRpc, "Starts the Bicep CLI listening for JSONRPC messages, for programatically interacting with Bicep.");

            var pipeOption = new Option<string?>(Constants.Option.Pipe)
            {
                Description = "Bicep CLI will connect to the supplied named pipe as a client, and start listening for JSONRPC requests.",
            };
            var socketOption = new Option<int?>(Constants.Option.Socket)
            {
                Description = "Bicep CLI will connect to the supplied TCP port on the loopback interface as a client, and start listening for JSONRPC requests.",
            };
            var stdioOption = new Option<bool>(Constants.Option.Stdio)
            {
                Description = "Bicep CLI will use stdin/stdout for JSONRPC requests.",
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
            var command = new Command(Constants.Command.Deploy, "[Experimental] Deploys infrastructure using a .bicepparam file.")
            {
                TreatUnmatchedTokensAsErrors = false,
            };

            var inputFileArgument = new Argument<string>(Constants.Argument.ParametersFile)
            {
                Description = "The path to the .bicepparam file.",
            };
            var noRestoreOption = new Option<bool>(Constants.Option.NoRestore)
            {
                Description = "Do not restore modules prior to deploying.",
            };
            var formatOption = new Option<DeploymentOutputFormat?>(Constants.Option.Format)
            {
                Description = "Output format for deployment results (Default, Json).",
            };

            command.Add(inputFileArgument);
            command.Add(noRestoreOption);
            command.Add(formatOption);
            command.Validators.Add(result => ValidateRequiredPositionalArgument(result, inputFileArgument));

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var additionalArguments = ParseAdditionalArguments(result.UnmatchedTokens);
                var args = new DeployArguments(
                    result.GetRequiredValue(inputFileArgument),
                    result.GetValue(noRestoreOption),
                    additionalArguments,
                    result.GetValue(formatOption));

                return await services.GetRequiredService<DeployCommand>().RunAsync(args, ct);
            }));

            return command;
        }

        private Command CreateWhatIfCommand()
        {
            var command = new Command(Constants.Command.WhatIf, "[Experimental] Previews the changes a deployment would make.")
            {
                TreatUnmatchedTokensAsErrors = false,
            };

            var inputFileArgument = new Argument<string>(Constants.Argument.ParametersFile)
            {
                Description = "The path to the .bicepparam file.",
            };
            var noRestoreOption = new Option<bool>(Constants.Option.NoRestore)
            {
                Description = "Do not restore modules prior to running what-if.",
            };

            command.Add(inputFileArgument);
            command.Add(noRestoreOption);
            command.Validators.Add(result => ValidateRequiredPositionalArgument(result, inputFileArgument));

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var additionalArguments = ParseAdditionalArguments(result.UnmatchedTokens);
                var args = new WhatIfArguments(
                    result.GetRequiredValue(inputFileArgument),
                    result.GetValue(noRestoreOption),
                    additionalArguments);

                return await services.GetRequiredService<WhatIfCommand>().RunAsync(args, ct);
            }));

            return command;
        }

        private Command CreateTeardownCommand()
        {
            var command = new Command(Constants.Command.Teardown, "[Experimental] Tears down resources deployed by a .bicepparam file.")
            {
                TreatUnmatchedTokensAsErrors = false,
            };

            var inputFileArgument = new Argument<string>(Constants.Argument.ParametersFile)
            {
                Description = "The path to the .bicepparam file.",
            };
            var noRestoreOption = new Option<bool>(Constants.Option.NoRestore)
            {
                Description = "Do not restore modules prior to tearing down.",
            };

            command.Add(inputFileArgument);
            command.Add(noRestoreOption);
            command.Validators.Add(result => ValidateRequiredPositionalArgument(result, inputFileArgument));

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var additionalArguments = ParseAdditionalArguments(result.UnmatchedTokens);
                var args = new TeardownArguments(
                    result.GetRequiredValue(inputFileArgument),
                    result.GetValue(noRestoreOption),
                    additionalArguments);

                return await services.GetRequiredService<TeardownCommand>().RunAsync(args, ct);
            }));

            return command;
        }

        private Command CreateLocalDeployCommand()
        {
            var command = new Command(Constants.Command.LocalDeploy, "[Experimental] Performs a local deployment.");

            var inputFileArgument = new Argument<string>(Constants.Argument.ParametersFile)
            {
                Description = "The path to the .bicepparam file.",
            };
            var noRestoreOption = new Option<bool>(Constants.Option.NoRestore)
            {
                Description = "Do not restore modules prior to deploying.",
            };
            var formatOption = new Option<DeploymentOutputFormat?>(Constants.Option.Format)
            {
                Description = "Output format for deployment results (Default, Json).",
            };

            command.Add(inputFileArgument);
            command.Add(noRestoreOption);
            command.Add(formatOption);
            command.Validators.Add(result => ValidateRequiredPositionalArgument(result, inputFileArgument));

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var args = new LocalDeployArguments(
                    result.GetRequiredValue(inputFileArgument),
                    result.GetValue(noRestoreOption),
                    result.GetValue(formatOption));

                return await services.GetRequiredService<LocalDeployCommand>().RunAsync(args, ct);
            }));

            return command;
        }

        private Command CreateSnapshotCommand()
        {
            var command = new Command(Constants.Command.Snapshot, "Generates or validates a deployment snapshot from a .bicepparam file.");

            var inputFileArgument = new Argument<string>(Constants.Argument.ParametersFile)
            {
                Description = "The path to the .bicepparam file.",
            };
            var modeOption = new Option<SnapshotArguments.SnapshotMode?>(Constants.Option.Mode)
            {
                Description = "Sets the snapshot mode. Valid values are (overwrite, validate).",
            };
            var tenantIdOption = new Option<string?>(Constants.Option.TenantId)
            {
                Description = "The tenant ID to use for the deployment.",
            };
            var subscriptionIdOption = new Option<string?>(Constants.Option.SubscriptionId)
            {
                Description = "The subscription ID to use for the deployment.",
            };
            var locationOption = new Option<string?>(Constants.Option.Location)
            {
                Description = "The location to use for the deployment.",
            };
            var resourceGroupOption = new Option<string?>(Constants.Option.ResourceGroup)
            {
                Description = "The resource group name to use for the deployment.",
            };
            var managementGroupIdOption = new Option<string?>(Constants.Option.ManagementGroupId)
            {
                Description = "The management group ID to use for the deployment.",
            };
            var deploymentNameOption = new Option<string?>(Constants.Option.DeploymentName)
            {
                Description = "The deployment name to use.",
            };

            command.Add(inputFileArgument);
            command.Add(modeOption);
            command.Add(tenantIdOption);
            command.Add(subscriptionIdOption);
            command.Add(managementGroupIdOption);
            command.Add(locationOption);
            command.Add(resourceGroupOption);
            command.Add(deploymentNameOption);
            command.Validators.Add(result => ValidateRequiredPositionalArgument(result, inputFileArgument));

            command.SetAction((result, ct) => RunCommandAsync(async () =>
            {
                var args = new SnapshotArguments(
                    result.GetRequiredValue(inputFileArgument),
                    result.GetValue(modeOption),
                    result.GetValue(tenantIdOption),
                    result.GetValue(subscriptionIdOption),
                    result.GetValue(managementGroupIdOption),
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
        
        private static void ValidatePositionalArgument(CommandResult result, Argument<string?> argument)
        {
            if (result.GetValue(argument) is {} inputValue && inputValue.StartsWith("--", StringComparison.Ordinal))
            {
                result.AddError($"Unrecognized parameter \"{inputValue}\"");
            }
        }
        
        private static void ValidateRequiredPositionalArgument(CommandResult result, Argument<string> argument)
        {
            if (result.GetRequiredValue(argument) is {} inputValue && inputValue.StartsWith("--", StringComparison.Ordinal))
            {
                result.AddError($"Unrecognized parameter \"{inputValue}\"");
            }
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
