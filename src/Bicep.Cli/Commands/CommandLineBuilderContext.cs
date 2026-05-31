// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.CommandLine;
using Bicep.Cli.Arguments;
using Bicep.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Cli.Commands;

public class CommandLineBuilderContext(IServiceProvider services, IOContext io)
{
    public TCommand GetCommand<TCommand>() where TCommand : class, ICommand
        => services.GetRequiredService<TCommand>();

    public async Task<int> RunCommandAsync(Func<Task<int>> action)
    {
        try
        {
            return await action();
        }
        catch (BicepException exception)
        {
            await io.Error.Writer.WriteLineAsync(exception.Message);
            return 1;
        }
    }

    public static void ValidatePositionalArgument(System.CommandLine.Parsing.CommandResult result, System.CommandLine.Argument<string?> argument)
    {
        if (result.GetValue(argument) is { } inputValue && inputValue.StartsWith("--", StringComparison.Ordinal))
        {
            result.AddError($"Unrecognized parameter \"{inputValue}\"");
        }
    }

    public static void ValidateRequiredPositionalArgument(System.CommandLine.Parsing.CommandResult result, System.CommandLine.Argument<string> argument)
    {
        if (result.GetValue(argument) is { } inputValue && inputValue.StartsWith("--", StringComparison.Ordinal))
        {
            result.AddError($"Unrecognized parameter \"{inputValue}\"");
        }
    }

    public static ImmutableDictionary<string, string> ParseAdditionalArguments(IReadOnlyList<string> unmatchedTokens)
    {
        var additionalArguments = new Dictionary<string, string>();
        for (var i = 0; i < unmatchedTokens.Count; i++)
        {
            var token = unmatchedTokens[i];
            if (token.StartsWith(ArgumentConstants.CliArgPrefix, StringComparison.OrdinalIgnoreCase))
            {
                var key = token[ArgumentConstants.CliArgPrefix.Length..];
                if (additionalArguments.ContainsKey(key))
                {
                    throw new CommandLineException($"Parameter \"{token}\" cannot be specified multiple times.");
                }

                if (i + 1 >= unmatchedTokens.Count)
                {
                    throw new CommandLineException($"Parameter \"{token}\" requires a value.");
                }

                additionalArguments[key] = unmatchedTokens[++i];
            }
            else
            {
                throw new CommandLineException($"Unrecognized parameter \"{token}\"");
            }
        }

        return additionalArguments.ToImmutableDictionary();
    }
}
