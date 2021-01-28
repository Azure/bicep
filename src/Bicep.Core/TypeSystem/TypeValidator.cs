// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
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
        /// <param name="diagnosticWriter">Diagnostic writer instance</param>
        public static void GetCompileTimeConstantViolation(SyntaxBase expression, IDiagnosticWriter diagnosticWriter)
        {
            var visitor = new CompileTimeConstantVisitor(diagnosticWriter);

            visitor.Visit(expression);
        }

        /// <summary>
        /// Checks if a value of the specified source type can be assigned to the specified target type. (Does not validate properties/schema on object types.)
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="targetType">The target type</param>
        /// <returns>Returns true if values of the specified source type are assignable to the target type. Returns false otherwise or null if assignability cannot be determined.</returns>
        public static bool AreTypesAssignable(TypeSymbol sourceType, TypeSymbol targetType)
        {
            if (sourceType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.PreventAssignment) || targetType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.PreventAssignment))
            {
                return false;
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

                case TypeSymbol _ when targetType is ResourceReferenceType scopeReference:
                    return sourceType is IResourceScopeType resourceScopeType && scopeReference.ResourceScopeType.HasFlag(resourceScopeType.ResourceScopeType);

                case TypeSymbol _ when sourceType is ResourceType sourceResourceType:
                    // When assigning a resource, we're really assigning the value of the resource body.
                    return AreTypesAssignable(sourceResourceType.Body.Type, targetType);

                case TypeSymbol _ when sourceType is ModuleType sourceModuleType:
                    // When assigning a module, we're really assigning the value of the module body.
                    return AreTypesAssignable(sourceModuleType.Body.Type, targetType);

                case StringLiteralType _ when sourceType is StringLiteralType:
                    // The name *is* the escaped string value, so we must have an exact match.
                    return targetType.Name == sourceType.Name;

                case StringLiteralType _ when sourceType is PrimitiveType:
                    // We allow string to string literal assignment only in the case where the "AllowLooseStringAssignment" validation flag has been set.
                    // This is to allow parameters without 'allowed' values to be assigned to fields expecting enums.
                    // At some point we may want to consider flowing the enum type backwards to solve this more elegantly.
                    return sourceType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.AllowLooseStringAssignment) && sourceType.Name == LanguageConstants.String.Name;

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

                case DiscriminatedObjectType _ when sourceType is DiscriminatedObjectType:
                    // validation left for later
                    return true;

                case DiscriminatedObjectType _ when sourceType is ObjectType:
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

        public static bool ShouldWarn(TypeSymbol targetType)
            => targetType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.WarnOnTypeMismatch);

        public static TypeSymbol NarrowTypeAndCollectDiagnostics(ITypeManager typeManager, SyntaxBase expression, TypeSymbol targetType, IDiagnosticWriter diagnosticWriter)
        {
            // generic error creator if a better one was not specified.
            TypeMismatchErrorFactory typeMismatchErrorFactory = (expectedType, actualType, errorExpression) => DiagnosticBuilder.ForPosition(errorExpression).ExpectedValueTypeMismatch(ShouldWarn(targetType), expectedType, actualType);

            return NarrowTypeInternal(typeManager, expression, targetType, diagnosticWriter, typeMismatchErrorFactory, skipConstantCheck: false, skipTypeErrors: false);
        }

        private static TypeSymbol NarrowTypeInternal(ITypeManager typeManager,
            SyntaxBase expression,
            TypeSymbol targetType,
            IDiagnosticWriter diagnosticWriter,
            TypeMismatchErrorFactory typeMismatchErrorFactory,
            bool skipConstantCheck,
            bool skipTypeErrors)
        {
            if (targetType is ResourceType targetResourceType)
            {
                // When assigning a resource, we're really assigning the value of the resource body.
                var narrowedBody = NarrowTypeInternal(typeManager, expression, targetResourceType.Body.Type, diagnosticWriter, typeMismatchErrorFactory, skipConstantCheck, skipTypeErrors);

                return new ResourceType(targetResourceType.TypeReference, narrowedBody);
            }
            
            if (targetType is ModuleType targetModuleType)
            {
                var narrowedBody = NarrowTypeInternal(typeManager, expression, targetModuleType.Body.Type, diagnosticWriter, typeMismatchErrorFactory, skipConstantCheck, skipTypeErrors);

                return new ModuleType(targetModuleType.Name, narrowedBody);
            }

            // TODO: The type of this expression and all subexpressions should be cached
            var expressionType = typeManager.GetTypeInfo(expression);

            // since we dynamically checked type, we need to collect the errors but only if the caller wants them
            if (skipTypeErrors == false && expressionType is ErrorType)
            {
                diagnosticWriter.WriteMultiple(expressionType.GetDiagnostics());
                return targetType;
            }

            // basic assignability check
            if (AreTypesAssignable(expressionType, targetType) == false)
            {
                // fundamentally different types - cannot assign
                diagnosticWriter.Write(typeMismatchErrorFactory(targetType, expressionType, expression));
                return targetType;
            }

            // object assignability check
            if (expression is ObjectSyntax objectValue && targetType is ObjectType targetObjectType)
            {
                return NarrowObjectType(typeManager, objectValue, targetObjectType, diagnosticWriter, skipConstantCheck);
            }

            if (expression is ObjectSyntax objectDiscriminated && targetType is DiscriminatedObjectType targetDiscriminated)
            {
                return NarrowDiscriminatedObjectType(typeManager, objectDiscriminated, targetDiscriminated, diagnosticWriter, skipConstantCheck);
            }

            // array assignability check
            if (expression is ArraySyntax arrayValue && targetType is ArrayType targetArrayType)
            {
                return NarrowArrayAssignmentType(typeManager, arrayValue, targetArrayType, diagnosticWriter, skipConstantCheck);
            }

            if (targetType is UnionType targetUnionType)
            {
                return UnionType.Create(targetUnionType.Members.Where(x => AreTypesAssignable(expressionType, x.Type) == true));
            }

            return targetType;
        }

        private static TypeSymbol NarrowArrayAssignmentType(ITypeManager typeManager, ArraySyntax expression, ArrayType targetType, IDiagnosticWriter diagnosticWriter, bool skipConstantCheck)
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
                    diagnosticWriter,
                    (expectedType, actualType, errorExpression) => DiagnosticBuilder.ForPosition(errorExpression).ArrayTypeMismatch(ShouldWarn(targetType), expectedType, actualType),
                    skipConstantCheck,
                    skipTypeErrors: true));
            }

            return new TypedArrayType(UnionType.Create(arrayProperties), targetType.ValidationFlags);
        }

        private static TypeSymbol NarrowDiscriminatedObjectType(ITypeManager typeManager, ObjectSyntax expression, DiscriminatedObjectType targetType, IDiagnosticWriter diagnosticWriter, bool skipConstantCheck)
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
                diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression).MissingRequiredProperty(ShouldWarn(targetType), targetType.DiscriminatorKey, targetType.DiscriminatorKeysUnionType));

                var propertyKeys = expression.Properties
                    .Select(x => x.TryGetKeyText())
                    .Where(key => !string.IsNullOrEmpty(key))
                    .Select(key => key!);

                // do a reverse lookup to check if there's any misspelled discriminator key
                var misspelledDiscriminatorKey = SpellChecker.GetSpellingSuggestion(targetType.DiscriminatorKey, propertyKeys);

                if (misspelledDiscriminatorKey != null)
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression).DisallowedPropertyWithSuggestion(ShouldWarn(targetType), misspelledDiscriminatorKey, targetType.DiscriminatorKeysUnionType, targetType.DiscriminatorKey));
                }


                return LanguageConstants.Any;
            }

            // At some point in the future we may want to relax the expectation of a string literal key, and allow a generic string.
            // In this case, the best we can do is validate against the union of all the settable properties.
            // Let's not do this just yet, and see if a use-case arises.

            var discriminatorType = typeManager.GetTypeInfo(discriminatorProperty.Value);
            if (!(discriminatorType is StringLiteralType stringLiteralDiscriminator))
            {
                diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression).PropertyTypeMismatch(ShouldWarn(targetType), targetType.DiscriminatorKey, targetType.DiscriminatorKeysUnionType, discriminatorType));
                return LanguageConstants.Any;
            }

            if (!targetType.UnionMembersByKey.TryGetValue(stringLiteralDiscriminator.Name, out var selectedObjectReference))
            {
                // no matches
                var discriminatorCandidates = targetType.UnionMembersByKey.Keys.OrderBy(x => x);
                string? suggestedDiscriminator = SpellChecker.GetSpellingSuggestion(stringLiteralDiscriminator.Name, discriminatorCandidates);
                var builder = DiagnosticBuilder.ForPosition(discriminatorProperty.Value);
                bool shouldWarn = ShouldWarn(targetType);

                diagnosticWriter.Write(suggestedDiscriminator != null
                    ? builder.PropertyStringLiteralMismatchWithSuggestion(shouldWarn, targetType.DiscriminatorKey, targetType.DiscriminatorKeysUnionType, stringLiteralDiscriminator.Name, suggestedDiscriminator)
                    : builder.PropertyTypeMismatch(shouldWarn, targetType.DiscriminatorKey, targetType.DiscriminatorKeysUnionType, discriminatorType));

                return LanguageConstants.Any;
            }

            if (!(selectedObjectReference.Type is ObjectType selectedObjectType))
            {
                throw new InvalidOperationException($"Discriminated type {targetType.Name} contains non-object member");
            }

            // we have a match!
            return NarrowObjectType(typeManager, expression, selectedObjectType, diagnosticWriter, skipConstantCheck);
        }

        private static TypeSymbol NarrowObjectType(ITypeManager typeManager, ObjectSyntax expression, ObjectType targetType, IDiagnosticWriter diagnosticWriter, bool skipConstantCheck)
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
                .OrderBy(p => p);

            if (missingRequiredProperties.Any())
            {
                var (positionable, blockName) = GetMissingPropertyContext(typeManager, expression);

                diagnosticWriter.Write(DiagnosticBuilder.ForPosition(positionable).MissingRequiredProperties(ShouldWarn(targetType), missingRequiredProperties, blockName));
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
                        GetCompileTimeConstantViolation(declaredPropertySyntax.Value, diagnosticWriter);

                        // disable compile-time constant validation for children
                        skipConstantCheckForProperty = true;
                    }

                    if (declaredProperty.Flags.HasFlag(TypePropertyFlags.ReadOnly))
                    {
                        // the declared property is read-only
                        // value cannot be assigned to a read-only property
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(declaredPropertySyntax.Key).CannotAssignToReadOnlyProperty(ShouldWarn(targetType), declaredProperty.Name));
                        narrowedProperties.Add(new TypeProperty(declaredProperty.Name, declaredProperty.TypeReference.Type, declaredProperty.Flags));
                        continue;
                    }

                    // declared property is specified in the value object
                    // validate type
                    var narrowedType = NarrowTypeInternal(
                        typeManager,
                        declaredPropertySyntax.Value,
                        declaredProperty.TypeReference.Type,
                        diagnosticWriter,
                        GetPropertyMismatchErrorFactory(ShouldWarn(targetType), declaredProperty.Name),
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
                bool shouldWarn = ShouldWarn(targetType);
                var validUnspecifiedProperties = targetType.Properties.Values
                    .Where(p => !p.Flags.HasFlag(TypePropertyFlags.ReadOnly) && !namedPropertyMap.ContainsKey(p.Name))
                    .Select(p => p.Name)
                    .OrderBy(x => x);

                // extra properties are not allowed by the type
                foreach (var extraProperty in extraProperties)
                {
                    Diagnostic error;
                    var builder = DiagnosticBuilder.ForPosition(extraProperty.Key);

                    if (extraProperty.TryGetKeyText() is string keyName)
                    {
                        error = validUnspecifiedProperties.Any() switch
                        {
                            true => SpellChecker.GetSpellingSuggestion(keyName, validUnspecifiedProperties) switch
                            {
                                string suggestedKeyName when suggestedKeyName != null
                                    => builder.DisallowedPropertyWithSuggestion(shouldWarn, keyName, targetType, suggestedKeyName),
                                _ => builder.DisallowedPropertyWithPermissibleProperties(shouldWarn, keyName, targetType, validUnspecifiedProperties)
                            },
                            _ => builder.DisallowedProperty(shouldWarn, targetType)
                        };
                    }
                    else
                    {
                        error = validUnspecifiedProperties.Any() ? 
                            builder.DisallowedInterpolatedKeyPropertyWithPermissibleProperties(shouldWarn, targetType, validUnspecifiedProperties) :
                            builder.DisallowedInterpolatedKeyProperty(shouldWarn, targetType);
                    }

                    diagnosticWriter.WriteMultiple(error.AsEnumerable());
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
                        GetCompileTimeConstantViolation(extraProperty.Value, diagnosticWriter);

                        // disable compile-time constant validation for children
                        skipConstantCheckForProperty = true;
                    }

                    TypeMismatchErrorFactory typeMismatchErrorFactory;
                    if (extraProperty.TryGetKeyText() is string keyName)
                    {
                        typeMismatchErrorFactory = GetPropertyMismatchErrorFactory(ShouldWarn(targetType), keyName);
                    }
                    else
                    {
                        typeMismatchErrorFactory = (expectedType, actualType, errorExpression) => DiagnosticBuilder.ForPosition(errorExpression).ExpectedValueTypeMismatch(ShouldWarn(targetType), expectedType, actualType);
                    }

                    var narrowedProperty = NarrowTypeInternal(
                        typeManager,
                        extraProperty.Value,
                        targetType.AdditionalPropertiesType.Type,
                        diagnosticWriter,
                        typeMismatchErrorFactory,
                        skipConstantCheckForProperty,
                        skipTypeErrors: true);

                    // TODO should we try and narrow the additional properties type? May be difficult
                }
            }

            return new NamedObjectType(targetType.Name, targetType.ValidationFlags, narrowedProperties, targetType.AdditionalPropertiesType, targetType.AdditionalPropertiesFlags);
        }

        private static (IPositionable positionable, string blockName) GetMissingPropertyContext(ITypeManager typeManager, SyntaxBase expression)
        {
            var parent = typeManager.GetParent(expression);
            
            // determine where to place the missing property error
            return parent switch
            {
                // for properties, put it on the property name in the parent object
                ObjectPropertySyntax objectPropertyParent => (objectPropertyParent.Key, "object"),

                // for declaration bodies, put it on the declaration identifier
                INamedDeclarationSyntax declarationParent => (declarationParent.Name, declarationParent.Keyword.Text),
                
                // for conditionals, put it on the parent declaration identifier
                // (the parent of a conditional can only be a resource or module declaration)
                IfConditionSyntax ifCondition => GetMissingPropertyContext(typeManager, ifCondition),

                // fall back to marking the entire object with the error
                _ => (expression, "object")
            };
        }

        private static TypeMismatchErrorFactory GetPropertyMismatchErrorFactory(bool shouldWarn, string propertyName)
        {
            return (expectedType, actualType, errorExpression) =>
            {
                var builder = DiagnosticBuilder.ForPosition(errorExpression);

                if (actualType is StringLiteralType)
                {
                    string? suggestedStringLiteral = null;

                    if (expectedType is StringLiteralType)
                    {
                        suggestedStringLiteral = SpellChecker.GetSpellingSuggestion(actualType.Name, expectedType.Name.AsEnumerable());
                    }

                    if (expectedType is UnionType unionType && unionType.Members.All(typeReference => typeReference.Type is StringLiteralType))
                    {
                        var stringLiteralCandidates = unionType.Members.Select(typeReference => typeReference.Type.Name).OrderBy(s => s);
                        suggestedStringLiteral = SpellChecker.GetSpellingSuggestion(actualType.Name, stringLiteralCandidates);
                    }

                    if (suggestedStringLiteral != null)
                    {
                        return builder.PropertyStringLiteralMismatchWithSuggestion(shouldWarn, propertyName, expectedType, actualType.Name, suggestedStringLiteral);
                    }
                }

                return builder.PropertyTypeMismatch(shouldWarn, propertyName, expectedType, actualType);
            };
        }
    }
}
