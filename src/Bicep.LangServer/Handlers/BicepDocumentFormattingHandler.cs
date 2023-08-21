// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.PrettyPrint;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Bicep.Core.PrettyPrint.Options;
using Microsoft.Extensions.Logging;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using Bicep.Core.Features;
using Bicep.Core.PrettyPrintV2;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepDocumentFormattingHandler : DocumentFormattingHandlerBase
    {
        private readonly ILogger<BicepDocumentSymbolHandler> logger;

        private readonly ICompilationManager compilationManager;

        private readonly IFeatureProviderFactory featureProviderFactory;

        public BicepDocumentFormattingHandler(
            ILogger<BicepDocumentSymbolHandler> logger,
            ICompilationManager compilationManager,
            IFeatureProviderFactory featureProviderFactory)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.featureProviderFactory = featureProviderFactory;
        }

        public override Task<TextEditContainer?> Handle(DocumentFormattingParams request, CancellationToken cancellationToken)
        {
            CompilationContext? context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context == null)
            {
                // we have not yet compiled this document, which shouldn't really happen
                this.logger.LogError("Document formatting request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                return Task.FromResult<TextEditContainer?>(null);
            }

            var lexingErrorLookup = context.Compilation.SourceFileGrouping.EntryPoint.LexingErrorLookup;
            var parsingErrorLookup = context.Compilation.SourceFileGrouping.EntryPoint.ParsingErrorLookup;
            var featureProvider = this.featureProviderFactory.GetFeatureProvider(request.TextDocument.Uri.ToUriEncoded());

            if (featureProvider.PrettyPrintingEnabled)
            {
                var v2Options = context.Compilation.GetEntrypointSemanticModel().Configuration.Formatting.Data;
                var printerV2Context = PrettyPrinterV2Context.Create(context.ProgramSyntax, v2Options, lexingErrorLookup, parsingErrorLookup);
                var v2Output = PrettyPrinterV2.Print(printerV2Context);

                return Task.FromResult<TextEditContainer?>(new TextEditContainer(new TextEdit
                {
                    Range = context.ProgramSyntax.Span.ToRange(context.LineStarts),
                    NewText = v2Output,
                }));
            }

            long indentSize = request.Options.TabSize;
            IndentKindOption indentKindOption = request.Options.InsertSpaces ? IndentKindOption.Space : IndentKindOption.Tab;

            ProgramSyntax programSyntax = context.ProgramSyntax;
            PrettyPrintOptions options = new PrettyPrintOptions(NewlineOption.Auto, indentKindOption, indentSize, request.Options.InsertFinalNewline);
            string? output = PrettyPrinter.PrintProgram(context.ProgramSyntax, options, lexingErrorLookup, parsingErrorLookup);

            if (output == null)
            {
                return Task.FromResult<TextEditContainer?>(null);
            }

            return Task.FromResult<TextEditContainer?>(new TextEditContainer(new TextEdit
            {
                Range = programSyntax.Span.ToRange(context.LineStarts),
                NewText = output
            }));
        }

        protected override DocumentFormattingRegistrationOptions CreateRegistrationOptions(DocumentFormattingCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = DocumentSelectorFactory.CreateForBicepAndParams()
        };
    }
}
