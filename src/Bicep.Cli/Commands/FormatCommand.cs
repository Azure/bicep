// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using System.IO.Abstractions;
using Bicep.Cli.Arguments;
using Bicep.Cli.Constants;
using Bicep.Cli.Helpers;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.SourceGraph;
using Bicep.Core.Text;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Cli.Commands;

public class FormatCommand(
    IOContext io,
    IFileExplorer fileExplorer,
    ISourceFileFactory sourceFileFactory,
    InputOutputArgumentsResolver inputOutputArgumentsResolver) : ICommand
{
    public int Run(FormatArguments args)
    {
        if (args.FilePattern is not null && args.OutputDir is not null)
        {
            throw new CommandLineException($"The {OptionNames.OutDir} parameter cannot be used with the {OptionNames.Pattern} parameter");
        }
        ArgumentHelper.ValidateOutputOptions(args.OutputToStdOut, args.OutputDir, args.OutputFile, args.FilePattern);

        foreach (var (inputUri, outputUri) in inputOutputArgumentsResolver.ResolveFilePatternInputOutputArguments(args))
        {
            ArgumentHelper.ValidateBicepOrBicepParamFile(inputUri);
            this.Format(args, inputUri, outputUri, args.OutputToStdOut);
        }

        return 0;
    }

    public void Format(FormatArguments args, IOUri inputUri, IOUri outputUri, bool outputToStdOut)
    {
        if (!fileExplorer.GetFile(inputUri).TryReadAllText().IsSuccess(out var fileContents, out var diagnosticBuilder))
        {
            var diagnostic = diagnosticBuilder(DiagnosticBuilder.ForPosition(TextSpan.TextDocumentStart));
            throw new DiagnosticException(diagnostic);
        }

        if (sourceFileFactory.CreateSourceFile(inputUri, fileContents) is not BicepSourceFile sourceFile)
        {
            throw new InvalidOperationException("Unable to create Bicep source file.");
        }

        if (sourceFile.LoadFeatures().LegacyFormatterEnabled)
        {
            var v2Options = GetPrettyPrinterOptions(sourceFile, args);
            var legacyOptions = PrettyPrintOptions.FromV2Options(v2Options);
            var output = PrettyPrinter.PrintProgram(sourceFile.ProgramSyntax, legacyOptions, sourceFile.LexingErrorLookup, sourceFile.ParsingErrorLookup);

            if (outputToStdOut)
            {
                io.Output.Writer.Write(output);
                io.Output.Writer.Flush();
            }
            else
            {
                fileExplorer.GetFile(outputUri).WriteAllText(output);
            }

            return;
        }

        var options = GetPrettyPrinterOptions(sourceFile, args);
        var context = PrettyPrinterV2Context.Create(options, sourceFile.LexingErrorLookup, sourceFile.ParsingErrorLookup);

        if (outputToStdOut)
        {
            PrettyPrinterV2.PrintTo(io.Output.Writer, sourceFile.ProgramSyntax, context);
            io.Output.Writer.Flush();
        }
        else
        {

            using var stream = fileExplorer.GetFile(outputUri).OpenWrite();
            using var writer = new StreamWriter(stream);


            PrettyPrinterV2.PrintTo(writer, sourceFile.ProgramSyntax, context);
        }

        return;
    }

    private static PrettyPrinterV2Options GetPrettyPrinterOptions(BicepSourceFile sourceFile, FormatArguments args)
    {
        var options = sourceFile.LoadConfiguration().Formatting.Data;

        if (args.NewlineKind is not null)
        {
            options = options with { NewlineKind = args.NewlineKind.Value };
        }

        if (args.IndentKind is not null)
        {
            options = options with { IndentKind = args.IndentKind.Value };
        }

        if (args.IndentSize is not null)
        {
            options = options with { IndentSize = args.IndentSize.Value };
        }

        if (args.InsertFinalNewline is not null)
        {
            options = options with { InsertFinalNewline = args.InsertFinalNewline.Value };
        }

        return options;
    }

    internal static Command CreateCommand(CommandLineBuilderContext context)
    {
        var inputFileArgument = CommonArguments.OptionalInputFile("The path to the input .bicep file.");
        var stdoutOption = CommonOptions.Stdout();
        var outDirOption = CommonOptions.OutDir();
        var outFileOption = CommonOptions.OutFile();
        var filePatternOption = CommonOptions.Pattern("Formats all files matching the specified glob pattern.");
        var newlineKindOption = new Option<NewlineKind?>(OptionNames.NewlineKind)
        {
            Description = "Set newline char. Valid values are (Auto, LF, CRLF, CR).",
        };
        var indentKindOption = new Option<IndentKind?>(OptionNames.IndentKind)
        {
            Description = "Set indentation kind. Valid values are (Space, Tab).",
        };
        var indentSizeOption = new Option<int?>(OptionNames.IndentSize)
        {
            Description = "Number of spaces to indent with (only valid with --indent-kind set to Space).",
        };
        var insertFinalNewlineOption = new Option<bool?>(OptionNames.InsertFinalNewline)
        {
            Description = "Insert a final newline.",
        };
        var configOption = CommonOptions.Config();

        var command = new Command(CommandNames.Format, "Formats a .bicep file.")
        {
            inputFileArgument,
            stdoutOption,
            outDirOption,
            outFileOption,
            filePatternOption,
            newlineKindOption,
            indentKindOption,
            indentSizeOption,
            insertFinalNewlineOption,
            configOption,
        };
        command.Validators.Add(result => CommandLineBuilderContext.ValidatePositionalArgument(result, inputFileArgument));

        command.SetAction((result, ct) => context.RunCommandAsync(result.GetValue(configOption), () =>
        {
            var args = new FormatArguments(
                result.GetValue(stdoutOption),
                result.GetValue(inputFileArgument),
                result.GetValue(outDirOption),
                result.GetValue(outFileOption),
                result.GetValue(filePatternOption),
                result.GetValue(newlineKindOption),
                result.GetValue(indentKindOption),
                result.GetValue(indentSizeOption),
                result.GetValue(insertFinalNewlineOption));

            return Task.FromResult(context.GetCommand<FormatCommand>().Run(args));
        }));

        return command;
    }
}
