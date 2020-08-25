// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.SemanticModel;

namespace Bicep.Core.TypeSystem
{
    public static class ErrorSymbolExtensions
    {
        public static ErrorTypeSymbol ToErrorType(this ErrorSymbol errorSymbol) => new ErrorTypeSymbol(errorSymbol.GetDiagnostics());
    }
}

