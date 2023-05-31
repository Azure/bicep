// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.PrettyPrintV2;
using Microsoft.Extensions.Logging;
using System.Configuration;
using System.IO;

namespace Bicep.Cli.Commands;

public class FormatCommand : ICommand
{
    private readonly ILogger logger;

    private readonly IOContext io;

    private readonly IFileResolver fileResolver;

    private readonly IConfigurationManager configurationManager;

    private readonly IFeatureProviderFactory featureProviderFactory;

    public FormatCommand(
        ILogger logger,
        IOContext io,
        IFileResolver fileResolver,
        IConfigurationManager configurationManager,
        IFeatureProviderFactory featureProviderFactory)
    {
        this.logger = logger;
        this.io = io;
        this.fileResolver = fileResolver;
        this.configurationManager = configurationManager;
        this.featureProviderFactory = featureProviderFactory;
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
        var program = parser.Program();
        var featureProvider = this.featureProviderFactory.GetFeatureProvider(inputUri);

        if (featureProvider.PrettyPrintingEnabled)
        {
            var v2Options = this.configurationManager.GetConfiguration(inputUri).Formatting.Data;
            var v2Context = PrettyPrinterV2Context.Create(program, v2Options, parser.LexingErrorLookup, parser.ParsingErrorLookup);

            if (args.OutputToStdOut)
            {
                PrettyPrinterV2.PrintTo(io.Output, v2Context);
                io.Output.Flush();
            }
            else
            {
                var outputPath = PathHelper.ResolveDefaultOutputPath(inputPath, args.OutputDir, args.OutputFile, path => path);
                using var writer = new StreamWriter(outputPath);

                PrettyPrinterV2.PrintTo(writer, v2Context);
            }

            return 0;
        }

        var options = new PrettyPrintOptions(
            args.Newline            ?? NewlineOption.Auto,
            args.IndentKind         ?? IndentKindOption.Space,
            args.IndentSize         ?? 2,
            args.InsertFinalNewline ?? false
        );

        var output = PrettyPrinter.PrintProgram(program, options, parser.LexingErrorLookup, parser.ParsingErrorLookup);
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
