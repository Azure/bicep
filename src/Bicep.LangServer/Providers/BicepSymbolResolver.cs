// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Utils;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Providers
{
    public class BicepSymbolResolver : ISymbolResolver
    {
        private readonly ILogger<BicepSymbolResolver> logger;
        private readonly ICompilationManager compilationManager;
        private readonly IParamsCompilationManager paramsCompilationManager;

        public BicepSymbolResolver(ILogger<BicepSymbolResolver> logger, ICompilationManager compilationManager, IParamsCompilationManager paramsCompilationManager)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.paramsCompilationManager = paramsCompilationManager; 
        }

        public SymbolResolutionResult? ResolveSymbol(DocumentUri uri, Position position)
        {
            var context = this.compilationManager.GetCompilation(uri);
            if (context == null)
            {
                // we have not yet compiled this document, which shouldn't really happen
                this.logger.LogError("The symbol resolution request arrived before the file {Uri} could be compiled.", uri);

                return null;
            }

            // convert text coordinates
            int offset = PositionHelper.GetOffset(context.LineStarts, position);
            var semanticModel = context.Compilation.GetEntrypointSemanticModel();

            // locate the most specific node that can be bound as a symbol
            var node = context.ProgramSyntax.TryFindMostSpecificNodeInclusive(
                offset,
                n => n is not IdentifierSyntax && n is not Token);

            if (node is null)
            {
                // the program node must enclose all locations in the file, so this should not happen
                this.logger.LogError("The symbol resolution request position exceeded the bounds of the file '{Uri}'.", uri);

                return null;
            }

            if (semanticModel.GetSymbolInfo(node) is { } symbol)
            {
                return new SymbolResolutionResult(node, symbol, context);
            }

            return null;
        }

        public ParamsSymbolResolutionResult? ResolveParamsSymbol(DocumentUri uri, Position position)
        {
            var context = this.paramsCompilationManager.GetCompilation(uri);
            if (context == null)
            {
                // we have not yet compiled this document, which shouldn't really happen
                this.logger.LogError("The symbol resolution request arrived before the file {Uri} could be compiled.", uri);

                return null;
            }

            // convert text coordinates
            int offset = PositionHelper.GetOffset(context.LineStarts, position);
            var semanticModel = context.ParamsSemanticModel;

            // locate the most specific node that can be bound as a symbol
            var node = context.ProgramSyntax.TryFindMostSpecificNodeInclusive(
                offset,
                n => n is not IdentifierSyntax && n is not Token);

            if (node is null)
            {
                // the program node must enclose all locations in the file, so this should not happen
                this.logger.LogError("The symbol resolution request position exceeded the bounds of the file '{Uri}'.", uri);

                return null;
            }

            if (semanticModel.ParamBinder.GetSymbolInfo(node) is { } symbol)
            {
                return new ParamsSymbolResolutionResult(node, symbol, context);
            }

            return null;
        }
    }
}
