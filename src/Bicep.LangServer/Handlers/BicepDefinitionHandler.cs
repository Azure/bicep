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
                int offset = PositionHelper.GetOffset(context.LineStarts, request.Position);
                var matchingNodes = SyntaxMatcher.FindNodesMatchingOffset(context.Compilation.SourceFileGrouping.EntryPoint.ProgramSyntax, offset);
                if (!SyntaxMatcher.IsTailMatch<ModuleDeclarationSyntax, StringSyntax, Token>(
                    matchingNodes, (moduleSyntax, stringSyntax, token) => moduleSyntax.Path == stringSyntax && token.Type == TokenType.StringComplete))
                {
                    return Task.FromResult(new LocationOrLocationLinks());
                }
                if (matchingNodes[^1] is Token stringToken &&
                    stringToken.Text is string modulePath &&
                    fileResolver.TryResolveFilePath(request.TextDocument.Uri.ToUri(), modulePath.Replace("'", string.Empty)) is Uri moduleUri)
                {
                    return Task.FromResult(new LocationOrLocationLinks(new LocationOrLocationLink(new LocationLink
                    {
                        OriginSelectionRange = stringToken.ToRange(context.LineStarts),
                        TargetUri = DocumentUri.From(moduleUri),
                        TargetRange = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range() {Start = new Position(0, 0), End = new Position(0, 0)},
                        TargetSelectionRange = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range() {Start = new Position(0, 0), End = new Position(0, 0)}
                    })));
                }
                return Task.FromResult(new LocationOrLocationLinks());
            }
            
            if (result.Symbol is PropertySymbol)
            {
                var semanticModel = context.Compilation.GetEntrypointSemanticModel();
                // Find the underlying VariableSyntax being accessed
                var syntax = result.Origin;
                while (syntax is PropertyAccessSyntax propertyAccessSyntax)
                {
                    syntax = propertyAccessSyntax.BaseExpression;
                }
                if (syntax is VariableAccessSyntax ancestor 
                    && semanticModel.GetSymbolInfo(ancestor) is DeclaredSymbol ancestorSymbol)
                {
                    switch (ancestorSymbol.DeclaringSyntax)
                    {
                        case VariableDeclarationSyntax variableDeclarationSyntax:
                            return Task.FromResult(new LocationOrLocationLinks(new LocationOrLocationLink(new LocationLink
                            {
                                // source of the link
                                OriginSelectionRange = result.Origin.ToRange(result.Context.LineStarts),
                                TargetUri = request.TextDocument.Uri,

                                // entire span of the variable
                                TargetRange = ancestorSymbol.DeclaringSyntax.ToRange(result.Context.LineStarts),

                                // span of the variable name
                                TargetSelectionRange = ancestorSymbol.NameSyntax.ToRange(result.Context.LineStarts)
                            })));
                    }
                    
                }
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
