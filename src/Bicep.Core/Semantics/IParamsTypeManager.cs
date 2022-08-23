// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public interface IParamsTypeManager
    {
        TypeSymbol GetTypeInfo(SyntaxBase syntax); // Gets the type of the syntax.
        TypeSymbol? GetDeclaredType(SyntaxBase syntax); // Gets the type of declared type of syntax.
    }
}
