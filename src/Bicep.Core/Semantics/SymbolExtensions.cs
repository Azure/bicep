// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

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

        /// <summary>
        /// Returns the expected argument type for a particular argument position, using function overload information.
        /// </summary>
        /// <param name="functionSymbol">The function symbol to inspect</param>
        /// <param name="argIndex">The index of the function argument</param>
        public static TypeSymbol GetDeclaredArgumentType(this FunctionSymbol functionSymbol, int argIndex)
        {
            // if we have a mix of wildcard and non-wildcard overloads, prioritize the non-wildcard overloads.
            // the wildcards have super generic type definitions, so don't result in helpful completions.
            var overloads = functionSymbol.Overloads.Any(x => x is not FunctionWildcardOverload) ?
                functionSymbol.Overloads.Where(x => x is not FunctionWildcardOverload) :
                functionSymbol.Overloads;

            var argTypes = overloads
                .Where(x => x.MaximumArgumentCount is null || argIndex < x.MaximumArgumentCount)
                .Select(x => argIndex < x.FixedParameters.Length ? x.FixedParameters[argIndex].Type : (x.VariableParameter?.Type ?? LanguageConstants.Never));

            return TypeHelper.CreateTypeUnion(argTypes);
        }
    }
}
