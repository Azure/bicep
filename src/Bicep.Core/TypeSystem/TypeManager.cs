// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class TypeManager : ITypeManager
    {
        // stores results of type checks
        private readonly TypeAssignmentVisitor typeAssignmentVisitor;

        public TypeManager(ResourceTypeRegistrar resourceTypeRegistrar, IReadOnlyDictionary<SyntaxBase, Symbol> bindings, IReadOnlyDictionary<SyntaxBase, ImmutableArray<DeclaredSymbol>> cyclesBySyntax)
        {
            // bindings will be modified by name binding after this object is created
            // so we can't make an immutable copy here
            // (using the IReadOnlyDictionary to prevent accidental mutation)
            this.typeAssignmentVisitor = new TypeAssignmentVisitor(resourceTypeRegistrar, this, bindings, cyclesBySyntax);
        }

        private TypeAssignmentVisitor.TypeAssignment GetTypeAssignment(SyntaxBase syntax)
            => typeAssignmentVisitor.GetTypeAssignment(syntax);

        public TypeSymbol GetTypeInfo(SyntaxBase syntax)
            => GetTypeAssignment(syntax).Reference.Type;

        public IEnumerable<Diagnostic> GetAllDiagnostics()
            => typeAssignmentVisitor.GetAllDiagnostics();
    }
}