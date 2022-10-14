// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;

namespace Bicep.Core.TypeSystem
{
    public class TypeManager : ITypeManager
    {
        // stores results of type checks
        private readonly TypeAssignmentVisitor typeAssignmentVisitor;
        private readonly DeclaredTypeManager declaredTypeManager;

        public TypeManager(IFeatureProvider features, IBinder binder, IFileResolver fileResolver, BicepSourceFileKind kind)
        {
            // bindings will be modified by name binding after this object is created
            // so we can't make an immutable copy here
            // (using the IReadOnlyDictionary to prevent accidental mutation)
            this.typeAssignmentVisitor = new TypeAssignmentVisitor(this, features, binder, fileResolver, kind);
            this.declaredTypeManager = new DeclaredTypeManager(this, binder);
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

        public object? GetMatchedFunctionResultValue(FunctionCallSyntaxBase syntax)
            => typeAssignmentVisitor.GetMatchedFunctionResultValue(syntax);

    }
}
