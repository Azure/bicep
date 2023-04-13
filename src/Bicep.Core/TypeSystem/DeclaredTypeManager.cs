// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class DeclaredTypeManager
    {
        // maps syntax nodes to their declared types
        // processed nodes found not to have a declared type will have a null value
        private readonly ConcurrentDictionary<SyntaxBase, DeclaredTypeAssignment?> declaredTypes = new();
        private readonly ConcurrentDictionary<TypeAliasSymbol, TypeSymbol> userDefinedTypeReferences = new();
        private readonly ITypeManager typeManager;
        private readonly IBinder binder;
        private readonly IFeatureProvider features;

        public DeclaredTypeManager(TypeManager typeManager, IBinder binder, IFeatureProvider features)
        {
            this.typeManager = typeManager;
            this.binder = binder;
            this.features = features;
        }

        public DeclaredTypeAssignment? GetDeclaredTypeAssignment(SyntaxBase syntax) =>
            this.declaredTypes.GetOrAdd(syntax, key => GetTypeAssignment(key));

        public TypeSymbol? GetDeclaredType(SyntaxBase syntax) => this.GetDeclaredTypeAssignment(syntax)?.Reference.Type;

        private DeclaredTypeAssignment? GetTypeAssignment(SyntaxBase syntax)
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();

            switch (syntax)
            {
                case ImportDeclarationSyntax import:
                    return GetImportType(import);

                case MetadataDeclarationSyntax metadata:
                    return new DeclaredTypeAssignment(this.typeManager.GetTypeInfo(metadata.Value), metadata);

                case ParameterDeclarationSyntax parameter:
                    return GetParameterType(parameter);

                case ParameterAssignmentSyntax parameterAssignment:
                    return GetParameterAssignmentType(parameterAssignment);

                case TypeDeclarationSyntax typeDeclaration:
                    return GetTypeType(typeDeclaration);

                case ResourceTypeSyntax resourceType:
                    return GetResourceTypeType(resourceType);

                case ObjectTypeSyntax objectType:
                    return new(GetObjectTypeType(objectType), objectType);

                case ObjectTypePropertySyntax typeProperty:
                    return GetTypePropertyType(typeProperty);

                case ObjectTypeAdditionalPropertiesSyntax typeAdditionalProperties:
                    return GetTypeAdditionalPropertiesType(typeAdditionalProperties);

                case TupleTypeSyntax tupleType:
                    return new(GetTupleTypeType(tupleType), tupleType);

                case TupleTypeItemSyntax tupleTypeItem:
                    return GetTupleTypeItemType(tupleTypeItem);

                case ArrayTypeMemberSyntax typeMember:
                    return GetTypeMemberType(typeMember);

                case UnionTypeSyntax unionType:
                    return new(GetUnionTypeType(unionType), unionType);

                case UnionTypeMemberSyntax unionTypeMember:
                    return GetTypeMemberType(unionTypeMember);

                case UnaryOperationSyntax unaryOperation:
                    return new(GetUnaryOperationType(unaryOperation), unaryOperation);

                case ResourceDeclarationSyntax resource:
                    return GetResourceType(resource);

                case ModuleDeclarationSyntax module:
                    return GetModuleType(module);

                case VariableAccessSyntax variableAccess:
                    return GetVariableAccessType(variableAccess);

                case OutputDeclarationSyntax output:
                    return GetOutputType(output);

                case TargetScopeSyntax targetScope:
                    return new DeclaredTypeAssignment(targetScope.GetDeclaredType(), targetScope, DeclaredTypeFlags.Constant);

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

                case ParenthesizedExpressionSyntax parenthesizedExpression:
                    return GetTypeAssignment(parenthesizedExpression.Expression);

                case NonNullAssertionSyntax nonNullAssertion:
                    return GetNonNullType(nonNullAssertion);
            }

            return null;
        }

        private DeclaredTypeAssignment GetParameterType(ParameterDeclarationSyntax syntax)
        {
            var declaredType = TryGetTypeFromTypeSyntax(syntax.Type, allowNamespaceReferences: false);
            declaredType ??= ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Type).InvalidParameterType(GetValidTypeNames()));

            return new(ApplyTypeModifyingDecorators(declaredType.Type, syntax, allowLooseAssignment: true), syntax);
        }

        private DeclaredTypeAssignment? GetParameterAssignmentType(ParameterAssignmentSyntax syntax)
        {
            if(GetDeclaredParameterAssignmentType(syntax) is { } declaredParamAssignmentType)
            {
                return new(declaredParamAssignmentType, syntax);
            }

            return null;
        }

        private TypeSymbol? GetDeclaredParameterAssignmentType(ParameterAssignmentSyntax syntax)
        {
            if (this.binder.GetSymbolInfo(syntax) is not ParameterAssignmentSymbol parameterAssignmentSymbol)
            {
                // no access to the compilation to get something better
                return null;
            }

            if(!parameterAssignmentSymbol.Context.Compilation.GetEntrypointSemanticModel().Root.TryGetBicepFileSemanticModelViaUsing(out var bicepSemanticModel, out var failureDiagnostic))
            {
                // failed to resolve using
                return failureDiagnostic is ErrorDiagnostic error
                    ? ErrorType.Create(error)
                    : null;
            }

            if(bicepSemanticModel.Parameters.TryGetValue(parameterAssignmentSymbol.Name, out var parameterMetadata))
            {
                return parameterMetadata.TypeReference.Type;
            }

            return null;
        }

        private DeclaredTypeAssignment GetTypeType(TypeDeclarationSyntax syntax)
        {
            if (!features.UserDefinedTypesEnabled)
            {
                return new(ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypeDeclarationStatementsUnsupported()), syntax);
            }

            var type = binder.GetSymbolInfo(syntax) switch
            {
                TypeAliasSymbol declaredType => userDefinedTypeReferences.GetOrAdd(declaredType, GetUserDefinedTypeType),
                ErrorSymbol errorSymbol => errorSymbol.ToErrorType(),
                // binder.GetSymbolInfo(TypeDeclarationSyntax) should always return a DeclaredTypeSymbol or an error, but just in case...
                _ => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).SymbolicNameIsNotAType(syntax.Name.IdentifierName, GetValidTypeNames())),
            };
            var typeRefType = type switch
            {
                ErrorType => type,
                _ => new TypeType(type),
            };

            return new(typeRefType, syntax);
        }

        private TypeSymbol GetUserDefinedTypeType(TypeAliasSymbol symbol)
        {
            if (binder.TryGetCycle(symbol) is {} cycle)
            {
                var builder = DiagnosticBuilder.ForPosition(symbol.DeclaringType.Name);
                var diagnostic = cycle.Length == 1
                    ? builder.CyclicTypeSelfReference()
                    : builder.CyclicType(cycle.Select(s => s.Name));

                return ErrorType.Create(diagnostic);
            }

            return ApplyTypeModifyingDecorators(GetTypeFromTypeSyntax(symbol.DeclaringType.Value, allowNamespaceReferences: false).Type, symbol.DeclaringType);
        }

        private DeclaredTypeAssignment? GetTypePropertyType(ObjectTypePropertySyntax syntax)
            => new(ApplyTypeModifyingDecorators(GetTypeFromTypeSyntax(syntax.Value, allowNamespaceReferences: false), syntax), syntax);

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
                    => TypeSymbolValidationFlags.AllowLooseAssignment,
                _ => TypeSymbolValidationFlags.Default,
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

            return TypeFactory.CreateStringType(minLength, maxLength, validationFlags);
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
            if (TryGetSealedDecorator(syntax) is DecoratorSyntax sealedDecorator)
            {
                return declaredObject.AdditionalPropertiesFlags.HasFlag(TypePropertyFlags.FallbackProperty)
                    ? new ObjectType(declaredObject.Name, validationFlags, declaredObject.Properties.Values, additionalPropertiesType: null)
                    : ErrorType.Create(DiagnosticBuilder.ForPosition(sealedDecorator).SealedIncompatibleWithAdditionalPropertiesDeclaration());
            }

            if (declaredObject.ValidationFlags == validationFlags)
            {
                return declaredObject;
            }

            return new ObjectType(declaredObject.Name, validationFlags, declaredObject.Properties.Values, declaredObject.AdditionalPropertiesType, declaredObject.AdditionalPropertiesFlags);
        }

        private DeclaredTypeAssignment? GetTypeAdditionalPropertiesType(ObjectTypeAdditionalPropertiesSyntax syntax)
            => new(ApplyTypeModifyingDecorators(GetTypeFromTypeSyntax(syntax.Value, allowNamespaceReferences: false), syntax), syntax);

        private DeclaredTypeAssignment? GetTypeMemberType(ArrayTypeMemberSyntax syntax)
            => new(GetTypeFromTypeSyntax(syntax.Value, allowNamespaceReferences: false), syntax);

        private DeclaredTypeAssignment? GetTypeMemberType(UnionTypeMemberSyntax syntax)
            => new(syntax.Value switch
            {
                // A `null` literal is usually too ambiguous to be a valid type (a `null` value could be valid for any nullable type), but it is permitted as a member of a union of literals.
                NullLiteralSyntax => LanguageConstants.Null,
                _ => GetTypeFromTypeSyntax(syntax.Value, allowNamespaceReferences: false),
            }, syntax);

        private DeclaredTypeAssignment GetOutputType(OutputDeclarationSyntax syntax)
        {
            var declaredType = TryGetTypeFromTypeSyntax(syntax.Type, allowNamespaceReferences: false) ??
                ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Type).InvalidOutputType());

            return new(ApplyTypeModifyingDecorators(declaredType.Type, syntax), syntax);
        }

        private ITypeReference GetTypeFromTypeSyntax(SyntaxBase syntax, bool allowNamespaceReferences) => TryGetTypeFromTypeSyntax(syntax, allowNamespaceReferences)
            ?? ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).InvalidTypeDefinition());

        private ITypeReference? TryGetTypeFromTypeSyntax(SyntaxBase syntax, bool allowNamespaceReferences)
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();

            // assume "any" type when the parameter has parse errors (either missing or was skipped)
            return syntax switch
            {
                SkippedTriviaSyntax => LanguageConstants.Any,
                ResourceTypeSyntax resource => GetDeclaredType(resource),
                VariableAccessSyntax typeRef => ConvertTypeExpressionToType(typeRef, allowNamespaceReferences),
                ArrayTypeSyntax array => ConvertTypeExpressionToType(array),
                ObjectTypeSyntax @object => GetDeclaredType(@object),
                TupleTypeSyntax tuple => GetDeclaredType(tuple),
                StringSyntax @string => ConvertTypeExpressionToType(@string),
                IntegerLiteralSyntax @int => ConvertTypeExpressionToType(@int),
                BooleanLiteralSyntax @bool => ConvertTypeExpressionToType(@bool),
                UnaryOperationSyntax unaryOperation => GetDeclaredTypeAssignment(unaryOperation)?.Reference,
                UnionTypeSyntax unionType => GetDeclaredTypeAssignment(unionType)?.Reference,
                UnionTypeMemberSyntax unionTypeMember => GetDeclaredTypeAssignment(unionTypeMember)?.Reference,
                ParenthesizedExpressionSyntax parenthesized => ConvertTypeExpressionToType(parenthesized, allowNamespaceReferences),
                PropertyAccessSyntax propertyAccess => ConvertTypeExpressionToType(propertyAccess),
                NullableTypeSyntax nullableType => ConvertTypeExpressionToType(nullableType),
                NonNullAssertionSyntax nonNullAssertion => ConvertTypeExpressionToType(nonNullAssertion),
                _ => null
            };
        }

        private DeclaredTypeAssignment GetResourceTypeType(ResourceTypeSyntax syntax)
            => new(GetTypeReferenceForResourceType(syntax), syntax);

        private TypeSymbol GetTypeReferenceForResourceType(ResourceTypeSyntax syntax)
        {
            if (!features.ResourceTypedParamsAndOutputsEnabled)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Span).ParamOrOutputResourceTypeUnsupported());
            }

            // The resource type of an output can be inferred.
            var type = syntax.Type == null && GetOutputValueType(syntax) is {} inferredType ? inferredType : GetDeclaredResourceType(syntax);

            if (type is ResourceType resourceType && IsExtensibilityType(resourceType))
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).UnsupportedResourceTypeParameterOrOutputType(resourceType.Name));
            }

            return type;
        }

        private bool IsExtensibilityType(ResourceType resourceType)
        {
            return resourceType.DeclaringNamespace.ProviderName != AzNamespaceType.BuiltInName;
        }

        private TypeSymbol? GetOutputValueType(SyntaxBase syntax) => binder.GetParent(syntax) switch
        {
            OutputDeclarationSyntax outputDeclaration => typeManager.GetTypeInfo(outputDeclaration.Value),
            ParenthesizedExpressionSyntax parenthesized => GetOutputValueType(parenthesized),
            _ => null,
        };

        private ITypeReference ConvertTypeExpressionToType(VariableAccessSyntax syntax, bool allowNamespaceReferences)
            => binder.GetSymbolInfo(syntax) switch
            {
                BuiltInNamespaceSymbol builtInNamespace when allowNamespaceReferences => builtInNamespace.Type,
                ImportedNamespaceSymbol importedNamespace when allowNamespaceReferences => importedNamespace.Type,
                BuiltInNamespaceSymbol or ImportedNamespaceSymbol => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).NamespaceSymbolUsedAsType(syntax.Name.IdentifierName)),
                AmbientTypeSymbol ambientType when ambientType.Type is TypeType assignableType => assignableType.Unwrapped,
                TypeAliasSymbol declaredType => TypeRefToType(syntax, declaredType),
                DeclaredSymbol declaredSymbol => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).ValueSymbolUsedAsType(declaredSymbol.Name)),
                _ => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).SymbolicNameIsNotAType(syntax.Name.IdentifierName, GetValidTypeNames())),
            };

        private IEnumerable<string> GetValidTypeNames() => binder.NamespaceResolver.GetKnownPropertyNames()
            .Concat(binder.FileSymbol.TypeDeclarations.Select(td => td.Name))
            .Distinct();

        private ITypeReference TypeRefToType(VariableAccessSyntax signifier, TypeAliasSymbol signified) => new DeferredTypeReference(() =>
        {
            var signifiedType = userDefinedTypeReferences.GetOrAdd(signified, GetUserDefinedTypeType);
            if (signifiedType is ErrorType error)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(signifier).ReferencedSymbolHasErrors(signified.Name));
            }

            return signifiedType;
        });

        private TypeSymbol ConvertTypeExpressionToType(ArrayTypeSyntax syntax)
        {
            if (!features.UserDefinedTypesEnabled)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypedArrayDeclarationsUnsupported());
            }

            var memberType = GetDeclaredTypeAssignment(syntax.Item)?.Reference ?? ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Item).InvalidTypeDefinition());
            var flags = TypeSymbolValidationFlags.Default;

            return memberType is DeferredTypeReference
                ? new TypedArrayType(syntax.ToText(), memberType, flags)
                : new TypedArrayType(memberType, flags);
        }

        private TypeSymbol GetObjectTypeType(ObjectTypeSyntax syntax)
        {
            if (!features.UserDefinedTypesEnabled)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypedObjectDeclarationsUnsupported());
            }

            List<TypeProperty> properties = new();
            List<ErrorDiagnostic> diagnostics = new();
            ObjectTypeNameBuilder nameBuilder = new();

            foreach (var prop in syntax.Properties)
            {
                var propertyType = GetDeclaredTypeAssignment(prop)?.Reference;
                propertyType ??= ErrorType.Create(DiagnosticBuilder.ForPosition(prop.Value).InvalidTypeDefinition());

                if (prop.TryGetKeyText() is string propertyName)
                {
                    properties.Add(new(propertyName, propertyType, TypePropertyFlags.Required, SemanticModelHelper.TryGetDescription(binder, typeManager.GetDeclaredType, prop)));
                    nameBuilder.AppendProperty(propertyName, GetPropertyTypeName(prop.Value, propertyType));
                } else
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
                1 => GetDeclaredTypeAssignment(additionalPropertiesDeclarations[0])?.Reference
                    ?? ErrorType.Create(DiagnosticBuilder.ForPosition(additionalPropertiesDeclarations[0].Value).InvalidTypeDefinition()),
                _ => LanguageConstants.Any,
            };
            var additionalPropertiesFlags = additionalPropertiesDeclarations.Any() ? TypePropertyFlags.None : TypePropertyFlags.FallbackProperty;

            if (additionalPropertiesDeclarations.Length > 1)
            {
                diagnostics.AddRange(additionalPropertiesDeclarations.Select(d => DiagnosticBuilder.ForPosition(d).MultipleAdditionalPropertiesDeclarations()));
            }

            if (additionalPropertiesType is not null && !additionalPropertiesFlags.HasFlag(TypePropertyFlags.FallbackProperty))
            {
                nameBuilder.AppendPropertyMatcher("*", GetPropertyTypeName(additionalPropertiesDeclarations[0].Value, additionalPropertiesType));
            }

            if (diagnostics.Any())
            {
                // foward any diagnostics gathered from parsing properties to the return type. normally, these diagnostics would be gathered by the SemanticDiagnosticVisitor (which would visit the properties of an ObjectType looking for errors).
                // Errors hidden behind DeferredTypeReferences will unfortunately be dropped, as we can't resolve their type without risking an infinite loop (in the case that a recursive object type has errors)
                return ErrorType.Create(diagnostics.Concat(properties.Select(p => p.TypeReference).OfType<TypeSymbol>().SelectMany(e => e.GetDiagnostics())));
            }

            return new ObjectType(nameBuilder.ToString(), default, properties, additionalPropertiesType, additionalPropertiesFlags);
        }

        private string GetPropertyTypeName(SyntaxBase typeSyntax, ITypeReference propertyType)
        {
            if (propertyType is DeferredTypeReference)
            {
                return typeSyntax.ToText();
            }

            return propertyType.Type.Name;
        }

        private bool HasSecureDecorator(DecorableSyntax syntax)
            => SemanticModelHelper.TryGetDecoratorInNamespace(binder, typeManager.GetDeclaredType, syntax, SystemNamespaceType.BuiltInName, LanguageConstants.ParameterSecurePropertyName) is not null;

        private DecoratorSyntax? TryGetSealedDecorator(DecorableSyntax syntax)
            => SemanticModelHelper.TryGetDecoratorInNamespace(binder, typeManager.GetDeclaredType, syntax, SystemNamespaceType.BuiltInName, LanguageConstants.ParameterSealedPropertyName);

        private ITypeReference GetTupleTypeType(TupleTypeSyntax syntax)
        {
            if (!features.UserDefinedTypesEnabled)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypedTupleDeclarationsUnsupported());
            }

            List<ITypeReference> items = new();
            TupleTypeNameBuilder nameBuilder = new();

            foreach (var item in syntax.Items)
            {
                var itemType = GetDeclaredTypeAssignment(item)?.Reference ?? ErrorType.Create(DiagnosticBuilder.ForPosition(item.Value).InvalidTypeDefinition());
                items.Add(itemType);
                nameBuilder.AppendItem(GetPropertyTypeName(item.Value, itemType));
            }

            return new TupleType(nameBuilder.ToString(), items.ToImmutableArray(), default);
        }

        private DeclaredTypeAssignment? GetTupleTypeItemType(TupleTypeItemSyntax syntax)
            => new(ApplyTypeModifyingDecorators(GetTypeFromTypeSyntax(syntax.Value, allowNamespaceReferences: false), syntax), syntax);

        private TypeSymbol ConvertTypeExpressionToType(StringSyntax syntax)
        {
            if (!features.UserDefinedTypesEnabled)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypeLiteralDeclarationsUnsupported());
            }

            if (typeManager.GetTypeInfo(syntax) is StringLiteralType literal)
            {
                return literal;
            }

            return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypeExpressionLiteralConversionFailed());
        }

        private TypeSymbol ConvertTypeExpressionToType(IntegerLiteralSyntax syntax)
        {
            if (!features.UserDefinedTypesEnabled)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypeLiteralDeclarationsUnsupported());
            }

            if (typeManager.GetTypeInfo(syntax) is IntegerLiteralType literal)
            {
                return literal;
            }

            return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypeExpressionLiteralConversionFailed());
        }

        private TypeSymbol ConvertTypeExpressionToType(BooleanLiteralSyntax syntax)
        {
            if (!features.UserDefinedTypesEnabled)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypeLiteralDeclarationsUnsupported());
            }

            return syntax.Value ? LanguageConstants.True : LanguageConstants.False;
        }

        private ITypeReference GetUnaryOperationType(UnaryOperationSyntax syntax)
        {
            if (!features.UserDefinedTypesEnabled)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypeLiteralDeclarationsUnsupported());
            }

            if (RequiresDeferral(syntax))
            {
                return new DeferredTypeReference(() => FinalizeUnaryType(syntax));
            }

            return FinalizeUnaryType(syntax);
        }

        private TypeSymbol FinalizeUnaryType(UnaryOperationSyntax syntax)
        {
            var baseExpressionType = GetTypeFromTypeSyntax(syntax.Expression, allowNamespaceReferences: false).Type;

            if (baseExpressionType is ErrorType)
            {
                return baseExpressionType;
            }

            var evaluated = OperationReturnTypeEvaluator.FoldUnaryExpression(syntax, baseExpressionType, out var foldDiags);
            foldDiags ??= ImmutableArray<IDiagnostic>.Empty;

            if (evaluated is {} result && TypeHelper.IsLiteralType(result) && !foldDiags.OfType<ErrorDiagnostic>().Any())
            {
                return result;
            }

            return ErrorType.Create(foldDiags.OfType<ErrorDiagnostic>()
                .Append(DiagnosticBuilder.ForPosition(syntax).TypeExpressionLiteralConversionFailed()));
        }

        private bool RequiresDeferral(SyntaxBase syntax) => syntax switch
        {
            NonNullAssertionSyntax nonNullAssertion => RequiresDeferral(nonNullAssertion.BaseExpression),
            ParenthesizedExpressionSyntax parenthesizedExpression => RequiresDeferral(parenthesizedExpression.Expression),
            NullableTypeSyntax nullableType => RequiresDeferral(nullableType.Base),
            UnaryOperationSyntax unaryOperation => RequiresDeferral(unaryOperation.Expression),
            UnionTypeSyntax unionType => unionType.Members.Any(m => RequiresDeferral(m.Value)),
            VariableAccessSyntax variableAccess when binder.GetSymbolInfo(variableAccess) is TypeAliasSymbol => true,
            _ => false,
        };

        private ITypeReference GetUnionTypeType(UnionTypeSyntax syntax)
        {
            if (!features.UserDefinedTypesEnabled)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypeUnionDeclarationsUnsupported());
            }

            if (RequiresDeferral(syntax))
            {
                return new DeferredTypeReference(() => FinalizeUnionType(syntax));
            }

            return FinalizeUnionType(syntax);
        }

        private TypeSymbol FinalizeUnionType(UnionTypeSyntax syntax)
            => TypeHelper.CreateTypeUnion(syntax.Members.Select(m => GetTypeFromTypeSyntax(m, allowNamespaceReferences: false)));

        private ITypeReference ConvertTypeExpressionToType(ParenthesizedExpressionSyntax syntax, bool allowNamespaceReferences)
            => GetTypeFromTypeSyntax(syntax.Expression, allowNamespaceReferences);

        private TypeSymbol ConvertTypeExpressionToType(PropertyAccessSyntax syntax)
        {
            var baseType = GetTypeFromTypeSyntax(syntax.BaseExpression, allowNamespaceReferences: true).Type;

            if (baseType is ErrorType error)
            {
                return error;
            }

            if (baseType is not ObjectType objectType)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.PropertyName).ObjectRequiredForPropertyAccess(baseType));
            }

            // Diagnostics will be surfaced by the TypeAssignmentVisitor, so we're only concerned here with whether the property access would be an error type
            return TypeHelper.GetNamedPropertyType(objectType, syntax.PropertyName, syntax.PropertyName.IdentifierName, shouldWarn: false, new SimpleDiagnosticWriter()) switch
            {
                TypeType tt => tt.Unwrapped,
                TypeSymbol otherwise => otherwise,
            };
        }

        private ITypeReference ConvertTypeExpressionToType(NullableTypeSyntax syntax)
        {
            if (!features.UserDefinedTypesEnabled)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).NullableTypesUnsupported());
            }

            var baseExpressionType = GetTypeFromTypeSyntax(syntax.Base, allowNamespaceReferences: false);

            return baseExpressionType is DeferredTypeReference
                ? new DeferredTypeReference(() => FinalizeNullableType(baseExpressionType))
                : FinalizeNullableType(baseExpressionType);
        }

        private TypeSymbol FinalizeNullableType(ITypeReference baseType) => baseType.Type switch
        {
            ErrorType errorType => errorType,
            TypeSymbol otherwise => TypeHelper.CreateTypeUnion(otherwise, LanguageConstants.Null)
        };

        private ITypeReference ConvertTypeExpressionToType(NonNullAssertionSyntax syntax)
        {
            if (!features.UserDefinedTypesEnabled)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).NullableTypesUnsupported());
            }

            var baseExpressionType = GetTypeFromTypeSyntax(syntax.BaseExpression, allowNamespaceReferences: false);

            return baseExpressionType is DeferredTypeReference
                ? new DeferredTypeReference(() => FinalizeNonNullableType(baseExpressionType))
                : FinalizeNonNullableType(baseExpressionType);
        }

        private TypeSymbol FinalizeNonNullableType(ITypeReference baseType) => baseType.Type switch
        {
            TypeSymbol maybeNullable when TypeHelper.TryRemoveNullability(maybeNullable) is TypeSymbol nonNullable => nonNullable,
            TypeSymbol otherwise => otherwise,
        };

        private DeclaredTypeAssignment? GetImportType(ImportDeclarationSyntax syntax)
        {
            if (this.binder.GetSymbolInfo(syntax) is ImportedNamespaceSymbol importedNamespace)
            {
                return new(importedNamespace.DeclaredType, syntax);
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

                case VariableSymbol variableSymbol when IsCycleFree(variableSymbol):
                    var variableType = this.typeManager.GetTypeInfo(variableSymbol.DeclaringVariable.Value);
                    return new DeclaredTypeAssignment(variableType, variableSymbol.DeclaringVariable);

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

            static TypeSymbol GetTypeAtIndex(TupleType baseType, IntegerLiteralType indexType, SyntaxBase indexSyntax) => indexType.Value switch
            {
                < 0 => ErrorType.Create(DiagnosticBuilder.ForPosition(indexSyntax).IndexOutOfBounds(baseType.Name, baseType.Items.Length, indexType.Value)),
                long value when value >= baseType.Items.Length => ErrorType.Create(DiagnosticBuilder.ForPosition(indexSyntax).IndexOutOfBounds(baseType.Name, baseType.Items.Length, value)),
                // unlikely to hit this given that we've established that the tuple has a item at the given position
                > int.MaxValue => ErrorType.Create(DiagnosticBuilder.ForPosition(indexSyntax).IndexOutOfBounds(baseType.Name, baseType.Items.Length, indexType.Value)),
                long otherwise => baseType.Items[(int) otherwise].Type,
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
                    return new(GetTypeAtIndex(tupleTypeWithKnownIndex, integerLiteral, syntax.IndexExpression), DeclaringSyntaxForArrayAccessIfCollectionBase(baseExpressionAssignment));

                case TupleType tupleTypeWithIndexPossibilities when indexAssignedType is UnionType indexUnion && indexUnion.Members.All(t => t.Type is IntegerLiteralType):
                    var possibilities = indexUnion.Members.Select(t => t.Type).OfType<IntegerLiteralType>().Select(ilt => GetTypeAtIndex(tupleTypeWithIndexPossibilities, ilt, syntax.IndexExpression));
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
                PropertyAccessSyntax access when access.BaseExpression is ForSyntax
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

                if (prevAccess?.SafeAccessMarker is not null || nextAccess.SafeAccessMarker is not null)
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
            var parent = this.binder.GetParent(syntax);

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
                    return GetNonNullableTypeAssignment(parent)?.ReplaceDeclaringSyntax(syntax);
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
                SymbolHelper.TryGetSymbolInfo(this.binder, this.GetDeclaredType, parent) is not FunctionSymbol functionSymbol)
            {
                return null;
            }

            var arguments = parentFunction.Arguments.ToImmutableArray();
            var argIndex = arguments.IndexOf(syntax);
            var declaredType = functionSymbol.GetDeclaredArgumentType(
                argIndex,
                getAssignedArgumentType: i => typeManager.GetTypeInfo(parentFunction.Arguments[i]));

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
            var parent = this.binder.GetParent(syntax);
            if (parent is null)
            {
                return null;
            }

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

                case ImportWithClauseSyntax:
                    parent = this.binder.GetParent(parent);

                    if (parent is null)
                    {
                        throw new InvalidOperationException("Expected ImportWithClauseSyntax to have a parent.");
                    }

                    if (GetDeclaredTypeAssignment(parent) is not { } importAssignment ||
                        importAssignment.Reference.Type is not NamespaceType namespaceType)
                    {
                        return null;
                    }

                    if (namespaceType.ConfigurationType is null)
                    {
                        // this namespace doesn't support configuration, but it has been provided.
                        // we'll check for this during type assignment.
                        return null;
                    }

                    // the object is an item in an array
                    // use the item's type and propagate flags
                    return TryCreateAssignment(ResolveDiscriminatedObjects(namespaceType.ConfigurationType.Type, syntax), syntax, importAssignment.Flags);
                case FunctionArgumentSyntax:
                    if (GetNonNullableTypeAssignment(parent) is not { } parentAssignment)
                    {
                        return null;
                    }

                    return TryCreateAssignment(ResolveDiscriminatedObjects(parentAssignment.Reference.Type, syntax), syntax, parentAssignment.Flags);
            }

            return null;
        }

        private DeclaredTypeAssignment? GetObjectPropertyType(ObjectPropertySyntax syntax)
        {
            var propertyName = syntax.TryGetKeyText();
            var parent = this.binder.GetParent(syntax);
            if (propertyName == null || !(parent is ObjectSyntax parentObject))
            {
                // the property name is an interpolated string (expression) OR the parent is missing OR the parent is not ObjectSyntax
                // cannot establish declared type
                // TODO: Improve this when we have constant folding
                return null;
            }

            var assignment = GetNonNullableTypeAssignment(parent);

            // we are in the process of establishing the declared type for the syntax nodes,
            // so we must set useSyntax to false to avoid a stack overflow
            return GetObjectPropertyType(assignment?.Reference.Type, parentObject, propertyName, useSyntax: false);
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
                    if (objectType.AdditionalPropertiesType != null)
                    {
                        return new DeclaredTypeAssignment(objectType.AdditionalPropertiesType.Type, declaringProperty, ConvertFlags(objectType.AdditionalPropertiesFlags));
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

        private static TypeSymbol? ResolveDiscriminatedObjects(TypeSymbol type, ObjectSyntax syntax)
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
            return matchingObjectType?.Type;
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

            if (!moduleSymbol.TryGetSemanticModel(out var moduleSemanticModel, out var failureDiagnostic))
            {
                return ErrorType.Create(failureDiagnostic);
            }

            // We need to bind and validate all of the parameters and outputs that declare resource types
            // within the context of this type manager. This will surface any issues where the type declared by
            // a module is not understood inside this compilation unit.
            var parameters = new List<TypeProperty>();

            foreach (var parameter in moduleSemanticModel.Parameters.Values)
            {
                var type = parameter.TypeReference.Type;
                if (type is UnboundResourceType unboundType)
                {
                    var boundType = GetResourceTypeFromString(module.Span, unboundType.TypeReference.FormatName(), ResourceTypeGenerationFlags.ExistingResource, parentResourceType: null);
                    if (boundType is ResourceType resourceType)
                    {
                        // We use a special type for Resource type parameters because they have different assignability rules.
                        type = new ResourceParameterType(resourceType.DeclaringNamespace, unboundType.TypeReference);
                    }
                }

                var flags = parameter.IsRequired ? TypePropertyFlags.Required | TypePropertyFlags.WriteOnly : TypePropertyFlags.WriteOnly;
                parameters.Add(new TypeProperty(parameter.Name, type, flags, parameter.Description));
            }

            var outputs = new List<TypeProperty>();
            foreach (var output in moduleSemanticModel.Outputs)
            {
                var type = output.TypeReference.Type;
                if (type is UnboundResourceType unboundType)
                {
                    type = GetResourceTypeFromString(module.Span, unboundType.TypeReference.FormatName(), ResourceTypeGenerationFlags.ExistingResource, parentResourceType: null);
                }

                outputs.Add(new TypeProperty(output.Name, type, TypePropertyFlags.ReadOnly, output.Description));
            }

            return LanguageConstants.CreateModuleType(
                parameters,
                outputs,
                moduleSemanticModel.TargetScope,
                binder.TargetScope,
                LanguageConstants.TypeNameModule);
        }

        private TypeSymbol GetResourceTypeFromString(TextSpan span, string stringContent, ResourceTypeGenerationFlags typeGenerationFlags, ResourceType? parentResourceType)
        {
            var colonIndex = stringContent.IndexOf(':');
            if (colonIndex > 0)
            {
                var scheme = stringContent.Substring(0, colonIndex);
                var typeString = stringContent.Substring(colonIndex + 1);

                if (binder.NamespaceResolver.TryGetNamespace(scheme) is not { } namespaceType)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(span).UnknownResourceReferenceScheme(scheme, binder.NamespaceResolver.GetNamespaceNames().OrderBy(x => x, StringComparer.OrdinalIgnoreCase)));
                }

                if (parentResourceType is not null &&
                    parentResourceType.DeclaringNamespace != namespaceType)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(span).ParentResourceInDifferentNamespace(namespaceType.Name, parentResourceType.DeclaringNamespace.Name));
                }

                var (errorType, typeReference) = GetCombinedTypeReference(span, typeGenerationFlags, parentResourceType, typeString);
                if (errorType is not null)
                {
                    return errorType;
                }

                if (typeReference is null)
                {
                    // this won't happen, because GetCombinedTypeReference will either return non-null errorType, or non-null typeReference.
                    // there's no great way to enforce this in the type system sadly - https://github.com/dotnet/roslyn/discussions/56962
                    throw new InvalidOperationException($"typeReference is null");
                }

                if (namespaceType.ResourceTypeProvider.TryGetDefinedType(namespaceType, typeReference, typeGenerationFlags) is { } definedResource)
                {
                    return definedResource;
                }

                if (namespaceType.ResourceTypeProvider.TryGenerateFallbackType(namespaceType, typeReference, typeGenerationFlags) is { } defaultResource)
                {
                    return defaultResource;
                }

                return ErrorType.Create(DiagnosticBuilder.ForPosition(span).FailedToFindResourceTypeInNamespace(namespaceType.ProviderName, typeReference.FormatName()));
            }
            else
            {
                var (errorType, typeReference) = GetCombinedTypeReference(span, typeGenerationFlags, parentResourceType, stringContent);
                if (errorType is not null)
                {
                    return errorType;
                }

                if (typeReference is null)
                {
                    // this won't happen, because GetCombinedTypeReference will either return non-null errorType, or non-null typeReference.
                    // there's no great way to enforce this in the type system sadly - https://github.com/dotnet/roslyn/discussions/56962
                    throw new InvalidOperationException($"qualifiedTypeReference is null");
                }

                var resourceTypes = binder.NamespaceResolver.GetMatchingResourceTypes(typeReference, typeGenerationFlags);
                return resourceTypes.Length switch {
                    0 => ErrorType.Create( DiagnosticBuilder.ForPosition(span).InvalidResourceType()),
                    1 => resourceTypes[0],
                    _ => ErrorType.Create(DiagnosticBuilder.ForPosition(span).AmbiguousResourceTypeBetweenImports(typeReference.FormatName(), resourceTypes.Select(x => x.DeclaringNamespace.Name))),
                };
            }
        }

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
                SyntaxHelper.UnwrapArrayAccessSyntax(referenceParentSyntax) is {} result &&
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

        private static (ErrorType? error, ResourceTypeReference? typeReference) GetCombinedTypeReference(TextSpan span, ResourceTypeGenerationFlags flags, ResourceType? parentResourceType, string typeString)
        {
            if (ResourceTypeReference.TryParse(typeString) is not { } typeReference)
            {
                return (ErrorType.Create(DiagnosticBuilder.ForPosition(span).InvalidResourceType()), null);
            }

            if (!flags.HasFlag(ResourceTypeGenerationFlags.NestedResource))
            {
                // this is not a syntactically nested resource - return the type reference as-is
                return (null, typeReference);
            }

            // we're dealing with a syntactically nested resource here
            if (parentResourceType is null)
            {
                return (ErrorType.Create(DiagnosticBuilder.ForPosition(span).InvalidAncestorResourceType()), null);
            }

            if (typeReference.TypeSegments.Length > 1)
            {
                // OK this resource is the one that's wrong.
                return (ErrorType.Create(DiagnosticBuilder.ForPosition(span).InvalidResourceTypeSegment(typeString)), null);
            }

            return (null, ResourceTypeReference.Combine(
                parentResourceType.TypeReference,
                typeReference));
        }
    }
}
