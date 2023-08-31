// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepSemanticTokensHandler : SemanticTokensHandlerBase
    {
        private readonly ICompilationManager compilationManager;

        // TODO: Not sure if this needs to be shared.
        private readonly SemanticTokensLegend legend = new();

        public BicepSemanticTokensHandler(ICompilationManager compilationManager)
        {
            this.compilationManager = compilationManager;
        }

        protected override Task<SemanticTokensDocument> GetSemanticTokensDocument(ITextDocumentIdentifierParams @params, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SemanticTokensDocument(this.legend));
        }

        protected override Task Tokenize(SemanticTokensBuilder builder, ITextDocumentIdentifierParams identifier, CancellationToken cancellationToken)
        {
            /*
             * do not check for file extension here because that will prevent untitled files from getting syntax highlighting
             */

            var compilationContext = this.compilationManager.GetCompilation(identifier.TextDocument.Uri);
            if (compilationContext is not null)
            {
                SemanticTokenVisitor.BuildSemanticTokens(builder, compilationContext.Compilation.GetEntrypointSemanticModel());
            }

            return Task.CompletedTask;
        }

        protected override SemanticTokensRegistrationOptions CreateRegistrationOptions(SemanticTokensCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            // the semantic tokens handler requests don't get routed like other handlers
            // it seems we can only have one and it must be shared between all the language IDs we support
            DocumentSelector = DocumentSelectorFactory.CreateForBicepAndParams(),
            Legend = this.legend,
            Full = new SemanticTokensCapabilityRequestFull
            {
                Delta = true
            },
            Range = true
        };
    }
}
