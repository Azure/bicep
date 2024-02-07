// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.CompilationManager;

namespace Bicep.LanguageServer.Providers
{
    public class SymbolResolutionResult(SyntaxBase origin, Symbol symbol, CompilationContext context)
    {

        /// <summary>
        /// Gets the resolved symbol
        /// </summary>
        public Symbol Symbol { get; } = symbol;

        /// <summary>
        /// Gets the syntax node that corresponds to the specified position.
        /// </summary>
        public SyntaxBase Origin { get; } = origin;

        public CompilationContext Context { get; } = context;
    }
}
