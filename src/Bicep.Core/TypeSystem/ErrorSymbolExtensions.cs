// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;

namespace Bicep.Core.TypeSystem
{
    public static class ErrorSymbolExtensions
    {
        public static ErrorType ToErrorType(this ErrorSymbol errorSymbol)
            => ErrorType.Create(errorSymbol.GetDiagnostics());
    }
}

