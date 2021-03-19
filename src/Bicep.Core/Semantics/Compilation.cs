// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class Compilation
    {
        private readonly ImmutableDictionary<SyntaxTree, Lazy<SemanticModel>> lazySemanticModelLookup;

        public Compilation(IResourceTypeProvider resourceTypeProvider, SyntaxTreeGrouping syntaxTreeGrouping)
        {
            this.SyntaxTreeGrouping = syntaxTreeGrouping;
            this.ResourceTypeProvider = resourceTypeProvider;
            this.lazySemanticModelLookup = syntaxTreeGrouping.SyntaxTrees.ToImmutableDictionary(
                syntaxTree => syntaxTree,
                syntaxTree => new Lazy<SemanticModel>(() => new SemanticModel(this, syntaxTree)));
        }

        public SyntaxTreeGrouping SyntaxTreeGrouping { get; }

        public IResourceTypeProvider ResourceTypeProvider { get; }

        public SemanticModel GetEntrypointSemanticModel()
            => GetSemanticModel(SyntaxTreeGrouping.EntryPoint);

        public SemanticModel GetSemanticModel(SyntaxTree syntaxTree)
            => this.lazySemanticModelLookup[syntaxTree].Value;

        public IReadOnlyDictionary<SyntaxTree, IEnumerable<IDiagnostic>> GetAllDiagnosticsBySyntaxTree()
            => SyntaxTreeGrouping.SyntaxTrees.ToDictionary(
                syntaxTree => syntaxTree,
                syntaxTree => GetSemanticModel(syntaxTree).GetAllDiagnostics());
    }
}
