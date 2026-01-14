// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Bicep.Cli.Arguments;
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

namespace Bicep.Cli.Commands;

public class FormatCommand(
    IOContext io,
    IFileExplorer fileExplorer,
    ISourceFileFactory sourceFileFactory,
    InputOutputArgumentsResolver inputOutputArgumentsResolver) : ICommand
{
    public int Run(FormatArguments args)
    {
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

        if (sourceFile.Features.LegacyFormatterEnabled)
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
        var options = sourceFile.Configuration.Formatting.Data;

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
}
