// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Features.Language.Definition;
using Bicep.LanguageServer.Features.Language.Rename;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Features.Language.PrepareRename
{
    public class BicepPrepareRenameHandler : PrepareRenameHandlerBase
    {
        private readonly ISymbolResolver symbolResolver;
        private readonly DocumentSelectorFactory documentSelectorFactory;

        public BicepPrepareRenameHandler(ISymbolResolver symbolResolver, DocumentSelectorFactory documentSelectorFactory)
        {
            this.symbolResolver = symbolResolver;
            this.documentSelectorFactory = documentSelectorFactory;
        }

        public override Task<RangeOrPlaceholderRange?> Handle(PrepareRenameParams request, CancellationToken cancellationToken)
        {
            var result = BicepRenameHandler.ResolveRenameableSymbol(this.symbolResolver, request.TextDocument.Uri, request.Position);

            var identifier = BicepRenameHandler.GetIdentifier(result.Origin);
            if (identifier is null || !identifier.IsValid)
            {
                throw BicepRenameHandler.CreateRenameError(BicepRenameHandler.CannotRenameSymbolMessage);
            }

            return Task.FromResult<RangeOrPlaceholderRange?>(new PlaceholderRange
            {
                Range = identifier.ToRange(result.Context.LineStarts),
                Placeholder = identifier.IdentifierName
            });
        }

        protected override RenameRegistrationOptions CreateRegistrationOptions(RenameCapability capability, global::OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities.ClientCapabilities clientCapabilities) => new()
        {
            // textDocument/prepareRename is advertised through renameProvider.prepareProvider.
            // BicepRenameHandler owns that advertisement; registering a second document selector here
            // causes VS Code to create a second matching rename provider and call prepareRename twice.
            DocumentSelector = new([]),
            PrepareProvider = false
        };
    }
}
