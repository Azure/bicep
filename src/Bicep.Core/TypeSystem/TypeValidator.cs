using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public static class TypeValidator
    {
        /// <summary>
        /// Checks if a value of the specified source type can be assigned to the specified target type. (Does not validate properties/schema on object types.)
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="targetType">The target type</param>
        /// <returns>Returns true if values of the specified source type are assignable to the target type. Returns false otherwise or null if assignability cannot be determined.</returns>
        public static bool? AreTypesAssignable(TypeSymbol? sourceType, TypeSymbol? targetType)
        {
            if (sourceType == null || targetType == null || sourceType is ErrorTypeSymbol || targetType is ErrorTypeSymbol)
            {
                return null;
            }

            switch (targetType)
            {
                case AnyType _:
                    // values of all types can be assigned to the "any" type
                    return true;

                case PrimitiveType _ when sourceType is PrimitiveType:
                    // both types are primitive
                    // compare by type name
                    return string.Equals(sourceType.Name, targetType.Name, StringComparison.Ordinal);

                case ObjectType _ when sourceType is ObjectType:
                    // both types are objects
                    // this function does not implement any schema validation, so this is far as we go
                    return true;

                default:
                    // expression cannot be assigned to the type
                    return false;
            }
        }

        public static IEnumerable<Error> GetExpressionAssignmentDiagnostics(ISemanticContext context, SyntaxBase expression, TypeSymbol targetType, Func<TypeSymbol, TypeSymbol, SyntaxBase, Error>? typeMismatchErrorFactory = null)
        {
            // generic error creator if a better one was not specified.
            typeMismatchErrorFactory ??= (expectedType, actualType, errorExpression)=> new Error($"Expected a value of type '{expectedType.Name}' but the provided value is of type '{actualType.Name}'.", errorExpression);

            TypeSymbol? expressionType = context.GetTypeInfo(expression);

            // basic assignability check
            if (AreTypesAssignable(expressionType, targetType) == false)
            {
                // fundamentally different types - cannot assign
                yield return typeMismatchErrorFactory(targetType, expressionType!, expression);
                yield break;
            }

            // deeper assignability check
            if (expression is ObjectSyntax objectValue && targetType is ObjectType targetObjectType)
            {
                foreach (Error diagnostic in GetObjectAssignmentDiagnostics(context, objectValue, targetObjectType))
                {
                    yield return diagnostic;
                }
            }
        }

        private static IEnumerable<Error> GetObjectAssignmentDiagnostics(ISemanticContext context, ObjectSyntax expression, ObjectType targetType)
        {
            // TODO: Rewrite using extension method syntax
            // TODO: Short-circuit on any object to avoid unnecessary processing?

            // if we have parse errors, there's no point to check equality
            // we should not return the parse errors however because they will get double collected
            if (expression.HasParseErrors())
            {
                yield break;
            }

            var propertyMap = expression.Properties.ToDictionary(p => p.Identifier.IdentifierName, StringComparer.Ordinal);

            var missingRequiredProperties = targetType.Properties.Values
                .Where(p => p.Required && propertyMap.ContainsKey(p.Name) == false)
                .Select(p => p.Name)
                .OrderBy(p => p)
                .ConcatString(LanguageConstants.ListSeparator);
            if (string.IsNullOrEmpty(missingRequiredProperties) == false)
            {
                yield return new Error($"The specified object is missing the following required properties: {missingRequiredProperties}.", expression);
            }

            foreach (var declaredProperty in targetType.Properties.Values)
            {
                if (propertyMap.TryGetValue(declaredProperty.Name, out var declaredPropertySyntax))
                {
                    // declared property is specified in the value object
                    // validate type
                    var diagnostics = GetExpressionAssignmentDiagnostics(
                        context,
                        declaredPropertySyntax.Value,
                        declaredProperty.Type,
                        (expectedType, actualType, errorExpression) => new Error($"Property '{declaredProperty.Name}' expected a value of type '{expectedType.Name}' but the provided value is of type '{actualType.Name}'.", errorExpression));

                    foreach (Error diagnostic in diagnostics)
                    {
                        yield return diagnostic;
                    }
                }
            }

            // find properties that are specified on in the expression object but not declared in the schema
            var extraProperties = expression.Properties
                .Select(p => p.Identifier.IdentifierName)
                .Except(targetType.Properties.Values.Select(p => p.Name), StringComparer.Ordinal)
                .Select(name => propertyMap[name]);

            if (targetType.AdditionalPropertiesType == null)
            {
                // extra properties are not allowed by the type
                foreach (ObjectPropertySyntax extraProperty in extraProperties)
                {
                    yield return new Error($"The property '{extraProperty.Identifier.IdentifierName}' is not allowed on objects of type '{targetType.Name}'.", extraProperty.Identifier);
                }
            }
            else
            {
                // extra properties must be assignable to the right type
                foreach (ObjectPropertySyntax extraProperty in extraProperties)
                {
                    var diagnostics = GetExpressionAssignmentDiagnostics(
                        context,
                        extraProperty.Value,
                        targetType.AdditionalPropertiesType,
                        (expectedType, actualType, errorExpression) => new Error($"The property '{extraProperty.Identifier.IdentifierName}' expected a value of type '{expectedType.Name}' but the provided value is of type '{actualType.Name}'.", errorExpression));

                    foreach (Error diagnostic in diagnostics)
                    {
                        yield return diagnostic;
                    }
                }
            }
        }
    }
}
