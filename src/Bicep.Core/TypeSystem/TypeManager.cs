// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class TypeManager : ITypeManager
    {
        private readonly IReadOnlyDictionary<SyntaxBase,Symbol> bindings;

        private readonly IReadOnlyDictionary<SyntaxBase, ImmutableArray<DeclaredSymbol>> cyclesBySyntax;

        // stores results of type checks
        private readonly TypeAssignmentVisitor typeAssignmentVisitor;

        public TypeManager(IReadOnlyDictionary<SyntaxBase, Symbol> bindings, IReadOnlyDictionary<SyntaxBase, ImmutableArray<DeclaredSymbol>> cyclesBySyntax)
        {
            // bindings will be modified by name binding after this object is created
            // so we can't make an immutable copy here
            // (using the IReadOnlyDictionary to prevent accidental mutation)
            this.bindings = bindings; 
            this.cyclesBySyntax = cyclesBySyntax;
            this.typeAssignmentVisitor = new TypeAssignmentVisitor(bindings, cyclesBySyntax);
        }

        public TypeSymbol GetTypeInfo(SyntaxBase syntax)
        {
            return typeAssignmentVisitor.GetTypeSymbol(syntax);
        }
    }
}