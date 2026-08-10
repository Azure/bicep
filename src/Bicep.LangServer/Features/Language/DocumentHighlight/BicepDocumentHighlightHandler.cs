// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.Features.Language.Definition;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Features.Language.DocumentHighlight
{
    public class BicepDocumentHighlightHandler(ISymbolResolver symbolResolver, DocumentSelectorFactory documentSelectorFactory) : DocumentHighlightHandlerBase
    {
        public override Task<DocumentHighlightContainer?> Handle(DocumentHighlightParams request, CancellationToken cancellationToken)
        {
            var result = symbolResolver.ResolveSymbol(request.TextDocument.Uri, request.Position);
            if (result == null)
            {
                return Task.FromResult<DocumentHighlightContainer?>(null);
            }

            var highlights = result.Context.Compilation.GetEntrypointSemanticModel()
                .FindReferences(result.Symbol)
                .Select(referenceSyntax => new global::OmniSharp.Extensions.LanguageServer.Protocol.Models.DocumentHighlight
                {
                    Range = PositionHelper.GetNameRange(result.Context.LineStarts, referenceSyntax),
                    Kind = referenceSyntax switch
                    {
                        INamedDeclarationSyntax _ => DocumentHighlightKind.Write,
                        ObjectPropertySyntax _ => DocumentHighlightKind.Write,
                        _ => DocumentHighlightKind.Read,
                    },
                });

            return Task.FromResult<DocumentHighlightContainer?>(new DocumentHighlightContainer(highlights));
        }

        protected override DocumentHighlightRegistrationOptions CreateRegistrationOptions(DocumentHighlightCapability capability, global::OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities.ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = documentSelectorFactory.CreateForBicepAndParams()
        };
    }
}

