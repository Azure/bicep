// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Threading;
using System.Threading.Tasks;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Utils;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepSemanticTokensHandler : SemanticTokensHandlerBase
    {
        private readonly ILogger<BicepSemanticTokensHandler> logger;
        private readonly ICompilationManager compilationManager;

        // TODO: Not sure if this needs to be shared.
        private readonly SemanticTokensLegend legend = new();

        public BicepSemanticTokensHandler(ILogger<BicepSemanticTokensHandler> logger, ICompilationManager compilationManager)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
        }

        protected override Task<SemanticTokensDocument> GetSemanticTokensDocument(ITextDocumentIdentifierParams @params, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SemanticTokensDocument(this.legend));
        }

        protected override Task Tokenize(SemanticTokensBuilder builder, ITextDocumentIdentifierParams identifier, CancellationToken cancellationToken)
        {
            var compilationContext = this.compilationManager.GetCompilation(identifier.TextDocument.Uri);

            if (compilationContext != null)
            {
                SemanticTokenVisitor.BuildSemanticTokens(builder, compilationContext.Compilation.SourceFileGrouping.EntryPoint);
            }

            return Task.CompletedTask;
        }

        protected override SemanticTokensRegistrationOptions CreateRegistrationOptions(SemanticTokensCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = DocumentSelectorFactory.Create(),
            Legend = this.legend,
            Full = new SemanticTokensCapabilityRequestFull
            {
                Delta = true
            },
            Range = true
        };
    }
}
