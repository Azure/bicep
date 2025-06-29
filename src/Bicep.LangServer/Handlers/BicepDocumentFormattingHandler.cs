// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrintV2;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Utils;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepDocumentFormattingHandler(
        ILogger<BicepDocumentSymbolHandler> logger,
        ICompilationManager compilationManager,
        DocumentSelectorFactory documentSelectorFactory) : DocumentFormattingHandlerBase
    {
        public override Task<TextEditContainer?> Handle(DocumentFormattingParams request, CancellationToken cancellationToken)
        {
            CompilationContext? context = compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context == null)
            {
                // we have not yet compiled this document, which shouldn't really happen
                logger.LogError("Document formatting request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                return Task.FromResult<TextEditContainer?>(null);
            }


            var lexingErrorLookup = context.Compilation.SourceFileGrouping.EntryPoint.LexingErrorLookup;
            var parsingErrorLookup = context.Compilation.SourceFileGrouping.EntryPoint.ParsingErrorLookup;
            var printerOptions = context.Compilation.GetEntrypointSemanticModel().Configuration.Formatting.Data;
            var features = context.Compilation.GetEntrypointSemanticModel().Features;

            if (features.LegacyFormatterEnabled)
            {
                var legacyOptions = PrettyPrintOptions.FromV2Options(printerOptions);
                var legacyOutput = PrettyPrinter.PrintProgram(context.ProgramSyntax, legacyOptions, lexingErrorLookup, parsingErrorLookup);

                if (legacyOutput is null)
                {
                    return Task.FromResult<TextEditContainer?>(null);
                }

                return Task.FromResult<TextEditContainer?>(new TextEditContainer(new TextEdit
                {
                    Range = context.ProgramSyntax.Span.ToRange(context.LineStarts),
                    NewText = legacyOutput,
                }));
            }

            var printerContext = PrettyPrinterV2Context.Create(printerOptions, lexingErrorLookup, parsingErrorLookup);
            var output = PrettyPrinterV2.Print(context.ProgramSyntax, printerContext);

            return Task.FromResult<TextEditContainer?>(new TextEditContainer(new TextEdit
            {
                Range = context.ProgramSyntax.Span.ToRange(context.LineStarts),
                NewText = output,
            }));
        }

        protected override DocumentFormattingRegistrationOptions CreateRegistrationOptions(DocumentFormattingCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = documentSelectorFactory.CreateForBicepAndParams()
        };
    }
}
