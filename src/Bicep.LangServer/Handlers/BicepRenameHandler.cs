// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepRenameHandler : RenameHandlerBase
    {
        internal const string CannotRenameSymbolMessage = "The selected location does not contain a renameable Bicep symbol.";
        private const string InvalidIdentifierMessage = "The new name must be a valid Bicep identifier.";

        private readonly ISymbolResolver symbolResolver;
        private readonly DocumentSelectorFactory documentSelectorFactory;

        public BicepRenameHandler(ISymbolResolver symbolResolver, DocumentSelectorFactory documentSelectorFactory)
        {
            this.symbolResolver = symbolResolver;
            this.documentSelectorFactory = documentSelectorFactory;
        }

        public override Task<WorkspaceEdit?> Handle(RenameParams request, CancellationToken cancellationToken)
        {
            // Re-validate here even though BicepPrepareRenameHandler already checks the location. textDocument/prepareRename
            // is only an optional preflight - clients are not required to call it before textDocument/rename, and some don't.
            // So the rename request must be self-sufficient and cannot rely on prepare having rejected invalid locations.
            var result = ResolveRenameableSymbol(this.symbolResolver, request.TextDocument.Uri, request.Position);

            if (!Lexer.IsValidIdentifier(request.NewName))
            {
                throw CreateRenameError(InvalidIdentifierMessage);
            }

            var textEdits = result.Context.Compilation.GetEntrypointSemanticModel()
                .FindReferences(result.Symbol)
                .Select(GetIdentifier)
                .Where(identifierSyntax => identifierSyntax != null && identifierSyntax.IsValid)
                .Select(identifierSyntax => new TextEdit
                {
                    Range = identifierSyntax!.ToRange(result.Context.LineStarts),
                    NewText = request.NewName
                });

            return Task.FromResult<WorkspaceEdit?>(new WorkspaceEdit
            {
                Changes = new Dictionary<DocumentUri, IEnumerable<TextEdit>>
                {
                    [request.TextDocument.Uri] = textEdits
                }
            });
        }

        internal static SymbolResolutionResult ResolveRenameableSymbol(ISymbolResolver symbolResolver, DocumentUri uri, Position position)
        {
            var result = symbolResolver.ResolveSymbol(uri, position);
            if (result is null || result.Symbol is not DeclaredSymbol || result.Symbol is PropertySymbol)
            {
                throw CreateRenameError(CannotRenameSymbolMessage);
            }

            return result;
        }

        internal static RpcErrorException CreateRenameError(string message) => new(ErrorCodes.RequestFailed, string.Empty, message);

        internal static IdentifierSyntax? GetIdentifier(SyntaxBase syntax)
        {
            switch (syntax)
            {
                case ISymbolReference symbolReference:
                    return symbolReference.Name;

                case INamedDeclarationSyntax declarationSyntax:
                    return declarationSyntax.Name;

                case PropertyAccessSyntax propertyAccessSyntax:
                    return propertyAccessSyntax.PropertyName;

                case ObjectPropertySyntax objectPropertySyntax:
                    return objectPropertySyntax.Key as IdentifierSyntax;

                default:
                    return null;
            }
        }

        protected override RenameRegistrationOptions CreateRegistrationOptions(RenameCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = this.documentSelectorFactory.CreateForBicepAndParams(),
            PrepareProvider = true
        };
    }
}

