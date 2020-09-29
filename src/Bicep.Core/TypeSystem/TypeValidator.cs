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
        public delegate Diagnostic TypeMismatchErrorFactory(TypeSymbol targetType, TypeSymbol expressionType, SyntaxBase expression);

        /// <summary>
        /// Gets the list of compile-time constant violations. An error is logged for every occurrence of an expression that is not entirely composed of literals.
        /// It may return inaccurate results for malformed trees.
        /// </summary>
        /// <param name="expression">the expression to check for compile-time constant violations</param>
        public static IList<Diagnostic> GetCompileTimeConstantViolation(SyntaxBase expression)
        {
            var diagnostics = new List<Diagnostic>();
            var visitor = new CompileTimeConstantVisitor(diagnostics);

            visitor.Visit(expression);

            return diagnostics;
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

                case TypeSymbol _ when sourceType is ResourceType sourceResourceType:
                    // When assigning a resource, we're really assigning the value of the resource body.
                    return AreTypesAssignable(sourceResourceType.Body.Type, targetType);

                case StringLiteralType _ when sourceType is StringLiteralType:
                    // The name *is* the escaped string value, so we must have an exact match.
                    return targetType.Name == sourceType.Name;

                case PrimitiveType _ when sourceType is StringLiteralType:
                    // string literals can be assigned to strings
                    return targetType.Name == LanguageConstants.String.Name;

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

                case TypeSymbol _ when sourceType is UnionType sourceUnion:
                    // union types are guaranteed to be flat
                    
                    // TODO: Replace with some sort of set intersection
                    // are all source type members assignable to the target type?
                    return sourceUnion.Members.All(sourceMember => AreTypesAssignable(sourceMember.Type, targetType) == true);

                case UnionType targetUnion:
                    // the source type should be a singleton type
                    Debug.Assert(!(sourceType is UnionType),"!(sourceType is UnionType)");

                    // can source type be assigned to any union member types
                    return targetUnion.Members.Any(targetMember => AreTypesAssignable(sourceType, targetMember.Type) == true);

                default:
                    // expression cannot be assigned to the type
                    return false;
            }
        }

        public static TypeSymbol NarrowTypeAndCollectDiagnostics(ITypeManager typeManager, SyntaxBase expression, TypeSymbol targetType, IList<Diagnostic> diagnostics)
        {
            // generic error creator if a better one was not specified.
            TypeMismatchErrorFactory typeMismatchErrorFactory = (expectedType, actualType, errorExpression) => DiagnosticBuilder.ForPosition(errorExpression).ExpectedValueTypeMismatch(expectedType.Name, actualType.Name);

            return NarrowTypeInternal(typeManager, expression, targetType, diagnostics, typeMismatchErrorFactory, skipConstantCheck: false, skipTypeErrors: false);
        }

        private static TypeSymbol NarrowTypeInternal(ITypeManager typeManager,
            SyntaxBase expression,
            TypeSymbol targetType,
            IList<Diagnostic> diagnostics,
            TypeMismatchErrorFactory typeMismatchErrorFactory,
            bool skipConstantCheck,
            bool skipTypeErrors)
        {
            if (targetType is ResourceType targetResourceType)
            {
                // When assigning a resource, we're really assigning the value of the resource body.
                var narrowedBody = NarrowTypeInternal(typeManager, expression, targetResourceType.Body.Type, diagnostics, typeMismatchErrorFactory, skipConstantCheck, skipTypeErrors);
                return new ResourceType(targetResourceType.TypeReference, narrowedBody);
            }

            // TODO: The type of this expression and all subexpressions should be cached
            TypeSymbol? expressionType = typeManager.GetTypeInfo(expression);

            // since we dynamically checked type, we need to collect the errors but only if the caller wants them
            if (skipTypeErrors == false && expressionType is ErrorTypeSymbol)
            {
                diagnostics.AddRange(expressionType.GetDiagnostics());
            }

            // basic assignability check
            if (AreTypesAssignable(expressionType, targetType) == false)
            {
                // fundamentally different types - cannot assign
                diagnostics.Add(typeMismatchErrorFactory(targetType, expressionType, expression));
                return targetType;
            }

            // object assignability check
            if (expression is ObjectSyntax objectValue && targetType is ObjectType targetObjectType)
            {
                return NarrowObjectType(typeManager, objectValue, targetObjectType, diagnostics, skipConstantCheck);
            }

            if (expression is ObjectSyntax objectDiscriminated && targetType is DiscriminatedObjectType targetDiscriminated)
            {
                return NarrowDiscriminatedObjectType(typeManager, objectDiscriminated, targetDiscriminated, diagnostics, skipConstantCheck);
            }

            // array assignability check
            if (expression is ArraySyntax arrayValue && targetType is ArrayType targetArrayType)
            {
                return NarrowArrayAssignmentType(typeManager, arrayValue, targetArrayType, diagnostics, skipConstantCheck);
            }

            if (targetType is UnionType targetUnionType)
            {
                return UnionType.Create(targetUnionType.Members.Where(x => AreTypesAssignable(expressionType, x.Type) == true));
            }

            return targetType;
        }

        private static TypeSymbol NarrowArrayAssignmentType(ITypeManager typeManager, ArraySyntax expression, ArrayType targetType, IList<Diagnostic> diagnostics, bool skipConstantCheck)
        {
            // if we have parse errors, no need to check assignability
            // we should not return the parse errors however because they will get double collected
            if (expression.HasParseErrors())
            {
                return targetType;
            }

            var arrayProperties = new List<TypeSymbol>();
            foreach (var arrayItemSyntax in expression.Items)
            {
                arrayProperties.Add(NarrowTypeInternal(
                    typeManager,
                    arrayItemSyntax.Value,
                    targetType.Item.Type,
                    diagnostics,
                    (expectedType, actualType, errorExpression) => DiagnosticBuilder.ForPosition(errorExpression).ArrayTypeMismatch(expectedType.Name, actualType.Name),
                    skipConstantCheck,
                    skipTypeErrors: true));
            }

            return new TypedArrayType(UnionType.Create(arrayProperties));
        }

        private static TypeSymbol NarrowDiscriminatedObjectType(ITypeManager typeManager, ObjectSyntax expression, DiscriminatedObjectType targetType, IList<Diagnostic> diagnostics, bool skipConstantCheck)
        {
            // if we have parse errors, there's no point to check assignability
            // we should not return the parse errors however because they will get double collected
            if (expression.HasParseErrors())
            {
                return LanguageConstants.Any;
            }

            var discriminatorProperty = expression.Properties.FirstOrDefault(x => LanguageConstants.IdentifierComparer.Equals(x.TryGetKeyText(), targetType.DiscriminatorKey));
            if (discriminatorProperty == null)
            {
                // object doesn't contain the discriminator field
                diagnostics.Add(DiagnosticBuilder.ForPosition(expression).MissingRequiredProperty(targetType.DiscriminatorKey, targetType.DiscriminatorKeysUnionType));
                return LanguageConstants.Any;
            }

            // At some point in the future we may want to relax the expectation of a string literal key, and allow a generic string.
            // In this case, the best we can do is validate against the union of all the settable properties.
            // Let's not do this just yet, and see if a use-case arises.

            var discriminatorType = typeManager.GetTypeInfo(discriminatorProperty.Value);
            if (!(discriminatorType is StringLiteralType stringLiteralDiscriminator))
            {
                diagnostics.Add(DiagnosticBuilder.ForPosition(expression).PropertyTypeMismatch(targetType.DiscriminatorKey, targetType.DiscriminatorKeysUnionType, discriminatorType));
                return LanguageConstants.Any;
            }

            if (!targetType.UnionMembersByKey.TryGetValue(stringLiteralDiscriminator.Name, out var selectedObjectReference))
            {
                // no matches
                diagnostics.Add(DiagnosticBuilder.ForPosition(discriminatorProperty.Value).PropertyTypeMismatch(targetType.DiscriminatorKey, targetType.DiscriminatorKeysUnionType, discriminatorType));
                return LanguageConstants.Any;
            }

            if (!(selectedObjectReference.Type is ObjectType selectedObjectType))
            {
                throw new InvalidOperationException($"Discriminated type {targetType.Name} contains non-object member");
            }

            // we have a match!
            return NarrowObjectType(typeManager, expression, selectedObjectType, diagnostics, skipConstantCheck);
        }

        private static TypeSymbol NarrowObjectType(ITypeManager typeManager, ObjectSyntax expression, ObjectType targetType, IList<Diagnostic> diagnostics, bool skipConstantCheck)
        {
            // TODO: Short-circuit on any object to avoid unnecessary processing?
            // TODO: Consider doing the schema check even if there are parse errors
            // if we have parse errors, there's no point to check assignability
            // we should not return the parse errors however because they will get double collected
            if (expression.HasParseErrors())
            {
                return targetType;
            }

            var namedPropertyMap = expression.ToNamedPropertyDictionary();

            var missingRequiredProperties = targetType.Properties.Values
                .Where(p => p.Flags.HasFlag(TypePropertyFlags.Required) && !namedPropertyMap.ContainsKey(p.Name))
                .Select(p => p.Name)
                .OrderBy(p => p)
                .ConcatString(LanguageConstants.ListSeparator);
            if (string.IsNullOrEmpty(missingRequiredProperties) == false)
            {
                diagnostics.Add(DiagnosticBuilder.ForPosition(expression).MissingRequiredProperties(missingRequiredProperties));
            }

            var narrowedProperties = new List<TypeProperty>();
            foreach (var declaredProperty in targetType.Properties.Values)
            {
                if (namedPropertyMap.TryGetValue(declaredProperty.Name, out var declaredPropertySyntax))
                {
                    bool skipConstantCheckForProperty = skipConstantCheck;

                    // is the property marked as requiring compile-time constants and has the parent already validated this?
                    if (skipConstantCheck == false && declaredProperty.Flags.HasFlag(TypePropertyFlags.Constant))
                    {
                        // validate that values are compile-time constants
                        diagnostics.AddRange(GetCompileTimeConstantViolation(declaredPropertySyntax.Value));

                        // disable compile-time constant validation for children
                        skipConstantCheckForProperty = true;
                    }

                    if (declaredProperty.Flags.HasFlag(TypePropertyFlags.ReadOnly))
                    {
                        // the declared property is read-only
                        // value cannot be assigned to a read-only property
                        diagnostics.Add(DiagnosticBuilder.ForPosition(declaredPropertySyntax.Key).CannotAssignToReadOnlyProperty(declaredProperty.Name));
                    }

                    // declared property is specified in the value object
                    // validate type
                    var narrowedType = NarrowTypeInternal(
                        typeManager,
                        declaredPropertySyntax.Value,
                        declaredProperty.TypeReference.Type,
                        diagnostics,
                        (expectedType, actualType, errorExpression) => DiagnosticBuilder.ForPosition(errorExpression).PropertyTypeMismatch(declaredProperty.Name, expectedType, actualType),
                        skipConstantCheckForProperty,
                        skipTypeErrors: true);
                        
                    narrowedProperties.Add(new TypeProperty(declaredProperty.Name, narrowedType, declaredProperty.Flags));
                }
                else
                {
                    narrowedProperties.Add(declaredProperty);
                }
            }

            // find properties that are specified on in the expression object but not declared in the schema
            var extraProperties = expression.Properties
                .Where(p => !(p.TryGetKeyText() is string keyName) || !targetType.Properties.ContainsKey(keyName));

            if (targetType.AdditionalPropertiesType == null)
            {
                var validUnspecifiedProperties = targetType.Properties.Values
                    .Where(p => !p.Flags.HasFlag(TypePropertyFlags.ReadOnly))
                    .Where(p => !namedPropertyMap.ContainsKey(p.Name))
                    .Select(p => p.Name)
                    .OrderBy(x => x);

                // extra properties are not allowed by the type
                foreach (var extraProperty in extraProperties)
                {
                    ErrorDiagnostic error;
                    if (extraProperty.TryGetKeyText() is string keyName)
                    {
                        error = validUnspecifiedProperties.Any() ? 
                            DiagnosticBuilder.ForPosition(extraProperty.Key).DisallowedPropertyWithPermissibleProperties(keyName, targetType.Name, validUnspecifiedProperties) :
                            DiagnosticBuilder.ForPosition(extraProperty.Key).DisallowedProperty(keyName, targetType.Name);
                    }
                    else
                    {
                        error = validUnspecifiedProperties.Any() ? 
                            DiagnosticBuilder.ForPosition(extraProperty.Key).DisallowedInterpolatedKeyPropertyWithPermissibleProperties(targetType.Name, validUnspecifiedProperties) :
                            DiagnosticBuilder.ForPosition(extraProperty.Key).DisallowedInterpolatedKeyProperty(targetType.Name);
                    }

                    diagnostics.AddRange(error.AsEnumerable());
                }
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
                        diagnostics.AddRange(GetCompileTimeConstantViolation(extraProperty.Value));

                        // disable compile-time constant validation for children
                        skipConstantCheckForProperty = true;
                    }

                    TypeMismatchErrorFactory typeMismatchErrorFactory;
                    if (extraProperty.TryGetKeyText() is string keyName)
                    {
                        typeMismatchErrorFactory = (expectedType, actualType, errorExpression) => DiagnosticBuilder.ForPosition(errorExpression).PropertyTypeMismatch(keyName, expectedType, actualType);
                    }
                    else
                    {
                        typeMismatchErrorFactory = (expectedType, actualType, errorExpression) => DiagnosticBuilder.ForPosition(errorExpression).ExpectedValueTypeMismatch(expectedType, actualType);
                    }

                    var narrowedProperty = NarrowTypeInternal(
                        typeManager,
                        extraProperty.Value,
                        targetType.AdditionalPropertiesType.Type,
                        diagnostics,
                        typeMismatchErrorFactory,
                        skipConstantCheckForProperty,
                        skipTypeErrors: true);

                    // TODO should we try and narrow the additional properties type? May be difficult
                }
            }

            return new NamedObjectType(targetType.Name, narrowedProperties, targetType.AdditionalPropertiesType, targetType.AdditionalPropertiesFlags);
        }
    }
}