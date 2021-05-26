// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Semantics;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepDefinitionHandler : DefinitionHandlerBase
    {
        private readonly ISymbolResolver symbolResolver;

        public BicepDefinitionHandler(ISymbolResolver symbolResolver) : base()
        {
            this.symbolResolver = symbolResolver;
        }

        public override Task<LocationOrLocationLinks> Handle(DefinitionParams request, CancellationToken cancellationToken)
        {
            var result = this.symbolResolver.ResolveSymbol(request.TextDocument.Uri, request.Position);
            if (result == null)
            {
                return Task.FromResult(new LocationOrLocationLinks());
            }

            if (result.Symbol is PropertySymbol)
            {
                // TODO: Implement for PropertySymbol
                return Task.FromResult(new LocationOrLocationLinks());
            }

            if (result.Symbol is DeclaredSymbol declaration)
            {
                return Task.FromResult(new LocationOrLocationLinks(new LocationOrLocationLink(new LocationLink
                {
                    // source of the link
                    OriginSelectionRange = result.Origin.ToRange(result.Context.LineStarts),
                    TargetUri = request.TextDocument.Uri,

                    // entire span of the variable
                    TargetRange = declaration.DeclaringSyntax.ToRange(result.Context.LineStarts),

                    // span of the variable name
                    TargetSelectionRange = declaration.NameSyntax.ToRange(result.Context.LineStarts)
                })));
            }

            return Task.FromResult(new LocationOrLocationLinks());
        }

        protected override DefinitionRegistrationOptions CreateRegistrationOptions(DefinitionCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = DocumentSelectorFactory.Create()
        };
    }
}
