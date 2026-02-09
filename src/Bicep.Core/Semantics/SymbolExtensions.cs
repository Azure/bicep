// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using static Bicep.Core.Semantics.FunctionOverloadBuilder;

namespace Bicep.Core.Semantics
{
    public static class SymbolExtensions
    {
        public static ObjectPropertySyntax? TryGetBodyProperty(this ResourceSymbol resourceSymbol, string propertyName)
            => resourceSymbol.DeclaringResource.TryGetBody()?.TryGetPropertyByName(propertyName);

        public static SyntaxBase? TryGetBodyPropertyValue(this ResourceSymbol resourceSymbol, string propertyName)
            => TryGetBodyProperty(resourceSymbol, propertyName)?.Value;

        public static ObjectPropertySyntax GetBodyProperty(this ResourceSymbol resourceSymbol, string propertyName)
            => resourceSymbol.TryGetBodyProperty(propertyName) ?? throw new ArgumentException($"Expected resource syntax body to contain property '{propertyName}'.");

        public static SyntaxBase GetBodyPropertyValue(this ResourceSymbol resourceSymbol, string propertyName)
            => resourceSymbol.TryGetBodyPropertyValue(propertyName) ?? throw new ArgumentException($"Expected resource syntax body to contain property '{propertyName}'.");

        public static ObjectPropertySyntax? TryGetBodyProperty(this ModuleSymbol moduleSymbol, string propertyName)
            => moduleSymbol.DeclaringModule.TryGetBody()?.TryGetPropertyByName(propertyName);

        public static SyntaxBase? TryGetBodyPropertyValue(this ModuleSymbol moduleSymbol, string propertyName)
            => TryGetBodyProperty(moduleSymbol, propertyName)?.Value;

        public static ObjectPropertySyntax GetBodyProperty(this ModuleSymbol moduleSymbol, string propertyName)
            => moduleSymbol.TryGetBodyProperty(propertyName) ?? throw new ArgumentException($"Expected module syntax body to contain property '{propertyName}'.");

        public static SyntaxBase GetBodyPropertyValue(this ModuleSymbol moduleSymbol, string propertyName)
            => moduleSymbol.TryGetBodyPropertyValue(propertyName) ?? throw new ArgumentException($"Expected module syntax body to contain property '{propertyName}'.");

        public static bool IsSecure(this ParameterSymbol parameterSymbol)
        {
            return HasDecorator(parameterSymbol, "secure");
        }

        public static bool HasDecorator(this DeclaredSymbol parameterSymbol, string decoratorName)
            => parameterSymbol?.DeclaringSyntax is DecorableSyntax decorable && HasDecorator(decorable, decoratorName);

        private static bool HasDecorator(DecorableSyntax decorable, string decoratorName)
        {
            // local function
            bool hasDecorator(DecoratorSyntax? value, string decoratorName) => value?.Expression is FunctionCallSyntax functionCallSyntax && functionCallSyntax.NameEquals(decoratorName);

            return decorable.Decorators.Any(d => hasDecorator(d, decoratorName));
        }

        /// <summary>
        /// Returns the expected argument type for a particular argument position, using function overload information.
        /// </summary>
        /// <param name="functionSymbol">The function symbol to inspect</param>
        /// <param name="argIndex">The index of the function argument</param>
        /// <param name="getAssignedArgumentType">Function to look up the assigned type of a given argument</param>
        /// <param name="getAttachedType">Function to look up the </param>
        public static TypeSymbol GetDeclaredArgumentType(
            this IFunctionSymbol functionSymbol,
            int argIndex,
            GetFunctionArgumentType? getAssignedArgumentType = null,
            GetAttachedType? getAttachedType = null)
        {
            // if we have a mix of wildcard and non-wildcard overloads, prioritize the non-wildcard overloads.
            // the wildcards have super generic type definitions, so don't result in helpful completions.
            var overloads = functionSymbol.Overloads.Any(x => x is not FunctionWildcardOverload) ?
                functionSymbol.Overloads.Where(x => x is not FunctionWildcardOverload) :
                functionSymbol.Overloads;

            var argTypes = overloads
                .Where(x => x.MaximumArgumentCount is null || argIndex < x.MaximumArgumentCount)
                .Select(overload =>
                {
                    if (argIndex < overload.FixedParameters.Length)
                    {
                        var parameter = overload.FixedParameters[argIndex];

                        if (parameter.Calculator is not null &&
                            getAssignedArgumentType is not null &&
                            getAttachedType is not null &&
                            parameter.Calculator(getAssignedArgumentType, getAttachedType) is { } calculatedType)
                        {
                            return calculatedType;
                        }

                        return parameter.Type;
                    }

                    return overload.VariableParameter?.Type ?? LanguageConstants.Never;
                });

            return TypeHelper.CreateTypeUnion(argTypes);
        }

        /// <summary>
        ///   Certain declarations (outputs and metadata) define symbols which can't be referenced by name. This method allows you to filter out non-referenceable symbols.
        /// </summary>
        public static bool CanBeReferenced(this DeclaredSymbol declaredSymbol)
            => declaredSymbol is not OutputSymbol and not MetadataSymbol;

        public static string? TryGetDescriptionFromDecorator(this DeclaredSymbol symbol, SemanticModel model)
            => symbol.DeclaringSyntax is DecorableSyntax decorableSyntax ? DescriptionHelper.TryGetFromDecorator(model, decorableSyntax) : null;

        public static DecoratorSyntax? TryGetDecorator(this Symbol symbol, SemanticModel model, string @namespace, string decoratorName)
            => symbol is DeclaredSymbol declaredSymbol && declaredSymbol.DeclaringSyntax is DecorableSyntax decorableSyntax ?
                SemanticModelHelper.TryGetDecoratorInNamespace(model, decorableSyntax, @namespace, decoratorName) :
                null;

        public static bool IsExported(this Symbol symbol, SemanticModel model)
            => TryGetDecorator(symbol, model, SystemNamespaceType.BuiltInName, LanguageConstants.ExportPropertyName) is { };

        public static ISemanticModel GetSemanticModel(this DeclaredSymbol symbol)
            => symbol.Context.ModelLookup.GetSemanticModel(symbol.Context.SourceFile);
    }
}
