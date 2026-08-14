// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Documentation;
using Bicep.Core.Exceptions;

namespace Bicep.Cli.Commands;

public static class DocsCommand
{
    internal static System.CommandLine.Command CreateCommand(CommandLineBuilderContext context)
    {
        var command = new System.CommandLine.Command(Constants.Command.Docs, "Generates documentation for Bicep modules.");
        command.Add(DocsGenerateCommand.CreateCommand(context));
        command.Add(DocsOutputCommand.CreateCommand(context));

        return command;
    }

    internal static BicepDocumentationPreset ParsePreset(string? value)
    {
        if (value is null || value.Equals("markdown", StringComparison.OrdinalIgnoreCase))
        {
            return BicepDocumentationPreset.Markdown;
        }

        throw new CommandLineException($"The preset \"{value}\" is not supported. The only supported preset is \"markdown\".");
    }

    internal static ImmutableSortedDictionary<string, string> ParseCustomValues(IEnumerable<string> values)
    {
        var customValues = ImmutableSortedDictionary.CreateBuilder<string, string>(StringComparer.Ordinal);

        foreach (var value in values)
        {
            var separatorIndex = value.IndexOf('=');
            if (separatorIndex <= 0)
            {
                throw new CommandLineException($"The --set value \"{value}\" must use the format key=value.");
            }

            var key = value[..separatorIndex];
            if (!customValues.TryAdd(key, value[(separatorIndex + 1)..]))
            {
                throw new CommandLineException($"The --set key \"{key}\" cannot be specified more than once.");
            }
        }

        return customValues.ToImmutable();
    }

    internal static void ValidateSetOption(
        System.CommandLine.ParseResult result,
        System.CommandLine.Option<string[]> setOption)
    {
        if (result.GetResult(setOption) is { Implicit: false, Tokens.Count: 0 })
        {
            throw new CommandLineException("The --set parameter expects a key=value argument.");
        }
    }
}
