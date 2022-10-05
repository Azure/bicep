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
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Bicep.Core.TypeSystem
{
    public class TypeValidator
    {
        private delegate void TypeMismatchDiagnosticWriter(TypeSymbol targetType, TypeSymbol expressionType, SyntaxBase expression);

        private readonly ITypeManager typeManager;
        private readonly IBinder binder;
        private readonly IDiagnosticWriter diagnosticWriter;

        private class TypeValidatorConfig
        {
            public TypeValidatorConfig(bool skipTypeErrors, bool skipConstantCheck, bool disallowAny, SyntaxBase? originSyntax, TypeMismatchDiagnosticWriter? onTypeMismatch, bool isResourceDeclaration)
            {
                this.SkipTypeErrors = skipTypeErrors;
                this.SkipConstantCheck = skipConstantCheck;
                this.DisallowAny = disallowAny;
                this.OriginSyntax = originSyntax;
                this.OnTypeMismatch = onTypeMismatch;
                this.IsResourceDeclaration = isResourceDeclaration;
            }

            public bool SkipTypeErrors { get; }

            public bool SkipConstantCheck { get; }

            public bool DisallowAny { get; }

            public SyntaxBase? OriginSyntax { get; }

            public TypeMismatchDiagnosticWriter? OnTypeMismatch { get; }

            public bool IsResourceDeclaration { get; }
        }

        private TypeValidator(ITypeManager typeManager, IBinder binder, IDiagnosticWriter diagnosticWriter)
        {
            this.typeManager = typeManager;
            this.binder = binder;
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

            switch (sourceType, targetType)
            {
                case (_, AnyType):
                    // values of all types can be assigned to the "any" type
                    return true;

                case (LambdaType sourceLambda, LambdaType targetLambda):
                    return AreLambdaTypesAssignable(sourceLambda, targetLambda);

                case (IScopeReference, IScopeReference):
                    // checking for valid combinations of scopes happens after type checking. this allows us to provide a richer & more intuitive error message.
                    return true;

                case (_, UnionType targetUnion) when ReferenceEquals(targetUnion, LanguageConstants.ResourceOrResourceCollectionRefItem):
                    return sourceType is IScopeReference || sourceType is ArrayType { Item: IScopeReference };

                case (ResourceType sourceResourceType, ResourceParentType targetResourceParentType):
                    // Assigning a resource to a parent property.
                    return sourceResourceType.TypeReference.IsParentOf(targetResourceParentType.ChildTypeReference);

                case (ResourceType sourceResourceType, ResourceParameterType resourceParameterType):
                    // Assigning a resource to a parameter ignores the API Version
                    return sourceResourceType.TypeReference.FormatType().Equals(resourceParameterType.TypeReference.FormatType(), StringComparison.OrdinalIgnoreCase);

                case (ResourceType sourceResourceType, _):
                    // When assigning a resource, we're really assigning the value of the resource body.
                    return AreTypesAssignable(sourceResourceType.Body.Type, targetType);

                case (ModuleType sourceModuleType, _):
                    // When assigning a module, we're really assigning the value of the module body.
                    return AreTypesAssignable(sourceModuleType.Body.Type, targetType);

                case (StringLiteralType, StringLiteralType):
                    // The name *is* the escaped string value, so we must have an exact match.
                    return targetType.Name == sourceType.Name;

                case (IntegerLiteralType sourceInt, IntegerLiteralType targetInt):
                    return targetInt.Value == sourceInt.Value;

                case (BooleanLiteralType sourceBool, BooleanLiteralType targetBool):
                    return sourceBool.Value == targetBool.Value;

                case (PrimitiveType, StringLiteralType):
                    // We allow primitive to like-typed literal assignment only in the case where the "AllowLooseAssignment" validation flag has been set.
                    // This is to allow parameters without 'allowed' values to be assigned to fields expecting enums.
                    // At some point we may want to consider flowing the enum type backwards to solve this more elegantly.
                    return sourceType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.AllowLooseAssignment) && sourceType.Name == LanguageConstants.String.Name;

                case (PrimitiveType, IntegerLiteralType):
                    return sourceType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.AllowLooseAssignment) && sourceType.Name == LanguageConstants.Int.Name;

                case (PrimitiveType, BooleanLiteralType):
                    return sourceType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.AllowLooseAssignment) && sourceType.Name == LanguageConstants.Bool.Name;

                case (StringLiteralType, PrimitiveType):
                    // string literals can be assigned to strings
                    return targetType.Name == LanguageConstants.String.Name;

                case (IntegerLiteralType, PrimitiveType):
                    // integer literals can be assigned to ints
                    return targetType.Name == LanguageConstants.Int.Name;

                case (BooleanLiteralType, PrimitiveType):
                    // boolean literals can be assigned to bools
                    return targetType.Name == LanguageConstants.Bool.Name;

                case (PrimitiveType, PrimitiveType):
                    // both types are primitive
                    // compare by type name
                    return string.Equals(sourceType.Name, targetType.Name, StringComparison.Ordinal);

                case (ObjectType, ObjectType):
                    // both types are objects
                    // this function does not implement any schema validation, so this is far as we go
                    return true;

                case (ArrayType, ArrayType):
                    // both types are arrays
                    // this function does not validate item types
                    return true;

                case (DiscriminatedObjectType, DiscriminatedObjectType):
                    // validation left for later
                    return true;

                case (ObjectType, DiscriminatedObjectType):
                    // validation left for later
                    return true;

                case (UnionType sourceUnion, _):
                    // union types are guaranteed to be flat

                    // TODO: Replace with some sort of set intersection
                    // are all source type members assignable to the target type?
                    return sourceUnion.Members.All(sourceMember => AreTypesAssignable(sourceMember.Type, targetType) == true);

                case (_, UnionType targetUnion):
                    // the source type should be a singleton type
                    Debug.Assert(!(sourceType is UnionType), "!(sourceType is UnionType)");

                    // can source type be assigned to any union member types
                    return targetUnion.Members.Any(targetMember => AreTypesAssignable(sourceType, targetMember.Type) == true);

                default:
                    // expression cannot be assigned to the type
                    return false;
            }
        }

        public static bool ShouldWarn(TypeSymbol targetType)
            => targetType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.WarnOnTypeMismatch);

        public static TypeSymbol NarrowTypeAndCollectDiagnostics(ITypeManager typeManager, IBinder binder, IDiagnosticWriter diagnosticWriter, SyntaxBase expression, TypeSymbol targetType, bool isResourceDeclaration = false)
        {
            var config = new TypeValidatorConfig(
                skipTypeErrors: false,
                skipConstantCheck: false,
                disallowAny: false,
                originSyntax: null,
                onTypeMismatch: null,
                isResourceDeclaration: isResourceDeclaration);

            var validator = new TypeValidator(typeManager, binder, diagnosticWriter);

            return validator.NarrowType(config, expression, targetType);
        }

        private TypeSymbol NarrowType(TypeValidatorConfig config, SyntaxBase expression, TypeSymbol targetType)
        {
            var expressionType = typeManager.GetTypeInfo(expression);

            if (config.DisallowAny && expressionType is AnyType)
            {
                // certain properties such as scope, parent, dependsOn do not allow values of "any" type
                diagnosticWriter.Write(config.OriginSyntax ?? expression, x => x.AnyTypeIsNotAllowed());

                // if we let the type narrowing continue, we could get more diagnostics
                // but it also leads to duplicate "disallow any" diagnostics caused by the union type narrowing
                // (occurs with "dependsOn: [ any(true) ]")
                return targetType;
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

                        return new ResourceType(
                            targetResourceType.DeclaringNamespace,
                            targetResourceType.TypeReference,
                            targetResourceType.ValidParentScopes,
                            targetResourceType.ReadOnlyScopes,
                            targetResourceType.Flags,
                            narrowedBody,
                            targetResourceType.UniqueIdentifierProperties);
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
                if (config.OnTypeMismatch is not null)
                {
                    config.OnTypeMismatch(targetType, expressionType, expression);
                }
                else
                {
                    diagnosticWriter.Write(config.OriginSyntax ?? expression, x => x.ExpectedValueTypeMismatch(ShouldWarn(targetType), targetType, expressionType));
                }

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

            if (expression is VariableAccessSyntax variableAccess)
            {
                return NarrowVariableAccessType(config, variableAccess, targetType);
            }

            if (targetType is UnionType targetUnionType)
            {
                // we need to narrow each union member so diagnostics get collected correctly
                // until we get union type simplification logic, this could generate duplicate diagnostics
                return TypeHelper.CreateTypeUnion(targetUnionType.Members
                    .Where(x => AreTypesAssignable(expressionType, x.Type))
                    .Select(x => NarrowType(config, expression, x.Type)));
            }

            if (expression is LambdaSyntax sourceLambda && targetType is LambdaType targetLambdaType)
            {
                return NarrowLambdaType(config, sourceLambda, targetLambdaType);
            }

            return expressionType;
        }

        private static bool AreLambdaTypesAssignable(LambdaType source, LambdaType target)
        {
            if (source.ArgumentTypes.Length != target.ArgumentTypes.Length)
            {
                return false;
            }

            if (!AreTypesAssignable(source.ReturnType.Type, target.ReturnType.Type))
            {
                return false;
            }

            var pairs = source.ArgumentTypes.Select((x, i) => (source: x, target: target.ArgumentTypes[i]));
            if (pairs.Any(x => !AreTypesAssignable(x.source.Type, x.target.Type)))
            {
                return false;
            }

            return true;
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
                    originSyntax: config.OriginSyntax,
                    onTypeMismatch: (expected, actual, position) => diagnosticWriter.Write(position, x => x.ArrayTypeMismatch(ShouldWarn(targetType), expected, actual)),
                    isResourceDeclaration: config.IsResourceDeclaration);

                var narrowedType = NarrowType(newConfig, arrayItemSyntax.Value, targetType.Item.Type);

                arrayProperties.Add(narrowedType);
            }

            return new TypedArrayType(TypeHelper.CreateTypeUnion(arrayProperties), targetType.ValidationFlags);
        }

        private TypeSymbol NarrowLambdaType(TypeValidatorConfig config, LambdaSyntax lambdaSyntax, LambdaType targetType)
        {
            var returnType = NarrowType(config, lambdaSyntax.Body, targetType.ReturnType.Type);

            var variables = lambdaSyntax.GetLocalVariables().ToImmutableArray();
            if (variables.Length != targetType.ArgumentTypes.Length)
            {
                diagnosticWriter.Write(lambdaSyntax.VariableSection, x => x.LambdaExpectedArgCountMismatch(targetType, targetType.ArgumentTypes.Length, variables.Length));
                return targetType;
            }

            var narrowedVariables = new ITypeReference[variables.Length];
            for (var i = 0; i < variables.Length; i++)
            {
                narrowedVariables[i] = NarrowType(config, variables[i], targetType.ArgumentTypes[i].Type);
            }

            return new LambdaType(narrowedVariables.ToImmutableArray(), returnType);
        }

        private TypeSymbol NarrowVariableAccessType(TypeValidatorConfig config, VariableAccessSyntax variableAccess, TypeSymbol targetType)
        {
            var newConfig = new TypeValidatorConfig(
                skipConstantCheck: config.SkipConstantCheck,
                skipTypeErrors: config.SkipTypeErrors,
                disallowAny: config.DisallowAny,
                originSyntax: variableAccess,
                onTypeMismatch: config.OnTypeMismatch,
                isResourceDeclaration: config.IsResourceDeclaration);

            // TODO: Implement for non-variable variable access (resource, module, param)
            switch (binder.GetSymbolInfo(variableAccess))
            {
                case VariableSymbol variableSymbol:
                    return NarrowType(newConfig, variableSymbol.DeclaringVariable.Value, targetType);
                case LocalVariableSymbol localVariableSymbol:
                    return NarrowType(newConfig, localVariableSymbol.DeclaringLocalVariable, targetType);
            }

            return targetType;
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
                diagnosticWriter.Write(config.OriginSyntax ?? expression, x => x.MissingRequiredProperty(ShouldWarn(targetType), targetType.DiscriminatorKey, targetType.DiscriminatorKeysUnionType));

                var propertyKeys = expression.Properties
                    .Select(x => x.TryGetKeyText())
                    .Where(key => !string.IsNullOrEmpty(key))
                    .Select(key => key!);

                // do a reverse lookup to check if there's any misspelled discriminator key
                var misspelledDiscriminatorKey = SpellChecker.GetSpellingSuggestion(targetType.DiscriminatorKey, propertyKeys);

                if (misspelledDiscriminatorKey is not null)
                {
                    var misspelledDiscriminatorProperty = expression.Properties.First(x => string.Equals(x.TryGetKeyText(), misspelledDiscriminatorKey));
                    diagnosticWriter.Write(config.OriginSyntax ?? misspelledDiscriminatorProperty.Key, x => x.DisallowedPropertyWithSuggestion(ShouldWarn(targetType), misspelledDiscriminatorKey, targetType.DiscriminatorKeysUnionType, targetType.DiscriminatorKey));
                }

                return LanguageConstants.Any;
            }

            // At some point in the future we may want to relax the expectation of a string literal key, and allow a generic string.
            // In this case, the best we can do is validate against the union of all the settable properties.
            // Let's not do this just yet, and see if a use-case arises.

            var discriminatorType = typeManager.GetTypeInfo(discriminatorProperty.Value);
            var shouldWarn = (config.IsResourceDeclaration && !targetType.DiscriminatorProperty.Flags.HasFlag(TypePropertyFlags.SystemProperty)) || ShouldWarn(targetType);
            switch (discriminatorType)
            {
                case AnyType:
                    return LanguageConstants.Any;

                case StringLiteralType stringLiteralDiscriminator:
                    if (!targetType.UnionMembersByKey.TryGetValue(stringLiteralDiscriminator.Name, out var selectedObjectReference))
                    {
                        // no matches
                        var discriminatorCandidates = targetType.UnionMembersByKey.Keys.OrderBy(x => x);


                        diagnosticWriter.Write(
                            config.OriginSyntax ?? discriminatorProperty.Value,
                            x =>
                            {
                                var sourceDeclaration = TryGetSourceDeclaration(config);

                                if (sourceDeclaration is null && SpellChecker.GetSpellingSuggestion(stringLiteralDiscriminator.Name, discriminatorCandidates) is { } suggestion)
                                {
                                    // only look up suggestions if we're not sourcing this type from another declaration.
                                    return x.PropertyStringLiteralMismatchWithSuggestion(shouldWarn, targetType.DiscriminatorKey, targetType.DiscriminatorKeysUnionType, stringLiteralDiscriminator.Name, suggestion);
                                }

                                return x.PropertyTypeMismatch(shouldWarn, sourceDeclaration, targetType.DiscriminatorKey, targetType.DiscriminatorKeysUnionType, discriminatorType, config.IsResourceDeclaration && !targetType.DiscriminatorProperty.Flags.HasFlag(TypePropertyFlags.SystemProperty));
                            });

                        return LanguageConstants.Any;
                    }

                    if (selectedObjectReference.Type is not ObjectType selectedObjectType)
                    {
                        throw new InvalidOperationException($"Discriminated type {targetType.Name} contains non-object member");
                    }

                    // we have a match!
                    return NarrowObjectType(config, expression, selectedObjectType);

                // ReSharper disable once ConvertTypeCheckPatternToNullCheck - using null pattern check causes compiler to think that discriminatorType might be null in the default clause.
                case TypeSymbol when AreTypesAssignable(discriminatorType, targetType.DiscriminatorKeysUnionType):
                    //check if discriminatorType is a subset of targetType.DiscriminatorKeysUnionType.
                    //If match - then warn with message that using property is not recommended and type validation is suspended and return generic object type
                    diagnosticWriter.Write(discriminatorProperty.Value, x => x.AmbiguousDiscriminatorPropertyValue(targetType.DiscriminatorKey));
                    //TODO: make a deep merge of the discriminator types to return combined object for type checking. Additionally, we need to cover hints.
                    return LanguageConstants.Any;

                default:
                    diagnosticWriter.Write(
                        config.OriginSyntax ?? discriminatorProperty.Value,
                        x => x.PropertyTypeMismatch(shouldWarn, TryGetSourceDeclaration(config), targetType.DiscriminatorKey, targetType.DiscriminatorKeysUnionType, discriminatorType, config.IsResourceDeclaration && !targetType.DiscriminatorProperty.Flags.HasFlag(TypePropertyFlags.SystemProperty)));
                    return LanguageConstants.Any;
            }
        }

        private TypeSymbol NarrowObjectType(TypeValidatorConfig config, ObjectSyntax expression, ObjectType targetType)
        {
            static (TypeSymbol type, bool typeWasPreserved) AddImplicitNull(TypeSymbol propertyType, TypePropertyFlags propertyFlags)
            {
                bool preserveType = propertyFlags.HasFlag(TypePropertyFlags.Required) || !propertyFlags.HasFlag(TypePropertyFlags.AllowImplicitNull);
                return (preserveType ? propertyType : TypeHelper.CreateTypeUnion(propertyType, LanguageConstants.Null), preserveType);
            }

            static TypeSymbol RemoveImplicitNull(TypeSymbol type, bool typeWasPreserved)
            {
                return typeWasPreserved || type is not UnionType unionType
                    ? type
                    : TypeHelper.CreateTypeUnion(unionType.Members.Where(m => m != LanguageConstants.Null));
            }

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
                .ToList();


            if (missingRequiredProperties.Count > 0)
            {
                var (positionable, blockName) = GetMissingPropertyContext(expression);

                var shouldWarn = (config.IsResourceDeclaration && missingRequiredProperties.All(p => !p.Flags.HasFlag(TypePropertyFlags.SystemProperty)))
                                 || ShouldWarn(targetType);

                var missingRequiredPropertiesNames = missingRequiredProperties.Select(p => p.Name).OrderBy(p => p).ToList();
                var showTypeInaccuracy = config.IsResourceDeclaration && missingRequiredProperties.Any(p => !p.Flags.HasFlag(TypePropertyFlags.SystemProperty));

                diagnosticWriter.Write(
                    config.OriginSyntax ?? positionable,
                    x => x.MissingRequiredProperties(shouldWarn, TryGetSourceDeclaration(config), expression, missingRequiredPropertiesNames, blockName, showTypeInaccuracy));
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
                        var parent = binder.GetParent(expression);
                        if (parent is ResourceDeclarationSyntax resourceSyntax && resourceSyntax.IsExistingResource())
                        {
                            diagnosticWriter.Write(config.OriginSyntax ?? declaredPropertySyntax.Key, x => x.CannotUsePropertyInExistingResource(declaredProperty.Name));
                        }
                        else
                        {
                            var resourceTypeInaccuracy = !declaredProperty.Flags.HasFlag(TypePropertyFlags.SystemProperty) && config.IsResourceDeclaration;
                            diagnosticWriter.Write(config.OriginSyntax ?? declaredPropertySyntax.Key, x => x.CannotAssignToReadOnlyProperty(resourceTypeInaccuracy || ShouldWarn(targetType), declaredProperty.Name, resourceTypeInaccuracy));
                        }

                        narrowedProperties.Add(new TypeProperty(declaredProperty.Name, declaredProperty.TypeReference.Type, declaredProperty.Flags));
                        continue;
                    }

                    if (declaredProperty.Flags.HasFlag(TypePropertyFlags.FallbackProperty))
                    {
                        diagnosticWriter.Write(config.OriginSyntax ?? declaredPropertySyntax.Key, x => x.FallbackPropertyUsed(declaredProperty.Name));
                    }

                    var newConfig = new TypeValidatorConfig(
                        skipConstantCheck: skipConstantCheckForProperty,
                        skipTypeErrors: true,
                        disallowAny: declaredProperty.Flags.HasFlag(TypePropertyFlags.DisallowAny),
                        originSyntax: config.OriginSyntax,
                        onTypeMismatch: GetPropertyMismatchDiagnosticWriter(config, (config.IsResourceDeclaration && !declaredProperty.Flags.HasFlag(TypePropertyFlags.SystemProperty)) || ShouldWarn(targetType), declaredProperty.Name, (config.IsResourceDeclaration && !declaredProperty.Flags.HasFlag(TypePropertyFlags.SystemProperty))),
                        isResourceDeclaration: config.IsResourceDeclaration);

                    // append "| null" to the property type for non-required properties
                    var (propertyAssignmentType, typeWasPreserved) = AddImplicitNull(declaredProperty.TypeReference.Type, declaredProperty.Flags);

                    var narrowedType = NarrowType(newConfig, declaredPropertySyntax.Value, propertyAssignmentType);
                    narrowedType = RemoveImplicitNull(narrowedType, typeWasPreserved);

                    narrowedProperties.Add(new TypeProperty(declaredProperty.Name, narrowedType, declaredProperty.Flags));
                }
                else
                {
                    narrowedProperties.Add(declaredProperty);
                }
            }

            // find properties that are specified on in the expression object but not declared in the schema
            var extraProperties = expression.Properties
                .Where(p => p.TryGetKeyText() is not string keyName || !targetType.Properties.ContainsKey(keyName));

            if (targetType.AdditionalPropertiesType == null)
            {
                // extra properties are not allowed by the type

                var shouldWarn = ShouldWarn(targetType);
                var validUnspecifiedProperties = targetType.Properties.Values
                    .Where(p => !p.Flags.HasFlag(TypePropertyFlags.ReadOnly) && !p.Flags.HasFlag(TypePropertyFlags.FallbackProperty) && !namedPropertyMap.ContainsKey(p.Name))
                    .Select(p => p.Name)
                    .OrderBy(x => x)
                    .ToList();

                foreach (var extraProperty in extraProperties)
                {

                    diagnosticWriter.Write(config.OriginSyntax ?? extraProperty.Key, x =>
                    {
                        var sourceDeclaration = TryGetSourceDeclaration(config);

                        if (extraProperty.TryGetKeyText() is not { } keyName)
                        {
                            return x.DisallowedInterpolatedKeyProperty(shouldWarn, sourceDeclaration, targetType, validUnspecifiedProperties);
                        }

                        if (sourceDeclaration is null && SpellChecker.GetSpellingSuggestion(keyName, validUnspecifiedProperties) is { } suggestedKeyName)
                        {
                            // only look up suggestions if we're not sourcing this type from another declaration.
                            return x.DisallowedPropertyWithSuggestion(shouldWarn, keyName, targetType, suggestedKeyName);
                        }

                        return x.DisallowedProperty(shouldWarn, sourceDeclaration, keyName, targetType, validUnspecifiedProperties, config.IsResourceDeclaration);
                    });
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

                    TypeMismatchDiagnosticWriter? onTypeMismatch = null;
                    if (extraProperty.TryGetKeyText() is { } keyName)
                    {
                        onTypeMismatch = GetPropertyMismatchDiagnosticWriter(config, ShouldWarn(targetType), keyName, false);
                    }

                    var newConfig = new TypeValidatorConfig(
                        skipConstantCheck: skipConstantCheckForProperty,
                        skipTypeErrors: true,
                        disallowAny: targetType.AdditionalPropertiesFlags.HasFlag(TypePropertyFlags.DisallowAny),
                        originSyntax: config.OriginSyntax,
                        onTypeMismatch: onTypeMismatch,
                        isResourceDeclaration: config.IsResourceDeclaration);

                    // append "| null" to the type on non-required properties
                    var (additionalPropertiesAssignmentType, _) = AddImplicitNull(targetType.AdditionalPropertiesType.Type, targetType.AdditionalPropertiesFlags);

                    // although we don't use the result here, it's important to call NarrowType to collect diagnostics
                    var narrowedType = NarrowType(newConfig, extraProperty.Value, additionalPropertiesAssignmentType);

                    // TODO should we try and narrow the additional properties type? May be difficult
                }
            }

            return new ObjectType(targetType.Name, targetType.ValidationFlags, narrowedProperties, targetType.AdditionalPropertiesType, targetType.AdditionalPropertiesFlags, targetType.MethodResolver.CopyToObject);
        }

        private (IPositionable positionable, string blockName) GetMissingPropertyContext(SyntaxBase expression)
        {
            var parent = binder.GetParent(expression);

            // determine where to place the missing property error
            return parent switch
            {
                // for properties, put it on the property name in the parent object
                ObjectPropertySyntax objectPropertyParent => (objectPropertyParent.Key, "object"),

                // for import declarations, mark the entire configuration object
                ImportDeclarationSyntax importParent => (expression, "object"),

                // for declaration bodies, put it on the declaration identifier
                ITopLevelNamedDeclarationSyntax declarationParent => (declarationParent.Name, declarationParent.Keyword.Text),

                // for conditionals, put it on the parent declaration identifier
                // (the parent of a conditional can only be a resource or module declaration)
                IfConditionSyntax ifCondition => GetMissingPropertyContext(ifCondition),

                // for loops, put it on the parent declaration identifier
                // (the parent of a loop can only be a resource or module declaration)
                ForSyntax @for => GetMissingPropertyContext(@for),

                // fall back to marking the entire object with the error
                _ => (expression, "object")
            };
        }

        private DeclaredSymbol? TryGetSourceDeclaration(TypeValidatorConfig config)
        {
            if (config.OriginSyntax is not null && binder.GetSymbolInfo(config.OriginSyntax) is DeclaredSymbol declaration)
            {
                return declaration;
            }

            return null;
        }

        private TypeMismatchDiagnosticWriter GetPropertyMismatchDiagnosticWriter(TypeValidatorConfig config, bool shouldWarn, string propertyName, bool showTypeInaccuracyClause)
        {
            return (expectedType, actualType, errorExpression) =>
            {
                diagnosticWriter.Write(
                    config.OriginSyntax ?? errorExpression,
                    x =>
                    {
                        var sourceDeclaration = TryGetSourceDeclaration(config);

                        if (sourceDeclaration is not null)
                        {
                            // only look up suggestions if we're not sourcing this type from another declaration.
                            return x.PropertyTypeMismatch(shouldWarn, sourceDeclaration, propertyName, expectedType, actualType, showTypeInaccuracyClause);
                        }

                        if (actualType is StringLiteralType actualStringLiteral && TryGetStringLiteralSuggestion(actualStringLiteral, expectedType) is { } suggestion)
                        {
                            return x.PropertyStringLiteralMismatchWithSuggestion(shouldWarn, propertyName, expectedType, actualType.Name, suggestion);
                        }

                        return x.PropertyTypeMismatch(shouldWarn, sourceDeclaration, propertyName, expectedType, actualType, showTypeInaccuracyClause);
                    });
            };
        }

        private static string? TryGetStringLiteralSuggestion(StringLiteralType actualType, TypeSymbol expectedType)
        {
            if (expectedType is StringLiteralType)
            {
                return SpellChecker.GetSpellingSuggestion(actualType.Name, expectedType.Name.AsEnumerable());
            }

            if (expectedType is UnionType unionType && unionType.Members.All(typeReference => typeReference.Type is StringLiteralType))
            {
                var stringLiteralCandidates = unionType.Members.Select(typeReference => typeReference.Type.Name).OrderBy(s => s);

                return SpellChecker.GetSpellingSuggestion(actualType.Name, stringLiteralCandidates);
            }

            return null;
        }
    }
}
