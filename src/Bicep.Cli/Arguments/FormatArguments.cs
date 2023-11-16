// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using Bicep.Cli.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.PrettyPrint.Options;

namespace Bicep.Cli.Arguments
{
    public class FormatArguments : ArgumentsBase
    {
        public FormatArguments(string[] args, IOContext io) : base(Constants.Command.Format)
        {
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "--stdout":
                        OutputToStdOut = true;
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

                    case "--newline":
                        if (args.Length == i + 1)
                        {
                            throw new CommandLineException($"The --newline parameter expects an argument");
                        }
                        if (Newline is not null)
                        {
                            throw new CommandLineException($"The --newline parameter cannot be specified twice");
                        }
                        if (!Enum.TryParse<NewlineOption>(args[i + 1], true, out var newline) || !Enum.IsDefined<NewlineOption>(newline))
                        {
                            throw new CommandLineException($"The --newline parameter only accepts values: {string.Join(" | ", Enum.GetNames(typeof(NewlineOption)))}");
                        }
                        Newline = newline;
                        i++;
                        break;

                    case "--indentkind":
                        // TODO: Uncomment this once Azure CLI is updated to support the new parameter.
                        //io.WriteParameterDeprecationWarning("--indentKind", "--indent-kind");

                        if (args.Length == i + 1)
                        {
                            throw new CommandLineException($"The --indentKind parameter expects an argument");
                        }
                        if (IndentKind is not null)
                        {
                            throw new CommandLineException($"The --indentKind parameter cannot be specified twice");
                        }
                        if (!Enum.TryParse<IndentKindOption>(args[i + 1], true, out var indentKind) || !Enum.IsDefined(indentKind))
                        {
                            throw new CommandLineException($"The --indentKind parameter only accepts values: {string.Join(" | ", Enum.GetNames(typeof(IndentKindOption)))}");
                        }
                        IndentKind = indentKind;
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
                        if (!Enum.TryParse(args[i + 1], true, out indentKind) || !Enum.IsDefined(indentKind))
                        {
                            throw new CommandLineException($"The --indent-kind parameter only accepts values: {string.Join(" | ", Enum.GetNames(typeof(IndentKindOption)))}");
                        }
                        IndentKind = indentKind;
                        i++;
                        break;

                    case "--indentsize":
                        // TODO: Uncomment this once Azure CLI is updated to support the new parameter.
                        //io.WriteParameterDeprecationWarning("--indentSize", "--indent-size");

                        if (args.Length == i + 1)
                        {
                            throw new CommandLineException($"The --indentSize parameter expects an argument");
                        }
                        if (IndentSize is not null)
                        {
                            throw new CommandLineException($"The --indentSize parameter cannot be specified twice");
                        }
                        if (!int.TryParse(args[i + 1], out var indentSize))
                        {
                            throw new CommandLineException($"The --indentSize parameter only accepts integer values");
                        }
                        IndentSize = indentSize;
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
                        if (!int.TryParse(args[i + 1], out indentSize))
                        {
                            throw new CommandLineException($"The --indent-size parameter only accepts integer values");
                        }
                        IndentSize = indentSize;
                        i++;
                        break;

                    case "--insertfinalnewline":
                        // TODO: Uncomment this once Azure CLI is updated to support the new parameter.
                        //io.WriteParameterDeprecationWarning("--insertFinalNewline", "--insert-final-newline");

                        if (InsertFinalNewline is not null)
                        {
                            throw new CommandLineException($"The --insertFinalNewline parameter cannot be specified twice");
                        }
                        InsertFinalNewline = true;
                        break;
                    case "--insert-final-newline":
                        if (InsertFinalNewline is not null)
                        {
                            throw new CommandLineException($"The --insert-final-newline parameter cannot be specified twice");
                        }
                        InsertFinalNewline = true;
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

            if (IndentSize is not null && IndentKind == IndentKindOption.Tab)
            {
                throw new CommandLineException($"The --indent-size cannot be used when --indent-kind is \"Tab\"");
            }

            if (OutputDir is not null)
            {
                var outputDir = PathHelper.ResolvePath(OutputDir);

                if (!Directory.Exists(outputDir))
                {
                    throw new CommandLineException(string.Format(CliResources.DirectoryDoesNotExistFormat, outputDir));
                }
            }
        }

        public bool OutputToStdOut { get; }

        public string InputFile { get; }

        public string? OutputDir { get; }

        public string? OutputFile { get; }

        public NewlineOption? Newline { get; }

        public IndentKindOption? IndentKind { get; }

        public int? IndentSize { get; }

        public bool? InsertFinalNewline { get; }

    }
}
