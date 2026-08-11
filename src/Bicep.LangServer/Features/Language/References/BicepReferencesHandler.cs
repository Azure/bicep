// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.LanguageServer.Features.Language.Definition;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using LspClientCapabilities = OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities.ClientCapabilities;

namespace Bicep.LanguageServer.Features.Language.References
{
    public class BicepReferencesHandler(ISymbolResolver symbolResolver, DocumentSelectorFactory documentSelectorFactory) : ReferencesHandlerBase
    {
        public override async Task<LocationContainer?> Handle(ReferenceParams request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var result = symbolResolver.ResolveSymbol(request.TextDocument.Uri, request.Position);
            if (result == null)
            {
                return null;
            }

            if (result.Symbol is PropertySymbol)
            {
                // TODO: Implement for PropertySymbol
                return null;
            }

            var references = result.Context.Compilation.GetEntrypointSemanticModel()
                .FindReferences(result.Symbol)
                .Where(referenceSyntax => request.Context.IncludeDeclaration || !(referenceSyntax is INamedDeclarationSyntax))
                .Select(referenceSyntax => new Location
                {
                    Uri = request.TextDocument.Uri,
                    Range = PositionHelper.GetNameRange(result.Context.LineStarts, referenceSyntax),
                });

            return new(references);
        }

        protected override ReferenceRegistrationOptions CreateRegistrationOptions(ReferenceCapability capability, LspClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = documentSelectorFactory.CreateForBicepAndParams()
        };
    }
}

