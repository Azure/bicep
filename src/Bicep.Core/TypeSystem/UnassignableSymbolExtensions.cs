// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.SemanticModel;

namespace Bicep.Core.TypeSystem
{
    public static class UnassignableSymbolExtensions
    {
        public static UnassignableTypeSymbol ToUnassignableType(this UnassignableSymbol unassignableSymbol) => new UnassignableTypeSymbol(unassignableSymbol.GetDiagnostics());
    }
}

