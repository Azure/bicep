// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using Bicep.Cli.Arguments;
using Bicep.Cli.Constants;

namespace Bicep.Cli.Commands;

/// <summary>
/// Creates options used by more than one command, so that their names, types, and help text stay consistent.
/// Options used by a single command should be created inline in that command instead.
/// Descriptions are only parameterized where they legitimately differ per command.
/// </summary>
internal static class CommonOptions
{
    // Input selection

    public static Option<string?> Pattern(string description) => new(OptionNames.Pattern)
    {
        Description = description,
    };

    // Output

    public static Option<bool> Stdout(string description = "Prints the output to stdout.") => new(OptionNames.Stdout)
    {
        Description = description,
    };

    public static Option<string?> OutDir(string description = "Saves the output at the specified directory.") => new(OptionNames.OutDir)
    {
        Description = description,
    };

    public static Option<string?> OutFile(string description = "Saves the output as the specified file path.") => new(OptionNames.OutFile)
    {
        Description = description,
    };

    public static Option<bool> Force(string description) => new(OptionNames.Force)
    {
        Description = description,
    };

    // Compilation

    /// <summary>
    /// Creates the <c>--config</c> option. Pass its value to <see cref="CommandLineBuilderContext.RunCommandAsync(string?, Func{Task{int}})"/>.
    /// </summary>
    public static Option<string?> Config() => new(OptionNames.Config)
    {
        Description = "Uses the specified configuration file instead of discovering bicepconfig.json files.",
    };

    public static Option<bool> NoRestore(string description) => new(OptionNames.NoRestore)
    {
        Description = description,
    };

    public static Option<DiagnosticsFormat?> DiagnosticsFormat() => new(OptionNames.DiagnosticsFormat)
    {
        Description = "Sets the diagnostics format. Valid values are (Default, SARIF).",
    };

    // Deployment

    public static Option<DeploymentOutputFormat?> DeploymentOutputFormat() => new(OptionNames.Format)
    {
        Description = "Output format for deployment results (Default, Json).",
    };
}
