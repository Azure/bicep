// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Cli.Helpers;
using Bicep.Core;

namespace Bicep.Cli.Arguments;

public class BuildParamsArguments : ArgumentsBase, IFilePatternInputOutputArguments<BuildParamsArguments>
{
    public BuildParamsArguments(string[] args) : base(Constants.Command.BuildParams)
    {
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLowerInvariant())
            {
                case "--bicep-file":
                    if (args.Length == i + 1)
                    {
                        throw new CommandLineException($"The --bicep-file parameter expects an argument");
                    }
                    if (BicepFile is not null)
                    {
                        throw new CommandLineException($"The --bicep-file parameter cannot be specified twice");
                    }
                    BicepFile = args[i + 1];
                    i++;
                    break;

                case ArgumentConstants.OutDir:
                    ArgumentHelper.ValidateNotAlreadySet(ArgumentConstants.OutDir, OutputDir);
                    OutputDir = ArgumentHelper.GetValueWithValidation(ArgumentConstants.OutDir, args, i);
                    i++;
                    break;

                case ArgumentConstants.OutFile:
                    ArgumentHelper.ValidateNotAlreadySet(ArgumentConstants.OutFile, OutputFile);
                    OutputFile = ArgumentHelper.GetValueWithValidation(ArgumentConstants.OutFile, args, i);
                    i++;
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

                case "--stdout":
                    OutputToStdOut = true;
                    break;

                case "--no-restore":
                    NoRestore = true;
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

            if (BicepFile is not null)
            {
                throw new CommandLineException($"The --bicep-file parameter cannot be used with the {ArgumentConstants.FilePattern} parameter");
            }

            if (OutputToStdOut)
            {
                throw new CommandLineException($"The --stdout parameter cannot be used with the {ArgumentConstants.FilePattern} parameter");
            }

            if (OutputFile is not null)
            {
                throw new CommandLineException($"The {ArgumentConstants.OutFile} parameter cannot be used with the {ArgumentConstants.FilePattern} parameter");
            }
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

        if (DiagnosticsFormat is null)
        {
            DiagnosticsFormat = Arguments.DiagnosticsFormat.Default;
        }
    }

    public static string OutputFileExtension => LanguageConstants.JsonFileExtension;

    public bool OutputToStdOut { get; }

    public string? InputFile { get; }

    public string? BicepFile { get; }

    public string? OutputDir { get; }

    public string? OutputFile { get; }

    public string? FilePattern { get; }

    public DiagnosticsFormat? DiagnosticsFormat { get; }

    public bool NoRestore { get; }
}
