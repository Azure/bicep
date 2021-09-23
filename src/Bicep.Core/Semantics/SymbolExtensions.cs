// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public static class SymbolExtensions
    {
        public static ObjectPropertySyntax? SafeGetBodyProperty(this ResourceSymbol resourceSymbol, string propertyName)
            => resourceSymbol.DeclaringResource.TryGetBody()?.SafeGetPropertyByName(propertyName);

        public static SyntaxBase? SafeGetBodyPropertyValue(this ResourceSymbol resourceSymbol, string propertyName)
            => SafeGetBodyProperty(resourceSymbol, propertyName)?.Value;

        public static ObjectPropertySyntax UnsafeGetBodyProperty(this ResourceSymbol resourceSymbol, string propertyName)
            => resourceSymbol.SafeGetBodyProperty(propertyName) ?? throw new ArgumentException($"Expected resource syntax body to contain property '{propertyName}'");

        public static SyntaxBase UnsafeGetBodyPropertyValue(this ResourceSymbol resourceSymbol, string propertyName)
            => resourceSymbol.SafeGetBodyPropertyValue(propertyName) ?? throw new ArgumentException($"Expected resource syntax body to contain property '{propertyName}'");

        public static ObjectPropertySyntax? SafeGetBodyProperty(this ModuleSymbol moduleSymbol, string propertyName)
            => moduleSymbol.DeclaringModule.TryGetBody()?.SafeGetPropertyByName(propertyName);

        public static SyntaxBase? SafeGetBodyPropertyValue(this ModuleSymbol moduleSymbol, string propertyName)
            => SafeGetBodyProperty(moduleSymbol, propertyName)?.Value;

        public static bool IsSecure(this ParameterSymbol parameterSymbol)
        {
            // local function
            bool isSecure(DecoratorSyntax? value) => value?.Expression is FunctionCallSyntax functionCallSyntax && functionCallSyntax.NameEquals("secure");

            if (parameterSymbol?.DeclaringSyntax is ParameterDeclarationSyntax paramDeclaration)
            {
                return paramDeclaration.Decorators.Any(d => isSecure(d));
            }
            return false;
        }
    }
}
