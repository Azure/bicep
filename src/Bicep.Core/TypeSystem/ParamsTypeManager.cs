// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class ParamsTypeManager : IParamsTypeManager
    {
        private readonly ParamsTypeAssignmentVisitor paramsTypeAssignmentVisitor;
        public ParamsTypeManager(ParamBinder binder)
        {
            this.paramsTypeAssignmentVisitor = new ParamsTypeAssignmentVisitor(this, binder);
        }

        // should return the corresponding declared type of parameter "foo" in the Bicep file
        public TypeSymbol GetDeclaredType(SyntaxBase syntax)
        {
            // use ParameterAssignmentSyntax to find Bicep SemanticModel, call GetType() to get ParameterDeclarationSyntax
            throw new NotImplementedException();
        }

        public TypeSymbol GetTypeInfo(SyntaxBase syntax)
            => paramsTypeAssignmentVisitor.GetTypeInfo(syntax);

        public IEnumerable<IDiagnostic> GetAllDiagnostics()
            => paramsTypeAssignmentVisitor.GetAllDiagnostics();
    }
}
