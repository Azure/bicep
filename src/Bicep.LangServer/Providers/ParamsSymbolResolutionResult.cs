// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.CompilationManager;

namespace Bicep.LanguageServer.Providers
{
    public record ParamsSymbolResolutionResult
    {
        public ParamsSymbolResolutionResult(SyntaxBase origin, Symbol symbol, ParamsCompilationContext context)
        {
            this.Origin = origin;
            this.Symbol = symbol;
            this.Context = context;
        }

        /// <summary>
        /// Gets the resolved symbol
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the syntax node that corresponds to the specified position.
        /// </summary>
        public SyntaxBase Origin { get; }

        public ParamsCompilationContext Context { get; }
    }
}
