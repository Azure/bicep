// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System;
using Bicep.Core.Parsing;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepDefinitionHandler : DefinitionHandlerBase
    {
        private readonly ISymbolResolver symbolResolver;

        private readonly ICompilationManager compilationManager;

        private readonly IFileResolver fileResolver;

        public BicepDefinitionHandler(ISymbolResolver symbolResolver, ICompilationManager compilationManager, IFileResolver fileResolver) : base()
        {
            this.symbolResolver = symbolResolver;
            this.compilationManager = compilationManager;
            this.fileResolver = fileResolver;
        }

        public override Task<LocationOrLocationLinks> Handle(DefinitionParams request, CancellationToken cancellationToken)
        {
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);
            if (context == null)
            {
                return Task.FromResult(new LocationOrLocationLinks());
            }
            var result = this.symbolResolver.ResolveSymbol(request.TextDocument.Uri, request.Position);
            if (result == null)
            {
                // try to resolve module path syntax from given offset
                int offset = PositionHelper.GetOffset(context.LineStarts, request.Position);
                var matchingNodes = SyntaxMatcher.FindNodesMatchingOffset(context.Compilation.SourceFileGrouping.EntryPoint.ProgramSyntax, offset);
                if (SyntaxMatcher.IsTailMatch<ModuleDeclarationSyntax, StringSyntax, Token>(
                                matchingNodes, (moduleSyntax, stringSyntax, token) => moduleSyntax.Path == stringSyntax && token.Type == TokenType.StringComplete)
                    && matchingNodes[^1] is Token stringToken)
                {
                    return GetModuleDefinitionLocationAsync(request.TextDocument.Uri.ToUri(), stringToken.Text, stringToken, context);
                }
                return Task.FromResult(new LocationOrLocationLinks());
            }
            if (result.Symbol is PropertySymbol)
            {
                var semanticModel = context.Compilation.GetEntrypointSemanticModel();

                // Find the underlying VariableSyntax being accessed
                var syntax = result.Origin;
                var propertyAccesses = new Stack<string>();
                while (syntax is PropertyAccessSyntax propertyAccessSyntax)
                {
                    propertyAccesses.Push(propertyAccessSyntax.PropertyName.IdentifierName);
                    syntax = propertyAccessSyntax.BaseExpression;
                }

                if (syntax is VariableAccessSyntax ancestor
                    && semanticModel.GetSymbolInfo(ancestor) is DeclaredSymbol ancestorSymbol)
                {
                    // If the symbol is a module, we need to redirect the user to the module file
                    // note: module.name, should refer to the declaration of the module in the current file
                    if (ancestorSymbol.DeclaringSyntax is ModuleDeclarationSyntax moduleDeclarationSyntax
                    && !string.Equals(propertyAccesses.Peek(), LanguageConstants.ResourceNamePropertyName)
                    && moduleDeclarationSyntax.Path is StringSyntax pathStringSyntax
                    && pathStringSyntax.StringTokens is ImmutableArray<Token> stringTokens
                    && stringTokens.Length == 1
                    && stringTokens[0] is Token stringToken
                    && stringToken.Type == TokenType.StringComplete)
                    {
                        // TODO direct user to the output that they clicked on instead of head of file
                        return GetModuleDefinitionLocationAsync(request.TextDocument.Uri.ToUri(), stringToken.Text, result.Origin, context);
                    }

                    // Otherwise, we redirect user to the specified parameter, variable, resource or output declaration
                    // TODO direct user to the property that they clicked on instead of the declaration of the symbol
                    return Task.FromResult(new LocationOrLocationLinks(new LocationOrLocationLink(new LocationLink
                    {
                        OriginSelectionRange = result.Origin.ToRange(result.Context.LineStarts),
                        TargetUri = request.TextDocument.Uri,
                        TargetRange = ancestorSymbol.DeclaringSyntax.ToRange(result.Context.LineStarts),
                        TargetSelectionRange = ancestorSymbol.NameSyntax.ToRange(result.Context.LineStarts)
                    })));
                }
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

        private async Task<LocationOrLocationLinks> GetModuleDefinitionLocationAsync(Uri requestDocumentUri, string path, SyntaxBase originalSelectionSyntax, CompilationContext context)
        {
            if (fileResolver.TryResolveFilePath(requestDocumentUri, path.Replace("'", string.Empty)) is Uri moduleUri
            &&  fileResolver.TryFileExists(moduleUri))
            {
                return await Task.FromResult(new LocationOrLocationLinks(new LocationOrLocationLink(new LocationLink
                {
                    OriginSelectionRange = originalSelectionSyntax.ToRange(context.LineStarts),
                    TargetUri = DocumentUri.From(moduleUri),
                    TargetRange = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range() { Start = new Position(0, 0), End = new Position(0, 0) },
                    TargetSelectionRange = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range() { Start = new Position(0, 0), End = new Position(0, 0) }
                })));
            }
            return await Task.FromResult(new LocationOrLocationLinks());
        }

        protected override DefinitionRegistrationOptions CreateRegistrationOptions(DefinitionCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = DocumentSelectorFactory.Create()
        };
    }
}
