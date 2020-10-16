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
    public class BicepDocumentHighlightHandler : DocumentHighlightHandler
    {
        private readonly ISymbolResolver symbolResolver;

        public BicepDocumentHighlightHandler(ISymbolResolver symbolResolver) : base(CreateRegistrationOptions())
        {
            this.symbolResolver = symbolResolver;
        }

        public override Task<DocumentHighlightContainer> Handle(DocumentHighlightParams request, CancellationToken cancellationToken)
        {
            var result = this.symbolResolver.ResolveSymbol(request.TextDocument.Uri, request.Position);
            if (result == null)
            {
                return Task.FromResult(new DocumentHighlightContainer());
            }

            var highlights = result.Context.Compilation.GetEntrypointSemanticModel()
                .FindReferences(result.Symbol)
                .Select(referenceSyntax => new DocumentHighlight
                {
                    Range = PositionHelper.GetNameRange(result.Context.LineStarts, referenceSyntax),
                    Kind = referenceSyntax is IDeclarationSyntax
                        ? DocumentHighlightKind.Write
                        : DocumentHighlightKind.Read
                });

            return Task.FromResult(new DocumentHighlightContainer(highlights));
        }

        private static DocumentHighlightRegistrationOptions CreateRegistrationOptions() =>
            new DocumentHighlightRegistrationOptions
            {
                DocumentSelector = DocumentSelectorFactory.Create()
            };
    }
}

