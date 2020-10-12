// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Navigation;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepRenameHandler : RenameHandler
    {
        private readonly ISymbolResolver symbolResolver;

        public BicepRenameHandler(ISymbolResolver symbolResolver) : base(CreateRegistrationOptions())
        {
            this.symbolResolver = symbolResolver;
        }

        public override Task<WorkspaceEdit> Handle(RenameParams request, CancellationToken cancellationToken)
        {
            var result = this.symbolResolver.ResolveSymbol(request.TextDocument.Uri, request.Position);
            if (result == null || !(result.Symbol is DeclaredSymbol))
            {
                // result is not a symbol or it's a built-in symbol that was not declared by the user (namespaces, functions, for example)
                // symbols that are not declared by the user cannot be renamed
                return Task.FromResult(new WorkspaceEdit());
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

            return Task.FromResult(new WorkspaceEdit
            {
                Changes = new Dictionary<DocumentUri, IEnumerable<TextEdit>>
                {
                    [request.TextDocument.Uri] = textEdits
                }
            });
        }

        private static IdentifierSyntax? GetIdentifier(SyntaxBase syntax)
        {
            switch (syntax)
            {
                case ISymbolReference symbolReference:
                    return symbolReference.Name;

                case IDeclarationSyntax declarationSyntax:
                    return declarationSyntax.Name;

                default:
                    return null;
            }
        }

        private static RenameRegistrationOptions CreateRegistrationOptions() => new RenameRegistrationOptions
        {
            DocumentSelector = DocumentSelectorFactory.Create(),
            PrepareProvider = false
        };
    }
}

