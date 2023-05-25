// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using Bicep.Core.FileSystem;

namespace Bicep.Cli.Arguments;

public class LintArguments : ArgumentsBase
{
    public LintArguments(string[] args)
        : base(Constants.Command.Lint)
    {
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLowerInvariant())
            {
                case "--ignore-warnings":
                    IgnoreWarnings = true;
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

        if (InputFile is null)
        {
            throw new CommandLineException($"The input file path was not specified");
        }
    }

    public string InputFile { get; }

    public bool IgnoreWarnings { get; }
}
