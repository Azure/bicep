// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.CommandLine;
using System.IO.Abstractions;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Option = Bicep.Cli.Constants.Option;

namespace Bicep.Cli.Commands;

public class DocsOutputCommand(
    IOContext io,
    InputOutputArgumentsResolver argumentsResolver,
    DocsCommandRunner runner,
    DiagnosticLogger diagnosticLogger,
    IFileSystem fileSystem) : ICommand
{
    public async Task<int> RunAsync(DocsOutputArguments arguments, CancellationToken cancellationToken = default)
    {
        var configuration = arguments.ConfigFilePath is not null
            ? DocsConfigurationLoader.Load(arguments.ConfigFilePath, argumentsResolver, fileSystem)
            : null;
        var targetDirectory = DocsConfigurationLoader.ResolveTargetDirectory(
            arguments.InputFile,
            filePattern: null,
            argumentsResolver,
            fileSystem);
        configuration ??= DocsConfigurationLoader.Discover(
            targetDirectory,
            argumentsResolver,
            fileSystem);
        var inputs = DocsConfigurationLoader.ResolveInputs(
            arguments.InputFile,
            filePattern: null,
            targetDirectory,
            configuration,
            argumentsResolver);
        if (inputs.InputUris.Count != 1)
        {
            throw new CommandLineException(
                $"The docs output command requires exactly one input file, but the docs configuration selected {inputs.InputUris.Count}.");
        }

        var module = inputs.InputUris[0];
        ArgumentHelper.ValidateBicepFile(module);

        var aggregateSarif = arguments.DiagnosticsFormat is DiagnosticsFormat.Sarif;
        var result = await runner.RenderAsync(
            module,
            DocsConfigurationLoader.ResolveTemplateFile(
                arguments.TemplateFile,
                configuration,
                argumentsResolver,
                fileSystem),
            DocsConfigurationLoader.ResolveTemplateRoot(
                arguments.TemplateRoot,
                configuration,
                argumentsResolver,
                fileSystem),
            DocsConfigurationLoader.MergeCustomValues(configuration, arguments.CustomValues),
            configuration.Configuration.Examples,
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
        var command = new System.CommandLine.Command(Constants.Command.DocsOutput, "[Experimental] Renders documentation for one Bicep module to stdout.")
        {
            TreatUnmatchedTokensAsErrors = true,
        };
        var inputFileArgument = new System.CommandLine.Argument<string?>(Constants.Argument.InputFile)
        {
            Description = "The path to an input .bicep file or module directory.",
            Arity = ArgumentArity.ZeroOrOne,
        };
        var configFilePathOption = new System.CommandLine.Option<string?>(Option.ConfigFilePath)
        {
            Description = "Loads docs configuration from a JSON file.",
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
        var noRestoreOption = new System.CommandLine.Option<bool>(Option.NoRestore)
        {
            Description = "Skips restoring external modules.",
        };
        var diagnosticsFormatOption = new System.CommandLine.Option<DiagnosticsFormat?>(Option.DiagnosticsFormat)
        {
            Description = "Sets the diagnostics format. Valid values are (Default, SARIF).",
        };

        command.Add(inputFileArgument);
        command.Add(configFilePathOption);
        command.Add(templateFileOption);
        command.Add(templateRootOption);
        command.Add(customTemplateValueOption);
        command.Add(customTemplateValueFilePathOption);
        command.Add(noRestoreOption);
        command.Add(diagnosticsFormatOption);
        command.Validators.Add(result => CommandLineBuilderContext.ValidatePositionalArgument(result, inputFileArgument));

        command.SetAction((result, ct) => context.RunCommandAsync(async () =>
        {
            var handler = context.GetCommand<DocsOutputCommand>();
            var customValues = handler.ParseCustomValues(
                result,
                customTemplateValueOption,
                customTemplateValueFilePathOption);
            var arguments = new DocsOutputArguments(
                result.GetValue(inputFileArgument),
                result.GetValue(configFilePathOption),
                result.GetValue(templateFileOption),
                result.GetValue(templateRootOption),
                customValues,
                result.GetValue(noRestoreOption),
                result.GetValue(diagnosticsFormatOption));

            return await handler.RunAsync(arguments, ct);
        }));

        return command;
    }
}
