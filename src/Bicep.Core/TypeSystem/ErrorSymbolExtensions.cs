// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;

namespace Bicep.Core.TypeSystem
{
    public static class ErrorSymbolExtensions
    {
        public static ErrorType ToErrorType(this ErrorSymbol errorSymbol)
            => ErrorType.Create(errorSymbol.GetDiagnostics());

        public static bool IsError(this TypeSymbol type) => type.TypeKind == TypeKind.Error;

        public static bool IsError(this Symbol symbol) => symbol.Kind == SymbolKind.Error;
    }
}

