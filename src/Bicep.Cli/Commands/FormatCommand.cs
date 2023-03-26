// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Bicep.Cli.Commands;

public class FormatCommand : ICommand
{
    private readonly ILogger logger;
    private readonly IOContext io;
    private readonly IFileResolver fileResolver;

    public FormatCommand(
        ILogger logger,
        IOContext io,
        IFileResolver fileResolver)
    {
        this.logger = logger;
        this.io = io;
        this.fileResolver = fileResolver;
    }

    public int Run(FormatArguments args)
    {
        var inputPath = PathHelper.ResolvePath(args.InputFile);
        var inputUri = PathHelper.FilePathToFileUrl(inputPath);

        if (!PathHelper.HasBicepExtension(inputUri) &&
            !PathHelper.HasBicepparamsExension(inputUri))
        {
            logger.LogError(CliResources.UnrecognizedBicepOrBicepparamsFileExtensionMessage, inputPath);
            return 1;
        }

        if (!fileResolver.TryRead(inputUri, out var fileContents, out var failureBuilder))
        {
            var diagnostic = failureBuilder(DiagnosticBuilder.ForPosition(new TextSpan(0, 0)));
            throw new ErrorDiagnosticException(diagnostic);
        }

        BaseParser parser = PathHelper.HasBicepExtension(inputUri) ? new Parser(fileContents) : new ParamsParser(fileContents);

        var options = new PrettyPrintOptions(
            args.Newline            ?? NewlineOption.Auto,
            args.IndentKind         ?? IndentKindOption.Space,
            args.IndentSize         ?? 2,
            args.InsertFinalNewline ?? false
        );

        var output = PrettyPrinter.PrintProgram(parser.Program(), options, parser.LexingErrorLookup, parser.ParsingErrorLookup);
        if (args.OutputToStdOut)
        {
            io.Output.Write(output);
            io.Output.Flush();
        }
        else
        {
            static string DefaultOutputPath(string path) => path;
            var outputPath = PathHelper.ResolveDefaultOutputPath(inputPath, args.OutputDir, args.OutputFile, DefaultOutputPath);

            File.WriteAllText(outputPath, output);
        }

        return 0;
    }
}