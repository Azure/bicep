using Bicep.Core.Errors;
using Bicep.Core.Extensions;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bicep.Core.TypeSystem
{
    public static class TypeValidator
    {
        /// <summary>
        /// Gets the list of compile-time constant violations. An error is logged for every occurrence of an expression that is not entirely composed of literals.
        /// It may return inaccurate results for malformed trees.
        /// </summary>
        /// <param name="expression">the expression to check for compile-time constant violations</param>
        public static IList<Error> GetCompileTimeConstantViolation(SyntaxBase expression)
        {
            var errors = new List<Error>();
            var visitor = new CompileTimeConstantVisitor(errors);

            visitor.Visit(expression);

            return errors;
        }

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

            if (sourceType is AnyType)
            {
                // "any" type is assignable to all types
                return true;
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

                case ArrayType _ when sourceType is ArrayType:
                    // both types are arrays
                    // this function does not validate item types
                    return true;

                case UnionType targetUnion when sourceType is UnionType sourceUnion:
                    // union types are guaranteed to be flat
                    
                    // TODO: Replace with some sort of set intersection
                    // are all source type members assignable to the target type?
                    return sourceUnion.Members.All(sourceMember => AreTypesAssignable(sourceMember, targetUnion) == true);

                case UnionType targetUnion:
                    // the source type should be a singleton type
                    Debug.Assert(!(sourceType is UnionType),"!(sourceType is UnionType)");

                    // can source type be assigned to any union member types
                    return targetUnion.Members.Any(targetMember => AreTypesAssignable(sourceType, targetMember) == true);

                default:
                    // expression cannot be assigned to the type
                    return false;
            }
        }

        public static IEnumerable<Error> GetExpressionAssignmentDiagnostics(ISemanticContext context, SyntaxBase expression, TypeSymbol targetType, Func<TypeSymbol, TypeSymbol, SyntaxBase, Error>? typeMismatchErrorFactory = null)
        {
            // generic error creator if a better one was not specified.
            typeMismatchErrorFactory ??= (expectedType, actualType, errorExpression) => new Error(errorExpression, ErrorCode.ErrExpectdValueTypeMismatch, expectedType.Name, actualType.Name);

            return GetExpressionAssignmentDiagnosticsInternal(context, expression, targetType, typeMismatchErrorFactory, skipConstantCheck: false, skipTypeErrors: false);
        }

        private static IEnumerable<Error> GetExpressionAssignmentDiagnosticsInternal(
            ISemanticContext context,
            SyntaxBase expression,
            TypeSymbol targetType,
            Func<TypeSymbol, TypeSymbol, SyntaxBase, Error> typeMismatchErrorFactory,
            bool skipConstantCheck,
            bool skipTypeErrors)
        {
            // TODO: The type of this expression and all subexpressions should be cached
            TypeSymbol? expressionType = context.GetTypeInfo(expression);

            // since we dynamically checked type, we need to collect the errors but only if the caller wants them
            var errors = Enumerable.Empty<Error>();
            if (skipTypeErrors == false && expressionType is ErrorTypeSymbol)
            {
                errors = errors.Concat(expressionType.GetDiagnostics());
            }

            // basic assignability check
            if (AreTypesAssignable(expressionType, targetType) == false)
            {
                // fundamentally different types - cannot assign
                return errors.Append(typeMismatchErrorFactory(targetType, expressionType, expression));
            }

            // object assignability check
            if (expression is ObjectSyntax objectValue && targetType is ObjectType targetObjectType)
            {
                return errors.Concat(GetObjectAssignmentDiagnostics(context, objectValue, targetObjectType, skipConstantCheck));
            }

            // array assignability check
            if (expression is ArraySyntax arrayValue && targetType is ArrayType targetArrayType)
            {
                return errors.Concat(GetArrayAssignmentDiagnostics(context, arrayValue, targetArrayType, skipConstantCheck));
            }

            return errors;
        }

        private static IEnumerable<Error> GetArrayAssignmentDiagnostics(ISemanticContext context, ArraySyntax expression, ArrayType targetType, bool skipConstantCheck)
        {
            // if we have parse errors, no need to check assignability
            // we should not return the parse errors however because they will get double collected
            if (expression.HasParseErrors())
            {
                return Enumerable.Empty<Error>();
            }

            return expression.Items
                .SelectMany(arrayItemSyntax => GetExpressionAssignmentDiagnosticsInternal(
                    context,
                    arrayItemSyntax.Value,
                    targetType.ItemType,
                    (expectedType, actualType, errorExpression) => new Error(errorExpression, ErrorCode.ErrArrayTypeMismatch, expectedType.Name, actualType.Name),
                    skipConstantCheck,
                    skipTypeErrors: true)); 
        }

        private static IEnumerable<Error> GetObjectAssignmentDiagnostics(ISemanticContext context, ObjectSyntax expression, ObjectType targetType, bool skipConstantCheck)
        {
            // TODO: Short-circuit on any object to avoid unnecessary processing?
            // TODO: Consider doing the schema check even if there are parse errors
            // if we have parse errors, there's no point to check assignability
            // we should not return the parse errors however because they will get double collected
            var result = Enumerable.Empty<Error>();
            if (expression.HasParseErrors())
            {
                return result;
            }

            var propertyMap = expression.Properties.ToDictionary(p => p.Identifier.IdentifierName, StringComparer.Ordinal);

            var missingRequiredProperties = targetType.Properties.Values
                .Where(p => p.Flags.HasFlag(TypePropertyFlags.Required) && propertyMap.ContainsKey(p.Name) == false)
                .Select(p => p.Name)
                .OrderBy(p => p)
                .ConcatString(LanguageConstants.ListSeparator);
            if (string.IsNullOrEmpty(missingRequiredProperties) == false)
            {
                result = result.Append(new Error(expression, ErrorCode.ErrMissingRequiredProperties, missingRequiredProperties));
            }

            foreach (var declaredProperty in targetType.Properties.Values)
            {
                if (propertyMap.TryGetValue(declaredProperty.Name, out var declaredPropertySyntax))
                {
                    bool skipConstantCheckForProperty = skipConstantCheck;

                    // is the property marked as requiring compile-time constants and has the parent already validated this?
                    if (skipConstantCheck == false && declaredProperty.Flags.HasFlag(TypePropertyFlags.Constant))
                    {
                        // validate that values are compile-time constants
                        result = result.Concat(GetCompileTimeConstantViolation(declaredPropertySyntax.Value));

                        // disable compile-time constant validation for children
                        skipConstantCheckForProperty = true;
                    }

                    // declared property is specified in the value object
                    // validate type
                    var diagnostics = GetExpressionAssignmentDiagnosticsInternal(
                        context,
                        declaredPropertySyntax.Value,
                        declaredProperty.Type,
                        (expectedType, actualType, errorExpression) => new Error(errorExpression, ErrorCode.ErrPropertyTypeMismatch, declaredProperty.Name, expectedType.Name, actualType.Name),
                        skipConstantCheckForProperty,
                        skipTypeErrors: true);

                    result = result.Concat(diagnostics);
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
                result = result.Concat(extraProperties.Select(extraProperty => new Error(extraProperty.Identifier, ErrorCode.ErrDisallowedProperty, extraProperty.Identifier.IdentifierName, targetType.Name)));
            }
            else
            {
                // extra properties must be assignable to the right type
                foreach (ObjectPropertySyntax extraProperty in extraProperties)
                {
                    bool skipConstantCheckForProperty = skipConstantCheck;

                    // is the property marked as requiring compile-time constants and has the parent already validated this?
                    if (skipConstantCheckForProperty == false && targetType.AdditionalPropertiesFlags.HasFlag(TypePropertyFlags.Constant))
                    {
                        // validate that values are compile-time constants
                        result = result.Concat(GetCompileTimeConstantViolation(extraProperty.Value));

                        // disable compile-time constant validation for children
                        skipConstantCheckForProperty = true;
                    }

                    var diagnostics = GetExpressionAssignmentDiagnosticsInternal(
                        context,
                        extraProperty.Value,
                        targetType.AdditionalPropertiesType,
                        (expectedType, actualType, errorExpression) => new Error(errorExpression, ErrorCode.ErrPropertyTypeMismatch, extraProperty.Identifier.IdentifierName, expectedType.Name, actualType.Name),
                        skipConstantCheckForProperty,
                        skipTypeErrors: true);

                    result = result.Concat(diagnostics);
                }
            }

            return result;
        }
    }
}
