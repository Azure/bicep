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
using Bicep.Core;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;
using System.Linq;
using Bicep.Core.Workspaces;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepDefinitionHandler : DefinitionHandlerBase
    {
        private readonly ISymbolResolver symbolResolver;

        private readonly ICompilationManager compilationManager;

        private readonly IFileResolver fileResolver;

        private readonly IWorkspace workspace;

        public BicepDefinitionHandler(ISymbolResolver symbolResolver, ICompilationManager compilationManager, IFileResolver fileResolver, IWorkspace workspace) : base()
        {
            this.symbolResolver = symbolResolver;
            this.compilationManager = compilationManager;
            this.fileResolver = fileResolver;
            this.workspace = workspace;
        }

        public override Task<LocationOrLocationLinks> Handle(DefinitionParams request, CancellationToken cancellationToken)
        {
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);
            if (context is null)
            {
                return Task.FromResult(new LocationOrLocationLinks());
            }
            var result = this.symbolResolver.ResolveSymbol(request.TextDocument.Uri, request.Position);
            // No parent Symbol: ad hoc syntax matching
            if (result is null)
            {
                return HandleUnboundSymbolLocationAsync(request, context);
            }
            // Declared symbols: go to definition
            else if (result.Symbol is DeclaredSymbol declaration)
            {
                return HandleDeclaredDefinitionLocationAsync(request, result, declaration);
            }
            // Object property: currently only used for module param goto
            else if (result.Origin is ObjectPropertySyntax objectPropertySyntax)
            {
                return HandleObjectPropertyLocationAsync(request, result, context);
            }
            // Used for module (name), variable or resource property access
            else if (result.Symbol is PropertySymbol)
            {
                return HandlePropertyLocationAsync(request, result, context);
            }
            return Task.FromResult(new LocationOrLocationLinks()); 
        }

        protected override DefinitionRegistrationOptions CreateRegistrationOptions(DefinitionCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = DocumentSelectorFactory.Create()
        };

        private Task<LocationOrLocationLinks> HandleUnboundSymbolLocationAsync(DefinitionParams request, CompilationContext context)
        {
            // Currently we only definition handler for a non symbol bound to implement module path goto.
            // try to resolve module path syntax from given offset using tail matching.
            int offset = PositionHelper.GetOffset(context.LineStarts, request.Position);
            var matchingNodes = SyntaxMatcher.FindNodesMatchingOffset(context.Compilation.SourceFileGrouping.EntryPoint.ProgramSyntax, offset);
            if (SyntaxMatcher.IsTailMatch<ModuleDeclarationSyntax, StringSyntax, Token>(
                matchingNodes,
                (moduleSyntax, stringSyntax, token) => moduleSyntax.Path == stringSyntax && token.Type == TokenType.StringComplete)
                && matchingNodes[^3] is ModuleDeclarationSyntax moduleDeclarationSyntax
                && matchingNodes[^2] is StringSyntax stringToken
                && context.Compilation.SourceFileGrouping.LookUpModuleSourceFile(moduleDeclarationSyntax) is ISourceFile sourceFile)
            {
                // goto beginning of the module file.
                return GetModuleDefinitionLocationAsync(
                    sourceFile.FileUri,
                    stringToken,
                    context,
                    new Range { Start = new Position(0, 0), End = new Position(0, 0) });
            }
            // all other unbound syntax nodes return no 
            return Task.FromResult(new LocationOrLocationLinks());
        }

        private static Task<LocationOrLocationLinks> HandleDeclaredDefinitionLocationAsync(DefinitionParams request, SymbolResolutionResult result, DeclaredSymbol declaration)
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

        private Task<LocationOrLocationLinks> HandleObjectPropertyLocationAsync(DefinitionParams request, SymbolResolutionResult result, CompilationContext context)
        {
            int offset = PositionHelper.GetOffset(context.LineStarts, request.Position);
            var matchingNodes = SyntaxMatcher.FindNodesMatchingOffset(context.Compilation.SourceFileGrouping.EntryPoint.ProgramSyntax, offset);
            // matchingNodes[0] should be ProgramSyntax
            if (matchingNodes[1] is ModuleDeclarationSyntax moduleDeclarationSyntax)
            {
                // capture the property accesses leading to this specific property access
                var propertyAccesses = matchingNodes.OfType<ObjectPropertySyntax>()
                    .Select(node => node.TryGetKeyText())
                    .OfType<string>().ToList();
                // only two level of traversals: mod.outputs.<outputName> or mod.params.<parameterName>
                if (propertyAccesses.Count == 2)
                {
                    return GetModuleSymbolLocationAsync(result, context, moduleDeclarationSyntax, propertyAccesses[0], propertyAccesses[1]);
                }
            }
            return Task.FromResult(new LocationOrLocationLinks()); 
        }

        private Task<LocationOrLocationLinks> HandlePropertyLocationAsync(DefinitionParams request, SymbolResolutionResult result, CompilationContext context)
        {
                var semanticModel = context.Compilation.GetEntrypointSemanticModel();

                // Find the underlying VariableSyntax being accessed
                var syntax = result.Origin;
                var propertyAccesses = new List<string>();
                while (syntax is PropertyAccessSyntax propertyAccessSyntax)
                {
                    // since we are traversing bottom up, add this access to the beginning of the list
                    propertyAccesses.Insert(0, propertyAccessSyntax.PropertyName.IdentifierName);
                    syntax = propertyAccessSyntax.BaseExpression;
                }

                if (syntax is VariableAccessSyntax ancestor
                    && semanticModel.GetSymbolInfo(ancestor) is DeclaredSymbol ancestorSymbol)
                {
                    // If the symbol is a module, we need to redirect the user to the module file
                    // note: module.name doesn't follow this: it should refer to the declaration of the module in the current file, like regular variable and resource property accesses
                    if (propertyAccesses.Count == 2
                    && ancestorSymbol.DeclaringSyntax is ModuleDeclarationSyntax moduleDeclarationSyntax)
                    {
                        return GetModuleSymbolLocationAsync(result, context, moduleDeclarationSyntax, propertyAccesses[0], propertyAccesses[1]);
                    }

                    // Otherwise, we redirect user to the specified module, variable, or resource declaration
                    if (GetObjectSyntaxFromDeclaration(ancestorSymbol.DeclaringSyntax) is ObjectSyntax objectSyntax
                        && ObjectSyntaxExtensions.SafeGetPropertyByNameRecursive(objectSyntax, propertyAccesses) is ObjectPropertySyntax resultingSyntax)
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

        private Task<LocationOrLocationLinks> GetModuleSymbolLocationAsync(
            SymbolResolutionResult result,
            CompilationContext context,
            ModuleDeclarationSyntax moduleDeclarationSyntax,
            string propertyType,
            string propertyName)
        {
            if (context.Compilation.SourceFileGrouping.LookUpModuleSourceFile(moduleDeclarationSyntax) is BicepFile bicepFile
            && context.Compilation.GetSemanticModel(bicepFile) is SemanticModel moduleModel)
            {
                switch (propertyType)
                {
                    case LanguageConstants.ModuleOutputsPropertyName:
                        if (moduleModel.Root.OutputDeclarations
                            .FirstOrDefault(d => string.Equals(d.Name, propertyName)) is OutputSymbol outputSymbol)
                        {
                            return GetModuleDefinitionLocationAsync(
                                bicepFile.FileUri,
                                result.Origin,
                                context,
                                outputSymbol.DeclaringSyntax.ToRange(bicepFile.LineStarts));
                        }
                        break;
                    case LanguageConstants.ModuleParamsPropertyName:
                        if (moduleModel.Root.ParameterDeclarations
                            .FirstOrDefault(d => string.Equals(d.Name, propertyName)) is ParameterSymbol parameterSymbol)
                        {
                            return GetModuleDefinitionLocationAsync(
                                bicepFile.FileUri,
                                result.Origin,
                                context,
                                parameterSymbol.DeclaringSyntax.ToRange(bicepFile.LineStarts));
                        }
                        break;
                }

            }
            return Task.FromResult(new LocationOrLocationLinks());
        }

        private async Task<LocationOrLocationLinks> GetModuleDefinitionLocationAsync(
            Uri moduleUri,
            SyntaxBase originalSelectionSyntax,
            CompilationContext context,
            Range targetTange)
        {
            return await Task.FromResult(new LocationOrLocationLinks(new LocationOrLocationLink(new LocationLink
            {
                OriginSelectionRange = originalSelectionSyntax.ToRange(context.LineStarts),
                TargetUri = DocumentUri.From(moduleUri),
                TargetRange = targetTange,
                TargetSelectionRange = targetTange
            })));
        }

        private static ObjectSyntax? GetObjectSyntaxFromDeclaration(SyntaxBase syntax) => syntax switch
        {
            ResourceDeclarationSyntax resourceDeclarationSyntax when resourceDeclarationSyntax.TryGetBody() is ObjectSyntax objectSyntax => objectSyntax,
            ModuleDeclarationSyntax moduleDeclarationSyntax when moduleDeclarationSyntax.TryGetBody() is ObjectSyntax objectSyntax => objectSyntax,
            VariableDeclarationSyntax variableDeclarationSyntax when variableDeclarationSyntax.Value is ObjectSyntax objectSyntax => objectSyntax,
            _ => null,
        };
    }
}
