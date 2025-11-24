// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Intermediate;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem
{
    public class DeclaredTypeManager
    {
        // maps syntax nodes to their declared types
        // processed nodes found not to have a declared type will have a null value
        private readonly ConcurrentDictionary<SyntaxBase, DeclaredTypeAssignment?> declaredTypes = new();
        private readonly ConcurrentDictionary<TypeAliasSymbol, TypeSymbol> userDefinedTypeReferences = new();
        private readonly ConcurrentDictionary<ParameterizedTypeInstantiationSyntaxBase, ResultWithDiagnostic<TypeExpression>> reifiedTypes = new();
        private readonly Lazy<ImmutableDictionary<TypeAliasSymbol, ImmutableArray<TypeAliasSymbol>>> typeCycles;
        private readonly ITypeManager typeManager;
        private readonly IBinder binder;
        private readonly IFeatureProvider features;
        private readonly ResourceDerivedTypeResolver resourceDerivedTypeResolver;

        public DeclaredTypeManager(ITypeManager typeManager, IBinder binder, IFeatureProvider features)
        {
            this.typeManager = typeManager;
            this.binder = binder;
            this.features = features;
            this.resourceDerivedTypeResolver = new(binder);
            this.typeCycles = new(() => CyclicTypeCheckVisitor.FindCycles(
                binder,
                syntax => TryGetTypeFromTypeSyntax(syntax)?.Type));
        }

        public DeclaredTypeAssignment? GetDeclaredTypeAssignment(SyntaxBase syntax) =>
            this.declaredTypes.GetOrAdd(syntax, key => GetTypeAssignment(key));

        public TypeSymbol? GetDeclaredType(SyntaxBase syntax) => this.GetDeclaredTypeAssignment(syntax)?.Reference.Type;

        public TypeExpression? TryGetReifiedType(ParameterizedTypeInstantiationSyntaxBase syntax)
            => GetReifiedTypeResult(syntax).TryUnwrap();

        private DeclaredTypeAssignment? GetTypeAssignment(SyntaxBase syntax)
        {
            if (binder.GetSymbolInfo(syntax) is TypeAliasSymbol typeAlias)
            {
                if (typeCycles.Value.TryGetValue(typeAlias, out var cycle))
                {
                    var builder = DiagnosticBuilder.ForPosition(typeAlias.DeclaringType.Name);
                    var diagnostic = cycle.Length == 1
                        ? builder.CyclicTypeSelfReference()
                        : builder.CyclicType(cycle.Select(s => s.Name));

                    return new(ErrorType.Create(diagnostic), syntax);
                }
            }
            else
            {
                foreach (var typeAccessSyntax in SyntaxAggregator.AggregateByType<TypeVariableAccessSyntax>(syntax))
                {
                    if (binder.GetSymbolInfo(typeAccessSyntax) is TypeAliasSymbol accessedTypeAlias &&
                        typeCycles.Value.TryGetValue(accessedTypeAlias, out var cycle))
                    {
                        var builder = DiagnosticBuilder.ForPosition(typeAccessSyntax);
                        var diagnostic = builder.ReferencedSymbolHasErrors(accessedTypeAlias.Name);
                        return new(ErrorType.Create(diagnostic), syntax);
                    }
                }
            }

            return GetTypeAssignmentWithoutCycleCheck(syntax);
        }

        private DeclaredTypeAssignment? GetTypeAssignmentWithoutCycleCheck(SyntaxBase syntax)
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();

            if (syntax is TypeSyntax)
            {
                return TryGetTypeAssignmentFromTypeSyntax(syntax);
            }

            switch (syntax)
            {
                case ExtensionDeclarationSyntax extension:
                    return GetExtensionType(extension);

                case ExtensionConfigAssignmentSyntax extConfigAssignment:
                    return GetExtensionConfigAssignmentType(extConfigAssignment);

                case MetadataDeclarationSyntax metadata:
                    return new DeclaredTypeAssignment(this.typeManager.GetTypeInfo(metadata.Value), metadata);

                case ParameterDeclarationSyntax parameter:
                    return GetParameterType(parameter);

                case ParameterAssignmentSyntax parameterAssignment:
                    return GetParameterAssignmentType(parameterAssignment);

                case TypeDeclarationSyntax typeDeclaration:
                    return GetTypeType(typeDeclaration);

                case ResourceDeclarationSyntax resource:
                    return GetResourceType(resource);

                case ModuleDeclarationSyntax module:
                    return GetModuleType(module);

                case TestDeclarationSyntax test:
                    return GetTestType(test);

                case VariableAccessSyntax variableAccess:
                    return GetVariableAccessType(variableAccess);

                case OutputDeclarationSyntax output:
                    return GetOutputType(output);

                case AssertDeclarationSyntax assert:
                    return new DeclaredTypeAssignment(TypeFactory.CreateBooleanType(), assert);

                case TargetScopeSyntax targetScope:
                    var supportedScopes = TargetScopeSyntax.GetDeclaredType(features);

                    return new DeclaredTypeAssignment(
                        supportedScopes,
                        targetScope, DeclaredTypeFlags.Constant);

                case IfConditionSyntax ifCondition:
                    return GetIfConditionType(ifCondition);

                case ForSyntax @for:
                    return GetForSyntaxType(@for);

                case AccessExpressionSyntax accessExpression:
                    return GetAccessExpressionType(accessExpression);

                case ResourceAccessSyntax resourceAccess:
                    return GetResourceAccessType(resourceAccess);

                case LocalVariableSyntax localVariable:
                    return new DeclaredTypeAssignment(this.typeManager.GetTypeInfo(localVariable), localVariable);

                case FunctionCallSyntax functionCall:
                    return GetFunctionType(functionCall);

                case InstanceFunctionCallSyntax instanceFunctionCall:
                    return GetFunctionType(instanceFunctionCall);

                case ArraySyntax array:
                    return GetArrayType(array);

                case ArrayItemSyntax arrayItem:
                    return GetArrayItemType(arrayItem);

                case ObjectSyntax @object:
                    return GetObjectType(@object);

                case ObjectPropertySyntax objectProperty:
                    return GetObjectPropertyType(objectProperty);

                case StringSyntax @string:
                    return GetStringType(@string);

                case FunctionArgumentSyntax functionArgument:
                    return GetFunctionArgumentType(functionArgument);

                case NonNullAssertionSyntax nonNullAssertion:
                    return GetNonNullType(nonNullAssertion);

                case TypedLocalVariableSyntax typedLocalVariable:
                    return GetTypedLocalVariableType(typedLocalVariable);

                case TypedLambdaSyntax typedLambda:
                    return GetTypedLambdaType(typedLambda);

                case VariableDeclarationSyntax variableDeclaration:
                    return GetVariableTypeIfDefined(variableDeclaration);

                case UsingWithClauseSyntax usingWithClause:
                    return GetUsingConfigAssignmentType(usingWithClause);
            }

            return null;
        }

        private DeclaredTypeAssignment GetTypedLocalVariableType(TypedLocalVariableSyntax syntax)
        {
            var declaredType = TryGetTypeFromTypeSyntax(syntax.Type) ??
                ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Type).InvalidParameterType(GetValidTypeNames()));

            return new(DisallowNamespaceTypes(declaredType, syntax.Type), syntax);
        }

        private DeclaredTypeAssignment GetTypedLambdaType(TypedLambdaSyntax syntax)
        {
            var argumentTypes = syntax.GetLocalVariables()
                .Select(x => GetTypedLocalVariableType(x).Reference)
                .ToImmutableArray();

            var returnType = TryGetTypeFromTypeSyntax(syntax.ReturnType) ??
                ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.ReturnType).InvalidOutputType(GetValidTypeNames()));

            var type = new LambdaType(argumentTypes, [], DisallowNamespaceTypes(returnType, syntax.ReturnType));
            return new(type, syntax);
        }

        private DeclaredTypeAssignment GetParameterType(ParameterDeclarationSyntax syntax)
        {
            var declaredType = TryGetTypeFromTypeSyntax(syntax.Type)
                ?? ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Type).InvalidParameterType(GetValidTypeNames()));

            return new(ApplyTypeModifyingDecorators(DisallowNamespaceTypes(declaredType.Type, syntax.Type), syntax, allowLooseAssignment: true), syntax);
        }

        private DeclaredTypeAssignment? GetParameterAssignmentType(ParameterAssignmentSyntax syntax)
        {
            if (GetDeclaredParameterAssignmentType(syntax) is { } declaredParamAssignmentType)
            {
                return new(declaredParamAssignmentType, syntax);
            }

            return null;
        }

        private TypeSymbol? GetDeclaredParameterAssignmentType(ParameterAssignmentSyntax syntax)
        {
            if (!binder.FileSymbol.TryGetBicepFileSemanticModelViaUsing().IsSuccess(out var semanticModel, out var failureDiagnostic))
            {
                // failed to resolve using
                return failureDiagnostic.IsError() ? ErrorType.Create(failureDiagnostic) : null;
            }

            if (semanticModel.Parameters.TryGetValue(syntax.Name.IdentifierName, out var parameterMetadata))
            {
                return parameterMetadata.TypeReference.Type;
            }

            return null;
        }

        private DeclaredTypeAssignment GetTypeType(TypeDeclarationSyntax syntax)
        {
            var type = binder.GetSymbolInfo(syntax) switch
            {
                TypeAliasSymbol declaredType => userDefinedTypeReferences.GetOrAdd(declaredType, GetUserDefinedTypeType),
                ErrorSymbol errorSymbol => errorSymbol.ToErrorType(),
                // binder.GetSymbolInfo(TypeDeclarationSyntax) should always return a TypeAliasSymbol or an error, but just in case...
                _ => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).SymbolicNameIsNotAType(syntax.Name.IdentifierName, GetValidTypeNames())),
            };
            var typeRefType = type switch
            {
                ErrorType or TypeType => type,
                _ => new TypeType(type),
            };

            return new(typeRefType, syntax);
        }

        private TypeSymbol GetUserDefinedTypeType(TypeAliasSymbol symbol)
        {
            if (binder.TryGetCycle(symbol) is { } cycle)
            {
                var builder = DiagnosticBuilder.ForPosition(symbol.DeclaringType.Name);
                var diagnostic = cycle.Length == 1
                    ? builder.CyclicTypeSelfReference()
                    : builder.CyclicType(cycle.Select(s => s.Name));

                return ErrorType.Create(diagnostic);
            }

            return ApplyTypeModifyingDecorators(
                DisallowNamespaceTypes(GetTypeFromTypeSyntax(symbol.DeclaringType.Value).Type, symbol.DeclaringType.Value),
                symbol.DeclaringType);
        }

        private static ITypeReference DisallowNamespaceTypes(ITypeReference typeReference, SyntaxBase syntax) => typeReference switch
        {
            DeferredTypeReference => new DeferredTypeReference(() => DisallowNamespaceTypes(typeReference.Type, syntax)),
            _ => DisallowNamespaceTypes(typeReference.Type, syntax),
        };

        private static TypeSymbol DisallowNamespaceTypes(TypeSymbol type, SyntaxBase syntax) => type switch
        {
            NamespaceType namespaceType => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).NamespaceSymbolUsedAsType(namespaceType.Name)),
            _ => type,
        };

        private ITypeReference GetTypePropertyType(ObjectTypePropertySyntax syntax) => GetTypeFromTypeSyntax(syntax.Value) switch
        {
            DeferredTypeReference deferred => new DeferredTypeReference(() => GetTypePropertyType(syntax, deferred.Type)),
            ITypeReference otherwise => GetTypePropertyType(syntax, otherwise.Type),
        };

        private TypeSymbol GetTypePropertyType(ObjectTypePropertySyntax syntax, TypeSymbol typeSymbol)
            => ApplyTypeModifyingDecorators(DisallowNamespaceTypes(UnwrapType(typeSymbol), syntax.Value), syntax);

        private TypeSymbol GetInstantiatedType(ParameterizedTypeInstantiationSyntaxBase syntax)
            => GetReifiedTypeResult(syntax).IsSuccess(out var typeExpression, out var error)
                ? typeExpression.ExpressedType
                : ErrorType.Create(error);

        private ITypeReference ApplyTypeModifyingDecorators(ITypeReference declaredType, DecorableSyntax syntax, bool allowLooseAssignment = false) => declaredType switch
        {
            DeferredTypeReference => new DeferredTypeReference(() => ApplyTypeModifyingDecorators(declaredType.Type, syntax, allowLooseAssignment)),
            _ => ApplyTypeModifyingDecorators(declaredType.Type, syntax, allowLooseAssignment),
        };

        // decorator diagnostics are raised by the TypeAssignmentVisitor, so we're only concerned in this method
        // with the happy path or any errors that produce an invalid type
        private TypeSymbol ApplyTypeModifyingDecorators(TypeSymbol declaredType, DecorableSyntax syntax, bool allowLooseAssignment = false)
        {
            var validationFlags = declaredType switch
            {
                BooleanType or IntegerType or StringType when allowLooseAssignment
                    => TypeSymbolValidationFlags.AllowLooseAssignment | declaredType.ValidationFlags,
                _ => declaredType.ValidationFlags,
            };

            if (HasSecureDecorator(syntax))
            {
                validationFlags |= TypeSymbolValidationFlags.IsSecure;
            }

            return declaredType switch
            {
                _ when declaredType.ValidationFlags == validationFlags && !syntax.Decorators.Any() => declaredType,
                _ when TypeHelper.TryRemoveNullability(declaredType) is TypeSymbol nonNullable
                    => TypeHelper.CreateTypeUnion(LanguageConstants.Null, ApplyTypeModifyingDecorators(nonNullable, syntax, allowLooseAssignment)),
                IntegerType declaredInt => GetModifiedInteger(declaredInt, syntax, validationFlags),
                // minLength/maxLength on a tuple are superfluous.
                TupleType declaredTuple => declaredTuple.ValidationFlags == validationFlags ? declaredTuple : new TupleType(declaredTuple.Items, validationFlags),
                ArrayType declaredArray => GetModifiedArray(declaredArray, syntax, validationFlags),
                StringType declaredString => GetModifiedString(declaredString, syntax, validationFlags),
                BooleanType declaredBoolean => TypeFactory.CreateBooleanType(validationFlags),
                ObjectType declaredObject => GetModifiedObject(declaredObject, syntax, validationFlags),
                _ => declaredType,
            };
        }

        private TypeSymbol GetModifiedInteger(IntegerType declaredInteger, DecorableSyntax syntax, TypeSymbolValidationFlags validationFlags)
        {
            var minValueDecorator = SemanticModelHelper.TryGetDecoratorInNamespace(binder, typeManager.GetDeclaredType, syntax, SystemNamespaceType.BuiltInName, LanguageConstants.ParameterMinValuePropertyName);
            var minValue = GetSingleIntDecoratorArgument(minValueDecorator) ?? declaredInteger.MinValue;
            var maxValueDecorator = SemanticModelHelper.TryGetDecoratorInNamespace(binder, typeManager.GetDeclaredType, syntax, SystemNamespaceType.BuiltInName, LanguageConstants.ParameterMaxValuePropertyName);
            var maxValue = GetSingleIntDecoratorArgument(maxValueDecorator) ?? declaredInteger.MaxValue;

            if (minValue.HasValue && maxValue.HasValue && minValue.Value > maxValue.Value)
            {
                // create at most one error diagnostic iff a min/maxValue decorator targets this statement.
                if (minValueDecorator is not null)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(minValueDecorator).MinMayNotExceedMax(
                        LanguageConstants.ParameterMinValuePropertyName,
                        minValue.Value,
                        LanguageConstants.ParameterMaxValuePropertyName,
                        maxValue.Value));
                }

                if (maxValueDecorator is not null)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(maxValueDecorator).MinMayNotExceedMax(
                        LanguageConstants.ParameterMinValuePropertyName,
                        minValue.Value,
                        LanguageConstants.ParameterMaxValuePropertyName,
                        maxValue.Value));
                }
            }

            return TypeFactory.CreateIntegerType(minValue, maxValue, validationFlags);
        }

        private long? GetSingleIntDecoratorArgument(DecoratorSyntax? syntax)
            => syntax?.Arguments.Count() == 1 && typeManager.GetTypeInfo(syntax.Arguments.Single()) is IntegerLiteralType integerLiteral
                ? integerLiteral.Value
                : null;

        private string? GetSingleStringDecoratorArgument(DecoratorSyntax? syntax)
            => syntax?.Arguments.Count() == 1 && typeManager.GetTypeInfo(syntax.Arguments.Single()) is StringLiteralType stringLiteral
                ? stringLiteral.RawStringValue
                : null;

        private TypeSymbol GetModifiedArray(ArrayType declaredArray, DecorableSyntax syntax, TypeSymbolValidationFlags validationFlags)
        {
            if (!GetLengthModifiers(syntax, declaredArray.MinLength, declaredArray.MaxLength, out var minLength, out var maxLength, out var errorType))
            {
                return errorType;
            }

            return TypeFactory.CreateArrayType(declaredArray.Item, minLength, maxLength, validationFlags);
        }

        private TypeSymbol GetModifiedString(StringType declaredString, DecorableSyntax syntax, TypeSymbolValidationFlags validationFlags)
        {
            if (!GetLengthModifiers(syntax, declaredString.MinLength, declaredString.MaxLength, out var minLength, out var maxLength, out var errorType))
            {
                return errorType;
            }

            return TypeFactory.CreateStringType(minLength, maxLength, validationFlags: validationFlags);
        }

        private bool GetLengthModifiers(DecorableSyntax syntax, long? defaultMinLength, long? defaultMaxLength, out long? minLength, out long? maxLength, [NotNullWhen(false)] out ErrorType? error)
        {
            var minLengthDecorator = SemanticModelHelper.TryGetDecoratorInNamespace(binder, typeManager.GetDeclaredType, syntax, SystemNamespaceType.BuiltInName, LanguageConstants.ParameterMinLengthPropertyName);
            minLength = GetSingleIntDecoratorArgument(minLengthDecorator) ?? defaultMinLength;
            var maxLengthDecorator = SemanticModelHelper.TryGetDecoratorInNamespace(binder, typeManager.GetDeclaredType, syntax, SystemNamespaceType.BuiltInName, LanguageConstants.ParameterMaxLengthPropertyName);
            maxLength = GetSingleIntDecoratorArgument(maxLengthDecorator) ?? defaultMaxLength;

            if (minLength.HasValue && maxLength.HasValue && minLength.Value > maxLength.Value)
            {
                // create at most one error diagnostic iff a min/maxLength decorator targets this statement.
                if (minLengthDecorator is not null)
                {
                    error = ErrorType.Create(DiagnosticBuilder.ForPosition(minLengthDecorator).MinMayNotExceedMax(
                        LanguageConstants.ParameterMinLengthPropertyName,
                        minLength.Value,
                        LanguageConstants.ParameterMaxLengthPropertyName,
                        maxLength.Value));
                    return false;
                }

                if (maxLengthDecorator is not null)
                {
                    error = ErrorType.Create(DiagnosticBuilder.ForPosition(maxLengthDecorator).MinMayNotExceedMax(
                        LanguageConstants.ParameterMinLengthPropertyName,
                        minLength.Value,
                        LanguageConstants.ParameterMaxLengthPropertyName,
                        maxLength.Value));
                    return false;
                }
            }

            error = null;
            return true;
        }

        private TypeSymbol GetModifiedObject(ObjectType declaredObject, DecorableSyntax syntax, TypeSymbolValidationFlags validationFlags)
        {
            if (TryGetSystemDecorator(syntax, LanguageConstants.ParameterSealedPropertyName) is not null)
            {
                return new ObjectType(declaredObject.Name, validationFlags, declaredObject.Properties.Values, null);
            }

            if (declaredObject.ValidationFlags == validationFlags)
            {
                return declaredObject;
            }

            return new ObjectType(declaredObject.Name, validationFlags, declaredObject.Properties.Values, declaredObject.AdditionalProperties);
        }

        private ITypeReference GetTypeAdditionalPropertiesType(ObjectTypeAdditionalPropertiesSyntax syntax)
            => ApplyTypeModifyingDecorators(DisallowNamespaceTypes(GetTypeFromTypeSyntax(syntax.Value), syntax.Value), syntax);

        private ITypeReference GetTypeMemberType(ArrayTypeMemberSyntax syntax)
            => DisallowNamespaceTypes(GetTypeFromTypeSyntax(syntax.Value), syntax.Value);

        private ITypeReference GetTypeMemberType(UnionTypeMemberSyntax syntax) => syntax.Value switch
        {
            // A `null` literal is usually too ambiguous to be a valid type (a `null` value could be valid for any nullable type), but it is permitted as a member of a union of literals.
            NullTypeLiteralSyntax => LanguageConstants.Null,
            _ => DisallowNamespaceTypes(GetTypeFromTypeSyntax(syntax.Value), syntax.Value),
        };

        private DeclaredTypeAssignment GetOutputType(OutputDeclarationSyntax syntax)
        {
            var declaredType = TryGetTypeFromTypeSyntax(syntax.Type) ??
                ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Type).InvalidOutputType(GetValidTypeNames()));

            return new(ApplyTypeModifyingDecorators(DisallowNamespaceTypes(declaredType.Type, syntax.Type), syntax), syntax);
        }

        private DeclaredTypeAssignment? GetVariableTypeIfDefined(VariableDeclarationSyntax syntax)
        {
            if (syntax.Type is null)
            {
                return null;
            }

            var declaredType = TryGetTypeFromTypeSyntax(syntax.Type) ??
                ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Type).InvalidVariableType(GetValidTypeNames()));

            return new(ApplyTypeModifyingDecorators(DisallowNamespaceTypes(declaredType.Type, syntax.Type), syntax), syntax);
        }

        private ITypeReference GetTypeFromTypeSyntax(SyntaxBase syntax) => TryGetTypeFromTypeSyntax(syntax)
            ?? ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).InvalidTypeDefinition());

        private ITypeReference? TryGetTypeFromTypeSyntax(SyntaxBase syntax)
            => TryGetTypeAssignmentFromTypeSyntax(syntax)?.Reference;

        private DeclaredTypeAssignment? TryGetTypeAssignmentFromTypeSyntax(SyntaxBase syntax) => declaredTypes.GetOrAdd(syntax, s =>
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();

            // assume "any" type when the parameter has parse errors (either missing or was skipped)
            var declaredType = syntax switch
            {
                SkippedTriviaSyntax => LanguageConstants.Any,
                ResourceTypeSyntax resource => GetTypeReferenceForResourceType(resource),
                TypeVariableAccessSyntax typeRef => ConvertTypeExpressionToType(typeRef),
                ParameterizedTypeInstantiationSyntaxBase parameterizedTypeInvocation => GetInstantiatedType(parameterizedTypeInvocation),
                ArrayTypeSyntax array => GetArrayTypeType(array),
                ArrayTypeMemberSyntax arrayMember => GetTypeMemberType(arrayMember),
                ObjectTypeSyntax @object => GetObjectTypeType(@object),
                ObjectTypePropertySyntax objectProperty => GetTypePropertyType(objectProperty),
                ObjectTypeAdditionalPropertiesSyntax objectAdditionalProperties => GetTypeAdditionalPropertiesType(objectAdditionalProperties),
                TupleTypeSyntax tuple => GetTupleTypeType(tuple),
                TupleTypeItemSyntax tupleItem => GetTupleTypeItemType(tupleItem),
                StringTypeLiteralSyntax @string => ConvertTypeExpressionToType(@string),
                IntegerTypeLiteralSyntax @int => ConvertTypeExpressionToType(@int),
                BooleanTypeLiteralSyntax @bool => ConvertTypeExpressionToType(@bool),
                UnaryTypeOperationSyntax unaryOperation => GetUnaryOperationType(unaryOperation),
                UnionTypeSyntax unionType => GetUnionTypeType(unionType),
                UnionTypeMemberSyntax unionTypeMember => GetTypeMemberType(unionTypeMember),
                ParenthesizedTypeSyntax parenthesized => ConvertTypeExpressionToType(parenthesized),
                TypePropertyAccessSyntax propertyAccess => ConvertTypeExpressionToType(propertyAccess),
                TypeAdditionalPropertiesAccessSyntax additionalPropertiesAccess => ConvertTypeExpressionToType(additionalPropertiesAccess),
                TypeArrayAccessSyntax arrayAccess => ConvertTypeExpressionToType(arrayAccess),
                TypeItemsAccessSyntax itemsAccess => ConvertTypeExpressionToType(itemsAccess),
                NullableTypeSyntax nullableType => ConvertTypeExpressionToType(nullableType),
                NonNullableTypeSyntax nonNullAssertion => ConvertTypeExpressionToType(nonNullAssertion),
                _ => null
            };

            return declaredType is not null ? new(declaredType, syntax, DeclaredTypeFlags.None) : null;
        });

        private TypeSymbol GetTypeReferenceForResourceType(ResourceTypeSyntax syntax)
        {
            if (!this.features.ResourceTypedParamsAndOutputsEnabled)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Span).ParamOrOutputResourceTypeUnsupported());
            }

            // The resource type of an output can be inferred.
            var type = syntax.Type == null && GetOutputValueType(syntax) is { } inferredType ? inferredType : GetDeclaredResourceType(syntax);

            if (type is ResourceType resourceType && !resourceType.IsAzResource())
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).UnsupportedResourceTypeParameterOrOutputType(resourceType.Name));
            }

            return type;
        }

        private TypeSymbol? GetOutputValueType(SyntaxBase syntax) => binder.GetParent(syntax) switch
        {
            OutputDeclarationSyntax outputDeclaration => typeManager.GetTypeInfo(outputDeclaration.Value),
            ParenthesizedExpressionSyntax parenthesized => GetOutputValueType(parenthesized),
            _ => null,
        };

        private ITypeReference ConvertTypeExpressionToType(TypeVariableAccessSyntax syntax)
            => binder.GetSymbolInfo(syntax) switch
            {
                BuiltInNamespaceSymbol builtInNamespace => builtInNamespace.Type,
                ExtensionNamespaceSymbol extensionNamespace => extensionNamespace.Type,
                WildcardImportSymbol wildcardImport => wildcardImport.Type,
                AmbientTypeSymbol ambientType => EnsureNonParameterizedType(syntax, UnwrapType(ambientType.Type)),
                ImportedTypeSymbol importedType => EnsureNonParameterizedType(syntax, UnwrapType(importedType.Type)),
                TypeAliasSymbol declaredType => EnsureNonParameterizedType(syntax, TypeRefToType(syntax, declaredType)),
                DeclaredSymbol declaredSymbol => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).ValueSymbolUsedAsType(declaredSymbol.Name)),
                _ => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).SymbolicNameIsNotAType(syntax.Name.IdentifierName, GetValidTypeNames())),
            };

        private IEnumerable<string> GetValidTypeNames() => binder.NamespaceResolver.GetKnownPropertyNames()
            .Concat(binder.FileSymbol.TypeDeclarations.Select(td => td.Name))
            .Concat(binder.FileSymbol.ImportedTypes.Select(i => i.Name))
            .Distinct();

        private ResultWithDiagnostic<TypeExpression> GetReifiedTypeResult(ParameterizedTypeInstantiationSyntaxBase syntax)
            => reifiedTypes.GetOrAdd(syntax, InstantiateType);

        private ResultWithDiagnostic<TypeExpression> InstantiateType(ParameterizedTypeInstantiationSyntaxBase syntax) => syntax switch
        {
            ParameterizedTypeInstantiationSyntax unqualified => InstantiateType(unqualified),
            InstanceParameterizedTypeInstantiationSyntax qualified => InstantiateType(qualified),
            _ => throw new UnreachableException($"Unrecognized subtype of {nameof(ParameterizedTypeInstantiationSyntaxBase)}: {syntax.GetType().FullName}"),
        };

        private ResultWithDiagnostic<TypeExpression> InstantiateType(ParameterizedTypeInstantiationSyntax syntax) => binder.GetSymbolInfo(syntax) switch
        {
            AmbientTypeSymbol ambientType => InstantiateType(syntax, ambientType.Name, ambientType.Type),
            ImportedTypeSymbol importedType => InstantiateType(syntax, importedType.Name, importedType.Type),
            TypeAliasSymbol typeAlias => InstantiateType(syntax, typeAlias.Name, GetUserDefinedTypeType(typeAlias)),
            DeclaredSymbol declaredSymbol => new(DiagnosticBuilder.ForPosition(syntax).ValueSymbolUsedAsType(declaredSymbol.Name)),
            _ => new(DiagnosticBuilder.ForPosition(syntax).SymbolicNameIsNotAType(syntax.Name.IdentifierName, GetValidTypeNames())),
        };

        private ResultWithDiagnostic<TypeExpression> InstantiateType(InstanceParameterizedTypeInstantiationSyntax syntax)
        {
            var baseType = GetTypeFromTypeSyntax(syntax.BaseExpression).Type;
            var propertyType = GetTypePropertyType(baseType, syntax.PropertyName.IdentifierName, syntax.PropertyName);

            return InstantiateType(syntax, $"{baseType.Name}.{syntax.PropertyName.IdentifierName}", propertyType);
        }

        private ResultWithDiagnostic<TypeExpression> InstantiateType(ParameterizedTypeInstantiationSyntaxBase syntax, string typeName, TypeSymbol symbolType)
            => symbolType switch
            {
                TypeTemplate tt => tt.Instantiate(binder, syntax, syntax.Arguments.Select(arg => DisallowNamespaceTypes(GetTypeFromTypeSyntax(arg.Expression).Type, arg.Expression))),
                _ => new(DiagnosticBuilder.ForPosition(syntax).TypeIsNotParameterizable(typeName)),
            };

        private DeferredTypeReference TypeRefToType(TypeVariableAccessSyntax signifier, TypeAliasSymbol signified) => new(() =>
        {
            var signifiedType = userDefinedTypeReferences.GetOrAdd(signified, GetUserDefinedTypeType);
            if (signifiedType is ErrorType error)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(signifier).ReferencedSymbolHasErrors(signified.Name));
            }

            return signifiedType;
        });

        private TypedArrayType GetArrayTypeType(ArrayTypeSyntax syntax)
        {
            var memberType = GetTypeFromTypeSyntax(syntax.Item);
            var flags = TypeSymbolValidationFlags.Default;

            return memberType is DeferredTypeReference
                ? new TypedArrayType(syntax.ToString(), memberType, flags)
                : new TypedArrayType(memberType, flags);
        }

        private TypeSymbol GetObjectTypeType(ObjectTypeSyntax syntax)
        {
            HashSet<string> propertyNamesEncountered = new();
            List<NamedTypeProperty> properties = new();
            List<IDiagnostic> diagnostics = new();
            ObjectTypeNameBuilder nameBuilder = new();

            foreach (var prop in syntax.Properties)
            {
                var propertyType = GetTypeFromTypeSyntax(prop);

                if (prop.TryGetKeyText() is string propertyName)
                {
                    if (propertyNamesEncountered.Contains(propertyName))
                    {
                        // if there is already a property with this name declared, log an error and move on
                        diagnostics.Add(DiagnosticBuilder.ForPosition(prop.Key).PropertyMultipleDeclarations(propertyName));
                        continue;
                    }
                    propertyNamesEncountered.Add(propertyName);

                    properties.Add(new(propertyName, propertyType, TypePropertyFlags.Required, DescriptionHelper.TryGetFromDecorator(binder, typeManager, prop)));
                    nameBuilder.AppendProperty(propertyName, GetPropertyTypeName(prop.Value, propertyType));
                }
                else
                {
                    diagnostics.Add(DiagnosticBuilder.ForPosition(prop.Key).NonConstantTypeProperty());
                    // since we're not attaching this property to the object due to the non-constant key, forward any property errors to the object type
                    if (propertyType is ErrorType error)
                    {
                        diagnostics.AddRange(error.GetDiagnostics());
                    }
                }
            }

            var additionalPropertiesDeclarations = syntax.Children.OfType<ObjectTypeAdditionalPropertiesSyntax>().ToImmutableArray();
            ITypeReference additionalPropertiesType = additionalPropertiesDeclarations.Length switch
            {
                1 => GetTypeFromTypeSyntax(additionalPropertiesDeclarations[0]),
                _ => LanguageConstants.Any,
            };
            var additionalPropertiesFlags = additionalPropertiesDeclarations.Any() ? TypePropertyFlags.None : TypePropertyFlags.FallbackProperty;

            if (additionalPropertiesDeclarations.Length > 1)
            {
                diagnostics.AddRange(additionalPropertiesDeclarations.Select(d => DiagnosticBuilder.ForPosition(d).MultipleAdditionalPropertiesDeclarations()));
            }

            if (additionalPropertiesType is not null && !additionalPropertiesFlags.HasFlag(TypePropertyFlags.FallbackProperty))
            {
                nameBuilder.AppendPropertyMatcher(GetPropertyTypeName(additionalPropertiesDeclarations[0].Value, additionalPropertiesType));
            }

            var additionalPropertiesDescription = !additionalPropertiesDeclarations.Any() ? null : DescriptionHelper.TryGetFromDecorator(binder, typeManager, additionalPropertiesDeclarations[0]);

            if (diagnostics.Any())
            {
                // forward any diagnostics gathered from parsing properties to the return type. normally, these diagnostics would be gathered by the SemanticDiagnosticVisitor (which would visit the properties of an ObjectType looking for errors).
                // Errors hidden behind DeferredTypeReferences will unfortunately be dropped, as we can't resolve their type without risking an infinite loop (in the case that a recursive object type has errors)
                return ErrorType.Create(diagnostics.Concat(properties.Select(p => p.TypeReference).OfType<TypeSymbol>().SelectMany(e => e.GetDiagnostics())));
            }

            return new ObjectType(nameBuilder.ToString(), default, properties, additionalPropertiesType is not null ? new(additionalPropertiesType, additionalPropertiesFlags, additionalPropertiesDescription) : null);
        }

        private static string GetPropertyTypeName(SyntaxBase typeSyntax, ITypeReference propertyType)
        {
            if (propertyType is DeferredTypeReference)
            {
                return typeSyntax.ToString().Trim(' ');
            }

            return propertyType.Type.Name;
        }

        private bool HasSecureDecorator(DecorableSyntax syntax)
            => syntax.HasSecureDecorator(binder, typeManager);

        private DecoratorSyntax? TryGetSystemDecorator(DecorableSyntax syntax, string decoratorName)
            => SemanticModelHelper.TryGetDecoratorInNamespace(binder, typeManager.GetDeclaredType, syntax, SystemNamespaceType.BuiltInName, decoratorName);

        private TupleType GetTupleTypeType(TupleTypeSyntax syntax)
        {
            var items = ImmutableArray.CreateBuilder<ITypeReference>();
            TupleTypeNameBuilder nameBuilder = new();

            foreach (var item in syntax.Items)
            {
                var itemType = GetTypeFromTypeSyntax(item);
                items.Add(itemType);
                nameBuilder.AppendItem(GetPropertyTypeName(item.Value, itemType));
            }

            return new(nameBuilder.ToString(), items.ToImmutable(), default);
        }

        private ITypeReference GetTupleTypeItemType(TupleTypeItemSyntax syntax)
            => ApplyTypeModifyingDecorators(DisallowNamespaceTypes(GetTypeFromTypeSyntax(syntax.Value), syntax.Value), syntax);

        private TypeSymbol ConvertTypeExpressionToType(StringTypeLiteralSyntax syntax)
        {
            StringBuilder literalText = new();
            for (int i = 0; i < syntax.SegmentValues.Length + syntax.Expressions.Length; i++)
            {
                // String syntax should have alternating blocks of literal values and expressions.
                // If i is even, process the next literal value. If it's odd, process the next expression
                if (i % 2 == 0)
                {
                    literalText.Append(syntax.SegmentValues[i / 2]);
                }
                else
                {
                    if (typeManager.GetTypeInfo(syntax.Expressions[i / 2]) is StringLiteralType literalSegment)
                    {
                        literalText.Append(literalSegment.RawStringValue);
                    }
                    else
                    {
                        return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypeExpressionLiteralConversionFailed());
                    }
                }
            }

            return TypeFactory.CreateStringLiteralType(literalText.ToString());
        }

        private static TypeSymbol ConvertTypeExpressionToType(IntegerTypeLiteralSyntax syntax) => syntax.Value switch
        {
            <= long.MaxValue => TypeFactory.CreateIntegerLiteralType((long)syntax.Value),
            // -9223372036854775808 is handled as a special case in FinalizeUnaryType
            _ => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypeExpressionLiteralConversionFailed()),
        };

        private static TypeSymbol ConvertTypeExpressionToType(BooleanTypeLiteralSyntax syntax)
            => syntax.Value ? LanguageConstants.True : LanguageConstants.False;

        private ITypeReference GetUnaryOperationType(UnaryTypeOperationSyntax syntax)
        {
            if (RequiresDeferral(syntax))
            {
                return new DeferredTypeReference(() => FinalizeUnaryType(syntax));
            }

            return FinalizeUnaryType(syntax);
        }

        private TypeSymbol FinalizeUnaryType(UnaryTypeOperationSyntax syntax)
        {
            // since abs(long.MinValue) is one greater than long.MaxValue, we need to handle that case specially
            if (syntax.Operator == UnaryOperator.Minus && syntax.Expression is IntegerTypeLiteralSyntax @int)
            {
                return @int.Value switch
                {
                    <= long.MaxValue => TypeFactory.CreateIntegerLiteralType(0L - (long)@int.Value),
                    1UL + long.MaxValue => TypeFactory.CreateIntegerLiteralType(long.MinValue),
                    _ => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypeExpressionLiteralConversionFailed()),
                };
            }

            var baseExpressionType = DisallowNamespaceTypes(GetTypeFromTypeSyntax(syntax.Expression), syntax.Expression).Type;

            if (baseExpressionType is ErrorType)
            {
                return baseExpressionType;
            }

            var diagnosticWriter = ToListDiagnosticWriter.Create();
            var evaluated = OperationReturnTypeEvaluator.TryFoldUnaryExpression(syntax.Operator, baseExpressionType, diagnosticWriter);
            if (diagnosticWriter.GetDiagnostics().Any(x => x.IsError()))
            {
                return ErrorType.Create(diagnosticWriter.GetDiagnostics());
            }

            if (evaluated is { } result && TypeHelper.IsLiteralType(result))
            {
                return result;
            }

            return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypeExpressionLiteralConversionFailed());
        }

        private bool RequiresDeferral(SyntaxBase syntax) => syntax switch
        {
            NonNullableTypeSyntax nonNullableType => RequiresDeferral(nonNullableType.Base),
            ParenthesizedTypeSyntax parenthesizedExpression => RequiresDeferral(parenthesizedExpression.Expression),
            NullableTypeSyntax nullableType => RequiresDeferral(nullableType.Base),
            UnaryTypeOperationSyntax unaryOperation => RequiresDeferral(unaryOperation.Expression),
            UnionTypeSyntax unionType => unionType.Members.Any(m => RequiresDeferral(m.Value)),
            TypePropertyAccessSyntax typePropertyAccess => RequiresDeferral(typePropertyAccess.BaseExpression),
            TypeAdditionalPropertiesAccessSyntax typeAdditionalPropertiesAccess => RequiresDeferral(typeAdditionalPropertiesAccess.BaseExpression),
            TypeArrayAccessSyntax typeArrayAccess => RequiresDeferral(typeArrayAccess.BaseExpression),
            TypeItemsAccessSyntax typeItemsAccess => RequiresDeferral(typeItemsAccess.BaseExpression),
            TypeVariableAccessSyntax variableAccess when binder.GetSymbolInfo(variableAccess) is TypeAliasSymbol => true,
            _ => false,
        };

        private ITypeReference GetUnionTypeType(UnionTypeSyntax syntax)
        {
            if (RequiresDeferral(syntax))
            {
                return new DeferredTypeReference(() => FinalizeUnionType(syntax));
            }

            return FinalizeUnionType(syntax);
        }

        private TypeSymbol FinalizeUnionType(UnionTypeSyntax syntax)
        {
            // TODO the discriminator can be inferred without the decorator, but we need to figure out what error to show
            // when a union is neither all literals nor a valid discriminated union
            if (TryResolveUnionImmediateDecorableSyntax(syntax) is { } decorableSyntax
                && TryGetSystemDecorator(decorableSyntax, LanguageConstants.TypeDiscriminatorDecoratorName) is { } discriminatorDecorator)
            {
                return FinalizeDiscriminatedObjectType(syntax, discriminatorDecorator);
            }

            return TypeHelper.CreateTypeUnion(syntax.Members.Select(GetTypeFromTypeSyntax));
        }

        private DecorableSyntax? TryResolveUnionImmediateDecorableSyntax(SyntaxBase? syntaxBase) =>
            syntaxBase switch
            {
                DecorableSyntax decorableSyntax => decorableSyntax,
                ParenthesizedTypeSyntax or UnionTypeSyntax or UnionTypeMemberSyntax or NullableTypeSyntax or NonNullableTypeSyntax =>
                    TryResolveUnionImmediateDecorableSyntax(binder.GetParent(syntaxBase)),
                _ => null
            };

        private TypeSymbol FinalizeDiscriminatedObjectType(UnionTypeSyntax syntax, DecoratorSyntax discriminatorDecorator)
        {
            if (GetSingleStringDecoratorArgument(discriminatorDecorator) is not string discriminator)
            {
                return ErrorType.Empty(); // the decorator validator handles this case
            }

            DiscriminatedObjectTypeBuilder builder = new(discriminator);
            List<(TypeSymbol rejected, UnionTypeMemberSyntax diagnosticTarget)> rejectedMembers = new();
            foreach (var memberSyntax in syntax.Members)
            {
                var memberType = GetTypeFromTypeSyntax(memberSyntax).Type;

                if (memberType is ObjectType memberObject)
                {
                    if (!builder.TryInclude(memberObject))
                    {
                        rejectedMembers.Add((memberObject, memberSyntax));
                    }
                }
                else if (memberType is DiscriminatedObjectType memberTaggedUnion)
                {
                    foreach (var memberVariant in memberTaggedUnion.UnionMembersByKey.Values)
                    {
                        if (!builder.TryInclude(memberVariant))
                        {
                            rejectedMembers.Add((memberVariant, memberSyntax));
                        }
                    }
                }
                else
                {
                    rejectedMembers.Add((memberType, memberSyntax));
                }
            }

            if (rejectedMembers.Count != 0)
            {
                return ErrorType.Create(rejectedMembers.Select(t =>
                {
                    var diagnosticBuilder = DiagnosticBuilder.ForPosition(t.diagnosticTarget);

                    if (t.rejected is not ObjectType rejectedObjectMember)
                    {
                        return diagnosticBuilder.InvalidUnionTypeMember(LanguageConstants.ObjectType);
                    }

                    if (!rejectedObjectMember.Properties.TryGetValue(discriminator, out var property) ||
                        !property.Flags.HasFlag(TypePropertyFlags.Required) ||
                        property.TypeReference.Type is not StringLiteralType discriminatorValue)
                    {
                        return diagnosticBuilder.DiscriminatorPropertyMustBeRequiredStringLiteral(discriminator);
                    }

                    return diagnosticBuilder.DiscriminatorPropertyMemberDuplicatedValue(discriminator, discriminatorValue.Name);
                }));
            }

            var (members, _) = builder.Build();

            return members.Count switch
            {
                0 => ErrorType.Empty(),
                _ => new DiscriminatedObjectType(
                    name: string.Join(" | ", TypeHelper.GetOrderedTypeNames(members)),
                    validationFlags: TypeSymbolValidationFlags.Default,
                    discriminatorKey: discriminator,
                    unionMembers: members),
            };
        }

        private ITypeReference ConvertTypeExpressionToType(ParenthesizedTypeSyntax syntax)
            => GetTypeFromTypeSyntax(syntax.Expression);

        private ITypeReference ConvertTypeExpressionToType(TypePropertyAccessSyntax syntax)
            => ConvertTypeExpressionToType(syntax, syntax.BaseExpression, syntax.PropertyName.IdentifierName, syntax.PropertyName);

        private ITypeReference ConvertTypeExpressionToType(TypeArrayAccessSyntax syntax) => syntax.IndexExpression switch
        {
            IntegerLiteralSyntax @int => ConvertTypeExpressionToType(syntax, @int.Value),
            UnaryOperationSyntax unaryOperation when unaryOperation.Operator == UnaryOperator.Minus
                => ErrorType.Create(DiagnosticBuilder.ForPosition(unaryOperation).NegatedTypeIndexSought()),
            StringSyntax @string when @string.TryGetLiteralValue() is string propertyName
                => ConvertTypeExpressionToType(syntax, syntax.BaseExpression, propertyName, @string),
            _ => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.IndexExpression).CompileTimeConstantRequired())
        };

        private ITypeReference ConvertTypeExpressionToType(SyntaxBase syntax, SyntaxBase baseExpression, string propertyName, SyntaxBase propertyNameSyntax)
        {
            var baseType = GetTypeFromTypeSyntax(baseExpression);

            return RequiresDeferral(baseExpression)
                ? new DeferredTypeReference(() => FinalizeTypePropertyType(syntax, baseExpression, baseType, propertyName, propertyNameSyntax))
                : FinalizeTypePropertyType(syntax, baseExpression, baseType, propertyName, propertyNameSyntax);
        }

        private bool IsPermittedTypeAccessExpressionBase(SyntaxBase baseExpression) => baseExpression switch
        {
            // if the base expression is itself an access expression, any error will bubble up from the innermost access expression
            TypePropertyAccessSyntax or
            TypeAdditionalPropertiesAccessSyntax or
            TypeArrayAccessSyntax or
            TypeItemsAccessSyntax => true,
            // Accessing properties or elements of a reference is permitted
            TypeVariableAccessSyntax => true,
            // as is accessing elements of a resource-derived type
            ParameterizedTypeInstantiationSyntax parameterized
                when binder.GetSymbolInfo(parameterized) is AmbientTypeSymbol ambient &&
                ambient.DeclaringNamespace.ExtensionNameEquals(SystemNamespaceType.BuiltInName) &&
                LanguageConstants.ResourceDerivedTypeNames.Contains(ambient.Name) => true,
            InstanceParameterizedTypeInstantiationSyntax parameterized
                when binder.GetSymbolInfo(parameterized.BaseExpression) is BuiltInNamespaceSymbol ns &&
                ns.TryGetNamespaceType()?.ExtensionNameEquals(SystemNamespaceType.BuiltInName) is true &&
                LanguageConstants.ResourceDerivedTypeNames.Contains(parameterized.Name.IdentifierName) => true,
            _ => false,
        };

        private TypeSymbol FinalizeTypePropertyType(
            SyntaxBase syntax,
            SyntaxBase baseExpression,
            ITypeReference baseExpressionType,
            string propertyName,
            SyntaxBase propertyNameSyntax)
        {
            var typePropertyType = GetTypePropertyType(baseExpressionType, propertyName, propertyNameSyntax);

            if (typePropertyType is ErrorType)
            {
                return typePropertyType;
            }

            if (!IsPermittedTypeAccessExpressionBase(baseExpression))
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).AccessExpressionForbiddenBase());
            }

            return EnsureNonParameterizedType(propertyNameSyntax, typePropertyType);
        }

        private static TypeSymbol GetTypePropertyType(ITypeReference baseExpressionType, string propertyName, SyntaxBase propertyNameSyntax)
        {
            var baseType = baseExpressionType.Type;

            if (TypeHelper.TryRemoveNullability(baseType) is TypeSymbol nonNullableBaseType)
            {
                baseType = nonNullableBaseType;
            }

            if (baseType is ErrorType error)
            {
                return error;
            }

            if (baseType is not ObjectType objectType)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(propertyNameSyntax).ObjectRequiredForPropertyAccess(baseType));
            }

            if (!objectType.Properties.TryGetValue(propertyName, out var typeProperty))
            {
                return ErrorType.Create(
                    TypeHelper.GetUnknownPropertyDiagnostic(objectType, propertyName, shouldWarn: false)
                        .Invoke(DiagnosticBuilder.ForPosition(propertyNameSyntax)));
            }

            return UnwrapType(typeProperty.TypeReference.Type);
        }

        private ITypeReference ConvertTypeExpressionToType(TypeArrayAccessSyntax syntax, ulong index)
        {
            var baseType = GetTypeFromTypeSyntax(syntax.BaseExpression);

            return RequiresDeferral(syntax.BaseExpression)
                ? new DeferredTypeReference(() => FinalizeTypeIndexAccessType(syntax, index, baseType))
                : FinalizeTypeIndexAccessType(syntax, index, baseType);
        }

        private TypeSymbol FinalizeTypeIndexAccessType(TypeArrayAccessSyntax syntax, ulong index, ITypeReference baseExpressionType)
        {
            var baseType = baseExpressionType.Type;

            if (TypeHelper.TryRemoveNullability(baseType) is TypeSymbol nonNullableBaseType)
            {
                baseType = nonNullableBaseType;
            }

            if (baseType is ErrorType error)
            {
                return error;
            }

            if (!IsPermittedTypeAccessExpressionBase(syntax.BaseExpression))
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).AccessExpressionForbiddenBase());
            }

            if (baseType is not TupleType tupleType)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.IndexExpression).TupleRequiredForIndexAccess(baseType));
            }

            if (index >= (uint)tupleType.Items.Length)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.IndexExpression)
                    .IndexOutOfBounds(baseType.Name, tupleType.Items.Length, index <= long.MaxValue ? (long)index : long.MaxValue));
            }

            return EnsureNonParameterizedType(syntax.IndexExpression, UnwrapType(tupleType.Items[(int)index].Type));
        }

        private ITypeReference ConvertTypeExpressionToType(TypeAdditionalPropertiesAccessSyntax syntax)
        {
            var baseType = DisallowNamespaceTypes(GetTypeFromTypeSyntax(syntax.BaseExpression), syntax.BaseExpression);

            return RequiresDeferral(syntax.BaseExpression)
                ? new DeferredTypeReference(() => FinalizeAdditionalPropertiesAccessType(syntax, baseType))
                : FinalizeAdditionalPropertiesAccessType(syntax, baseType);
        }

        private TypeSymbol FinalizeAdditionalPropertiesAccessType(TypeAdditionalPropertiesAccessSyntax syntax, ITypeReference baseExpressionType)
        {
            var baseType = baseExpressionType.Type;

            if (TypeHelper.TryRemoveNullability(baseType) is TypeSymbol nonNullableBaseType)
            {
                baseType = nonNullableBaseType;
            }

            if (baseType is ErrorType error)
            {
                return error;
            }

            if (!IsPermittedTypeAccessExpressionBase(syntax.BaseExpression))
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).AccessExpressionForbiddenBase());
            }

            if (baseType is not ObjectType @object || @object.AdditionalProperties is null || @object.AdditionalProperties.TypeReference.Type == LanguageConstants.Any)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Asterisk).ExplicitAdditionalPropertiesTypeRequiredForAccessThereto(baseType));
            }

            return EnsureNonParameterizedType(syntax.Asterisk, UnwrapType(@object.AdditionalProperties.TypeReference.Type));
        }

        private ITypeReference ConvertTypeExpressionToType(TypeItemsAccessSyntax syntax)
        {
            var baseType = DisallowNamespaceTypes(GetTypeFromTypeSyntax(syntax.BaseExpression), syntax.BaseExpression);

            return RequiresDeferral(syntax.BaseExpression)
                ? new DeferredTypeReference(() => FinalizeItemsAccessType(syntax, baseType))
                : FinalizeItemsAccessType(syntax, baseType);
        }

        private TypeSymbol FinalizeItemsAccessType(TypeItemsAccessSyntax syntax, ITypeReference baseExpressionType)
        {
            var baseType = baseExpressionType.Type;

            if (TypeHelper.TryRemoveNullability(baseType) is TypeSymbol nonNullableBaseType)
            {
                baseType = nonNullableBaseType;
            }

            if (baseType is ErrorType error)
            {
                return error;
            }

            if (!IsPermittedTypeAccessExpressionBase(syntax.BaseExpression))
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).AccessExpressionForbiddenBase());
            }

            if (baseType is not TypedArrayType @array)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(TextSpan.Between(syntax.OpenSquare, syntax.CloseSquare)).ExplicitItemsTypeRequiredForAccessThereto());
            }

            return EnsureNonParameterizedType(syntax.Asterisk, UnwrapType(@array.Item.Type));
        }

        private static TypeSymbol EnsureNonParameterizedType(SyntaxBase syntax, TypeSymbol type) => type switch
        {
            TypeTemplate template => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypeRequiresParameterization(template.Name, template.Parameters.Length)),
            _ => type,
        };

        private static ITypeReference EnsureNonParameterizedType(SyntaxBase syntax, ITypeReference type) => type switch
        {
            DeferredTypeReference => new DeferredTypeReference(() => EnsureNonParameterizedType(syntax, type.Type)),
            _ => EnsureNonParameterizedType(syntax, type.Type),
        };

        private static TypeSymbol UnwrapType(TypeSymbol type) => type switch
        {
            TypeType tt => tt.Unwrapped,
            _ => type,
        };

        private ITypeReference ConvertTypeExpressionToType(NullableTypeSyntax syntax)
        {
            var baseExpressionType = DisallowNamespaceTypes(GetTypeFromTypeSyntax(syntax.Base), syntax.Base);

            return baseExpressionType is DeferredTypeReference
                ? new DeferredTypeReference(() => FinalizeNullableType(baseExpressionType))
                : FinalizeNullableType(baseExpressionType);
        }

        private TypeSymbol FinalizeNullableType(ITypeReference baseType) => baseType.Type switch
        {
            ErrorType errorType => errorType,
            TypeSymbol otherwise => TypeHelper.CreateTypeUnion(otherwise, LanguageConstants.Null)
        };

        private ITypeReference ConvertTypeExpressionToType(NonNullableTypeSyntax syntax)
        {
            var baseExpressionType = GetTypeFromTypeSyntax(syntax.Base);

            return baseExpressionType is DeferredTypeReference
                ? new DeferredTypeReference(() => FinalizeNonNullableType(baseExpressionType))
                : FinalizeNonNullableType(baseExpressionType);
        }

        private TypeSymbol FinalizeNonNullableType(ITypeReference baseType) => baseType.Type switch
        {
            TypeSymbol maybeNullable when TypeHelper.TryRemoveNullability(maybeNullable) is TypeSymbol nonNullable => nonNullable,
            TypeSymbol otherwise => otherwise,
        };

        private DeclaredTypeAssignment? GetExtensionType(ExtensionDeclarationSyntax syntax)
        {
            if (this.binder.GetSymbolInfo(syntax) is ExtensionNamespaceSymbol importedNamespace)
            {
                return new(importedNamespace.DeclaredType, syntax);
            }

            return null;
        }

        private TypeSymbol? GetDeclaredExtensionConfigAssignmentType(ExtensionConfigAssignmentSyntax syntax)
        {
            if (!binder.FileSymbol.TryGetBicepFileSemanticModelViaUsing().IsSuccess(out var semanticModel, out var failureDiagnostic))
            {
                // failed to resolve using
                return failureDiagnostic.IsError() ? ErrorType.Create(failureDiagnostic) : null;
            }

            if (syntax.TryGetAlias() is not { } extAlias || !semanticModel.Extensions.TryGetValue(extAlias, out var extMetadata))
            {
                return null;
            }

            return extMetadata.ConfigAssignmentDeclaredType;
        }

        private DeclaredTypeAssignment? GetUsingConfigAssignmentType(UsingWithClauseSyntax syntax)
        {
            var usingConfigType = LanguageConstants.CreateUsingConfigType();

            return TryCreateAssignment(usingConfigType, syntax);
        }

        private DeclaredTypeAssignment? GetExtensionConfigAssignmentType(ExtensionConfigAssignmentSyntax extConfigAssignment)
        {
            if (GetDeclaredExtensionConfigAssignmentType(extConfigAssignment) is { } configType)
            {
                return new(configType, extConfigAssignment);
            }

            return null;
        }

        private DeclaredTypeAssignment GetResourceType(ResourceDeclarationSyntax syntax)
        {
            var declaredResourceType = GetDeclaredResourceType(syntax);

            // if the value is a loop (not a condition or object), the type is an array of the declared resource type
            return new DeclaredTypeAssignment(
                syntax.Value is ForSyntax ? new TypedArrayType(declaredResourceType, TypeSymbolValidationFlags.Default) : declaredResourceType,
                syntax);
        }

        private DeclaredTypeAssignment GetModuleType(ModuleDeclarationSyntax syntax)
        {
            var declaredModuleType = GetDeclaredModuleType(syntax);

            // if the value is a loop (not a condition or object), the type is an array of the declared module type
            return new DeclaredTypeAssignment(
                syntax.Value is ForSyntax ? new TypedArrayType(declaredModuleType, TypeSymbolValidationFlags.Default) : declaredModuleType,
                syntax);
        }

        private DeclaredTypeAssignment GetTestType(TestDeclarationSyntax syntax)
        {
            var declaredTestType = GetDeclaredTestType(syntax);

            return new DeclaredTypeAssignment(
                declaredTestType,
                syntax);
        }

        private DeclaredTypeAssignment? GetVariableAccessType(VariableAccessSyntax syntax)
        {
            // because all variable access nodes are normally bound to something, this should always return true
            // (if not, the following code handles that gracefully)
            var symbol = this.binder.GetSymbolInfo(syntax);

            switch (symbol)
            {
                case ResourceSymbol resourceSymbol when IsCycleFree(resourceSymbol):
                    // the declared type of the resource/loop/if body is more useful to us than the declared type of the resource itself
                    var innerResourceBody = resourceSymbol.DeclaringResource.Value;
                    return this.GetDeclaredTypeAssignment(innerResourceBody);

                case ModuleSymbol moduleSymbol when IsCycleFree(moduleSymbol):
                    // the declared type of the module/loop/if body is more useful to us than the declared type of the module itself
                    var innerModuleBody = moduleSymbol.DeclaringModule.Value;
                    return this.GetDeclaredTypeAssignment(innerModuleBody);

                case VariableSymbol variableSymbol when variableSymbol.DeclaringVariable.Type is null && IsCycleFree(variableSymbol):
                    var variableType = this.typeManager.GetTypeInfo(variableSymbol.DeclaringVariable.Value);
                    return new DeclaredTypeAssignment(variableType, variableSymbol.DeclaringVariable);

                case ImportedVariableSymbol importedVariable:
                    return new DeclaredTypeAssignment(importedVariable.Type, declaringSyntax: null);

                case WildcardImportSymbol wildcardImportSymbol:
                    return new DeclaredTypeAssignment(wildcardImportSymbol.Type, declaringSyntax: null);

                case LocalThisNamespaceSymbol localThisNamespace:
                    // the syntax node is referencing a local 'this' namespace - use its declared type
                    return new DeclaredTypeAssignment(localThisNamespace.DeclaredType, declaringSyntax: null);

                case DeclaredSymbol declaredSymbol when IsCycleFree(declaredSymbol):
                    // the syntax node is referencing a declared symbol
                    // use its declared type
                    return this.GetDeclaredTypeAssignment(declaredSymbol.DeclaringSyntax);

                case BuiltInNamespaceSymbol namespaceSymbol:
                    // the syntax node is referencing a built in namespace - use its type
                    return new DeclaredTypeAssignment(namespaceSymbol.Type, declaringSyntax: null);
            }

            return null;
        }

        private DeclaredTypeAssignment? GetPropertyAccessType(DeclaredTypeAssignment baseExpressionAssignment, PropertyAccessSyntax syntax)
        {
            if (!syntax.PropertyName.IsValid)
            {
                return null;
            }

            // As a special case, a 'resource' parameter or output is a reference to an existing resource
            // we can't rely on it's syntax because it doesn't declare the resource body.
            if (baseExpressionAssignment?.DeclaringSyntax is ParameterDeclarationSyntax parameterSyntax &&
                baseExpressionAssignment.Reference.Type is ResourceType parameterResourceType)
            {
                return GetObjectPropertyType(
                    parameterResourceType.Body.Type,
                    null,
                    syntax.PropertyName.IdentifierName,
                    useSyntax: false);
            }

            // If we get here, it's ok to rely on useSyntax=true because those types have already been established

            var body = baseExpressionAssignment?.DeclaringSyntax switch
            {
                ResourceDeclarationSyntax resourceDeclarationSyntax => resourceDeclarationSyntax.TryGetBody(),
                ModuleDeclarationSyntax moduleDeclarationSyntax => moduleDeclarationSyntax.TryGetBody(),
                _ => baseExpressionAssignment?.DeclaringSyntax as ObjectSyntax,
            };

            return GetObjectPropertyType(
                baseExpressionAssignment?.Reference.Type,
                body,
                syntax.PropertyName.IdentifierName,
                useSyntax: true);
        }

        private DeclaredTypeAssignment? GetNonNullType(NonNullAssertionSyntax syntax)
        {
            var baseExpressionAssignment = GetDeclaredTypeAssignment(syntax.BaseExpression);

            return baseExpressionAssignment?.Reference switch
            {
                DeferredTypeReference deferredType => new(new DeferredTypeReference(() => TypeHelper.TryRemoveNullability(deferredType.Type) ?? deferredType.Type), syntax, baseExpressionAssignment.Flags),
                ITypeReference otherwise => new(TypeHelper.TryRemoveNullability(otherwise.Type) ?? otherwise.Type, syntax, baseExpressionAssignment.Flags),
                null => null,
            };
        }

        private DeclaredTypeAssignment? GetResourceAccessType(ResourceAccessSyntax syntax)
        {
            if (!syntax.ResourceName.IsValid)
            {
                return null;
            }

            // We should already have a symbol, use its type.
            var symbol = this.binder.GetSymbolInfo(syntax);
            if (symbol == null)
            {
                throw new InvalidOperationException("ResourceAccessSyntax was not assigned a symbol during name binding.");
            }

            if (symbol is ErrorSymbol error)
            {
                return new DeclaredTypeAssignment(ErrorType.Create(error.GetDiagnostics()), syntax);
            }
            else if (symbol is not ResourceSymbol resourceSymbol)
            {
                var baseType = GetDeclaredType(syntax.BaseExpression);
                var typeString = baseType?.Kind.ToString() ?? LanguageConstants.ErrorName;
                return new DeclaredTypeAssignment(ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.ResourceName).ResourceRequiredForResourceAccess(typeString)), syntax);
            }
            else if (IsCycleFree(resourceSymbol))
            {
                // cycle: bail
            }

            // This is a valid nested resource. Return its type.
            return this.GetDeclaredTypeAssignment(((ResourceSymbol)symbol).DeclaringResource.Value);
        }


        private DeclaredTypeAssignment? GetArrayAccessType(DeclaredTypeAssignment baseExpressionAssignment, ArrayAccessSyntax syntax)
        {
            var indexAssignedType = this.typeManager.GetTypeInfo(syntax.IndexExpression);
            static TypeSymbol GetTypeAtIndex(TupleType baseType, IntegerLiteralType indexType, SyntaxBase indexSyntax, bool fromEnd) => indexType.Value switch
            {
                long value when value < 0 ||
                    (value == 0 && fromEnd) ||
                    value > baseType.Items.Length ||
                    (value == baseType.Items.Length && !fromEnd) ||
                    // unlikely to hit this given that we've established that the tuple has a item at the given position
                    value > int.MaxValue => ErrorType.Create(DiagnosticBuilder.ForPosition(indexSyntax)
                        .IndexOutOfBounds(baseType.Name, baseType.Items.Length, value)),
                long otherwise when fromEnd => baseType.Items[^(int)otherwise].Type,
                long otherwise => baseType.Items[(int)otherwise].Type,
            };

            // identify the correct syntax so property access can provide completions correctly for resource and module loops
            static SyntaxBase? DeclaringSyntaxForArrayAccessIfCollectionBase(DeclaredTypeAssignment baseExpressionAssignment) => baseExpressionAssignment.DeclaringSyntax switch
            {
                ForSyntax { Body: ObjectSyntax loopBody } => loopBody,
                ForSyntax { Body: IfConditionSyntax { Body: ObjectSyntax loopBody } } => loopBody,
                _ => null
            };

            // TODO: Currently array access is broken with discriminated object types - revisit when that is fixed
            switch (baseExpressionAssignment?.Reference.Type)
            {
                case TupleType tupleTypeWithKnownIndex when indexAssignedType is IntegerLiteralType integerLiteral:
                    return new(
                        GetTypeAtIndex(tupleTypeWithKnownIndex, integerLiteral, syntax.IndexExpression, syntax.FromEndMarker is not null),
                        DeclaringSyntaxForArrayAccessIfCollectionBase(baseExpressionAssignment));

                case TupleType tupleTypeWithIndexPossibilities when indexAssignedType is UnionType indexUnion && indexUnion.Members.All(t => t.Type is IntegerLiteralType):
                    var possibilities = indexUnion.Members.Select(t => t.Type)
                        .OfType<IntegerLiteralType>()
                        .Select(ilt => GetTypeAtIndex(tupleTypeWithIndexPossibilities, ilt, syntax.IndexExpression, syntax.FromEndMarker is not null));
                    if (possibilities.OfType<ErrorType>().Any())
                    {
                        return new(ErrorType.Create(possibilities.SelectMany(t => t.GetDiagnostics())), syntax);
                    }

                    return new(TypeHelper.CreateTypeUnion(possibilities), DeclaringSyntaxForArrayAccessIfCollectionBase(baseExpressionAssignment));

                case TupleType tupleTypeWithUnknownIndex when TypeValidator.AreTypesAssignable(indexAssignedType, LanguageConstants.Int):
                    // we don't know which index will be accessed, so return a union of all types contained in the tuple
                    return new(tupleTypeWithUnknownIndex.Item, DeclaringSyntaxForArrayAccessIfCollectionBase(baseExpressionAssignment));

                case ArrayType arrayType when TypeValidator.AreTypesAssignable(indexAssignedType, LanguageConstants.Int):
                    // we are accessing an array by an expression of a numeric type
                    // return the item type of the array

                    // for regular array we can't evaluate the array index at this point, but for loops the index is irrelevant

                    return new(arrayType.Item.Type, DeclaringSyntaxForArrayAccessIfCollectionBase(baseExpressionAssignment));

                case ObjectType objectType when syntax.IndexExpression is StringSyntax potentialLiteralValue && potentialLiteralValue.TryGetLiteralValue() is { } propertyName:
                    // string literal indexing over an object is the same as dot property access
                    // it's ok to rely on useSyntax=true because those types have already been established
                    return this.GetObjectPropertyType(
                        objectType,
                        baseExpressionAssignment.DeclaringSyntax as ObjectSyntax,
                        propertyName,
                        useSyntax: true);
            }

            return null;
        }

        private DeclaredTypeAssignment? GetAccessExpressionType(AccessExpressionSyntax syntax)
        {
            Stack<AccessExpressionSyntax> chainedAccesses = syntax.ToAccessExpressionStack();
            var baseAssignment = chainedAccesses.Peek() switch
            {
                AccessExpressionSyntax access when access.BaseExpression is ForSyntax
                    // in certain parser recovery scenarios, the parser can produce a PropertyAccessSyntax operating on a ForSyntax
                    // this leads to a stack overflow which we don't really want, so let's short circuit here.
                    => null,
                var otherwise => GetDeclaredTypeAssignment(otherwise.BaseExpression),
            };

            var nullVariantRemoved = false;
            AccessExpressionSyntax? prevAccess = null;
            while (chainedAccesses.TryPop(out var nextAccess))
            {
                if (baseAssignment is null)
                {
                    break;
                }

                if (prevAccess?.IsSafeAccess is true || nextAccess.IsSafeAccess)
                {
                    // if the first access definitely returns null, short-circuit the whole chain
                    if (ReferenceEquals(baseAssignment.Reference.Type, LanguageConstants.Null))
                    {
                        return baseAssignment;
                    }

                    // if the first access might return null, evaluate the rest of the chain as if it does not return null, the create a union of the result and null
                    if (TypeHelper.TryRemoveNullability(baseAssignment.Reference.Type) is TypeSymbol nonNullable)
                    {
                        nullVariantRemoved = true;
                        baseAssignment = new(nonNullable, baseAssignment.DeclaringSyntax, baseAssignment.Flags);
                    }
                }

                baseAssignment = nextAccess switch
                {
                    ArrayAccessSyntax arrayAccess => GetArrayAccessType(baseAssignment, arrayAccess),
                    PropertyAccessSyntax propertyAccess => GetPropertyAccessType(baseAssignment, propertyAccess),
                    _ => null,
                };

                prevAccess = nextAccess;
            }

            return nullVariantRemoved && baseAssignment is not null
                ? new(TypeHelper.CreateTypeUnion(baseAssignment.Reference.Type, LanguageConstants.Null), baseAssignment.DeclaringSyntax, baseAssignment.Flags)
                : baseAssignment;
        }

        private DeclaredTypeAssignment? GetArrayType(ArraySyntax syntax)
        {
            var parent = GetClosestMaybeTypedAncestor(syntax);

            // we are only handling paths in the AST that are going to produce a declared type
            // arrays can exist under a variable declaration, but variables don't have declared types,
            // so we don't need to check that case
            switch (parent)
            {
                case ObjectPropertySyntax:
                    // this array is a value of the property
                    // the declared type should be the same as the array and we should propagate the flags
                    return GetNonNullableTypeAssignment(parent)?.ReplaceDeclaringSyntax(syntax);
                case FunctionArgumentSyntax:
                case OutputDeclarationSyntax parentOutput when syntax == parentOutput.Value:
                case VariableDeclarationSyntax parentVariable when syntax == parentVariable.Value:
                    return GetNonNullableTypeAssignment(parent)?.ReplaceDeclaringSyntax(syntax);
                case ParameterDefaultValueSyntax when this.binder.GetParent(parent) is ParameterDeclarationSyntax parameterDeclaration:
                    return GetNonNullableTypeAssignment(parameterDeclaration)?.ReplaceDeclaringSyntax(syntax);
                case ParameterAssignmentSyntax:
                    return GetNonNullableTypeAssignment(parent)?.ReplaceDeclaringSyntax(syntax);
                case SpreadExpressionSyntax when GetClosestMaybeTypedAncestor(parent) is { } grandParent &&
                    GetDeclaredTypeAssignment(grandParent)?.Reference is ArrayType enclosingArrayType:

                    return TryCreateAssignment(enclosingArrayType, syntax);
                default:
                    return null;
            }
        }

        /// <remarks>
        /// This function should be used instead of <see cref="GetDeclaredTypeAssignment" /> when the caller is certain that the null branch of a nullable type
        /// is irrelevant (such as when syntax representing a non-null value has been supplied for a location whose type is nullable).
        /// </remarks>
        private DeclaredTypeAssignment? GetNonNullableTypeAssignment(SyntaxBase syntax)
        {
            var typeAssignment = GetDeclaredTypeAssignment(syntax);

            if (typeAssignment?.Reference.Type is TypeSymbol declaredType && TypeHelper.TryRemoveNullability(declaredType) is TypeSymbol nonNullable)
            {
                // if the declared type is nullable and we're supplying a non-null value, then the null branch of the declared type can be dropped
                return new(nonNullable, typeAssignment.DeclaringSyntax, typeAssignment.Flags);
            }

            return typeAssignment;
        }

        private DeclaredTypeAssignment? GetStringType(StringSyntax syntax)
        {
            var parent = this.binder.GetParent(syntax);

            // we are only handling paths in the AST that are going to produce a declared type
            // strings can exist under a variable declaration, but variables don't have declared types,
            // so we don't need to check that case
            switch (parent)
            {
                case ObjectPropertySyntax:
                case ArrayItemSyntax:
                    // this string is a value of the property
                    // the declared type should be the same as the string and we should propagate the flags
                    return GetNonNullableTypeAssignment(parent)?.ReplaceDeclaringSyntax(syntax);
                case FunctionArgumentSyntax:
                    return GetNonNullableTypeAssignment(parent)?.ReplaceDeclaringSyntax(syntax);
                default:
                    return null;
            }
        }

        private DeclaredTypeAssignment? GetFunctionType(FunctionCallSyntaxBase syntax)
        {
            return new DeclaredTypeAssignment(this.typeManager.GetTypeInfo(syntax), declaringSyntax: null);
        }

        private DeclaredTypeAssignment? GetFunctionArgumentType(FunctionArgumentSyntax syntax)
        {
            var parent = this.binder.GetParent(syntax);
            if (parent is not FunctionCallSyntaxBase parentFunction ||
                SymbolHelper.TryGetSymbolInfo(this.binder, this.GetDeclaredType, parent) is not IFunctionSymbol functionSymbol)
            {
                return null;
            }

            var arguments = parentFunction.Arguments;
            var argIndex = arguments.IndexOf(syntax);
            var declaredType = functionSymbol.GetDeclaredArgumentType(
                argIndex,
                getAssignedArgumentType: i => typeManager.GetTypeInfo(parentFunction.Arguments[i]),
                getAttachedType: () => binder.GetParent(parent) is DecoratorSyntax decorator && binder.GetParent(decorator) is DecorableSyntax decorated
                    ? GetDeclaredType(decorated) ?? ErrorType.Empty()
                    : throw new InvalidOperationException("Cannot get attached type of function not used as decorator."));

            return new DeclaredTypeAssignment(declaredType, declaringSyntax: null);
        }

        private DeclaredTypeAssignment? GetArrayItemType(ArrayItemSyntax syntax)
        {
            var parent = this.binder.GetParent(syntax);
            switch (parent)
            {
                case ArraySyntax _:
                    // array items can only have array parents
                    // use the declared item type
                    var parentType = GetNonNullableTypeAssignment(parent)?.Reference.Type;
                    if (parentType is ArrayType arrayType)
                    {
                        return new DeclaredTypeAssignment(arrayType.Item.Type, syntax);
                    }

                    break;
            }

            return null;
        }

        private static DeclaredTypeAssignment? TryCreateAssignment(ITypeReference? typeRef, SyntaxBase declaringSyntax, DeclaredTypeFlags flags = DeclaredTypeFlags.None) => typeRef == null
            ? null
            : new DeclaredTypeAssignment(typeRef, declaringSyntax, flags);

        private DeclaredTypeAssignment? GetIfConditionType(IfConditionSyntax syntax)
        {
            if (syntax.Body is not ObjectSyntax @object)
            {
                // no point to propagate types if body isn't an object
                return null;
            }

            var parent = this.binder.GetParent(syntax);
            if (parent == null)
            {
                return null;
            }

            var parentTypeAssignment = GetDeclaredTypeAssignment(parent);
            if (parentTypeAssignment == null)
            {
                return null;
            }

            var parentType = parentTypeAssignment.Reference.Type;
            switch (parentType)
            {
                case ResourceType resourceType:
                    // parent is an if-condition under a resource
                    // use the object as declaring syntax to make property access and variable access code easier
                    return TryCreateAssignment(ResolveDiscriminatedObjects(resourceType.Body.Type, @object), @object, parentTypeAssignment.Flags);

                case ModuleType moduleType:
                    // parent is an if-condition under a module
                    // use the object as declaring syntax to make property access and variable access code easier
                    return TryCreateAssignment(ResolveDiscriminatedObjects(moduleType.Body.Type, @object), @object, parentTypeAssignment.Flags);

                case ArrayType arrayType:
                    // parent is an if-condition used as a resource/module loop filter
                    // discriminated objects are already resolved by the parent
                    return TryCreateAssignment(arrayType.Item.Type, @object, parentTypeAssignment.Flags);
            }

            return null;
        }

        private DeclaredTypeAssignment? GetForSyntaxType(ForSyntax syntax)
        {
            var parent = this.binder.GetParent(syntax);
            if (parent == null)
            {
                return null;
            }

            var parentTypeAssignment = parent switch
            {
                // variable declared type is calculated using its assigned type, so querying it here causes endless recursion.
                // we can shortcut that by returning any[] here
                VariableDeclarationSyntax var => new DeclaredTypeAssignment(LanguageConstants.Array, var),
                _ => GetDeclaredTypeAssignment(parent),
            };

            if (parentTypeAssignment is null)
            {
                return null;
            }

            var parentType = parentTypeAssignment.Reference.Type;

            // a for-loop expressions are semantically valid in places that allow array values
            // for non-array types, there's no need to propagate them further since it won't lead to anything useful
            if (parentType is not ArrayType arrayType)
            {
                return null;
            }

            // local function
            DeclaredTypeAssignment? ResolveType(ObjectSyntax @object)
            {
                // the object may be a discriminated object type - we need to resolve it
                var itemType = arrayType.Item.Type switch
                {
                    ResourceType resourceType => ResolveDiscriminatedObjects(resourceType.Body.Type, @object),

                    ModuleType moduleType => ResolveDiscriminatedObjects(moduleType.Body.Type, @object),

                    _ => ResolveDiscriminatedObjects(arrayType.Item.Type, @object)
                };

                return itemType is null
                    ? null
                    : TryCreateAssignment(new TypedArrayType(itemType, TypeSymbolValidationFlags.Default), syntax, parentTypeAssignment.Flags);
            }

            return syntax.Body switch
            {
                ObjectSyntax @object => ResolveType(@object),
                IfConditionSyntax { Body: ObjectSyntax @object } => ResolveType(@object),

                // pass the type through
                _ => new DeclaredTypeAssignment(parentType, syntax, parentTypeAssignment.Flags)
            };
        }

        private DeclaredTypeAssignment? GetObjectType(ObjectSyntax syntax)
        {
            var parent = GetClosestMaybeTypedAncestor(syntax);

            switch (parent)
            {
                case ResourceDeclarationSyntax:
                    if (GetDeclaredTypeAssignment(parent)?.Reference.Type is not ResourceType resourceType)
                    {
                        return null;
                    }

                    // the object literal's parent is a resource declaration, which makes this the body of the resource
                    // the declared type will be the same as the parent
                    return TryCreateAssignment(ResolveDiscriminatedObjects(resourceType.Body.Type, syntax), syntax);

                case ModuleDeclarationSyntax:
                    if (GetDeclaredTypeAssignment(parent)?.Reference.Type is not ModuleType moduleType)
                    {
                        return null;
                    }

                    // the object literal's parent is a module declaration, which makes this the body of the module
                    // the declared type will be the same as the parent
                    return TryCreateAssignment(ResolveDiscriminatedObjects(moduleType.Body.Type, syntax), syntax);

                case IfConditionSyntax:
                    if (GetDeclaredTypeAssignment(parent) is not { } ifParentTypeAssignment)
                    {
                        return null;
                    }

                    // if-condition declared type already resolved discriminators and used the object as the declaring syntax
                    Debug.Assert(ReferenceEquals(syntax, ifParentTypeAssignment.DeclaringSyntax), "ReferenceEquals(syntax,parentTypeAssignment.DeclaringSyntax)");

                    // the declared type will be the same as the parent
                    return ifParentTypeAssignment;

                case ForSyntax:
                    if (GetDeclaredTypeAssignment(parent) is not { } forParentTypeAssignment ||
                        forParentTypeAssignment.Reference.Type is not ArrayType arrayType)
                    {
                        return null;
                    }

                    // the parent is a for-expression
                    // this object is the body of the array, so its declared type is the type of the item
                    // (discriminators have already been resolved when declared type was determined for the for-expression
                    return TryCreateAssignment(arrayType.Item.Type, syntax, forParentTypeAssignment.Flags);

                case ObjectPropertySyntax:
                    if (GetNonNullableTypeAssignment(parent) is not { } objectPropertyAssignment ||
                        objectPropertyAssignment.Reference.Type is not { } objectPropertyParent)
                    {
                        return null;
                    }

                    // the object is the value of a property of another object
                    // use the declared type of the property and propagate the flags
                    return TryCreateAssignment(ResolveDiscriminatedObjects(objectPropertyParent, syntax), syntax, objectPropertyAssignment.Flags);

                case ArrayItemSyntax:
                    if (GetNonNullableTypeAssignment(parent) is not { } arrayItemAssignment ||
                        arrayItemAssignment.Reference.Type is not { } arrayParent)
                    {
                        return null;
                    }

                    // the object is an item in an array
                    // use the item's type and propagate flags
                    return TryCreateAssignment(ResolveDiscriminatedObjects(arrayParent, syntax), syntax, arrayItemAssignment.Flags);

                case ExtensionWithClauseSyntax:
                    parent = this.binder.GetParent(parent);

                    if (parent is null)
                    {
                        throw new InvalidOperationException("Expected ImportWithClauseSyntax to have a parent.");
                    }

                    ObjectLikeType? configType = null;

                    if (GetDeclaredTypeAssignment(parent) is not { } extensionAssignment)
                    {
                        return null;
                    }

                    if (extensionAssignment.Reference.Type is NamespaceType namespaceType)
                    {
                        // This case is extension declarations in bicep files.
                        configType = namespaceType.ConfigurationType;
                    }
                    else if (parent is ExtensionConfigAssignmentSyntax && extensionAssignment.Reference.Type is ObjectType configTypeFromAssignment)
                    {
                        // This case is extension config assignments in bicepparam files.
                        configType = configTypeFromAssignment;
                    }

                    if (configType is null)
                    {
                        // this namespace doesn't support configuration, but it has been provided.
                        // we'll check for this during type assignment.
                        return null;
                    }

                    // the object is an item in an array
                    // use the item's type and propagate flags
                    return TryCreateAssignment(ResolveDiscriminatedObjects(configType.Type, syntax), syntax, extensionAssignment.Flags);

                case FunctionArgumentSyntax:
                case OutputDeclarationSyntax parentOutput when syntax == parentOutput.Value:
                case VariableDeclarationSyntax parentVariable when syntax == parentVariable.Value:
                case UsingWithClauseSyntax usingWith when syntax == usingWith.Config:
                    if (GetNonNullableTypeAssignment(parent) is not { } parentAssignment)
                    {
                        return null;
                    }

                    return TryCreateAssignment(ResolveDiscriminatedObjects(parentAssignment.Reference.Type, syntax), syntax, parentAssignment.Flags);
                case ParameterDefaultValueSyntax:
                    // if we're in a parameter default value, get the declared type of the parameter itself
                    parent = this.binder.GetParent(parent);

                    if (parent is null)
                    {
                        throw new InvalidOperationException("Expected ParameterDefaultValueSyntax to have a parent.");
                    }

                    if (GetDeclaredTypeAssignment(parent) is not { } parameterAssignment)
                    {
                        return null;
                    }

                    return TryCreateAssignment(ResolveDiscriminatedObjects(parameterAssignment.Reference.Type, syntax), syntax, parameterAssignment.Flags);
                case ParameterAssignmentSyntax:
                    if (GetDeclaredTypeAssignment(parent) is not { } parameterAssignmentTypeAssignment)
                    {
                        return null;
                    }
                    ;

                    return TryCreateAssignment(parameterAssignmentTypeAssignment.Reference.Type, syntax);

                case ExtensionConfigAssignmentSyntax:
                    if (GetDeclaredTypeAssignment(parent) is not { } extConfigAssignment)
                    {
                        return null;
                    }

                    return TryCreateAssignment(ResolveDiscriminatedObjects(extConfigAssignment.Reference.Type, syntax), syntax);

                case SpreadExpressionSyntax when GetClosestMaybeTypedAncestor(parent) is { } grandParent &&
                    GetDeclaredTypeAssignment(grandParent)?.Reference is ObjectType enclosingObjectType:
                    var type = enclosingObjectType.WithModifiedProperties(p => p.WithoutFlags(TypePropertyFlags.Required));

                    return TryCreateAssignment(type, syntax);

                case TypedLambdaSyntax lambda when binder.IsEqualOrDescendent(syntax, lambda.Body):
                    if (GetTypedLambdaType(lambda).Reference is not LambdaType lambdaType)
                    {
                        return null;
                    }

                    return TryCreateAssignment(lambdaType.ReturnType, syntax);
            }

            return null;
        }

        private SyntaxBase? GetClosestMaybeTypedAncestor(SyntaxBase syntax)
        {
            // to avoid infinite recursion, this method deliberately only searches UP the syntax hierarchy.
            // otherwise, you can end up in an infinite loop e.g. trying to calculate the type of "foo" in "(foo).prop".
            foreach (var parent in binder.EnumerateAncestorsUpwards(syntax))
            {
                switch (parent)
                {
                    case ParenthesizedExpressionSyntax:
                    case TernaryOperationSyntax ternary when syntax == ternary.TrueExpression || syntax == ternary.FalseExpression:
                        continue;
                    default:
                        return parent;
                }
            }

            return null;
        }

        private DeclaredTypeAssignment? GetObjectPropertyType(ObjectPropertySyntax syntax)
        {
            // `syntax.TryGetKeyText()` will only return a non-null value if the key is a bare identifier or a non-interpolated string
            // if it does return null, look at the *type* of the key and see if it's a string literal. If an interpolated key can be folded
            // to a literal type at compile time, this will likely already have been calculated and cached in the type manager
            var propertyName = syntax.TryGetKeyText() ?? (typeManager.GetTypeInfo(syntax.Key) as StringLiteralType)?.RawStringValue;
            var parent = this.binder.GetParent(syntax);
            if (parent is not ObjectSyntax parentObject)
            {
                // the parent is missing OR the parent is not ObjectSyntax
                // cannot establish declared type
                return null;
            }

            var parentAssignment = GetNonNullableTypeAssignment(parent);

            if (propertyName is null)
            {
                // if we don't know the property name BUT we know that the parent is a simple dictionary (has an additional properties type and has no named
                // properties with their own types), then use the dictionary value type
                if (parentAssignment?.Reference.Type is ObjectType parentObjectType &&
                    parentObjectType.Properties.IsEmpty &&
                    parentObjectType.AdditionalProperties?.TypeReference.Type is { } additionalPropertiesType)
                {
                    return new(additionalPropertiesType, syntax, DeclaredTypeFlags.None);
                }

                return null;
            }

            // we are in the process of establishing the declared type for the syntax nodes,
            // so we must set useSyntax to false to avoid a stack overflow
            return GetObjectPropertyType(parentAssignment?.Reference.Type, parentObject, propertyName, useSyntax: false);
        }

        private DeclaredTypeAssignment? GetObjectPropertyType(TypeSymbol? type, ObjectSyntax? objectSyntax, string propertyName, bool useSyntax)
        {
            // local function
            DeclaredTypeFlags ConvertFlags(TypePropertyFlags flags) => flags.HasFlag(TypePropertyFlags.Constant) ? DeclaredTypeFlags.Constant : DeclaredTypeFlags.None;

            // the declared types on the declaration side of things will take advantage of properties
            // set on objects to resolve discriminators at all levels
            // to take advantage of this, we should first try looking up the property's declared type
            var declaringProperty = objectSyntax?.TryGetPropertyByName(propertyName);
            if (useSyntax && declaringProperty != null)
            {
                // it is important to get the property value's decl type instead of the property's decl type
                // (the property has the unresolved discriminated object type and the value will have it resolved)
                var declaredPropertyAssignment = this.GetDeclaredTypeAssignment(declaringProperty.Value);
                if (declaredPropertyAssignment != null)
                {
                    return declaredPropertyAssignment;
                }
            }

            if (type is ResourceType resourceType)
            {
                // We can see a resource type here for an expression like: `mod.outputs.foo.|properties|.bar`
                // The type of foo is a resource type, but since it's part of the module there's no corresponding declaration.
                //
                // For that case resolve the property lookup against the body.
                type = resourceType.Body.Type;
            }

            // could not get the declared type via syntax
            // let's use the type info instead
            switch (type)
            {
                case ObjectType objectType:
                    // lookup declared property
                    if (objectType.Properties.TryGetValue(propertyName, out var property))
                    {
                        return new DeclaredTypeAssignment(property.TypeReference.Type, declaringProperty, ConvertFlags(property.Flags));
                    }

                    // if there are additional properties, try those
                    if (objectType.AdditionalProperties is { } additionalProperties)

                    {
                        return new DeclaredTypeAssignment(additionalProperties.TypeReference.Type, declaringProperty, ConvertFlags(additionalProperties.Flags));
                    }

                    break;

                case DiscriminatedObjectType discriminated:
                    if (string.Equals(propertyName, discriminated.DiscriminatorProperty.Name, LanguageConstants.IdentifierComparison))
                    {
                        // the property is the discriminator property - use its type
                        return new DeclaredTypeAssignment(discriminated.DiscriminatorProperty.TypeReference.Type, declaringProperty);
                    }

                    break;
            }

            return null;
        }

        private static TypeSymbol ResolveDiscriminatedObjects(TypeSymbol type, ObjectSyntax syntax)
        {
            if (type is not DiscriminatedObjectType discriminated)
            {
                // not a discriminated object type - return as-is
                return type;
            }

            var discriminatorProperties = syntax.Properties
                .Where(p => discriminated.TryGetDiscriminatorProperty(p.TryGetKeyText()) is not null)
                .ToList();

            if (discriminatorProperties.Count != 1)
            {
                // the object has duplicate properties with name matching the discriminator key
                // don't select any of the union members
                return type;
            }

            // calling the type check here would prevent the declared type from being assigned to the property
            // because we haven't yet assigned the declared type to the object
            // for the purposes of resolving the discriminated object, we just need to check if it's a literal string
            // which doesn't require the full type check, so we're fine
            var discriminatorProperty = discriminatorProperties.Single();
            if (discriminatorProperty.Value is not StringSyntax stringSyntax)
            {
                // the discriminator property value is not a string
                return type;
            }

            var discriminatorValue = stringSyntax.TryGetLiteralValue();
            if (discriminatorValue == null)
            {
                // the string value was interpolated
                return type;
            }

            // discriminator values are stored in the dictionary as bicep literal string text
            // we must escape the literal value to successfully retrieve a match
            var matchingObjectType = discriminated.UnionMembersByKey.TryGetValue(StringUtils.EscapeBicepString(discriminatorValue));

            // return the match if we have it
            return matchingObjectType?.Type ?? type;
        }

        // references to symbols can be involved in cycles
        // we should not try to obtain the declared type for such symbols because we will likely never finish
        private bool IsCycleFree(DeclaredSymbol declaredSymbol) => this.binder.TryGetCycle(declaredSymbol) is null;

        /// <summary>
        /// Returns the declared type of the parameter/output based on a resource type.
        /// </summary>
        private TypeSymbol GetDeclaredResourceType(ResourceTypeSyntax typeSyntax)
        {
            // NOTE: this is closely related to the logic in the other overload. Keep them in sync.
            var stringSyntax = typeSyntax.TypeString;
            if (stringSyntax != null && stringSyntax.IsInterpolated())
            {
                // TODO: in the future, we can relax this check to allow interpolation with compile-time constants.
                // right now, codegen will still generate a format string however, which will cause problems for the type.
                return ErrorType.Create(DiagnosticBuilder.ForPosition(typeSyntax.Type!).ResourceTypeInterpolationUnsupported());
            }

            var stringContent = stringSyntax?.TryGetLiteralValue();
            if (stringContent == null)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(typeSyntax.Type!).InvalidResourceType());
            }

            // A parameter/output always refers to an 'existing' resource.
            var typeGenerationFlags = ResourceTypeGenerationFlags.ExistingResource;
            return GetResourceTypeFromString(typeSyntax.Type!.Span, stringContent, typeGenerationFlags, parentResourceType: null);
        }

        /// <summary>
        /// Returns the declared type of the resource body (based on the type string).
        /// Returns the same value for single resource or resource loops declarations.
        /// </summary>
        private TypeSymbol GetDeclaredResourceType(ResourceDeclarationSyntax resource)
        {
            // NOTE: this is closely related to the logic in the other overload. Keep them in sync.
            var stringSyntax = resource.TypeString;

            if (stringSyntax != null && stringSyntax.IsInterpolated())
            {
                // TODO: in the future, we can relax this check to allow interpolation with compile-time constants.
                // right now, codegen will still generate a format string however, which will cause problems for the type.
                return ErrorType.Create(DiagnosticBuilder.ForPosition(resource.Type).ResourceTypeInterpolationUnsupported());
            }

            var stringContent = stringSyntax?.TryGetLiteralValue();
            if (stringContent == null)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(resource.Type).InvalidResourceType());
            }

            var (typeGenerationFlags, parentResourceType) = GetResourceTypeGenerationFlags(resource);
            return GetResourceTypeFromString(resource.Type.Span, stringContent, typeGenerationFlags, parentResourceType);
        }

        private TypeSymbol GetDeclaredModuleType(ModuleDeclarationSyntax module)
        {
            if (binder.GetSymbolInfo(module) is not ModuleSymbol moduleSymbol)
            {
                // TODO: Ideally we'd still be able to return a type here, but we'd need access to the compilation to get it.
                return ErrorType.Empty();
            }

            if (!moduleSymbol.TryGetSemanticModel().IsSuccess(out var moduleSemanticModel, out var failureDiagnostic))
            {
                return ErrorType.Create(failureDiagnostic);
            }

            // We need to bind and validate all of the parameters and outputs that declare resource types
            // within the context of this type manager. This will surface any issues where the type declared by
            // a module is not understood inside this compilation unit.
            var parameters = new List<NamedTypeProperty>();

            foreach (var parameter in moduleSemanticModel.Parameters.Values)
            {
                var type = parameter.TypeReference.Type;
                if (type is UnresolvedResourceType unresolvedType)
                {
                    var boundType = GetResourceTypeFromString(module.Span, unresolvedType.TypeReference.FormatName(), ResourceTypeGenerationFlags.ExistingResource, parentResourceType: null);
                    if (boundType is ResourceType resourceType)
                    {
                        // We use a special type for Resource type parameters because they have different assignability rules.
                        type = new ResourceParameterType(resourceType.DeclaringNamespace, unresolvedType.TypeReference);
                    }
                }
                else
                {
                    type = resourceDerivedTypeResolver.ResolveResourceDerivedTypes(type);
                }

                var flags = parameter.IsRequired ? TypePropertyFlags.Required | TypePropertyFlags.WriteOnly : TypePropertyFlags.WriteOnly;

                // add implicit nullability for optional parameters
                if (!parameter.IsRequired)
                {
                    type = TypeHelper.CreateTypeUnion(type, LanguageConstants.Null);
                }

                parameters.Add(new NamedTypeProperty(parameter.Name, type, flags, parameter.Description));
            }

            List<NamedTypeProperty>? extensionConfigs = null;

            if (features.ModuleExtensionConfigsEnabled)
            {
                extensionConfigs = [];

                foreach (var extension in moduleSemanticModel.Extensions.Values)
                {
                    if (extension.ConfigAssignmentDeclaredType is null)
                    {
                        continue;
                    }

                    var extAliasProperty = new NamedTypeProperty(
                        extension.Alias,
                        extension.ConfigAssignmentDeclaredType!.Type,
                        TypePropertyFlags.WriteOnly | (extension.RequiresConfigAssignment ? TypePropertyFlags.Required : TypePropertyFlags.None));

                    extensionConfigs.Add(extAliasProperty);
                }
            }

            var outputs = new List<NamedTypeProperty>();
            foreach (var output in moduleSemanticModel.Outputs)
            {
                var type = output.TypeReference.Type;
                if (type is UnresolvedResourceType unresolvedType)
                {
                    type = GetResourceTypeFromString(module.Span, unresolvedType.TypeReference.FormatName(), ResourceTypeGenerationFlags.ExistingResource, parentResourceType: null);
                }
                else
                {
                    type = resourceDerivedTypeResolver.ResolveResourceDerivedTypes(type);
                }

                outputs.Add(new NamedTypeProperty(output.Name, type, TypePropertyFlags.ReadOnly, output.Description));
            }

            return LanguageConstants.CreateModuleType(
                this.features,
                parameters,
                extensionConfigs,
                outputs,
                moduleSemanticModel.TargetScope,
                binder.TargetScope,
                LanguageConstants.TypeNameModule);
        }

        private TypeSymbol GetDeclaredTestType(TestDeclarationSyntax test)
        {
            if (binder.GetSymbolInfo(test) is not TestSymbol testSymbol)
            {
                // TODO: Ideally we'd still be able to return a type here, but we'd need access to the compilation to get it.
                return ErrorType.Empty();
            }

            if (!testSymbol.TryGetSemanticModel().IsSuccess(out var testSemanticModel, out var failureDiagnostic))
            {
                return ErrorType.Create(failureDiagnostic);
            }

            var parameters = new List<NamedTypeProperty>();

            foreach (var parameter in testSemanticModel.Parameters.Values)
            {
                var type = parameter.TypeReference.Type;

                var flags = parameter.IsRequired ? TypePropertyFlags.Required | TypePropertyFlags.WriteOnly : TypePropertyFlags.WriteOnly;
                parameters.Add(new NamedTypeProperty(parameter.Name, type, flags, parameter.Description));
            }

            return CreateTestType(
                parameters,
                LanguageConstants.TypeNameTest);
        }

        private TypeSymbol CreateTestType(IEnumerable<NamedTypeProperty> paramsProperties, string typeName)
        {
            var paramsType = new ObjectType(LanguageConstants.TestParamsPropertyName, TypeSymbolValidationFlags.Default, paramsProperties, null);
            // If none of the params are required, we can allow the 'params' declaration to be omitted entirely
            var paramsRequiredFlag = paramsProperties.Any(x => x.Flags.HasFlag(TypePropertyFlags.Required)) ? TypePropertyFlags.Required : TypePropertyFlags.None;

            var testBody = new ObjectType(
                typeName,
                TypeSymbolValidationFlags.Default,
                new[]
                {
                    new NamedTypeProperty(LanguageConstants.TestParamsPropertyName, paramsType, paramsRequiredFlag | TypePropertyFlags.WriteOnly),
                },
                null);

            return new TestType(typeName, testBody);
        }

        private TypeSymbol GetResourceTypeFromString(TextSpan span, string stringContent, ResourceTypeGenerationFlags typeGenerationFlags, ResourceType? parentResourceType)
            => TypeHelper.GetResourceTypeFromString(binder, stringContent, typeGenerationFlags, parentResourceType).IsSuccess(out var resourceType, out var errorBuilder)
                ? resourceType
                : ErrorType.Create(errorBuilder(DiagnosticBuilder.ForPosition(span)));

        private (ResourceTypeGenerationFlags flags, ResourceType? parentResourceType) GetResourceTypeGenerationFlags(ResourceDeclarationSyntax resource)
        {
            var isSyntacticallyNested = false;
            TypeSymbol? parentType = null;

            var parentResource = binder.GetAllAncestors<ResourceDeclarationSyntax>(resource).LastOrDefault();
            if (parentResource is not null)
            {
                isSyntacticallyNested = true;
                parentType = GetDeclaredType(parentResource);
            }
            else if (binder.GetSymbolInfo(resource) is ResourceSymbol resourceSymbol &&
                binder.TryGetCycle(resourceSymbol) is null &&
                resourceSymbol.TryGetBodyPropertyValue(LanguageConstants.ResourceParentPropertyName) is { } referenceParentSyntax &&
                SyntaxHelper.UnwrapArrayAccessSyntax(referenceParentSyntax) is { } result &&
                binder.GetSymbolInfo(result.baseSyntax) is ResourceSymbol parentResourceSymbol)
            {
                parentResource = parentResourceSymbol.DeclaringResource;
                parentType = GetDeclaredType(referenceParentSyntax);
            }

            var flags = ResourceTypeGenerationFlags.None;
            if (resource.IsExistingResource())
            {
                flags |= ResourceTypeGenerationFlags.ExistingResource;
            }

            if (isSyntacticallyNested)
            {
                flags |= ResourceTypeGenerationFlags.NestedResource;
            }

            if (parentResource is not null)
            {
                flags |= ResourceTypeGenerationFlags.HasParentDefined;
            }

            return (flags, parentType as ResourceType);
        }
    }
}
