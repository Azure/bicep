// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.SemanticModel;

namespace Bicep.Core.TypeSystem
{
    public static class ErrorSymbolExtensions
    {
        public static ErrorType ToUnassignableType(this ErrorSymbol unassignableSymbol) => ErrorType.Create(unassignableSymbol.GetDiagnostics());
    }
}

