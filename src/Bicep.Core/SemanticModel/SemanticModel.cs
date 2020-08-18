﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class SemanticModel
    {
        private readonly TypeManager typeManager;

        private readonly ImmutableDictionary<SyntaxBase, Symbol> bindings;

        private readonly SymbolDependencyGraph symbolGraph;

        public SemanticModel(FileSymbol root, TypeManager typeManager, IDictionary<SyntaxBase, Symbol> bindings, SymbolDependencyGraph symbolGraph)
        {
            this.Root = root;
            this.typeManager = typeManager;
            this.bindings = bindings.ToImmutableDictionary();
            this.symbolGraph = symbolGraph;
        }

        /// <summary>
        /// Gets all the parser and lexer diagnostics unsorted. Does not include diagnostics from the semantic model.
        /// </summary>
        public IEnumerable<Diagnostic> GetParseDiagnostics() => this.Root.DeclaringSyntax.GetParseDiagnostics();

        /// <summary>
        /// Gets all the semantic diagnostics unsorted. Does not include parser and lexer diagnostics.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ErrorDiagnostic> GetSemanticDiagnostics()
        {
            var diagnostics = new List<ErrorDiagnostic>();
            var visitor = new SemanticErrorVisitor(diagnostics);
            visitor.Visit(this.Root);

            return diagnostics;
        }

        /// <summary>
        /// Gets all the diagnostics sorted by span position ascending. This includes lexer, parser, and semantic diagnostics.
        /// </summary>
        public IEnumerable<Diagnostic> GetAllDiagnostics() => GetParseDiagnostics()
            .Concat(GetSemanticDiagnostics())
            .OrderBy(diag => diag.Span.Position);

        public Symbol? GetSymbolInfo(SyntaxBase syntax) => this.bindings.TryGetValue(syntax);

        /// <summary>
        /// Gets the file that was compiled.
        /// </summary>
        public FileSymbol Root { get; }

        public ImmutableArray<ResourceSymbol> GetResourceDependencies(ResourceSymbol symbol)
            => symbolGraph.Graph[symbol].Resources;

        public bool RequiresInlining(VariableSymbol symbol)
            => symbolGraph.Graph[symbol].Resources.Any();
    }
}