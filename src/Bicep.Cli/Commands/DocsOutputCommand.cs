// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Services;
using Option = Bicep.Cli.Constants.Option;

namespace Bicep.Cli.Commands;

public class DocsOutputCommand(
    IOContext io,
    DocsModuleScanner moduleScanner,
    DocsCommandRunner runner) : ICommand
{
    public async Task<int> RunAsync(DocsOutputArguments arguments)
    {
        var module = moduleScanner.ResolveModule(arguments.InputFile);
        ArgumentHelper.ValidateBicepFile(module);

        var result = await runner.RenderAsync(
            module,
            arguments.Preset,
            moduleScanner.ResolveOptionalFile(arguments.TemplateFile),
            moduleScanner.ResolveOptionalDirectory(arguments.TemplateRoot),
            DocsCommand.ParseCustomValues(arguments.CustomValues),
            arguments.NoRestore,
            arguments.DiagnosticsFormat);

        if (!result.Success || result.Contents is null)
        {
            return 1;
        }

        await io.Output.Writer.WriteAsync(result.Contents);
        return 0;
    }

    internal static System.CommandLine.Command CreateCommand(CommandLineBuilderContext context)
    {
        var command = new System.CommandLine.Command(Constants.Command.DocsOutput, "Renders documentation for one Bicep module to stdout.")
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
                DocsCommand.ParsePreset(result.GetValue(presetOption)),
                result.GetValue(templateFileOption),
                result.GetValue(templateRootOption),
                [.. customValues],
                result.GetValue(noRestoreOption),
                result.GetValue(diagnosticsFormatOption));

            return await context.GetCommand<DocsOutputCommand>().RunAsync(arguments);
        }));

        return command;
    }
}
