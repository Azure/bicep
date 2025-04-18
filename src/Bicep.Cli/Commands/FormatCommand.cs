// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.SourceGraph;
using Bicep.Core.Text;
using Bicep.Core.Utils;

namespace Bicep.Cli.Commands;

public class FormatCommand(
    IOContext io,
    IEnvironment environment,
    IFileResolver fileResolver,
    IFileSystem fileSystem,
    ISourceFileFactory sourceFileFactory) : ICommand
{
    public int Run(FormatArguments args)
    {
        if (args.InputFile is null)
        {
            FormatMultiple(args);
            return 0;
        }

        var inputUri = ArgumentHelper.GetFileUri(args.InputFile, fileSystem);
        ArgumentHelper.ValidateBicepOrBicepParamFile(inputUri);

        var outputUri = GetOutputUri(inputUri, args.OutputDir, args.OutputFile);

        Format(args, inputUri, outputUri, args.OutputToStdOut);
        return 0;
    }

    public void Format(FormatArguments args, Uri inputUri, Uri outputUri, bool outputToStdOut)
    {
        if (!fileResolver.TryRead(inputUri).IsSuccess(out var fileContents, out var failureBuilder))
        {
            var diagnostic = failureBuilder(DiagnosticBuilder.ForPosition(new TextSpan(0, 0)));
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
                io.Output.Write(output);
                io.Output.Flush();
            }
            else
            {
                fileSystem.File.WriteAllText(outputUri.LocalPath, output);
            }

            return;
        }

        var options = GetPrettyPrinterOptions(sourceFile, args);
        var context = PrettyPrinterV2Context.Create(options, sourceFile.LexingErrorLookup, sourceFile.ParsingErrorLookup);

        if (outputToStdOut)
        {
            PrettyPrinterV2.PrintTo(io.Output, sourceFile.ProgramSyntax, context);
            io.Output.Flush();
        }
        else
        {
            using var fileStream = fileSystem.File.Open(outputUri.LocalPath, FileMode.Create, FileAccess.Write, FileShare.Read);
            using var writer = new StreamWriter(fileStream);


            PrettyPrinterV2.PrintTo(writer, sourceFile.ProgramSyntax, context);
        }

        return;
    }

    public void FormatMultiple(FormatArguments args)
    {
        foreach (var inputUri in CommandHelper.GetInputFilesForPattern(environment, args.FilePattern))
        {
            ArgumentHelper.ValidateBicepOrBicepParamFile(inputUri);

            // only allow in-place formatting
            var outputUri = inputUri;

            Format(args, inputUri, outputUri, false);
        }
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

    private Uri GetOutputUri(Uri inputUri, string? outputDir, string? outputFile)
    {
        var outputPath = PathHelper.ResolveOutputPath(inputUri.LocalPath, outputDir, outputFile, path => path, fileSystem);
        return PathHelper.FilePathToFileUrl(outputPath);
    }
}
