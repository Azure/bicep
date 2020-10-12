// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
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

        public BicepSymbolResolver(ILogger<BicepSymbolResolver> logger, ICompilationManager compilationManager)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
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

            // locate the most specific node that intersects with the text coordinate that isn't an identifier
            SyntaxBase? node = context.ProgramSyntax.TryFindMostSpecificNodeInclusive(offset, n => !(n is IdentifierSyntax) && !(n is Token));
            if (node == null)
            {
                // the program node must enclose all locations in the file, so this should not happen
                this.logger.LogError("The symbol resolution request position exceeded the bounds of the file '{Uri}'.", uri);

                return null;
            }

            // resolve symbol (if any)
            Symbol? symbol = context.Compilation.GetEntrypointSemanticModel().GetSymbolInfo(node);
            if (symbol == null)
            {
                // not a symbol
                return null;
            }

            return new SymbolResolutionResult(node, symbol, context);
        }
    }
}
