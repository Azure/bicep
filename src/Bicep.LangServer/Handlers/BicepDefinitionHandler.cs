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
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;
using System.Linq;

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
                // Currently we only definition handler for a non symbol bound to implement module path goto.
                // try to resolve module path syntax from given offset using syntax matching.
                int offset = PositionHelper.GetOffset(context.LineStarts, request.Position);
                var matchingNodes = SyntaxMatcher.FindNodesMatchingOffset(context.Compilation.SourceFileGrouping.EntryPoint.ProgramSyntax, offset);
                if (SyntaxMatcher.IsTailMatch<ModuleDeclarationSyntax, StringSyntax, Token>(
                                matchingNodes, (moduleSyntax, stringSyntax, token) => moduleSyntax.Path == stringSyntax && token.Type == TokenType.StringComplete)
                    && matchingNodes[^1] is Token stringToken)
                {
                    // goto beginning of the module file.
                    return GetModuleDefinitionLocationAsync(
                        request.TextDocument.Uri.ToUri(),
                        stringToken.Text, stringToken, context,
                        new Range() { Start = new Position(0, 0), End = new Position(0, 0) });
                }
                return Task.FromResult(new LocationOrLocationLinks());
            }
            else if (result.Symbol is DeclaredSymbol declaration)
            {
                return DeclaredDefinitionLocationAsync(request, result, declaration);
            }
            else if (result.Origin is ObjectPropertySyntax objectPropertySyntax)
            {
                // Currently only used for module param goto
                int offset = PositionHelper.GetOffset(context.LineStarts, request.Position);
                var matchingNodes = SyntaxMatcher.FindNodesMatchingOffset(context.Compilation.SourceFileGrouping.EntryPoint.ProgramSyntax, offset);
                if (matchingNodes[1] is ModuleDeclarationSyntax moduleDeclarationSyntax)
                {
                    var propertyAccesses = matchingNodes.OfType<ObjectPropertySyntax>()
                    .Select(node => node.TryGetKeyText())
                    .Where(s => s != null).ToList();
                    // only two level of traversals: mod.outputs.<outputName> or mod.params.<parameterName>
                    if (propertyAccesses.Count == 2)
                    {
                        return GetModuleSymbolLocationAsync(request, result, context, moduleDeclarationSyntax, propertyAccesses[0]!, propertyAccesses[1]!);
                    }
                }
            }
            else if (result.Symbol is PropertySymbol)
            {
                var semanticModel = context.Compilation.GetEntrypointSemanticModel();

                // Find the underlying VariableSyntax being accessed
                var syntax = result.Origin;
                var propertyAccesses = new List<string>();
                while (syntax is PropertyAccessSyntax propertyAccessSyntax)
                {
                    propertyAccesses.Insert(0, propertyAccessSyntax.PropertyName.IdentifierName);
                    syntax = propertyAccessSyntax.BaseExpression;
                }

                if (syntax is VariableAccessSyntax ancestor
                    && semanticModel.GetSymbolInfo(ancestor) is DeclaredSymbol ancestorSymbol)
                {
                    // If the symbol is a module, we need to redirect the user to the module file
                    // note: module.name doesn't follow this: it should refer to the declaration of the module in the current file
                    if (ancestorSymbol.DeclaringSyntax is ModuleDeclarationSyntax moduleDeclarationSyntax
                    && propertyAccesses.Count == 2)
                    {
                        return GetModuleSymbolLocationAsync(request, result, context, moduleDeclarationSyntax, propertyAccesses[0], propertyAccesses[1]);
                    }

                    // Otherwise, we redirect user to the specified module, variable, or resource declaration
                    if (GetObjectSyntaxFromDeclaration(ancestorSymbol.DeclaringSyntax) is ObjectSyntax objectSyntax
                        && ObjectSyntaxExtensions.SafeGetPropertyByNameRecursive(objectSyntax, propertyAccesses.ToArray()) is ObjectPropertySyntax resultingSyntax)
                    {
                        return Task.FromResult(new LocationOrLocationLinks(new LocationOrLocationLink(new LocationLink
                        {
                            OriginSelectionRange = result.Origin.ToRange(result.Context.LineStarts),
                            TargetUri = request.TextDocument.Uri,
                            TargetRange = resultingSyntax.ToRange(result.Context.LineStarts),
                            TargetSelectionRange = resultingSyntax.ToRange(result.Context.LineStarts)
                        })));
                    }
                }
                return Task.FromResult(new LocationOrLocationLinks());
            }

            return Task.FromResult(new LocationOrLocationLinks());
        }

        private Task<LocationOrLocationLinks> GetModuleSymbolLocationAsync(
            DefinitionParams request,
            SymbolResolutionResult result,
            CompilationContext context,
            ModuleDeclarationSyntax moduleDeclarationSyntax,
            string propertyType,
            string propertyName)
        {
            if (moduleDeclarationSyntax.TryGetPath() is StringSyntax pathSyntax
            && pathSyntax.StringTokens is ImmutableArray<Token> stringTokens
            && stringTokens.Length == 1
            && stringTokens[0] is Token stringToken
            && stringToken.Type == TokenType.StringComplete
            && fileResolver.TryResolveFilePath(request.TextDocument.Uri.ToUri(), stringToken.Text.Replace("'", string.Empty)) is Uri moduleUri
            && fileResolver.TryFileExists(moduleUri))
            {
                CompilationContext? moduleContext = null;
                // if the file's been opened in the workspace, we should be able to get its compilation
                if (this.compilationManager.GetCompilation(DocumentUri.From(moduleUri)) is CompilationContext existingModuleContext)
                {
                    moduleContext = existingModuleContext;
                }
                // otherwise, we have to add it manually
                else if (fileResolver.TryRead(moduleUri, out var fileContents, out var _))
                {
                    this.compilationManager.UpsertCompilation(DocumentUri.From(moduleUri), 0, fileContents, LanguageConstants.LanguageId);
                    moduleContext = this.compilationManager.GetCompilation(DocumentUri.From(moduleUri));
                }
                if (moduleContext == null)
                {
                    return Task.FromResult(new LocationOrLocationLinks());
                }

                switch (propertyType)
                {
                    case LanguageConstants.ModuleOutputsPropertyName:
                        if (moduleContext.Compilation.GetEntrypointSemanticModel().Root.OutputDeclarations
                        .FirstOrDefault(d => string.Equals(d.Name, propertyName)) is OutputSymbol outputSymbol)
                        {
                            return GetModuleDefinitionLocationAsync(
                                request.TextDocument.Uri.ToUri(),
                                stringToken.Text,
                                result.Origin,
                                context,
                                outputSymbol.DeclaringSyntax.ToRange(moduleContext.LineStarts));
                        }
                        break;
                    case LanguageConstants.ModuleParamsPropertyName:
                        if (moduleContext.Compilation.GetEntrypointSemanticModel().Root.ParameterDeclarations
                        .FirstOrDefault(d => string.Equals(d.Name, propertyName)) is ParameterSymbol parameterSymbol)
                        {
                            return GetModuleDefinitionLocationAsync(
                                request.TextDocument.Uri.ToUri(),
                                stringToken.Text,
                                result.Origin,
                                context,
                                parameterSymbol.DeclaringSyntax.ToRange(moduleContext.LineStarts));
                        }
                        break;
                }

            }
            return Task.FromResult(new LocationOrLocationLinks());
        }

        private static ObjectSyntax? GetObjectSyntaxFromDeclaration(SyntaxBase syntax) => syntax switch
        {
            ResourceDeclarationSyntax resourceDeclarationSyntax when resourceDeclarationSyntax.TryGetBody() is ObjectSyntax objectSyntax => objectSyntax,
            ModuleDeclarationSyntax moduleDeclarationSyntax when moduleDeclarationSyntax.TryGetBody() is ObjectSyntax objectSyntax => objectSyntax,
            VariableDeclarationSyntax variableDeclarationSyntax when variableDeclarationSyntax.Value is ObjectSyntax objectSyntax => objectSyntax,
            _ => null,
        };

        private static Task<LocationOrLocationLinks> DeclaredDefinitionLocationAsync(DefinitionParams request, SymbolResolutionResult result, DeclaredSymbol declaration)
        {
            return Task.FromResult(new LocationOrLocationLinks(new LocationOrLocationLink(new LocationLink
            {
                // source of the link
                OriginSelectionRange = result.Origin.ToRange(result.Context.LineStarts),
                TargetUri = request.TextDocument.Uri,

                // entire span of the declaredSymbol
                TargetRange = declaration.DeclaringSyntax.ToRange(result.Context.LineStarts),
                TargetSelectionRange = declaration.NameSyntax.ToRange(result.Context.LineStarts)
            })));
        }

        private async Task<LocationOrLocationLinks> GetModuleDefinitionLocationAsync(
            Uri requestDocumentUri,
            string path,
            SyntaxBase originalSelectionSyntax,
            CompilationContext context,
            Range targetTange)
        {
            if (fileResolver.TryResolveFilePath(requestDocumentUri, path.Replace("'", string.Empty)) is Uri moduleUri
            && fileResolver.TryFileExists(moduleUri))
            {
                return await Task.FromResult(new LocationOrLocationLinks(new LocationOrLocationLink(new LocationLink
                {
                    OriginSelectionRange = originalSelectionSyntax.ToRange(context.LineStarts),
                    TargetUri = DocumentUri.From(moduleUri),
                    TargetRange = targetTange,
                    TargetSelectionRange = targetTange
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
