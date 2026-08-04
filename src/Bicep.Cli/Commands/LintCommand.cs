// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using Bicep.Cli.Arguments;
using Bicep.Cli.Constants;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Option = Bicep.Cli.Constants.Option;

namespace Bicep.Cli.Commands;

public class LintCommand(
    ILogger logger,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler,
    InputOutputArgumentsResolver inputOutputArgumentsResolver,
    IConfigurationManager configurationManager) : ICommand
{
    public async Task<int> RunAsync(LintArguments args)
    {
        var hasErrors = false;

        foreach (var inputUri in inputOutputArgumentsResolver.ResolveFilePatternInputArguments(args))
        {
            ArgumentHelper.ValidateBicepOrBicepParamFile(inputUri);

            var result = await Lint(inputUri, args.NoRestore, args.DiagnosticsFormat, args.Config);
            hasErrors |= result.HasErrors;
        }

        return CommandHelper.GetExitCode(new(hasErrors));
    }

    private async Task<DiagnosticSummary> Lint(IOUri inputUri, bool noRestore, DiagnosticsFormat? diagnosticsFormat, string? config)
    {

        var lintCompiler = GetCompiler(config);

        var compilation = await lintCompiler.CreateCompilation(inputUri, skipRestore: noRestore);

        CommandHelper.LogExperimentalWarning(logger, compilation);

        var summary = diagnosticLogger.LogDiagnostics(ArgumentHelper.GetDiagnosticOptions(diagnosticsFormat) with { SarifToStdout = true }, compilation);

        return summary;
    }

    internal static System.CommandLine.Command CreateCommand(CommandLineBuilderContext context)
    {
        var command = new System.CommandLine.Command(Constants.Command.Lint, "Lints a .bicep file.");

        var inputFileArgument = new System.CommandLine.Argument<string?>(Constants.Argument.InputFile)
        {
            Description = "The path to the .bicep file.",
            Arity = ArgumentArity.ZeroOrOne,
        };
        var filePatternOption = new System.CommandLine.Option<string?>(Option.Pattern)
        {
            Description = "Lints all files matching the specified glob pattern.",
        };
        var noRestoreOption = new System.CommandLine.Option<bool>(Option.NoRestore)
        {
            Description = "Skips restoring external modules.",
        };
        var diagnosticsFormatOption = new System.CommandLine.Option<DiagnosticsFormat?>(Option.DiagnosticsFormat)
        {
            Description = "Sets the diagnostics format. Valid values are (Default, SARIF).",
        };
        var configOption = new System.CommandLine.Option<string?>(Option.Config)
        {
            Description = "Specifies the path to a bicepconfig.json file to use.",
        };

        command.Add(inputFileArgument);
        command.Add(filePatternOption);
        command.Add(noRestoreOption);
        command.Add(diagnosticsFormatOption);
        command.Add(configOption);
        command.Validators.Add((System.CommandLine.Parsing.CommandResult result) => CommandLineBuilderContext.ValidatePositionalArgument(result, inputFileArgument));

        command.SetAction((result, ct) => context.RunCommandAsync(async () =>
        {
            var diagnosticsFormat = result.GetValue(diagnosticsFormatOption) ?? Arguments.DiagnosticsFormat.Default;
            var args = new LintArguments(
                result.GetValue(inputFileArgument),
                result.GetValue(filePatternOption),
                diagnosticsFormat,
                result.GetValue(noRestoreOption),
                result.GetValue(configOption));

            return await context.GetCommand<LintCommand>().RunAsync(args);
        }));

        return command;
    }

    private BicepCompiler GetCompiler(string? config)
    {
        if (config is null)
        {
            return compiler;
        }

        var configUri = IOUri.FromFilePath(config);

        var configuration = configurationManager.LoadConfiguration(configUri);

        return BicepCompiler.Create(services =>
        {
            services.AddSingleton<IConfigurationManager>(
                IConfigurationManager.WithStaticConfiguration(configuration));
        });
    }
}
