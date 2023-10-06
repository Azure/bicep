// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using Bicep.Core.FileSystem;

namespace Bicep.Cli.Arguments
{
    public class TestArguments : ArgumentsBase
    {
        public TestArguments(string[] args) : base(Constants.Command.Test)
        {
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "--stdout":
                        OutputToStdOut = true;
                        break;

                    case "--no-restore":
                        NoRestore = true;
                        break;

                    case "--outdir":
                        if (args.Length == i + 1)
                        {
                            throw new CommandLineException($"The --outdir parameter expects an argument");
                        }
                        if (OutputDir is not null)
                        {
                            throw new CommandLineException($"The --outdir parameter cannot be specified twice");
                        }
                        OutputDir = args[i + 1];
                        i++;
                        break;

                    case "--outfile":
                        if (args.Length == i + 1)
                        {
                            throw new CommandLineException($"The --outfile parameter expects an argument");
                        }
                        if (OutputFile is not null)
                        {
                            throw new CommandLineException($"The --outfile parameter cannot be specified twice");
                        }
                        OutputFile = args[i + 1];
                        i++;
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
                        DiagnosticsFormat = ToDiagnosticsFormat(args[i + 1]);
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

            if (OutputToStdOut && OutputDir is not null)
            {
                throw new CommandLineException($"The --outdir and --stdout parameters cannot both be used");
            }

            if (OutputToStdOut && OutputFile is not null)
            {
                throw new CommandLineException($"The --outfile and --stdout parameters cannot both be used");
            }

            if (OutputDir is not null && OutputFile is not null)
            {
                throw new CommandLineException($"The --outdir and --outfile parameters cannot both be used");
            }

            if (OutputDir is not null)
            {
                var outputDir = PathHelper.ResolvePath(OutputDir);

                if (!Directory.Exists(outputDir))
                {
                    throw new CommandLineException(string.Format(CliResources.DirectoryDoesNotExistFormat, outputDir));
                }
            }

            if (DiagnosticsFormat is null)
            {
                DiagnosticsFormat = Arguments.DiagnosticsFormat.Default;
            }
        }

        private static DiagnosticsFormat ToDiagnosticsFormat(string? format)
        {
            if (format is null || (format is not null && format.Equals("default", StringComparison.OrdinalIgnoreCase)))
            {
                return Arguments.DiagnosticsFormat.Default;
            }
            else if (format is not null && format.Equals("sarif", StringComparison.OrdinalIgnoreCase))
            {
                return Arguments.DiagnosticsFormat.Sarif;
            }

            throw new ArgumentException($"Unrecognized diagnostics format {format}");
        }

        public bool OutputToStdOut { get; }

        public string InputFile { get; }

        public string? OutputDir { get; }

        public string? OutputFile { get; }

        public DiagnosticsFormat? DiagnosticsFormat { get; }

        public bool NoRestore { get; }
    }
}
