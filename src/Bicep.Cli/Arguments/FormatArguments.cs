// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Abstractions;
using Bicep.Cli.Extensions;
using Bicep.Cli.Helpers;
using Bicep.Core.PrettyPrintV2;
using Bicep.IO.Abstraction;

namespace Bicep.Cli.Arguments;

public class FormatArguments : ArgumentsBase, IFilePatternInputOutputArguments<FormatArguments>
{
    public FormatArguments(string[] args) : base(Constants.Command.Format)
    {
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLowerInvariant())
            {
                case "--stdout":
                    OutputToStdOut = true;
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

                case ArgumentConstants.FilePattern:
                    ArgumentHelper.ValidateNotAlreadySet(ArgumentConstants.FilePattern, FilePattern);
                    FilePattern = ArgumentHelper.GetValueWithValidation(ArgumentConstants.FilePattern, args, i);
                    i++;
                    break;

                case "--newline-kind":
                    if (args.Length == i + 1)
                    {
                        throw new CommandLineException($"The --newline-kind parameter expects an argument");
                    }
                    if (NewlineKind is not null)
                    {
                        throw new CommandLineException($"The --newline-kind parameter cannot be specified twice");
                    }
                    if (!Enum.TryParse<NewlineKind>(args[i + 1], true, out var newline) || !Enum.IsDefined(newline))
                    {
                        throw new CommandLineException($"The --newline-kind parameter only accepts these values: {string.Join(" | ", Enum.GetNames(typeof(NewlineKind)))}");
                    }
                    NewlineKind = newline;
                    i++;
                    break;

                case "--indent-kind":
                    if (args.Length == i + 1)
                    {
                        throw new CommandLineException($"The --indent-kind parameter expects an argument");
                    }
                    if (IndentKind is not null)
                    {
                        throw new CommandLineException($"The --indent-kind parameter cannot be specified twice");
                    }
                    if (!Enum.TryParse<IndentKind>(args[i + 1], true, out var indentKind) || !Enum.IsDefined(indentKind))
                    {
                        throw new CommandLineException($"The --indent-kind parameter only accepts these values: {string.Join(" | ", Enum.GetNames(typeof(IndentKind)))}");
                    }
                    IndentKind = indentKind;
                    i++;
                    break;

                case "--indent-size":
                    if (args.Length == i + 1)
                    {
                        throw new CommandLineException($"The --indent-size parameter expects an argument");
                    }
                    if (IndentSize is not null)
                    {
                        throw new CommandLineException($"The --indent-size parameter cannot be specified twice");
                    }
                    if (!int.TryParse(args[i + 1], out var indentSize))
                    {
                        throw new CommandLineException($"The --indent-size parameter only accepts integer values");
                    }
                    IndentSize = indentSize;
                    i++;
                    break;

                case "--insert-final-newline":
                    if (InsertFinalNewline is not null)
                    {
                        throw new CommandLineException($"The --insert-final-newline parameter cannot be specified twice");
                    }

                    if (args.Length == i + 1)
                    {
                        InsertFinalNewline = true;
                        break;
                    }

                    if (bool.TryParse(args[i + 1], out var insertFinalNewline))
                    {
                        InsertFinalNewline = insertFinalNewline;
                        i++;
                    }
                    else
                    {
                        // Either "true" or "false" is not supplied after "--insert-final-newline", or the value is not a valid boolean.
                        // Treat it as only "--insert-final-newline" is specified without a value, and default to true.
                        InsertFinalNewline = true;
                    }
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

            if (OutputToStdOut)
            {
                throw new CommandLineException($"The --stdout parameter cannot be used with the {ArgumentConstants.FilePattern} parameter");
            }

            if (OutputDir is not null)
            {
                throw new CommandLineException($"The {ArgumentConstants.OutDir} parameter cannot be used with the {ArgumentConstants.FilePattern} parameter");
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
    }

    public static Func<FormatArguments, IOUri, string> OutputFileExtensionResolver { get; } = (_, inputUri) => inputUri.GetExtension().ToString();

    public bool OutputToStdOut { get; }

    public string? InputFile { get; }

    public string? OutputDir { get; }

    public string? OutputFile { get; }

    public string? FilePattern { get; }

    public NewlineKind? NewlineKind { get; }

    public IndentKind? IndentKind { get; }

    public int? IndentSize { get; }

    public bool? InsertFinalNewline { get; }
}
