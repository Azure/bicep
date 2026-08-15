// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Option = Bicep.Cli.Constants.Option;

namespace Bicep.Cli.Commands;

public class DocsOutputCommand(
    IOContext io,
    DocsModuleScanner moduleScanner,
    DocsCommandRunner runner,
    DiagnosticLogger diagnosticLogger) : ICommand
{
    public async Task<int> RunAsync(DocsOutputArguments arguments, CancellationToken cancellationToken = default)
    {
        var module = moduleScanner.ResolveModule(arguments.InputFile);
        ArgumentHelper.ValidateBicepFile(module);

        var aggregateSarif = arguments.DiagnosticsFormat is DiagnosticsFormat.Sarif;
        var result = await runner.RenderAsync(
            module,
            moduleScanner.ResolveOptionalFile(arguments.TemplateFile),
            moduleScanner.ResolveOptionalDirectory(arguments.TemplateRoot),
            DocsCommand.ParseCustomValues(arguments.CustomValues),
            arguments.NoRestore,
            arguments.DiagnosticsFormat,
            logDiagnostics: !aggregateSarif,
            cancellationToken: cancellationToken);

        if (aggregateSarif && result.CompilationResult is { } compilation)
        {
            var diagnostics = DocsCommand.MergeDiagnostics(
                [(result.SourceUri, compilation, result.DocumentationDiagnostic)]);
            diagnosticLogger.LogSarifDiagnostics(
                diagnostics.ByFile,
                diagnostics.Additional);
        }
        else if (aggregateSarif && result.DocumentationDiagnostic is { } documentationDiagnostic)
        {
            var diagnostics = DocsCommand.MergeDiagnostics(
                [(result.SourceUri, null, documentationDiagnostic)]);
            diagnosticLogger.LogSarifDiagnostics(
                diagnostics.ByFile,
                diagnostics.Additional);
        }

        if (result is not DocsRenderResult.Succeeded success)
        {
            return 1;
        }

        await io.Output.Writer.WriteAsync(success.Contents.AsMemory(), cancellationToken);
        return 0;
    }

    internal static System.CommandLine.Command CreateCommand(CommandLineBuilderContext context)
    {
        var command = new System.CommandLine.Command(Constants.Command.DocsOutput, "[Experimental] Renders documentation for one Bicep module to stdout.")
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
        var setOption = new System.CommandLine.Option<string[]>(Option.Set)
        {
            Description = "Supplies a custom template value in key=value form. May be repeated.",
            Arity = ArgumentArity.ZeroOrMore,
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
        command.Add(setOption);
        command.Add(noRestoreOption);
        command.Add(diagnosticsFormatOption);
        command.Validators.Add(result => CommandLineBuilderContext.ValidatePositionalArgument(result, inputFileArgument));

        command.SetAction((result, ct) => context.RunCommandAsync(async () =>
        {
            DocsCommand.ValidateSetOption(result, setOption);
            var customValues = result.GetValue(setOption);
            ArgumentNullException.ThrowIfNull(customValues);
            var arguments = new DocsOutputArguments(
                result.GetValue(inputFileArgument),
                result.GetValue(templateFileOption),
                result.GetValue(templateRootOption),
                [.. customValues],
                result.GetValue(noRestoreOption),
                result.GetValue(diagnosticsFormatOption));

            return await context.GetCommand<DocsOutputCommand>().RunAsync(arguments, ct);
        }));

        return command;
    }
}
