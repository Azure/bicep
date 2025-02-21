// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Cli.Helpers;

namespace Bicep.Cli.Arguments;

public class RestoreArguments : ArgumentsBase
{
    public RestoreArguments(string[] args) : base(Constants.Command.Restore)
    {
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLowerInvariant())
            {
                case "--force":
                    ForceModulesRestore = true;
                    break;

                case ArgumentConstants.FilePatternRoot:
                    ArgumentHelper.ValidateNotAlreadySet(ArgumentConstants.FilePatternRoot, FilePatternRoot);
                    FilePatternRoot = ArgumentHelper.GetDirectoryPathWithValidation(ArgumentConstants.FilePatternRoot, args, i);
                    i++;
                    break;

                case ArgumentConstants.FilePattern:
                    var value = ArgumentHelper.GetValueWithValidation(ArgumentConstants.FilePattern, args, i);
                    FilePatterns = FilePatterns.Add(value);
                    i++;
                    break;

                default:
                    if (args[i].StartsWith("--"))
                    {
                        throw new CommandLineException($"Unrecognized parameter \"{args[i]}\"");
                    }
                    if (InputFile is not null)
                    {
                        throw new CommandLineException($"The input file path cannot be specified multiple times");
                    }
                    InputFile = args[i];
                    break;
            }
        }

        if (InputFile is null && FilePatterns.IsEmpty)
        {
            throw new CommandLineException($"Either the input file path or the {ArgumentConstants.FilePattern} parameter must be specified");
        }

        if (!FilePatterns.IsEmpty)
        {
            if (InputFile is not null)
            {
                throw new CommandLineException($"The input file path and the {ArgumentConstants.FilePattern} parameter cannot both be specified");
            }
        }
    }

    public string? InputFile { get; }

    public string? FilePatternRoot { get; }

    public ImmutableArray<string> FilePatterns { get; } = [];

    public bool ForceModulesRestore { get; }
}
