// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class TypeManager : ITypeManager
    {
        // stores results of type checks
        private readonly TypeAssignmentVisitor typeAssignmentVisitor;
        private readonly DeclaredTypeManager declaredTypeManager;

        public TypeManager(IResourceTypeProvider resourceTypeProvider, IBinder binder, IFileResolver fileResolver)
        {
            // bindings will be modified by name binding after this object is created
            // so we can't make an immutable copy here
            // (using the IReadOnlyDictionary to prevent accidental mutation)
            this.typeAssignmentVisitor = new TypeAssignmentVisitor(resourceTypeProvider, this, binder, fileResolver);

            this.declaredTypeManager = new DeclaredTypeManager(resourceTypeProvider, this, binder);
        }

        public TypeSymbol GetTypeInfo(SyntaxBase syntax)
            => typeAssignmentVisitor.GetTypeInfo(syntax);

        public TypeSymbol? GetDeclaredType(SyntaxBase syntax)
            => declaredTypeManager.GetDeclaredType(syntax);

        public DeclaredTypeAssignment? GetDeclaredTypeAssignment(SyntaxBase syntax)
            => declaredTypeManager.GetDeclaredTypeAssignment(syntax);

        public IEnumerable<IDiagnostic> GetAllDiagnostics()
            => typeAssignmentVisitor.GetAllDiagnostics();

        public FunctionOverload? GetMatchedFunctionOverload(FunctionCallSyntaxBase syntax)
            => typeAssignmentVisitor.GetMatchedFunctionOverload(syntax);

    }
}
