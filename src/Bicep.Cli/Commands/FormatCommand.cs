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

namespace Bicep.Cli.Commands;

public class FormatCommand : ICommand
{
    private readonly IOContext io;
    private readonly IFileResolver fileResolver;
    private readonly IFileSystem fileSystem;
    private readonly IConfigurationManager configurationManager;
    private readonly IFeatureProviderFactory featureProviderFactory;

    public FormatCommand(
        IOContext io,
        IFileResolver fileResolver,
        IFileSystem fileSystem,
        IConfigurationManager configurationManager,
        IFeatureProviderFactory featureProviderFactory)
    {
        this.io = io;
        this.fileResolver = fileResolver;
        this.fileSystem = fileSystem;
        this.configurationManager = configurationManager;
        this.featureProviderFactory = featureProviderFactory;
    }

    public int Run(FormatArguments args)
    {
        var inputUri = ArgumentHelper.GetFileUri(args.InputFile, this.fileSystem);
        ArgumentHelper.ValidateBicepOrBicepParamFile(inputUri);

        if (!this.fileResolver.TryRead(inputUri).IsSuccess(out var fileContents, out var failureBuilder))
        {
            var diagnostic = failureBuilder(DiagnosticBuilder.ForPosition(new TextSpan(0, 0)));
            throw new ErrorDiagnosticException(diagnostic);
        }

        BaseParser parser = PathHelper.HasBicepExtension(inputUri) ? new Parser(fileContents) : new ParamsParser(fileContents);
        var program = parser.Program();
        var featureProvider = this.featureProviderFactory.GetFeatureProvider(inputUri);

        if (featureProvider.LegacyFormatterEnabled)
        {
            var v2Options = this.GetPrettyPrinterOptions(inputUri, args);
            var legacyOptions = PrettyPrintOptions.FromV2Options(v2Options);
            var output = PrettyPrinter.PrintProgram(program, legacyOptions, parser.LexingErrorLookup, parser.ParsingErrorLookup);

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

        var options = GetPrettyPrinterOptions(inputUri, args);
        var context = PrettyPrinterV2Context.Create(options, parser.LexingErrorLookup, parser.ParsingErrorLookup);

        if (args.OutputToStdOut)
        {
            PrettyPrinterV2.PrintTo(this.io.Output, program, context);
            this.io.Output.Flush();
        }
        else
        {
            var outputPath = PathHelper.ResolveDefaultOutputPath(inputUri.LocalPath, args.OutputDir, args.OutputFile, path => path, this.fileSystem);
            using var fileStream = this.fileSystem.File.Open(outputPath, FileMode.Create, FileAccess.Write, FileShare.Read);
            using var writer = new StreamWriter(fileStream);


            PrettyPrinterV2.PrintTo(writer, program, context);
        }

        return 0;
    }

    private PrettyPrinterV2Options GetPrettyPrinterOptions(Uri inputUri, FormatArguments args)
    {
        var options = this.configurationManager.GetConfiguration(inputUri).Formatting.Data;

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
