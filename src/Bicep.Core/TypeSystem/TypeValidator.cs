// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
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
        public static IList<ErrorDiagnostic> GetCompileTimeConstantViolation(SyntaxBase expression)
        {
            var errors = new List<ErrorDiagnostic>();
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

                case ResourceRefType _:
                    return sourceType.TypeKind == TypeKind.Resource;

                case StringLiteralType _ when sourceType is StringLiteralType:
                    // The name *is* the escaped string value, so we must have an exact match.
                    return targetType.Name == sourceType.Name;

                case PrimitiveType _ when sourceType is StringLiteralType:
                    // string literals can be assigned to strings
                    return targetType.Name == LanguageConstants.String.Name;

                case StringLiteralType _ when sourceType is PrimitiveType:
                    // string literals can be assigned from strings
                    return sourceType.Name == LanguageConstants.String.Name;

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

                case DiscriminatedObjectType targetDiscriminated when sourceType is DiscriminatedObjectType sourceDiscriminated:
                    // validation left for later
                    return true;

                case DiscriminatedObjectType targetDiscriminated when sourceType is ObjectType sourceObject:
                    // validation left for later
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

        public static IEnumerable<ErrorDiagnostic> GetExpressionAssignmentDiagnostics(ITypeManager typeManager, SyntaxBase expression, TypeSymbol targetType, Func<TypeSymbol, TypeSymbol, SyntaxBase, ErrorDiagnostic>? typeMismatchErrorFactory = null)
        {
            // generic error creator if a better one was not specified.
            typeMismatchErrorFactory ??= (expectedType, actualType, errorExpression) => DiagnosticBuilder.ForPosition(errorExpression).ExpectedValueTypeMismatch(expectedType.Name, actualType.Name);

            return GetExpressionAssignmentDiagnosticsInternal(typeManager, expression, targetType, typeMismatchErrorFactory, skipConstantCheck: false, skipTypeErrors: false);
        }

        private static IEnumerable<ErrorDiagnostic> GetExpressionAssignmentDiagnosticsInternal(ITypeManager typeManager,
            SyntaxBase expression,
            TypeSymbol targetType,
            Func<TypeSymbol, TypeSymbol, SyntaxBase, ErrorDiagnostic> typeMismatchErrorFactory,
            bool skipConstantCheck,
            bool skipTypeErrors)
        {
            // TODO: The type of this expression and all subexpressions should be cached
            TypeSymbol? expressionType = typeManager.GetTypeInfo(expression, new TypeManagerContext());

            // since we dynamically checked type, we need to collect the errors but only if the caller wants them
            var errors = Enumerable.Empty<ErrorDiagnostic>();
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
                return errors.Concat(GetObjectAssignmentDiagnostics(typeManager, objectValue, targetObjectType, skipConstantCheck));
            }

            if (expression is ObjectSyntax objectDiscriminated && targetType is DiscriminatedObjectType targetDiscriminated)
            {
                return errors.Concat(GetDiscriminatedObjectAssignmentDiagnostics(typeManager, objectDiscriminated, targetDiscriminated, skipConstantCheck));
            }

            // array assignability check
            if (expression is ArraySyntax arrayValue && targetType is ArrayType targetArrayType)
            {
                return errors.Concat(GetArrayAssignmentDiagnostics(typeManager, arrayValue, targetArrayType, skipConstantCheck));
            }

            return errors;
        }

        private static IEnumerable<ErrorDiagnostic> GetArrayAssignmentDiagnostics(ITypeManager typeManager, ArraySyntax expression, ArrayType targetType, bool skipConstantCheck)
        {
            // if we have parse errors, no need to check assignability
            // we should not return the parse errors however because they will get double collected
            if (expression.HasParseErrors())
            {
                return Enumerable.Empty<ErrorDiagnostic>();
            }

            return expression.Items
                .SelectMany(arrayItemSyntax => GetExpressionAssignmentDiagnosticsInternal(
                    typeManager,
                    arrayItemSyntax.Value,
                    targetType.ItemType,
                    (expectedType, actualType, errorExpression) => DiagnosticBuilder.ForPosition(errorExpression).ArrayTypeMismatch(expectedType.Name, actualType.Name),
                    skipConstantCheck,
                    skipTypeErrors: true)); 
        }

        private static IEnumerable<ErrorDiagnostic> GetDiscriminatedObjectAssignmentDiagnostics(ITypeManager typeManager, ObjectSyntax expression, DiscriminatedObjectType targetType, bool skipConstantCheck)
        {
            // if we have parse errors, there's no point to check assignability
            // we should not return the parse errors however because they will get double collected
            if (expression.HasParseErrors())
            {
                yield break;
            }

            var propertyMap = expression.ToPropertyDictionary();

            if (!propertyMap.TryGetValue(targetType.DiscriminatorKey, out var discriminatorProperty))
            {
                // object doesn't contain the discriminator field
                yield return DiagnosticBuilder.ForPosition(expression).MissingDiscriminator(targetType.DiscriminatorKey, targetType.UnionMembersByKey.Keys);
                yield break;
            }

            // At some point in the future we may want to relax the expectation of a string literal key, and allow a generic string.
            // In this case, the best we can do is validate against the union of all the settable properties.
            // Let's not do this just yet, and see if a use-case arises.

            var discriminatorType = typeManager.GetTypeInfo(discriminatorProperty.Value, new TypeManagerContext());
            if (!(discriminatorType is StringLiteralType stringLiteralDiscriminator))
            {
                yield return DiagnosticBuilder.ForPosition(expression).ExpectedDiscriminatorStringLiteral(targetType.DiscriminatorKey, targetType.UnionMembersByKey.Keys);
                yield break;
            }

            if (!targetType.UnionMembersByKey.TryGetValue(stringLiteralDiscriminator.Name, out var selectedObjectType))
            {
                // no matches
                yield return DiagnosticBuilder.ForPosition(discriminatorProperty.Value).FailedToMatchDiscriminator(targetType.DiscriminatorKey, targetType.UnionMembersByKey.Keys);
                yield break;
            }

            // we have a match!
            foreach (var diagnostic in GetObjectAssignmentDiagnostics(typeManager, expression, selectedObjectType, skipConstantCheck))
            {
                yield return diagnostic;
            }
            yield break;
        }

        private static IEnumerable<ErrorDiagnostic> GetObjectAssignmentDiagnostics(ITypeManager typeManager, ObjectSyntax expression, ObjectType targetType, bool skipConstantCheck)
        {
            // TODO: Short-circuit on any object to avoid unnecessary processing?
            // TODO: Consider doing the schema check even if there are parse errors
            // if we have parse errors, there's no point to check assignability
            // we should not return the parse errors however because they will get double collected
            var result = Enumerable.Empty<ErrorDiagnostic>();
            if (expression.HasParseErrors())
            {
                return result;
            }

            var propertyMap = expression.ToPropertyDictionary();

            var missingRequiredProperties = targetType.Properties.Values
                .Where(p => p.Flags.HasFlag(TypePropertyFlags.Required) && propertyMap.ContainsKey(p.Name) == false)
                .Select(p => p.Name)
                .OrderBy(p => p)
                .ConcatString(LanguageConstants.ListSeparator);
            if (string.IsNullOrEmpty(missingRequiredProperties) == false)
            {
                result = result.Append(DiagnosticBuilder.ForPosition(expression).MissingRequiredProperties(missingRequiredProperties));
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

                    if (declaredProperty.Flags.HasFlag(TypePropertyFlags.ReadOnly))
                    {
                        // the declared property is read-only
                        // value cannot be assigned to a read-only property
                        result = result.Append(DiagnosticBuilder.ForPosition(declaredPropertySyntax.Key).CannotAssignToReadOnlyProperty(declaredProperty.Name));
                    }

                    // declared property is specified in the value object
                    // validate type
                    var diagnostics = GetExpressionAssignmentDiagnosticsInternal(
                        typeManager,
                        declaredPropertySyntax.Value,
                        declaredProperty.Type,
                        (expectedType, actualType, errorExpression) => DiagnosticBuilder.ForPosition(errorExpression).PropertyTypeMismatch(declaredProperty.Name, expectedType, actualType),
                        skipConstantCheckForProperty,
                        skipTypeErrors: true);

                    result = result.Concat(diagnostics);
                }
            }

            // find properties that are specified on in the expression object but not declared in the schema
            var extraProperties = expression.Properties
                .Select(p => p.GetKeyText())
                .Except(targetType.Properties.Values.Select(p => p.Name), LanguageConstants.IdentifierComparer)
                .Select(name => propertyMap[name]);

            if (targetType.AdditionalPropertiesType == null)
            {
                // extra properties are not allowed by the type
                result = result.Concat(extraProperties.Select(extraProperty => DiagnosticBuilder.ForPosition(extraProperty.Key).DisallowedProperty(extraProperty.GetKeyText(), targetType.Name)));
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
                        typeManager,
                        extraProperty.Value,
                        targetType.AdditionalPropertiesType,
                        (expectedType, actualType, errorExpression) => DiagnosticBuilder.ForPosition(errorExpression).PropertyTypeMismatch(extraProperty.GetKeyText(), expectedType, actualType),
                        skipConstantCheckForProperty,
                        skipTypeErrors: true);

                    result = result.Concat(diagnostics);
                }
            }

            return result;
        }
    }
}

