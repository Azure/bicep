// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepDocumentHighlightHandler : DocumentHighlightHandlerBase
    {
        private readonly ISymbolResolver symbolResolver;

        public BicepDocumentHighlightHandler(ISymbolResolver symbolResolver) : base()
        {
            this.symbolResolver = symbolResolver;
        }

        public override Task<DocumentHighlightContainer?> Handle(DocumentHighlightParams request, CancellationToken cancellationToken)
        {
            var result = this.symbolResolver.ResolveSymbol(request.TextDocument.Uri, request.Position);
            if (result == null)
            {
                return Task.FromResult<DocumentHighlightContainer?>(null);
            }

            var highlights = result.Context.Compilation.GetEntrypointSemanticModel()
                .FindReferences(result.Symbol)
                .Select(referenceSyntax => new DocumentHighlight
                {
                    Range = PositionHelper.GetNameRange(result.Context.LineStarts, referenceSyntax),
                    Kind = referenceSyntax switch {
                        INamedDeclarationSyntax _ => DocumentHighlightKind.Write,
                        ObjectPropertySyntax _ => DocumentHighlightKind.Write,
                        _ => DocumentHighlightKind.Read,
                    },
                });

            return Task.FromResult<DocumentHighlightContainer?>(new DocumentHighlightContainer(highlights));
        }

        protected override DocumentHighlightRegistrationOptions CreateRegistrationOptions(DocumentHighlightCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = DocumentSelectorFactory.Create()
        };
    }
}

