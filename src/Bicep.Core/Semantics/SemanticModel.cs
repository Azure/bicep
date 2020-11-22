// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class SemanticModel
    {
        private readonly ITypeManager typeManager;
        private readonly ImmutableDictionary<SyntaxBase, Symbol> bindings;
        private readonly Lazy<EmitLimitationInfo> emitLimitationInfoLazy;

        public SemanticModel(FileSymbol root, ITypeManager typeManager, IDictionary<SyntaxBase, Symbol> bindings, ResourceScopeType targetScope)
        {
            this.Root = root;
            this.typeManager = typeManager;
            this.TargetScope = targetScope;
            this.bindings = bindings.ToImmutableDictionary();
            this.emitLimitationInfoLazy = new Lazy<EmitLimitationInfo>(() => EmitLimitationCalculator.Calculate(this));
        }

        public IResourceTypeProvider ResourceTypeProvider => this.typeManager.ResourceTypeProvider;

        public EmitLimitationInfo EmitLimitationInfo => emitLimitationInfoLazy.Value;

        /// <summary>
        /// Gets all the parser and lexer diagnostics unsorted. Does not include diagnostics from the semantic model.
        /// </summary>
        public IEnumerable<Diagnostic> GetParseDiagnostics() => this.Root.Syntax.GetParseDiagnostics();

        /// <summary>
        /// Gets all the semantic diagnostics unsorted. Does not include parser and lexer diagnostics.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<Diagnostic> GetSemanticDiagnostics()
        {
            var diagnosticWriter = ToListDiagnosticWriter.Create();
            
            var visitor = new SemanticDiagnosticVisitor(diagnosticWriter);
            visitor.Visit(this.Root);

            var typeValidationDiagnostics = typeManager.GetAllDiagnostics();
            diagnosticWriter.WriteMultiple(typeValidationDiagnostics);

            diagnosticWriter.WriteMultiple(EmitLimitationInfo.Diagnostics);

            return diagnosticWriter.GetDiagnostics();
        }

        /// <summary>
        /// Gets all the diagnostics sorted by span position ascending. This includes lexer, parser, and semantic diagnostics.
        /// </summary>
        public IEnumerable<Diagnostic> GetAllDiagnostics() => GetParseDiagnostics()
            .Concat(GetSemanticDiagnostics())
            .OrderBy(diag => diag.Span.Position);

        public bool HasErrors()
            => GetAllDiagnostics().Any(x => x.Level == DiagnosticLevel.Error);

        public TypeSymbol GetTypeInfo(SyntaxBase syntax) => this.typeManager.GetTypeInfo(syntax);

        public TypeSymbol? GetDeclaredType(SyntaxBase syntax) => this.typeManager.GetDeclaredType(syntax);

        public DeclaredTypeAssignment? GetDeclaredTypeAssignment(SyntaxBase syntax) => this.typeManager.GetDeclaredTypeAssignment(syntax);

        /// <summary>
        /// Returns the symbol that was bound to the specified syntax node. Will return null for syntax nodes that never get bound to symbols. Otherwise,
        /// a symbol will always be returned. Binding failures are represented with a non-null error symbol.
        /// </summary>
        /// <param name="syntax">the syntax node</param>
        public Symbol? GetSymbolInfo(SyntaxBase syntax) => this.bindings.TryGetValue(syntax);

        /// <summary>
        /// Returns all syntax nodes that represent a reference to the specified symbol. This includes the definitions of the symbol as well.
        /// Unusued declarations will return 1 result. Unused and undeclared symbols (functions, namespaces, for example) may return an empty list.
        /// </summary>
        /// <param name="symbol">The symbol</param>
        public IEnumerable<SyntaxBase> FindReferences(Symbol symbol) => this.bindings
            .Where(binding => ReferenceEquals(binding.Value, symbol))
            .Select(binding => binding.Key);

        /// <summary>
        /// Gets the file that was compiled.
        /// </summary>
        public FileSymbol Root { get; }

        public ResourceScopeType TargetScope { get; }
    }
}
