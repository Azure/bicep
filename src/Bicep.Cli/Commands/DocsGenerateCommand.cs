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
    DocsModuleScanner moduleScanner,
    DocsCommandRunner runner,
    IDocsFileWriter writer,
    DiagnosticLogger diagnosticLogger,
    IFileSystem fileSystem) : ICommand
{
    public async Task<int> RunAsync(DocsGenerateArguments arguments, CancellationToken cancellationToken = default)
    {
        var modules = moduleScanner.ResolveModules(arguments);
        var inputOutputPairs = moduleScanner.ResolveOutputFiles(modules, arguments.OutputFile);
        var templateFile = moduleScanner.ResolveOptionalFile(arguments.TemplateFile);
        var templateRoot = moduleScanner.ResolveOptionalDirectory(arguments.TemplateRoot);
        var customValues = arguments.CustomValues;
        var workspace = new ActiveSourceFileSet();
        var sarifResults = new List<(
            Bicep.IO.Abstraction.IOUri SourceUri,
            Compilation? Compilation,
            Bicep.Core.Diagnostics.IDiagnostic? DocumentationDiagnostic)>();
        var aggregateSarif = arguments.DiagnosticsFormat is DiagnosticsFormat.Sarif;
        var experimentalWarningLogged = false;
        var hasErrors = false;

        foreach (var (module, outputUri) in inputOutputPairs)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentHelper.ValidateBicepFile(module);
            var result = await runner.RenderAsync(
                module,
                templateFile,
                templateRoot,
                customValues,
                arguments.NoRestore,
                arguments.DiagnosticsFormat,
                workspace,
                logExperimentalWarning: !experimentalWarningLogged,
                logDiagnostics: !aggregateSarif,
                cancellationToken: cancellationToken);

            if (result.CompilationResult is { } compilation)
            {
                sarifResults.Add((result.SourceUri, compilation, result.DocumentationDiagnostic));
                experimentalWarningLogged = true;
            }
            else if (aggregateSarif)
            {
                sarifResults.Add((result.SourceUri, null, result.DocumentationDiagnostic));
            }

            if (result is not DocsRenderResult.Succeeded success)
            {
                hasErrors = true;
                continue;
            }

            try
            {
                await writer.WriteAsync(outputUri, success.Contents, cancellationToken);
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

        if (aggregateSarif && sarifResults.Count > 0)
        {
            var diagnostics = DocsCommand.MergeDiagnostics(sarifResults);
            diagnosticLogger.LogSarifDiagnostics(
                diagnostics.ByFile,
                diagnostics.Additional);
        }

        return hasErrors ? 1 : 0;
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
            Description = "The path to a .bicep file or module directory. Defaults to the current directory.",
            Arity = ArgumentArity.ZeroOrOne,
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
        var outputFileOption = new System.CommandLine.Option<string?>(Option.OutputFile)
        {
            Description = "Sets the output file name without a directory or Bicep source extension. Defaults to README.md.",
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
        command.Add(templateFileOption);
        command.Add(templateRootOption);
        command.Add(customTemplateValueOption);
        command.Add(customTemplateValueFilePathOption);
        command.Add(outputFileOption);
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
            var arguments = new DocsGenerateArguments(
                result.GetValue(inputFileArgument),
                result.GetValue(patternOption),
                result.GetValue(templateFileOption),
                result.GetValue(templateRootOption),
                customValues,
                result.GetValue(outputFileOption) ?? "README.md",
                result.GetValue(noRestoreOption),
                result.GetValue(diagnosticsFormatOption));

            return await handler.RunAsync(arguments, ct);
        }));

        return command;
    }
}
