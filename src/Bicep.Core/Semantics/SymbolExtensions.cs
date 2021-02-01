// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public static class SymbolExtensions
    {
        public static SyntaxBase? SafeGetBodyPropertyValue(this ResourceSymbol resourceSymbol, string propertyName) => 
            resourceSymbol.DeclaringResource.TryGetBody()?.SafeGetPropertyByName(propertyName)?.Value;

        public static SyntaxBase? SafeGetBodyPropertyValue(this ModuleSymbol moduleSymbol, string propertyName) => 
            moduleSymbol.DeclaringModule.TryGetBody()?.SafeGetPropertyByName(propertyName)?.Value;
    }
}