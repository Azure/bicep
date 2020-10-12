// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class TypeManager : ITypeManager
    {
        // stores results of type checks
        private readonly TypeAssignmentVisitor typeAssignmentVisitor;

        public TypeManager(IResourceTypeProvider resourceTypeProvider, IReadOnlyDictionary<SyntaxBase, Symbol> bindings, IReadOnlyDictionary<DeclaredSymbol, ImmutableArray<DeclaredSymbol>> cyclesBySymbol, SyntaxHierarchy hierarchy)
        {
            // bindings will be modified by name binding after this object is created
            // so we can't make an immutable copy here
            // (using the IReadOnlyDictionary to prevent accidental mutation)
            this.typeAssignmentVisitor = new TypeAssignmentVisitor(resourceTypeProvider, this, bindings, cyclesBySymbol, hierarchy);
        }

        public TypeSymbol GetTypeInfo(SyntaxBase syntax)
            => typeAssignmentVisitor.GetTypeInfo(syntax);

        public TypeSymbol? GetDeclaredType(SyntaxBase syntax)
            => typeAssignmentVisitor.GetDeclaredType(syntax);

        public IEnumerable<Diagnostic> GetAllDiagnostics()
            => typeAssignmentVisitor.GetAllDiagnostics();
    }
}