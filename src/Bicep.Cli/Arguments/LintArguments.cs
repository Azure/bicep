// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Cli.Helpers;

namespace Bicep.Cli.Arguments;

public class LintArguments : ArgumentsBase, IFilePatternInputArguments
{
    public LintArguments(string[] args)
        : base(Constants.Command.Lint)
    {
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLowerInvariant())
            {
                case "--no-restore":
                    NoRestore = true;
                    break;

                case ArgumentConstants.DiagnosticsFormat:
                    ArgumentHelper.ValidateNotAlreadySet(ArgumentConstants.DiagnosticsFormat, DiagnosticsFormat);
                    DiagnosticsFormat = ArgumentHelper.ToDiagnosticsFormat(ArgumentHelper.GetValueWithValidation(ArgumentConstants.DiagnosticsFormat, args, i));
                    i++;
                    break;

                case ArgumentConstants.FilePattern:
                    ArgumentHelper.ValidateNotAlreadySet(ArgumentConstants.FilePattern, FilePattern);
                    FilePattern = ArgumentHelper.GetValueWithValidation(ArgumentConstants.FilePattern, args, i);
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

        if (InputFile is null && FilePattern is null)
        {
            throw new CommandLineException($"Either the input file path or the {ArgumentConstants.FilePattern} parameter must be specified");
        }

        if (FilePattern != null)
        {
            if (InputFile is not null)
            {
                throw new CommandLineException($"The input file path and the {ArgumentConstants.FilePattern} parameter cannot both be specified");
            }
        }

        DiagnosticsFormat ??= Arguments.DiagnosticsFormat.Default;
    }

    public string? InputFile { get; }

    public string? FilePattern { get; }

    public DiagnosticsFormat? DiagnosticsFormat { get; }

    public bool NoRestore { get; }
}
