// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2;

namespace Bicep.Cli.Commands;

public class FormatCommand(IOContext io, IFileResolver fileResolver, IFileSystem fileSystem, IConfigurationManager configurationManager) : ICommand
{
    public int Run(FormatArguments args)
    {
        var inputUri = ArgumentHelper.GetFileUri(args.InputFile, fileSystem);
        ArgumentHelper.ValidateBicepOrBicepParamFile(inputUri);

        if (!fileResolver.TryRead(inputUri).IsSuccess(out var fileContents, out var failureBuilder))
        {
            var diagnostic = failureBuilder(DiagnosticBuilder.ForPosition(new TextSpan(0, 0)));
            throw new ErrorDiagnosticException(diagnostic);
        }

        BaseParser parser = PathHelper.HasBicepExtension(inputUri) ? new Parser(fileContents) : new ParamsParser(fileContents);
        var program = parser.Program();
        var options = GetPrettyPrinterOptions(inputUri, args);
        var context = PrettyPrinterV2Context.Create(options, parser.LexingErrorLookup, parser.ParsingErrorLookup);

        if (args.OutputToStdOut)
        {
            PrettyPrinterV2.PrintTo(io.Output, program, context);
            io.Output.Flush();
        }
        else
        {
            var outputPath = PathHelper.ResolveDefaultOutputPath(inputUri.LocalPath, args.OutputDir, args.OutputFile, path => path, fileSystem);
            using var fileStream = fileSystem.File.Open(outputPath, FileMode.Create, FileAccess.Write, FileShare.Read);
            using var writer = new StreamWriter(fileStream);


            PrettyPrinterV2.PrintTo(writer, program, context);
        }

        return 0;
    }

    private PrettyPrinterV2Options GetPrettyPrinterOptions(Uri inputUri, FormatArguments args)
    {
        var options = configurationManager.GetConfiguration(inputUri).Formatting.Data;

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

        if (args.Width is not null)
        {
            options = options with { Width = args.Width.Value };
        }

        if (args.InsertFinalNewline is not null)
        {
            options = options with { InsertFinalNewline = args.InsertFinalNewline.Value };
        }

        return options;
    }
}
