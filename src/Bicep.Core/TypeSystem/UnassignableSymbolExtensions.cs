// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.SemanticModel;

namespace Bicep.Core.TypeSystem
{
    public static class UnassignableSymbolExtensions
    {
        public static UnassignableTypeSymbol ToErrorType(this UnassignableSymbol errorSymbol) => new UnassignableTypeSymbol(errorSymbol.GetDiagnostics());
    }
}

