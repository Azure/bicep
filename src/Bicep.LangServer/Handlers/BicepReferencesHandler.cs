// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Navigation;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepReferencesHandler : ReferencesHandler
    {
        private readonly ISymbolResolver symbolResolver;

        public BicepReferencesHandler(ISymbolResolver symbolResolver) : base(CreateRegistrationOptions())
        {
            this.symbolResolver = symbolResolver;
        }

        public override Task<LocationContainer> Handle(ReferenceParams request, CancellationToken cancellationToken)
        {
            var result = this.symbolResolver.ResolveSymbol(request.TextDocument.Uri, request.Position);
            if (result == null)
            {
                return Task.FromResult(new LocationContainer());
            }

            var references = result.Context.Compilation.GetEntrypointSemanticModel()
                .FindReferences(result.Symbol)
                .Where(referenceSyntax => request.Context.IncludeDeclaration || !(referenceSyntax is IDeclarationSyntax))
                .Select(referenceSyntax => new Location
                {
                    Uri = request.TextDocument.Uri,
                    Range = PositionHelper.GetNameRange(result.Context.LineStarts, referenceSyntax)
                });

            return Task.FromResult(new LocationContainer(references));
        }

        private static ReferenceRegistrationOptions CreateRegistrationOptions() => new ReferenceRegistrationOptions
        {
            DocumentSelector = DocumentSelectorFactory.Create()
        };
    }
}

