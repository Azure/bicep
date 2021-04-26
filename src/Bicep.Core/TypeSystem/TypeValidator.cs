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
    public class TypeValidator
    {
        private delegate Diagnostic TypeMismatchErrorFactory(TypeSymbol targetType, TypeSymbol expressionType, SyntaxBase expression);

        private readonly ITypeManager typeManager;
        private readonly IDiagnosticWriter diagnosticWriter;
        
        private class TypeValidatorConfig
        {
            public TypeValidatorConfig(bool skipTypeErrors, bool skipConstantCheck, bool disallowAny, TypeMismatchErrorFactory onTypeMismatch)
            {
                this.SkipTypeErrors = skipTypeErrors;
                this.SkipConstantCheck = skipConstantCheck;
                this.DisallowAny = disallowAny;
                this.OnTypeMismatch = onTypeMismatch;
            }

            public bool SkipTypeErrors { get; }

            public bool SkipConstantCheck { get; }

            public bool DisallowAny { get; }

            public TypeMismatchErrorFactory OnTypeMismatch { get; }
        }

        private TypeValidator(ITypeManager typeManager, IDiagnosticWriter diagnosticWriter)
        {
            this.typeManager = typeManager;
            this.diagnosticWriter = diagnosticWriter;
        }

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

                case IScopeReference targetScope:
                    // checking for valid combinations of scopes happens after type checking. this allows us to provide a richer & more intuitive error message.
                    return sourceType is IScopeReference;

                case UnionType union when ReferenceEquals(union, LanguageConstants.ResourceOrResourceCollectionRefItem):
                    return sourceType is IScopeReference || sourceType is ArrayType {Item: IScopeReference};

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
            var config = new TypeValidatorConfig(
                skipTypeErrors: false,
                skipConstantCheck: false,
                disallowAny: false,
                onTypeMismatch: (expectedType, actualType, errorExpression) => DiagnosticBuilder.ForPosition(errorExpression).ExpectedValueTypeMismatch(ShouldWarn(targetType), expectedType, actualType));

            var validator = new TypeValidator(typeManager, diagnosticWriter);

            return validator.NarrowType(config, expression, targetType);
        }

        private TypeSymbol NarrowType(TypeValidatorConfig config, SyntaxBase expression, TypeSymbol targetType)
        {
            var expressionType = typeManager.GetTypeInfo(expression);

            if (config.DisallowAny && expressionType is AnyType)
            {
                // certain properties such as scope, parent, dependsOn do not allow values of "any" type
                diagnosticWriter.Write(expression, x => x.AnyTypeIsNotAllowed());
            }

            if (config.SkipTypeErrors == false && expressionType is ErrorType)
            {
                // since we dynamically checked type, we need to collect the errors but only if the caller wants them
                diagnosticWriter.WriteMultiple(expressionType.GetDiagnostics());
                return targetType;
            }

            switch (targetType)
            {
                case ResourceType targetResourceType:
                {
                    var narrowedBody = NarrowType(config, expression, targetResourceType.Body.Type);

                    return new ResourceType(targetResourceType.TypeReference, targetResourceType.ValidParentScopes, narrowedBody);                    
                }
                case ModuleType targetModuleType:
                {
                    var narrowedBody = NarrowType(config, expression, targetModuleType.Body.Type);

                    return new ModuleType(targetModuleType.Name, targetModuleType.ValidParentScopes, narrowedBody);
                }
                case ArrayType loopArrayType when expression is ForSyntax @for:
                {
                    // for-expression assignability check
                    var narrowedBody = NarrowType(config, @for.Body, loopArrayType.Item.Type);

                    return new TypedArrayType(narrowedBody, TypeSymbolValidationFlags.Default);
                }
            }

            // basic assignability check
            if (AreTypesAssignable(expressionType, targetType) == false)
            {
                // fundamentally different types - cannot assign
                diagnosticWriter.Write(config.OnTypeMismatch(targetType, expressionType, expression));
                return targetType;
            }

            // object assignability check
            if (expression is ObjectSyntax objectValue)
            {
                switch (targetType)
                {
                    case ObjectType targetObjectType:
                        return NarrowObjectType(config, objectValue, targetObjectType);

                    case DiscriminatedObjectType targetDiscriminated:
                        return NarrowDiscriminatedObjectType(config, objectValue, targetDiscriminated);
                }
            }

            // if-condition assignability check
            if (expression is IfConditionSyntax { Body: ObjectSyntax body })
            {
                switch (targetType)
                {
                    case ObjectType targetObjectType:
                        return NarrowObjectType(config, body, targetObjectType);

                    case DiscriminatedObjectType targetDiscriminated:
                        return NarrowDiscriminatedObjectType(config, body, targetDiscriminated);
                }
            }

            // array assignability check
            if (expression is ArraySyntax arrayValue && targetType is ArrayType targetArrayType)
            {
                return NarrowArrayAssignmentType(config, arrayValue, targetArrayType);
            }

            if (targetType is UnionType targetUnionType)
            {
                return UnionType.Create(targetUnionType.Members.Where(x => AreTypesAssignable(expressionType, x.Type)));
            }

            return targetType;
        }

        private TypeSymbol NarrowArrayAssignmentType(TypeValidatorConfig config, ArraySyntax expression, ArrayType targetType)
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
                var newConfig = new TypeValidatorConfig(
                    skipConstantCheck: config.SkipConstantCheck,
                    skipTypeErrors: true,
                    disallowAny: config.DisallowAny,
                    onTypeMismatch: (expected, actual, position) => DiagnosticBuilder.ForPosition(position).ArrayTypeMismatch(ShouldWarn(targetType), expected, actual));

                var narrowedType = NarrowType(newConfig, arrayItemSyntax.Value, targetType.Item.Type);

                arrayProperties.Add(narrowedType);
            }

            return new TypedArrayType(UnionType.Create(arrayProperties), targetType.ValidationFlags);
        }

        private TypeSymbol NarrowDiscriminatedObjectType(TypeValidatorConfig config, ObjectSyntax expression, DiscriminatedObjectType targetType)
        {
            // if we have parse errors, there's no point to check assignability
            // we should not return the parse errors however because they will get double collected
            if (expression.HasParseErrors())
            {
                return LanguageConstants.Any;
            }

            var discriminatorProperty = expression.Properties.FirstOrDefault(p => targetType.TryGetDiscriminatorProperty(p.TryGetKeyText()) is not null);
            if (discriminatorProperty == null)
            {
                // object doesn't contain the discriminator field
                diagnosticWriter.Write(expression, x => x.MissingRequiredProperty(ShouldWarn(targetType), targetType.DiscriminatorKey, targetType.DiscriminatorKeysUnionType));

                var propertyKeys = expression.Properties
                    .Select(x => x.TryGetKeyText())
                    .Where(key => !string.IsNullOrEmpty(key))
                    .Select(key => key!);

                // do a reverse lookup to check if there's any misspelled discriminator key
                var misspelledDiscriminatorKey = SpellChecker.GetSpellingSuggestion(targetType.DiscriminatorKey, propertyKeys);

                if (misspelledDiscriminatorKey != null)
                {
                    diagnosticWriter.Write(expression, x => x.DisallowedPropertyWithSuggestion(ShouldWarn(targetType), misspelledDiscriminatorKey, targetType.DiscriminatorKeysUnionType, targetType.DiscriminatorKey));
                }

                return LanguageConstants.Any;
            }

            // At some point in the future we may want to relax the expectation of a string literal key, and allow a generic string.
            // In this case, the best we can do is validate against the union of all the settable properties.
            // Let's not do this just yet, and see if a use-case arises.

            var discriminatorType = typeManager.GetTypeInfo(discriminatorProperty.Value);
            if (discriminatorType is not StringLiteralType stringLiteralDiscriminator)
            {
                diagnosticWriter.Write(expression, x => x.PropertyTypeMismatch(ShouldWarn(targetType), targetType.DiscriminatorKey, targetType.DiscriminatorKeysUnionType, discriminatorType));
                return LanguageConstants.Any;
            }

            if (!targetType.UnionMembersByKey.TryGetValue(stringLiteralDiscriminator.Name, out var selectedObjectReference))
            {
                // no matches
                var discriminatorCandidates = targetType.UnionMembersByKey.Keys.OrderBy(x => x);
                string? suggestedDiscriminator = SpellChecker.GetSpellingSuggestion(stringLiteralDiscriminator.Name, discriminatorCandidates);
                bool shouldWarn = ShouldWarn(targetType);

                diagnosticWriter.Write(
                    discriminatorProperty.Value,
                    suggestedDiscriminator != null
                        ? x => x.PropertyStringLiteralMismatchWithSuggestion(shouldWarn, targetType.DiscriminatorKey, targetType.DiscriminatorKeysUnionType, stringLiteralDiscriminator.Name, suggestedDiscriminator)
                        : x => x.PropertyTypeMismatch(shouldWarn, targetType.DiscriminatorKey, targetType.DiscriminatorKeysUnionType, discriminatorType));

                return LanguageConstants.Any;
            }

            if (selectedObjectReference.Type is not ObjectType selectedObjectType)
            {
                throw new InvalidOperationException($"Discriminated type {targetType.Name} contains non-object member");
            }

            // we have a match!
            return NarrowObjectType(config, expression, selectedObjectType);
        }

        private TypeSymbol NarrowObjectType(TypeValidatorConfig config, ObjectSyntax expression, ObjectType targetType)
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

                diagnosticWriter.Write(positionable, x => x.MissingRequiredProperties(ShouldWarn(targetType), missingRequiredProperties, blockName));
            }

            var narrowedProperties = new List<TypeProperty>();
            foreach (var declaredProperty in targetType.Properties.Values)
            {
                if (namedPropertyMap.TryGetValue(declaredProperty.Name, out var declaredPropertySyntax))
                {
                    var skipConstantCheckForProperty = config.SkipConstantCheck;

                    // is the property marked as requiring compile-time constants and has the parent already validated this?
                    if (skipConstantCheckForProperty == false && declaredProperty.Flags.HasFlag(TypePropertyFlags.Constant))
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
                        var parent = typeManager.GetParent(expression);
                        if (parent is ResourceDeclarationSyntax resourceSyntax && resourceSyntax.IsExistingResource())
                        {
                            diagnosticWriter.Write(declaredPropertySyntax.Key, x => x.CannotUsePropertyInExistingResource(declaredProperty.Name));
                        }
                        else
                        {
                            diagnosticWriter.Write(declaredPropertySyntax.Key, x => x.CannotAssignToReadOnlyProperty(ShouldWarn(targetType), declaredProperty.Name));
                        }
                        narrowedProperties.Add(new TypeProperty(declaredProperty.Name, declaredProperty.TypeReference.Type, declaredProperty.Flags));
                        continue;
                    }

                    var newConfig = new TypeValidatorConfig(
                        skipConstantCheck: skipConstantCheckForProperty, 
                        skipTypeErrors: true,
                        disallowAny: declaredProperty.Flags.HasFlag(TypePropertyFlags.DisallowAny),
                        onTypeMismatch: GetPropertyMismatchErrorFactory(ShouldWarn(targetType), declaredProperty.Name));

                    var narrowedType = NarrowType(newConfig, declaredPropertySyntax.Value, declaredProperty.TypeReference.Type);

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
                foreach (var extraProperty in extraProperties)
                {
                    var skipConstantCheckForProperty = config.SkipConstantCheck;

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

                    var newConfig = new TypeValidatorConfig(
                        skipConstantCheck: skipConstantCheckForProperty, 
                        skipTypeErrors: true,
                        disallowAny: targetType.AdditionalPropertiesFlags.HasFlag(TypePropertyFlags.DisallowAny),
                        onTypeMismatch: typeMismatchErrorFactory);

                    // although we don't use the result here, it's important to call NarrowType to collect diagnostics
                    var narrowedType = NarrowType(newConfig, extraProperty.Value, targetType.AdditionalPropertiesType.Type);

                    // TODO should we try and narrow the additional properties type? May be difficult
                }
            }

            return new ObjectType(targetType.Name, targetType.ValidationFlags, narrowedProperties, targetType.AdditionalPropertiesType, targetType.AdditionalPropertiesFlags);
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
                ITopLevelNamedDeclarationSyntax declarationParent => (declarationParent.Name, declarationParent.Keyword.Text),
                
                // for conditionals, put it on the parent declaration identifier
                // (the parent of a conditional can only be a resource or module declaration)
                IfConditionSyntax ifCondition => GetMissingPropertyContext(typeManager, ifCondition),

                // for loops, put it on the parent declaration identifier
                // (the parent of a loop can only be a resource or module declaration)
                ForSyntax @for => GetMissingPropertyContext(typeManager, @for),

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
