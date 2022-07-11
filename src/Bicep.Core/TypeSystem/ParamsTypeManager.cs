// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class ParamsTypeManager : IParamsTypeManager
    {
        //private readonly TypeAssignmentVisitor typeAssignmentVisitor;

        public ParamsTypeManager(IBinder binder, IFileResolver fileResolver)
        {
            // 
            this.typeAssignmentVisitor = new TypeAssignmentVisitor(this, binder, fileResolver);
            this.declaredTypeManager = new DeclaredTypeManager(this, binder);
        }

        // should return the corresponding declared type of parameter "foo" in the Bicep file
        public TypeSymbol GetDeclaredType(SyntaxBase syntax)
            => declaredTypeManager.GetDeclaredType(syntax);

        // should return type "integer"
        public TypeSymbol GetType(SyntaxBase syntax)
            => typeAssignmentVisitor.GetTypeInfo(syntax);
    }
}