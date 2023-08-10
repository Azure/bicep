// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using Bicep.Cli.Helpers;
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
                case "--no-restore":
                    NoRestore = true;
                    break;

                case "--diagnostics-format":
                    if (args.Length == i + 1)
                    {
                        throw new CommandLineException($"The --diagnostics-format parameter expects an argument");
                    }
                    if (DiagnosticsFormat is not null)
                    {
                        throw new CommandLineException($"The --diagnostics-format parameter cannot be specified twice");
                    }
                    DiagnosticsFormat = ArgumentHelper.ToDiagnosticsFormat(args[i + 1]);
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

        if (InputFile is null)
        {
            throw new CommandLineException($"The input file path was not specified");
        }

        if (DiagnosticsFormat is null)
        {
            DiagnosticsFormat = Arguments.DiagnosticsFormat.Default;
        }
    }

    public string InputFile { get; }

    public DiagnosticsFormat? DiagnosticsFormat { get; }

    public bool NoRestore { get; }
}
