// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Exceptions;
using Bicep.Core.Extensions;
using Bicep.Core.SourceGraph;
using Bicep.IO.Abstraction;
using Option = Bicep.Cli.Constants.Option;

namespace Bicep.Cli.Commands;

public class DocsGenerateCommand(
    IOContext io,
    InputOutputArgumentsResolver argumentsResolver,
    DocsCommandRunner runner,
    OutputWriter writer) : ICommand
{
    public async Task<int> RunAsync(DocsGenerateArguments arguments, CancellationToken cancellationToken = default)
    {
        var (inputRoot, inputUris) = ResolveInputs(arguments);
        var workspace = new ActiveSourceFileSet();
        var successes = new Dictionary<IOUri, DocsRenderResult.Succeeded>();
        var experimentalWarningLogged = false;
        var hasErrors = false;

        foreach (var module in inputUris)
        {
            ArgumentHelper.ValidateBicepFile(module);
            var result = await runner.RenderAsync(
                module,
                arguments.NoRestore,
                arguments.DiagnosticsFormat,
                workspace,
                logExperimentalWarning: !experimentalWarningLogged,
                cancellationToken: cancellationToken);

            if (result is not DocsRenderResult.Succeeded success)
            {
                hasErrors = true;
                continue;
            }

            experimentalWarningLogged = true;
            successes.Add(module, success);
        }

        if (arguments.OutputToStdOut)
        {
            if (successes.Values.SingleOrDefault() is { } success)
            {
                await io.Output.Writer.WriteAsync(success.Contents.AsMemory(), cancellationToken);
            }
        }
        else if (successes.Count > 0)
        {
            var inputOutputPairs = argumentsResolver.ResolveFileSetInputOutputArguments(
                arguments,
                inputRoot,
                successes.Keys.ToArray(),
                (_, inputUri) => successes[inputUri].Configuration.Documentation.Data.Output.File);
            ValidateOutputPaths(inputOutputPairs);

            foreach (var (inputUri, outputUri) in inputOutputPairs)
            {
                var success = successes[inputUri];
                await writer.WriteToFileAsync(outputUri, success.Contents);
            }
        }

        return hasErrors ? 1 : 0;
    }

    private (IOUri RootUri, IReadOnlyList<IOUri> InputUris) ResolveInputs(
        DocsGenerateArguments arguments)
    {
        if (arguments.InputFile is not null)
        {
            var inputUri = argumentsResolver.ResolveInputArguments(arguments);
            return (inputUri.Resolve("."), [inputUri]);
        }

        if (arguments.FilePattern is not null)
        {
            var (rootUri, relativePaths) = argumentsResolver.ResolveFilePattern(arguments.FilePattern);
            return (rootUri, relativePaths.Select(rootUri.Resolve).ToArray());
        }

        throw new CommandLineException("Either the input file path or the --pattern parameter must be specified");
    }

    internal static Command CreateCommand(CommandLineBuilderContext context)
    {
        var command = new Command(
            Constants.Command.Docs,
            "[Experimental] Generates documentation for Bicep modules.");
        command.Add(CreateGenerateCommand(context));

        return command;
    }

    private static Command CreateGenerateCommand(CommandLineBuilderContext context)
    {
        var command = new Command(Constants.Command.DocsGenerate, "[Experimental] Generates documentation files for Bicep modules.")
        {
            TreatUnmatchedTokensAsErrors = true,
        };
        var inputFileArgument = new Argument<string?>(Constants.Argument.InputFile)
        {
            Description = "The path to an input .bicep file.",
            Arity = ArgumentArity.ZeroOrOne,
        };
        var stdoutOption = new Option<bool>(Option.Stdout)
        {
            Description = "Prints the generated documentation to stdout.",
        };
        var outDirOption = new Option<string?>(Option.OutDir)
        {
            Description = "Saves the generated README.md files beneath the specified directory.",
        };
        var outFileOption = new Option<string?>(Option.OutFile)
        {
            Description = "Saves the generated documentation as the specified file path.",
        };
        var patternOption = new Option<string?>(Option.Pattern)
        {
            Description = "Generates documentation for all files matching the glob pattern. Cannot be used with the input path.",
        };
        var noRestoreOption = new Option<bool>(Option.NoRestore)
        {
            Description = "Skips restoring external modules.",
        };
        var diagnosticsFormatOption = new Option<DiagnosticsFormat?>(Option.DiagnosticsFormat)
        {
            Description = "Sets the diagnostics format. Valid values are (Default, SARIF).",
        };

        command.Add(inputFileArgument);
        command.Add(stdoutOption);
        command.Add(outDirOption);
        command.Add(outFileOption);
        command.Add(patternOption);
        command.Add(noRestoreOption);
        command.Add(diagnosticsFormatOption);
        command.Validators.Add(result =>
        {
            CommandLineBuilderContext.ValidatePositionalArgument(result, inputFileArgument);
            if (result.GetValue(inputFileArgument) is not null && result.GetValue(patternOption) is not null)
            {
                result.AddError("The input path and --pattern parameter cannot both be specified.");
            }
        });

        command.SetAction((result, ct) => context.RunCommandAsync(async () =>
        {
            var outputDir = result.GetValue(outDirOption);
            var outputFile = result.GetValue(outFileOption);
            var filePattern = result.GetValue(patternOption);
            var outputToStdOut = result.GetValue(stdoutOption);
            ArgumentHelper.ValidateOutputOptions(outputToStdOut, outputDir, outputFile, filePattern);
            var arguments = new DocsGenerateArguments(
                result.GetValue(inputFileArgument),
                filePattern,
                outputToStdOut,
                outputDir,
                outputFile,
                result.GetValue(noRestoreOption),
                result.GetValue(diagnosticsFormatOption));

            return await context.GetCommand<DocsGenerateCommand>().RunAsync(arguments, ct);
        }));

        return command;
    }

    private static void ValidateOutputPaths(IReadOnlyList<(IOUri InputUri, IOUri OutputUri)> paths)
    {
        var outputUris = new HashSet<IOUri>();
        foreach (var (inputUri, outputUri) in paths)
        {
            if (inputUri.Equals(outputUri))
            {
                throw new CommandLineException("The documentation output path cannot overwrite the input Bicep file.");
            }

            if (outputUri.HasBicepExtension() || outputUri.HasBicepParamExtension())
            {
                throw new CommandLineException("Documentation output cannot use a Bicep source file extension.");
            }

            if (!outputUris.Add(outputUri))
            {
                throw new CommandLineException($"Multiple input files resolve to the output file \"{outputUri}\".");
            }
        }
    }
}
