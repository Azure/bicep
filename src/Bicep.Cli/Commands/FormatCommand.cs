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
using Bicep.Core.Workspaces;

namespace Bicep.Cli.Commands;

public class FormatCommand : ICommand
{
    private readonly IOContext io;
    private readonly IFileResolver fileResolver;
    private readonly IFileSystem fileSystem;
    private readonly ISourceFileFactory sourceFileFactory;

    public FormatCommand(
        IOContext io,
        IFileResolver fileResolver,
        IFileSystem fileSystem,
        ISourceFileFactory sourceFileFactory)
    {
        this.io = io;
        this.fileResolver = fileResolver;
        this.fileSystem = fileSystem;
        this.sourceFileFactory = sourceFileFactory;
    }

    public int Run(FormatArguments args)
    {
        var inputUri = ArgumentHelper.GetFileUri(args.InputFile, this.fileSystem);
        ArgumentHelper.ValidateBicepOrBicepParamFile(inputUri);

        if (!this.fileResolver.TryRead(inputUri).IsSuccess(out var fileContents, out var failureBuilder))
        {
            var diagnostic = failureBuilder(DiagnosticBuilder.ForPosition(new TextSpan(0, 0)));
            throw new DiagnosticException(diagnostic);
        }

        if (this.sourceFileFactory.CreateSourceFile(inputUri, fileContents) is not BicepSourceFile sourceFile)
        {
            throw new InvalidOperationException("Unable to create Bicep source file.");
        }

        if (sourceFile.Features.LegacyFormatterEnabled)
        {
            var v2Options = GetPrettyPrinterOptions(sourceFile, args);
            var legacyOptions = PrettyPrintOptions.FromV2Options(v2Options);
            var output = PrettyPrinter.PrintProgram(sourceFile.ProgramSyntax, legacyOptions, sourceFile.LexingErrorLookup, sourceFile.ParsingErrorLookup);

            if (args.OutputToStdOut)
            {
                io.Output.Write(output);
                io.Output.Flush();
            }
            else
            {
                var outputPath = PathHelper.ResolveDefaultOutputPath(inputUri.LocalPath, args.OutputDir, args.OutputFile, path => path, this.fileSystem);

                this.fileSystem.File.WriteAllText(outputPath, output);
            }

            return 0;
        }

        var options = GetPrettyPrinterOptions(sourceFile, args);
        var context = PrettyPrinterV2Context.Create(options, sourceFile.LexingErrorLookup, sourceFile.ParsingErrorLookup);

        if (args.OutputToStdOut)
        {
            PrettyPrinterV2.PrintTo(this.io.Output, sourceFile.ProgramSyntax, context);
            this.io.Output.Flush();
        }
        else
        {
            var outputPath = PathHelper.ResolveDefaultOutputPath(inputUri.LocalPath, args.OutputDir, args.OutputFile, path => path, this.fileSystem);
            using var fileStream = this.fileSystem.File.Open(outputPath, FileMode.Create, FileAccess.Write, FileShare.Read);
            using var writer = new StreamWriter(fileStream);


            PrettyPrinterV2.PrintTo(writer, sourceFile.ProgramSyntax, context);
        }

        return 0;
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
