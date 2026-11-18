// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using Bicep.Cli.Constants;

namespace Bicep.Cli.Commands;

/// <summary>
/// Creates positional arguments shared by multiple commands, so that their names, arity, and help text stay consistent.
/// </summary>
internal static class CommonArguments
{
    /// <summary>
    /// Creates an optional input file argument, for commands that alternatively accept <c>--pattern</c>
    /// or report a missing input themselves.
    /// </summary>
    public static Argument<string?> OptionalInputFile(string description) => new(ArgumentNames.InputFile)
    {
        Description = description,
        Arity = ArgumentArity.ZeroOrOne,
    };

    public static Argument<string> ParametersFile() => new(ArgumentNames.ParametersFile)
    {
        Description = "The path to the .bicepparam file.",
    };
}
