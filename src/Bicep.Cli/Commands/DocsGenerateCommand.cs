// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.CommandLine;
using System.IO.Abstractions;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Exceptions;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Option = Bicep.Cli.Constants.Option;

namespace Bicep.Cli.Commands;

public class DocsGenerateCommand(
    IOContext io,
    InputOutputArgumentsResolver argumentsResolver,
    DocsCommandRunner runner,
    OutputWriter writer,
    DiagnosticLogger diagnosticLogger,
    IFileSystem fileSystem) : ICommand
{
    public async Task<int> RunAsync(DocsGenerateArguments arguments, CancellationToken cancellationToken = default)
    {
        var (inputRoot, inputUris) = ResolveInputs(arguments);
        var workspace = new ActiveSourceFileSet();
        var successes = new Dictionary<Bicep.IO.Abstraction.IOUri, DocsRenderResult.Succeeded>();
        var sarifResults = new List<(
            Bicep.IO.Abstraction.IOUri SourceUri,
            Compilation? Compilation,
            Bicep.Core.Diagnostics.IDiagnostic? DocumentationDiagnostic)>();
        var aggregateSarif = arguments.DiagnosticsFormat is DiagnosticsFormat.Sarif;
        var experimentalWarningLogged = false;
        var hasErrors = false;

        foreach (var module in inputUris)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentHelper.ValidateBicepFile(module);
            var result = await runner.RenderAsync(
                module,
                arguments.TemplateFile,
                arguments.TemplateRoot,
                arguments.CustomValues,
                arguments.NoRestore,
                arguments.DiagnosticsFormat,
                workspace,
                logExperimentalWarning: !experimentalWarningLogged,
                logDiagnostics: !aggregateSarif,
                cancellationToken: cancellationToken);

            if (result.CompilationResult is { } compilation)
            {
                sarifResults.Add((result.SourceUri, compilation, result.DocumentationDiagnostic));
            }
            else if (aggregateSarif)
            {
                sarifResults.Add((result.SourceUri, null, result.DocumentationDiagnostic));
            }

            if (result is not DocsRenderResult.Succeeded success)
            {
                if (result.DocumentationDiagnostic?.Code == DocsCommand.RenderFailureCode)
                {
                    experimentalWarningLogged = true;
                }

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
            DocsCommand.ValidateOutputPaths(inputOutputPairs);

            foreach (var (inputUri, outputUri) in inputOutputPairs)
            {
                var success = successes[inputUri];
                try
                {
                    await writer.WriteToFileAsync(outputUri, success.Contents);
                }
                catch (BicepException exception)
                {
                    if (aggregateSarif)
                    {
                        sarifResults.Add((
                            success.SourceUri,
                            success.Compilation,
                            DocsCommand.CreateDiagnostic(DocsCommand.WriteFailureCode, exception.Message)));
                    }
                    else
                    {
                        await io.Error.Writer.WriteLineAsync(exception.Message);
                    }

                    hasErrors = true;
                }
            }
        }

        if (aggregateSarif && sarifResults.Count > 0)
        {
            var diagnostics = DocsCommand.MergeDiagnostics(sarifResults);
            diagnosticLogger.LogSarifDiagnostics(
                diagnostics.ByFile,
                diagnostics.Additional);
        }

        return hasErrors ? 1 : 0;
    }

    private (Bicep.IO.Abstraction.IOUri RootUri, IReadOnlyList<Bicep.IO.Abstraction.IOUri> InputUris) ResolveInputs(
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

    private ImmutableSortedDictionary<string, string> ParseCustomValues(
        System.CommandLine.ParseResult result,
        System.CommandLine.Option<string[]> customTemplateValueOption,
        System.CommandLine.Option<string[]> customTemplateValueFilePathOption) =>
        DocsCommand.ParseCustomValues(
            result,
            customTemplateValueOption,
            customTemplateValueFilePathOption,
            fileSystem);

    internal static System.CommandLine.Command CreateCommand(CommandLineBuilderContext context)
    {
        var command = new System.CommandLine.Command(Constants.Command.DocsGenerate, "[Experimental] Generates documentation files for Bicep modules.")
        {
            TreatUnmatchedTokensAsErrors = true,
        };
        var inputFileArgument = new System.CommandLine.Argument<string?>(Constants.Argument.InputFile)
        {
            Description = "The path to an input .bicep file.",
            Arity = ArgumentArity.ZeroOrOne,
        };
        var stdoutOption = new System.CommandLine.Option<bool>(Option.Stdout)
        {
            Description = "Prints the generated documentation to stdout.",
        };
        var templateFileOption = new System.CommandLine.Option<string?>(Option.TemplateFile)
        {
            Description = "Uses a custom Scriban template file.",
        };
        var templateRootOption = new System.CommandLine.Option<string?>(Option.TemplateRoot)
        {
            Description = "Sets the root directory for template includes. Defaults to the module directory.",
        };
        var customTemplateValueOption = new System.CommandLine.Option<string[]>(Option.CustomTemplateValue)
        {
            Description = "Supplies a custom template value in key=value form. May be repeated.",
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = false,
        };
        var customTemplateValueFilePathOption = new System.CommandLine.Option<string[]>(Option.CustomTemplateValueFilePath)
        {
            Description = "Loads custom template string values from a JSON object file. May be repeated.",
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = false,
        };
        var outDirOption = new System.CommandLine.Option<string?>(Option.OutDir)
        {
            Description = "Saves the generated README.md files beneath the specified directory.",
        };
        var outFileOption = new System.CommandLine.Option<string?>(Option.OutFile)
        {
            Description = "Saves the generated documentation as the specified file path.",
        };
        var patternOption = new System.CommandLine.Option<string?>(Option.Pattern)
        {
            Description = "Generates documentation for all files matching the glob pattern. Cannot be used with the input path.",
        };
        var noRestoreOption = new System.CommandLine.Option<bool>(Option.NoRestore)
        {
            Description = "Skips restoring external modules.",
        };
        var diagnosticsFormatOption = new System.CommandLine.Option<DiagnosticsFormat?>(Option.DiagnosticsFormat)
        {
            Description = "Sets the diagnostics format. Valid values are (Default, SARIF).",
        };

        command.Add(inputFileArgument);
        command.Add(stdoutOption);
        command.Add(templateFileOption);
        command.Add(templateRootOption);
        command.Add(customTemplateValueOption);
        command.Add(customTemplateValueFilePathOption);
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
            var handler = context.GetCommand<DocsGenerateCommand>();
            var customValues = handler.ParseCustomValues(
                result,
                customTemplateValueOption,
                customTemplateValueFilePathOption);
            var outputDir = result.GetValue(outDirOption);
            var outputFile = result.GetValue(outFileOption);
            var filePattern = result.GetValue(patternOption);
            var outputToStdOut = result.GetValue(stdoutOption);
            ArgumentHelper.ValidateOutputOptions(outputToStdOut, outputDir, outputFile, filePattern);
            var arguments = new DocsGenerateArguments(
                result.GetValue(inputFileArgument),
                filePattern,
                result.GetValue(templateFileOption),
                result.GetValue(templateRootOption),
                customValues,
                outputToStdOut,
                outputDir,
                outputFile,
                result.GetValue(noRestoreOption),
                result.GetValue(diagnosticsFormatOption));

            return await handler.RunAsync(arguments, ct);
        }));

        return command;
    }
}
