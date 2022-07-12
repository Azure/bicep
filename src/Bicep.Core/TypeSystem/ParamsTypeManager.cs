// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class ParamsTypeManager : IParamsTypeManager
    {
        private readonly ParamsTypeAssignmentVisitor paramsTypeAssignmentVisitor;
        public ParamsTypeManager(IBinder binder, IFileResolver fileResolver)
        {
            this.paramsTypeAssignmentVisitor = new ParamsTypeAssignmentVisitor(this, binder, fileResolver);
        }

        // should return the corresponding declared type of parameter "foo" in the Bicep file
        public TypeSymbol GetDeclaredType(SyntaxBase syntax)
        {
            // use ParameterAssignmentSyntax to find Bicep SemanticModel, call GetType() to get ParameterDeclarationSyntax
            throw new NotImplementedException();
        }

        // should return type "integer"
        public TypeSymbol GetTypeInfo(SyntaxBase syntax)
            => paramsTypeAssignmentVisitor.GetTypeInfo(syntax);

        public IEnumerable<IDiagnostic> GetAllDiagnostics()
            => paramsTypeAssignmentVisitor.GetAllDiagnostics();


        // var foo = 1
        // set bp on line 28
            // typeManager
            // use Bicep build to debug it

    }
}
