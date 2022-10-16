// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
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
        private readonly ConcurrentDictionary<DeclaredTypeSymbol, TypeSymbol> declaredTypeReferences = new();
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

                case ObjectTypePropertySyntax typeProperty:
                    return GetTypePropertyType(typeProperty);

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

                case PropertyAccessSyntax propertyAccess:
                    return GetPropertyAccessType(propertyAccess);

                case ResourceAccessSyntax resourceAccess:
                    return GetResourceAccessType(resourceAccess);

                case ArrayAccessSyntax arrayAccess:
                    return GetArrayAccessType(arrayAccess);

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
            }

            return null;
        }

        private DeclaredTypeAssignment GetParameterType(ParameterDeclarationSyntax syntax)
        {
            var declaredType = TryGetTypeFromTypeSyntax(syntax.Type);
            declaredType ??= ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Type).InvalidParameterType());

            return new(declaredType, syntax);
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

            var parameterDeclarations = bicepSemanticModel.Root.ParameterDeclarations;

            // TODO: Needs to be optimized
            foreach (var parameterSymbol in parameterDeclarations)
            {
                if (LanguageConstants.IdentifierComparer.Equals(parameterSymbol.Name, parameterAssignmentSymbol.Name))
                {
                    return bicepSemanticModel.GetDeclaredType(parameterSymbol.DeclaringParameter);
                }
            }

            return null;
        }

        private DeclaredTypeAssignment GetTypeType(TypeDeclarationSyntax syntax)
        {
            if (!features.AggregateTypesEnabled)
            {
                return new(ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypeDeclarationStatementsUnsupported()), syntax);
            }

            var type = binder.GetSymbolInfo(syntax) switch
            {
                DeclaredTypeSymbol declaredType => declaredTypeReferences.GetOrAdd(declaredType, GetTypeDeclared),
                ErrorSymbol errorSymbol => errorSymbol.ToErrorType(),
                // binder.GetSymbolInfo(TypeDeclarationSyntax) should always return a DeclaredTypeSymbol or an error, but just in case...
                _ => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).SymbolicNameDoesNotExist(syntax.Name.IdentifierName)),
            };

            return new(type, syntax);
        }

        private TypeSymbol GetTypeDeclared(DeclaredTypeSymbol symbol)
        {
            if (binder.TryGetCycle(symbol) is {} cycle)
            {
                var builder = DiagnosticBuilder.ForPosition(symbol.DeclaringType.Name);
                var diagnostic = cycle.Length == 1
                    ? builder.CyclicExpressionSelfReference()
                    : builder.CyclicExpression(cycle.Select(s => s.Name));

                return ErrorType.Create(diagnostic);
            }

            return GetTypeFromTypeSyntax(symbol.DeclaringType.Value);
        }

        private DeclaredTypeAssignment? GetTypePropertyType(ObjectTypePropertySyntax syntax)
            => GetTypeReferencForTypeProperty(syntax) is {} @ref ? new(@ref, syntax) : null;

        private ITypeReference? GetTypeReferencForTypeProperty(ObjectTypePropertySyntax syntax)
        {
            if (syntax.Value is TypeAccessSyntax signifier && binder.GetSymbolInfo(signifier) is DeclaredTypeSymbol signified)
            {
                return new DeferredTypeReference(() => declaredTypeReferences.GetOrAdd(signified, GetTypeDeclared));
            }

            return TryGetTypeFromTypeSyntax(syntax.Value);
        }

        private DeclaredTypeAssignment GetOutputType(OutputDeclarationSyntax syntax)
        {
            ITypeReference declaredType;
            if (syntax.Type is ResourceTypeSyntax resourceTypeSyntax && resourceTypeSyntax.Type is null)
            {
                // The resource type of an output can be inferred.
                declaredType = this.typeManager.GetTypeInfo(syntax.Value);
            }
            else
            {
                declaredType = TryGetTypeFromTypeSyntax(syntax.Type) ??
                    ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Type).InvalidOutputType());
            }

            return new(declaredType, syntax);
        }

        private TypeSymbol GetTypeFromTypeSyntax(SyntaxBase syntax) => TryGetTypeFromTypeSyntax(syntax)
            ?? ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).InvalidTypeDefinition());

        private TypeSymbol? TryGetTypeFromTypeSyntax(SyntaxBase syntax)
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();

            // assume "any" type when the parameter has parse errors (either missing or was skipped)
            return syntax switch
            {
                SkippedTriviaSyntax => LanguageConstants.Any,
                SimpleTypeSyntax simple => LanguageConstants.TryGetDeclarationType(simple.TypeName),
                ResourceTypeSyntax resource => GetDeclaredType(resource),
                TypeAccessSyntax typeRef => ParseTypeExpression(typeRef),
                ArrayTypeSyntax array => ParseTypeExpression(array),
                ObjectTypeSyntax @object => ParseTypeExpression(@object),
                StringSyntax @string => ParseTypeExpression(@string),
                IntegerLiteralSyntax @int => ParseTypeExpression(@int),
                BooleanLiteralSyntax @bool => ParseTypeExpression(@bool),
                UnaryOperationSyntax unaryOperation => ParseTypeExpression(unaryOperation),
                UnionTypeSyntax unionType => ParseTypeExpression(unionType),
                ParenthesizedExpressionSyntax parenthesized => ParseTypeExpression(parenthesized),
                _ => null
            };
        }

        private DeclaredTypeAssignment GetResourceTypeType(ResourceTypeSyntax syntax)
            => new(GetTypeReferenceForResourceType(syntax), syntax);

        private TypeSymbol GetTypeReferenceForResourceType(ResourceTypeSyntax syntax)
        {
            var type = GetDeclaredResourceType(syntax);

            if (type is ResourceType resourceType)
            {
                if (IsExtensibilityType(resourceType))
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).UnsupportedResourceTypeParameterOrOutputType(resourceType.Name));
                }
            }

            return type;
        }

        private bool IsExtensibilityType(ResourceType resourceType)
        {
            return resourceType.DeclaringNamespace.ProviderName != AzNamespaceType.BuiltInName;
        }

        private TypeSymbol ParseTypeExpression(TypeAccessSyntax syntax)
            => binder.GetSymbolInfo(syntax) switch
            {
                DeclaredTypeSymbol declaredType => TypeRefToType(syntax, declaredType),
                ErrorSymbol errorSymbol => errorSymbol.ToErrorType(),
                DeclaredSymbol declaredSymbol => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).ValueSymbolUsedAsType(declaredSymbol.Name)),
                _ => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).SymbolicNameDoesNotExist(syntax.Name.IdentifierName)),
            };

        private TypeSymbol TypeRefToType(TypeAccessSyntax signifier, DeclaredTypeSymbol signified)
        {
            var signifiedType = declaredTypeReferences.GetOrAdd(signified, GetTypeDeclared);
            if (signifiedType is ErrorType error)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(signifier).ReferencedSymbolHasErrors(signified.Name));
            }

            return signifiedType;
        }

        private TypeSymbol ParseTypeExpression(ArrayTypeSyntax syntax)
        {
            if (!features.AggregateTypesEnabled)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypedArrayDeclarationsUnsupported());
            }

            return new TypedArrayType(GetTypeFromTypeSyntax(syntax.Item), TypeSymbolValidationFlags.Default);
        }

        private TypeSymbol ParseTypeExpression(ObjectTypeSyntax syntax)
        {
            if (!features.AggregateTypesEnabled)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypedObjectDeclarationsUnsupported());
            }

            List<TypeProperty> properties = new();
            List<ErrorDiagnostic> diagnostics = new();
            StringBuilder objectName = new("{ ");

            foreach (var prop in syntax.Properties)
            {
                var propertyType = GetDeclaredTypeAssignment(prop)?.Reference;
                propertyType ??= ErrorType.Create(DiagnosticBuilder.ForPosition(prop.Value).InvalidTypeDefinition());

                if (prop.TryGetKeyText() is string propertyName)
                {
                    if (Lexer.IsValidIdentifier(propertyName))
                    {
                        objectName.Append(propertyName);
                    } else
                    {
                        objectName.Append('\'');
                        objectName.Append(StringUtils.EscapeBicepString(propertyName));
                        objectName.Append('\'');
                    }

                    objectName.Append(": ");
                    objectName.Append(GetPropertyTypeName(prop.Value, propertyType));
                    objectName.Append(", ");

                    var propertyFlags = prop.OptionalityMarker is null ? TypePropertyFlags.Required : TypePropertyFlags.None;
                    properties.Add(new(propertyName, propertyType, propertyFlags, TryGetTypePropertyDescription(prop)));
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

            if (diagnostics.Any())
            {
                // foward any diagnostics gathered from parsing properties to the return type. normally, this diagnostics would be gathered by the SemanticDiagnosticVisitor (which would visit the properties of an ObjectType looking for errors).
                // Errors hidden behind DeferredTypeReferences will unfortunately be dropped, as we can't call their type function without risking an infinite loop (in the case that a recursive object type has errors)
                return ErrorType.Create(diagnostics.Concat(properties.Select(p => p.TypeReference).OfType<TypeSymbol>().SelectMany(e => e.GetDiagnostics())));
            }

            objectName.Remove(objectName.Length - 2, 1);
            objectName.Append('}');

            var parentSyntax = binder.GetParent(syntax);

            var typeFlags = UnwrapUntilDecorable(syntax, HasSecureDecorator, TypeSymbolValidationFlags.IsSecure, TypeSymbolValidationFlags.Default);

            (TypeSymbol? type, TypePropertyFlags flags) additionalProperties
                = UnwrapUntilDecorable(syntax, HasSealedDecorator, (null, TypePropertyFlags.None), (LanguageConstants.Any, TypePropertyFlags.FallbackProperty));

            return new ObjectType(objectName.ToString(), typeFlags, properties, additionalProperties.type, additionalProperties.flags);
        }

        private string GetPropertyTypeName(SyntaxBase typeSyntax, ITypeReference propertyType)
        {
            if (typeSyntax is TypeAccessSyntax typeAccess)
            {
                return typeAccess.Name.IdentifierName;
            }

            return propertyType switch
            {
                DeferredTypeReference deferred => "<deferred>",
                _ => propertyType.Type.Name,
            };
        }

        private string? TryGetTypePropertyDescription(ObjectTypePropertySyntax syntax)
            => (TryGetDecorator(syntax.Decorators, SystemNamespaceType.BuiltInName, LanguageConstants.MetadataDescriptionPropertyName)
                ?.Arguments.FirstOrDefault()?.Expression as StringSyntax)?.TryGetLiteralValue();

        private T UnwrapUntilDecorable<T>(SyntaxBase syntax, Predicate<DecorableSyntax> condition, T valueIfTrue, T valueIfFalse) => binder.GetParent(syntax) switch
        {
            DecorableSyntax decorable when condition(decorable) => valueIfTrue,
            ParenthesizedExpressionSyntax parenthesized => UnwrapUntilDecorable(parenthesized, condition, valueIfTrue, valueIfFalse),
            TernaryOperationSyntax ternary when ternary.TrueExpression == syntax || ternary.FalseExpression == syntax
                => UnwrapUntilDecorable(ternary, condition, valueIfTrue, valueIfFalse),
            _ => valueIfFalse,
        };

        private bool HasSecureDecorator(DecorableSyntax syntax)
            => TryGetDecorator(syntax.Decorators, SystemNamespaceType.BuiltInName, LanguageConstants.ParameterSecurePropertyName) is not null;

        private bool HasSealedDecorator(DecorableSyntax syntax)
            => TryGetDecorator(syntax.Decorators, SystemNamespaceType.BuiltInName, LanguageConstants.ParameterSealedPropertyName) is not null;

        private DecoratorSyntax? TryGetDecorator(IEnumerable<DecoratorSyntax> decoratorSyntaxes, string namespaceName, string decoratorName)
            => decoratorSyntaxes
                .Where(decoratorSyntax => binder.GetSymbolInfo(decoratorSyntax.Expression) is FunctionSymbol functionSymbol
                    && functionSymbol.DeclaringObject is NamespaceType namespaceType
                    && LanguageConstants.IdentifierComparer.Equals(namespaceType.ProviderName, namespaceName)
                    && LanguageConstants.IdentifierComparer.Equals(functionSymbol.Name, decoratorName))
                .FirstOrDefault();

        private TypeSymbol ParseTypeExpression(StringSyntax syntax)
        {
            if (!features.AggregateTypesEnabled)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypeLiteralDeclarationsUnsupported());
            }

            if (typeManager.GetTypeInfo(syntax) is StringLiteralType literal)
            {
                return literal;
            }

            return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypeExpressionLiteralConversionFailed());
        }

        private TypeSymbol ParseTypeExpression(IntegerLiteralSyntax syntax)
        {
            if (!features.AggregateTypesEnabled)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypeLiteralDeclarationsUnsupported());
            }

            if (typeManager.GetTypeInfo(syntax) is IntegerLiteralType literal)
            {
                return literal;
            }

            return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypeExpressionLiteralConversionFailed());
        }

        private TypeSymbol ParseTypeExpression(BooleanLiteralSyntax syntax)
        {
            if (!features.AggregateTypesEnabled)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypeLiteralDeclarationsUnsupported());
            }

            return syntax.Value ? LanguageConstants.True : LanguageConstants.False;
        }

        private TypeSymbol ParseTypeExpression(UnaryOperationSyntax syntax)
        {
            var baseExpressionType = GetTypeFromTypeSyntax(syntax.Expression);

            var evaluated = OperationReturnTypeEvaluator.FoldUnaryExpression(syntax, baseExpressionType, out var foldDiags);
            foldDiags ??= ImmutableArray<IDiagnostic>.Empty;

            if (evaluated is {} result && IsLiteralType(result) && !foldDiags.OfType<ErrorDiagnostic>().Any())
            {
                return result;
            }

            return ErrorType.Create(foldDiags.OfType<ErrorDiagnostic>()
                .Append(DiagnosticBuilder.ForPosition(syntax).TypeExpressionLiteralConversionFailed()));
        }

        private TypeSymbol ParseTypeExpression(UnionTypeSyntax syntax)
        {
            if (!features.AggregateTypesEnabled)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).TypeUnionDeclarationsUnsupported());
            }

            // ARM's allowedValues constraint permits mixed type arrays (so long as none of the members are themselves arrays).
            // The runtime in that case will validate that the submitted array contains a subset of the allowed values
            // (e.g., `[1, 2]` or `[2, 3]` would both be permitted with `"type": "array", "allowedValues": [1, 2, 3]`)
            // Ergo, syntax like `type foo = (1|true|'a string')[]` should bypass the validity checker
            var mightBeArrayAny = MightBeArrayAny(syntax);

            TypeSymbol? keystoneType = null;
            List<ITypeReference> matchingMembers = new();
            List<(ITypeReference, UnionTypeMemberSyntax)> nonMatchingMembers = new();
            List<ErrorDiagnostic> memberDiagnostics = new();

            foreach (var member in syntax.Members)
            {
                // Array<any> is the only location in which a null literal is a valid type
                var memberType = mightBeArrayAny && IsNullLiteral(member.Value)
                    ? LanguageConstants.Null
                    : GetTypeFromTypeSyntax(member.Value);


                if (memberType is ErrorType error)
                {
                    memberDiagnostics.AddRange(error.GetDiagnostics());
                    continue;
                }

                foreach (var flattenedType in FlattenUnionMemberType(memberType))
                {
                    // null complicates the type check and is only permissible in a specific circumstance. Treat it as non-matching and skip the check
                    if (mightBeArrayAny && ReferenceEquals(flattenedType, LanguageConstants.Null))
                    {
                        nonMatchingMembers.Add((flattenedType, member));
                        continue;
                    }

                    if (!IsLiteralType(flattenedType))
                    {
                        memberDiagnostics.Add(DiagnosticBuilder.ForPosition(member).NonLiteralUnionMember());
                        break;
                    }

                    if (keystoneType is null)
                    {
                        if (GetNonLiteralType(flattenedType) is not {} nonLiteral)
                        {
                            memberDiagnostics.Add(DiagnosticBuilder.ForPosition(member).NonLiteralUnionMember());
                            break;
                        }

                        keystoneType = nonLiteral;
                    }

                    if (mightBeArrayAny && flattenedType is ArrayType)
                    {
                        mightBeArrayAny = false;
                        var mismatchForCurrentMember = false;
                        foreach (var nonMatchingMemberSyntax in nonMatchingMembers.Select(t => t.Item2).Distinct())
                        {
                            memberDiagnostics.Add(DiagnosticBuilder.ForPosition(nonMatchingMemberSyntax).InvalidUnionTypeMember(keystoneType.Name));
                            mismatchForCurrentMember = mismatchForCurrentMember || nonMatchingMemberSyntax == member;
                        }

                        if (mismatchForCurrentMember)
                        {
                            break;
                        }
                    }

                    if (TypeValidator.AreTypesAssignable(flattenedType, keystoneType))
                    {
                        matchingMembers.Add(flattenedType);
                    } else if (mightBeArrayAny)
                    {
                        nonMatchingMembers.Add((flattenedType, member));
                    } else
                    {
                        memberDiagnostics.Add(DiagnosticBuilder.ForPosition(member).InvalidUnionTypeMember(keystoneType.Name));
                        break;
                    }
                }
            }

            if (memberDiagnostics.Any())
            {
                return ErrorType.Create(memberDiagnostics);
            }

            return TypeHelper.CreateTypeUnion(matchingMembers.Concat(nonMatchingMembers.Select(t => t.Item1)));
        }

        private bool MightBeArrayAny(SyntaxBase syntax) => binder.GetParent(syntax) switch
        {
            ParenthesizedExpressionSyntax parenthesized => MightBeArrayAny(parenthesized),
            ArrayTypeSyntax => true,
            _ => false,
        };

        private bool IsNullLiteral(SyntaxBase syntax) => syntax switch
        {
            ParenthesizedExpressionSyntax parenthesized => IsNullLiteral(parenthesized.Expression),
            NullLiteralSyntax => true,
            _ => false,
        };

        private bool IsLiteralType(TypeSymbol type) => type switch
        {
            StringLiteralType => true,
            IntegerLiteralType => true,
            BooleanLiteralType => true,
            ObjectType objectType => objectType.Properties.All(kvp => IsLiteralType(kvp.Value.TypeReference.Type)),
            // TODO for array literals when type system adds support for tuples
            _ => false,
        };

        private TypeSymbol? GetNonLiteralType(TypeSymbol? type) => type switch {
            StringLiteralType => LanguageConstants.String,
            IntegerLiteralType => LanguageConstants.Int,
            BooleanLiteralType => LanguageConstants.Bool,
            ObjectType => LanguageConstants.Object,
            // TODO for array literals when type system adds support for tuples
            _ => null,
        };

        private IEnumerable<TypeSymbol> FlattenUnionMemberType(ITypeReference memberType)
            => memberType.Type is UnionType union ? union.Members.SelectMany(FlattenUnionMemberType) : memberType.Type.AsEnumerable();

        private TypeSymbol ParseTypeExpression(ParenthesizedExpressionSyntax syntax) => GetTypeFromTypeSyntax(syntax.Expression);

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

        private DeclaredTypeAssignment? GetPropertyAccessType(PropertyAccessSyntax syntax)
        {
            if (!syntax.PropertyName.IsValid)
            {
                return null;
            }

            if(syntax.BaseExpression is ForSyntax)
            {
                // in certain parser recovery scenarios, the parser can produce a PropertyAccessSyntax operating on a ForSyntax
                // this leads to a stack overflow which we don't really want, so let's short circuit here.
                return null;
            }

            var baseExpressionAssignment = GetDeclaredTypeAssignment(syntax.BaseExpression);

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


        private DeclaredTypeAssignment? GetArrayAccessType(ArrayAccessSyntax syntax)
        {
            var baseExpressionAssignment = GetDeclaredTypeAssignment(syntax.BaseExpression);
            var indexAssignedType = this.typeManager.GetTypeInfo(syntax.IndexExpression);

            // TODO: Currently array access is broken with discriminated object types - revisit when that is fixed
            switch (baseExpressionAssignment?.Reference.Type)
            {
                case ArrayType arrayType when TypeValidator.AreTypesAssignable(indexAssignedType, LanguageConstants.Int):
                    // we are accessing an array by an expression of a numeric type
                    // return the item type of the array

                    // for regular array we can't evaluate the array index at this point, but for loops the index is irrelevant
                    // and we need to set declaring syntax, so property access can provide completions correctly for resource and module loops
                    var declaringSyntax = baseExpressionAssignment.DeclaringSyntax switch
                    {
                        ForSyntax { Body: ObjectSyntax loopBody } => loopBody,
                        ForSyntax { Body: IfConditionSyntax { Body: ObjectSyntax loopBody } } => loopBody,
                        _ => null
                    };

                    return new DeclaredTypeAssignment(arrayType.Item.Type, declaringSyntax);

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
                    return GetDeclaredTypeAssignment(parent)?.ReplaceDeclaringSyntax(syntax);
                case FunctionArgumentSyntax:
                    return GetDeclaredTypeAssignment(parent)?.ReplaceDeclaringSyntax(syntax);
                default:
                    return null;
            }
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
                    return GetDeclaredTypeAssignment(parent)?.ReplaceDeclaringSyntax(syntax);
                case FunctionArgumentSyntax:
                    return GetDeclaredTypeAssignment(parent)?.ReplaceDeclaringSyntax(syntax);
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
                    var parentType = GetDeclaredTypeAssignment(parent)?.Reference.Type;
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
                    if (GetDeclaredTypeAssignment(parent) is not { } objectPropertyAssignment ||
                        objectPropertyAssignment.Reference.Type is not { } objectPropertyParent)
                    {
                        return null;
                    }

                    // the object is the value of a property of another object
                    // use the declared type of the property and propagate the flags
                    return TryCreateAssignment(ResolveDiscriminatedObjects(objectPropertyParent, syntax), syntax, objectPropertyAssignment.Flags);

                case ArrayItemSyntax:
                    if (GetDeclaredTypeAssignment(parent) is not { } arrayItemAssignment ||
                        arrayItemAssignment.Reference.Type is not { } arrayParent)
                    {
                        return null;
                    }

                    // the object is an item in an array
                    // use the item's type and propagate flags
                    return TryCreateAssignment(ResolveDiscriminatedObjects(arrayParent, syntax), syntax, arrayItemAssignment.Flags);

                case ImportDeclarationSyntax:
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
                    if (GetDeclaredTypeAssignment(parent) is not { } parentAssignment)
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

            var assignment = GetDeclaredTypeAssignment(parent);

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
            if (!features.ResourceTypedParamsAndOutputsEnabled)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(typeSyntax).ParamOrOutputResourceTypeUnsupported());
            }

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
            foreach (var parameter in moduleSemanticModel.Parameters)
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
