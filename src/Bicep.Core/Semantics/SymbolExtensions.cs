// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public static class SymbolExtensions
    {
        public static ObjectPropertySyntax? SafeGetBodyProperty(this ResourceSymbol resourceSymbol, string propertyName)
            => resourceSymbol.DeclaringResource.TryGetBody()?.SafeGetPropertyByName(propertyName);

        public static SyntaxBase? SafeGetBodyPropertyValue(this ResourceSymbol resourceSymbol, string propertyName)
            => SafeGetBodyProperty(resourceSymbol, propertyName)?.Value;

        public static ObjectPropertySyntax? SafeGetBodyProperty(this ModuleSymbol moduleSymbol, string propertyName)
            => moduleSymbol.DeclaringModule.TryGetBody()?.SafeGetPropertyByName(propertyName);

        public static SyntaxBase? SafeGetBodyPropertyValue(this ModuleSymbol moduleSymbol, string propertyName)
            => SafeGetBodyProperty(moduleSymbol, propertyName)?.Value;
    }
}