// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Services;
using Bicep.Core.Exceptions;
using Option = Bicep.Cli.Constants.Option;

namespace Bicep.Cli.Commands;

public class DocsGenerateCommand(
    IOContext io,
    DocsModuleScanner moduleScanner,
    DocsCommandRunner runner,
    OutputWriter writer) : ICommand
{
    public async Task<int> RunAsync(DocsGenerateArguments arguments)
    {
        var modules = moduleScanner.ResolveModules(arguments);
        var inputOutputPairs = moduleScanner.ResolveOutputFiles(modules, arguments.OutputFile);
        var templateFile = moduleScanner.ResolveOptionalFile(arguments.TemplateFile);
        var templateRoot = moduleScanner.ResolveOptionalDirectory(arguments.TemplateRoot);
        var customValues = DocsCommand.ParseCustomValues(arguments.CustomValues);
        var hasErrors = false;

        foreach (var (module, outputUri) in inputOutputPairs)
        {
            ArgumentHelper.ValidateBicepFile(module);
            var result = await runner.RenderAsync(
                module,
                arguments.Preset,
                templateFile,
                templateRoot,
                customValues,
                arguments.NoRestore,
                arguments.DiagnosticsFormat);

            if (!result.Success || result.Contents is null)
            {
                hasErrors = true;
                continue;
            }

            try
            {
                await writer.WriteToFileAtomicallyAsync(outputUri, result.Contents);
            }
            catch (BicepException exception)
            {
                await io.Error.Writer.WriteLineAsync(exception.Message);
                hasErrors = true;
            }
        }

        return hasErrors ? 1 : 0;
    }

    internal static System.CommandLine.Command CreateCommand(CommandLineBuilderContext context)
    {
        var command = new System.CommandLine.Command(Constants.Command.DocsGenerate, "Generates documentation files for Bicep modules.")
        {
            TreatUnmatchedTokensAsErrors = true,
        };
        var inputFileArgument = new System.CommandLine.Argument<string?>(Constants.Argument.InputFile)
        {
            Description = "The path to a .bicep file or module directory. Defaults to the current directory.",
            Arity = ArgumentArity.ZeroOrOne,
        };
        var presetOption = new System.CommandLine.Option<string?>(Option.Preset)
        {
            Description = "Selects a built-in preset. The only supported value is markdown.",
        };
        var templateFileOption = new System.CommandLine.Option<string?>(Option.TemplateFile)
        {
            Description = "Uses a custom Scriban template file.",
        };
        var templateRootOption = new System.CommandLine.Option<string?>(Option.TemplateRoot)
        {
            Description = "Sets the root directory for template includes.",
        };
        var setOption = new System.CommandLine.Option<string[]>(Option.Set)
        {
            Description = "Supplies a custom template value in key=value form. May be repeated.",
            Arity = ArgumentArity.ZeroOrMore,
        };
        var outputFileOption = new System.CommandLine.Option<string?>(Option.OutputFile)
        {
            Description = "Sets the output file name. Defaults to README.md.",
        };
        var patternOption = new System.CommandLine.Option<string?>(Option.Pattern)
        {
            Description = "Generates documentation for all files matching the glob pattern.",
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
        command.Add(presetOption);
        command.Add(templateFileOption);
        command.Add(templateRootOption);
        command.Add(setOption);
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
            DocsCommand.ValidateSetOption(result, setOption);
            var customValues = result.GetValue(setOption);
            ArgumentNullException.ThrowIfNull(customValues);
            var arguments = new DocsGenerateArguments(
                result.GetValue(inputFileArgument),
                result.GetValue(patternOption),
                DocsCommand.ParsePreset(result.GetValue(presetOption)),
                result.GetValue(templateFileOption),
                result.GetValue(templateRootOption),
                [.. customValues],
                result.GetValue(outputFileOption) ?? "README.md",
                result.GetValue(noRestoreOption),
                result.GetValue(diagnosticsFormatOption));

            return await context.GetCommand<DocsGenerateCommand>().RunAsync(arguments);
        }));

        return command;
    }
}
