// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Intermediate;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using System.Collections.Generic;

namespace Bicep.Core.Semantics
{
    public interface ITypeManager
    {
        TypeSymbol GetTypeInfo(SyntaxBase syntax);

        TypeSymbol? GetDeclaredType(SyntaxBase syntax);

        DeclaredTypeAssignment? GetDeclaredTypeAssignment(SyntaxBase syntax);

        IEnumerable<IDiagnostic> GetAllDiagnostics();

        FunctionOverload? GetMatchedFunctionOverload(FunctionCallSyntaxBase syntax);

        Expression? GetMatchedFunctionResultValue(FunctionCallSyntaxBase syntax);
    }
}
