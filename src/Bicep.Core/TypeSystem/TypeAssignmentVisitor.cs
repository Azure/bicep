// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Intermediate;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Workspaces;

namespace Bicep.Core.TypeSystem
{
    public sealed class TypeAssignmentVisitor : AstVisitor
    {
        private readonly IFeatureProvider features;
        private readonly ITypeManager typeManager;
        private readonly IBinder binder;
        private readonly IFileResolver fileResolver;
        private readonly IDiagnosticLookup parsingErrorLookup;
        private readonly ISourceFileLookup sourceFileLookup;
        private readonly ISemanticModelLookup semanticModelLookup;
        private readonly BicepSourceFileKind fileKind;
        private readonly ConcurrentDictionary<SyntaxBase, TypeAssignment> assignedTypes;
        private readonly ConcurrentDictionary<FunctionCallSyntaxBase, FunctionOverload> matchedFunctionOverloads;
        private readonly ConcurrentDictionary<FunctionCallSyntaxBase, Expression> matchedFunctionResultValues;
        private readonly ConcurrentDictionary<CompileTimeImportDeclarationSyntax, ImmutableDictionary<string, TypeTypeProperty>?> importableTypesByCompileTimeImportDeclaration;

        public TypeAssignmentVisitor(ITypeManager typeManager, IFeatureProvider features, IBinder binder, IFileResolver fileResolver, IDiagnosticLookup parsingErrorLookup, ISourceFileLookup sourceFileLookup, ISemanticModelLookup semanticModelLookup, Workspaces.BicepSourceFileKind fileKind)
        {
            this.typeManager = typeManager;
            this.features = features;
            this.binder = binder;
            this.fileResolver = fileResolver;
            this.parsingErrorLookup = parsingErrorLookup;
            this.sourceFileLookup = sourceFileLookup;
            this.semanticModelLookup = semanticModelLookup;
            this.fileKind = fileKind;
            assignedTypes = new();
            matchedFunctionOverloads = new();
            matchedFunctionResultValues = new();
            importableTypesByCompileTimeImportDeclaration = new();
        }

        private TypeAssignment GetTypeAssignment(SyntaxBase syntax)
        {
            Visit(syntax);

            if (!assignedTypes.TryGetValue(syntax, out var typeAssignment))
            {
                return new TypeAssignment(ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).InvalidExpression()));
            }

            return typeAssignment;
        }

        public TypeSymbol GetTypeInfo(SyntaxBase syntax)
            => GetTypeAssignment(syntax).Reference.Type;

        public IEnumerable<IDiagnostic> GetAllDiagnostics()
        {
            // ensure we've visited all of the syntax nodes
            Visit(this.binder.FileSymbol.Syntax);

            return assignedTypes.Values.SelectMany(x => x.Diagnostics);
        }

        public FunctionOverload? GetMatchedFunctionOverload(FunctionCallSyntaxBase syntax)
        {
            Visit(syntax);
            return matchedFunctionOverloads.TryGetValue(syntax, out var overload) ? overload : null;
        }
        public Expression? GetMatchedFunctionResultValue(FunctionCallSyntaxBase syntax)
        {
            Visit(syntax);
            return matchedFunctionResultValues.TryGetValue(syntax, out var metadata) ? metadata : null;
        }

        private void AssignTypeWithCaching(SyntaxBase syntax, Func<TypeAssignment> assignFunc) =>
            assignedTypes.GetOrAdd(syntax, key =>
                CheckForCyclicError(key) is { } cyclicErrorType
                    ? new TypeAssignment(cyclicErrorType)
                    : assignFunc());

        private void AssignType(SyntaxBase syntax, Func<ITypeReference> assignFunc)
            => AssignTypeWithCaching(syntax, () => new TypeAssignment(assignFunc()));

        private void AssignTypeWithDiagnostics(SyntaxBase syntax, Func<IDiagnosticWriter, ITypeReference> assignFunc)
            => AssignTypeWithCaching(syntax, () =>
            {
                var diagnosticWriter = ToListDiagnosticWriter.Create();
                var reference = assignFunc(diagnosticWriter);

                return new TypeAssignment(reference, diagnosticWriter.GetDiagnostics());
            });

        private TypeSymbol? CheckForCyclicError(SyntaxBase syntax)
        {
            if (this.binder.GetSymbolInfo(syntax) is not DeclaredSymbol declaredSymbol)
            {
                return null;
            }

            if (declaredSymbol.DeclaringSyntax == syntax)
            {
                // Report cycle errors on accesses to cyclic symbols, not on the declaration itself
                return null;
            }

            if (this.binder.TryGetCycle(declaredSymbol) is { } cycle)
            {
                // there's a cycle. stop visiting now or we never will!

                if (!cycle.Any(symbol => this.binder.IsDescendant(syntax, symbol.DeclaringSyntax)))
                {
                    // the supplied syntax is not part of the cycle, just a reference to the cyclic symbol.
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).ReferencedSymbolHasErrors(declaredSymbol.Name));
                }

                if (cycle.Length == 1)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).CyclicExpressionSelfReference());
                }

                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).CyclicExpression(cycle.Select(x => x.Name)));
            }

            return null;
        }

        public override void VisitIfConditionSyntax(IfConditionSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                diagnostics.WriteMultiple(this.ValidateIfCondition(syntax));
                return this.typeManager.GetTypeInfo(syntax.Body);
            });

        public override void VisitLocalVariableSyntax(LocalVariableSyntax syntax)
            => AssignType(syntax, () =>
            {
                // local function
                ITypeReference GetItemType(ForSyntax @for)
                {
                    // get type of the loop array expression
                    // (this shouldn't cause a stack overflow because it's a peer node of this one)
                    var arrayExpressionType = this.typeManager.GetTypeInfo(@for.Expression);

                    if (arrayExpressionType.TypeKind == TypeKind.Any || arrayExpressionType is not ArrayType arrayType)
                    {
                        // the array is of "any" type or the loop array expression isn't actually an array
                        // in the former case, there isn't much we can do
                        // in the latter case, we will let the ForSyntax type check rules produce the error for it
                        return LanguageConstants.Any;
                    }

                    // the array expression is actually an array
                    return arrayType.Item;
                }

                var symbol = this.binder.GetSymbolInfo(syntax);
                if (symbol is not LocalVariableSymbol localVariableSymbol)
                {
                    throw new InvalidOperationException($"{syntax.GetType().Name} is bound to unexpected type '{symbol?.GetType().Name}'.");
                }

                var parent = this.binder.GetParent(syntax);


                switch (localVariableSymbol.LocalKind)
                {
                    case LocalKind.ForExpressionItemVariable:
                        // this local variable is a loop item variable
                        // we should return item type of the array (if feasible)
                        var @for = parent switch
                        {
                            ForSyntax forParent => forParent,
                            VariableBlockSyntax block when this.binder.GetParent(block) is ForSyntax forParent => forParent,
                            _ => throw new InvalidOperationException($"{syntax.GetType().Name} at {syntax.Span} has an unexpected parent of type {parent?.GetType().Name}")
                        };

                        return GetItemType(@for);

                    case LocalKind.ForExpressionIndexVariable:
                        // the local variable is an index variable
                        // index variables are always of type int
                        return LanguageConstants.Int;

                    case LocalKind.LambdaItemVariable:
                        var (lambda, argumentIndex) = parent switch
                        {
                            LambdaSyntax lambdaSyntax => (lambdaSyntax, 0),
                            VariableBlockSyntax block when this.binder.GetParent(block) is LambdaSyntax lambdaSyntax => (lambdaSyntax, block.Arguments.IndexOf(syntax)),
                            _ => throw new InvalidOperationException($"{syntax.GetType().Name} at {syntax.Span} has an unexpected parent of type {parent?.GetType().Name}"),
                        };

                        if (binder.GetParent(lambda) is {} lambdaParent &&
                            typeManager.GetDeclaredType(lambdaParent) is LambdaType lambdaType &&
                            argumentIndex < lambdaType.ArgumentTypes.Length)
                        {
                            return lambdaType.ArgumentTypes[argumentIndex];
                        }

                        return LanguageConstants.Any;

                    default:
                        throw new InvalidOperationException($"Unexpected local kind '{localVariableSymbol.LocalKind}'.");
                }
            });

        public override void VisitTypedLocalVariableSyntax(TypedLocalVariableSyntax syntax)
            => AssignType(syntax, () =>
            {
                if (typeManager.GetDeclaredType(syntax) is not {} declaredType)
                {
                    return ErrorType.Empty();
                }

                return declaredType;
            });

        public override void VisitForSyntax(ForSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var errors = new List<ErrorDiagnostic>();

                if (syntax.ItemVariable is null)
                {
                    // we don't have an item variable due to parse errors
                    // no need to add additional errors
                    return ErrorType.Empty();
                }

                var loopItemType = typeManager.GetTypeInfo(syntax.ItemVariable);
                CollectErrors(errors, loopItemType);

                var arrayExpressionType = typeManager.GetTypeInfo(syntax.Expression);
                CollectErrors(errors, arrayExpressionType);

                var bodyType = typeManager.GetTypeInfo(syntax.Body);
                CollectErrors(errors, bodyType);

                if (PropagateErrorType(errors, loopItemType, arrayExpressionType, bodyType))
                {
                    return ErrorType.Create(errors);
                }

                if (!TypeValidator.AreTypesAssignable(arrayExpressionType, LanguageConstants.Array))
                {
                    var builder = DiagnosticBuilder.ForPosition(syntax.Expression);
                    if (TypeHelper.WouldBeAssignableIfNonNullable(arrayExpressionType, LanguageConstants.Array, out var nonNullable))
                    {
                        diagnostics.Write(builder.PossibleNullReferenceAssignment(LanguageConstants.Array, arrayExpressionType, syntax.Expression));
                    }
                    else
                    {
                        // the array expression isn't actually an array
                        return ErrorType.Create(builder.LoopArrayExpressionTypeMismatch(arrayExpressionType));
                    }
                }

                if (PropagateErrorType(errors, loopItemType, arrayExpressionType, bodyType))
                {
                    return ErrorType.Create(errors);
                }

                // the return type of a loop is the array of the body type
                return new TypedArrayType(bodyType, TypeSymbolValidationFlags.Default);
            });

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax);
                if (declaredType is null)
                {
                    return ErrorType.Empty();
                }

                var singleDeclaredType = declaredType.UnwrapArrayType();
                this.ValidateDecorators(syntax.Decorators, declaredType, diagnostics);

                if (singleDeclaredType is ErrorType)
                {
                    return singleDeclaredType;
                }

                if (singleDeclaredType is ResourceType resourceType)
                {
                    if (!resourceType.DeclaringNamespace.ResourceTypeProvider.HasDefinedType(resourceType.TypeReference))
                    {
                        // TODO move into Az extension
                        var typeSegments = resourceType.TypeReference.TypeSegments;

                        if (resourceType.DeclaringNamespace.ProviderName == AzNamespaceType.BuiltInName &&
                            typeSegments.Length > 2 &&
                            typeSegments.Where((type, i) => i > 1 && i < (typeSegments.Length - 1) && StringComparer.OrdinalIgnoreCase.Equals(type, "providers")).Any())
                        {
                            // Special check for (<type>/)+providers(/<type>)+
                            // This indicates someone is trying to deploy an extension resource without using the 'scope' property.
                            // We should instead point them towards documentation on the 'scope' property.
                            diagnostics.Write(syntax.Type, x => x.ResourceTypeContainsProvidersSegment());
                        }
                        else
                        {
                            diagnostics.Write(syntax.Type, x => x.ResourceTypesUnavailable(resourceType.TypeReference));
                        }
                    }

                    if (!syntax.IsExistingResource() && resourceType.Flags.HasFlag(ResourceFlags.ReadOnly))
                    {
                        diagnostics.Write(syntax.Type, x => x.ResourceTypeIsReadonly(resourceType.TypeReference));
                    }
                }

                return TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, this.parsingErrorLookup, diagnostics, syntax.Value, declaredType, true);
            });

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax);
                if (declaredType is null)
                {
                    return ErrorType.Empty();
                }

                var singleDeclaredType = declaredType.UnwrapArrayType();

                this.ValidateDecorators(syntax.Decorators, declaredType, diagnostics);

                if (singleDeclaredType is ErrorType)
                {
                    return singleDeclaredType;
                }

                // We need to validate all of the parameters and outputs to make sure they are valid types.
                // This is where we surface errors for 'unknown' resource types.
                if (singleDeclaredType is ModuleType moduleType &&
                    moduleType.Body is ObjectType objectType)
                {
                    if (objectType.Properties.TryGetValue(LanguageConstants.ModuleParamsPropertyName, out var paramsProperty)
                        && paramsProperty.TypeReference.Type is ObjectType paramsType)
                    {
                        foreach (var property in paramsType.Properties.Values)
                        {
                            if (property.TypeReference.Type is ResourceParameterType resourceType)
                            {
                                if (!features.ResourceTypedParamsAndOutputsEnabled)
                                {
                                    diagnostics.Write(DiagnosticBuilder.ForPosition(syntax.Path).ParamOrOutputResourceTypeUnsupported());
                                }

                                if (!resourceType.DeclaringNamespace.ResourceTypeProvider.HasDefinedType(resourceType.TypeReference))
                                {
                                    diagnostics.Write(DiagnosticBuilder.ForPosition(syntax.Path).ModuleParamOrOutputResourceTypeUnavailable(resourceType.TypeReference));
                                }
                            }
                        }
                    }

                    if (objectType.Properties.TryGetValue(LanguageConstants.ModuleOutputsPropertyName, out var outputsProperty)
                        && outputsProperty.TypeReference.Type is ObjectType outputsType)
                    {
                        foreach (var property in outputsType.Properties.Values)
                        {
                            if (property.TypeReference.Type is ResourceType resourceType)
                            {
                                if (!features.ResourceTypedParamsAndOutputsEnabled)
                                {
                                    diagnostics.Write(DiagnosticBuilder.ForPosition(syntax.Path).ParamOrOutputResourceTypeUnsupported());
                                }

                                if (!resourceType.DeclaringNamespace.ResourceTypeProvider.HasDefinedType(resourceType.TypeReference))
                                {
                                    diagnostics.Write(DiagnosticBuilder.ForPosition(syntax.Path).ModuleParamOrOutputResourceTypeUnavailable(resourceType.TypeReference));
                                }
                            }
                        }
                    }
                }

                if (this.binder.GetSymbolInfo(syntax) is ModuleSymbol moduleSymbol &&
                    moduleSymbol.TryGetSemanticModel(out var moduleSemanticModel, out var _) &&
                    moduleSemanticModel.HasErrors())
                {
                    diagnostics.Write(moduleSemanticModel is ArmTemplateSemanticModel
                        ? DiagnosticBuilder.ForPosition(syntax.Path).ReferencedArmTemplateHasErrors()
                        : DiagnosticBuilder.ForPosition(syntax.Path).ReferencedModuleHasErrors());
                }


                return TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, this.parsingErrorLookup, diagnostics, syntax.Value, declaredType);
            });

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = GetDeclaredTypeAndValidateDecorators(syntax, syntax.Type, diagnostics);

                base.VisitParameterDeclarationSyntax(syntax);

                if (syntax.Modifier != null)
                {
                    diagnostics.WriteMultiple(this.ValidateIdentifierAccess(syntax.Modifier));

                    if (TypeValidator.AreTypesAssignable(LanguageConstants.Null, declaredType))
                    {
                        diagnostics.Write(DiagnosticBuilder.ForPosition(syntax.Modifier).NullableTypedParamsMayNotHaveDefaultValues());
                    }
                }

                if (declaredType is ErrorType)
                {
                    return declaredType;
                }

                var allowedDecoratorSyntax = GetNamedDecorator(syntax, LanguageConstants.ParameterAllowedPropertyName);

                var assignedType = allowedDecoratorSyntax?.Arguments.Single().Expression is ArraySyntax allowedValuesSyntax
                    ? syntax.GetAssignedType(this.typeManager, allowedValuesSyntax)
                    : syntax.GetAssignedType(this.typeManager, null);

                if (GetNamedDecorator(syntax, LanguageConstants.ParameterSecurePropertyName) is not null)
                {
                    if (ReferenceEquals(assignedType, LanguageConstants.LooseString))
                    {
                        assignedType = LanguageConstants.SecureString;
                    }
                    else if (ReferenceEquals(assignedType, LanguageConstants.Object))
                    {
                        assignedType = LanguageConstants.SecureObject;
                    }
                }

                switch (syntax.Modifier)
                {
                    case ParameterDefaultValueSyntax defaultValueSyntax:
                        diagnostics.WriteMultiple(ValidateDefaultValue(defaultValueSyntax, assignedType));
                        break;
                }

                return assignedType;
            });

        public override void VisitParameterAssignmentSyntax(ParameterAssignmentSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var errors = new List<ErrorDiagnostic>();

                var valueType = this.typeManager.GetTypeInfo(syntax.Value);
                CollectErrors(errors, valueType);

                if (PropagateErrorType(errors, valueType))
                {
                    return ErrorType.Create(errors);
                }

                return valueType;
            });

        public override void VisitTypeDeclarationSyntax(TypeDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax);

                if (LanguageConstants.ReservedTypeNames.Contains(syntax.Name.IdentifierName))
                {
                    diagnostics.Write(DiagnosticBuilder.ForPosition(syntax.Name).ReservedTypeName(syntax.Name.IdentifierName));
                }

                base.VisitTypeDeclarationSyntax(syntax);

                diagnostics.WriteMultiple(declaredType?.GetDiagnostics() ?? Enumerable.Empty<IDiagnostic>());

                if (declaredType is not null)
                {
                    ValidateDecorators(syntax.Decorators,
                        declaredType is TypeType wrapped ? wrapped.Unwrapped : declaredType,
                        diagnostics);
                }

                return declaredType ?? ErrorType.Empty();
            });

        public override void VisitObjectTypeSyntax(ObjectTypeSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax);

                base.VisitObjectTypeSyntax(syntax);

                return declaredType ?? ErrorType.Empty();
            });

        public override void VisitObjectTypePropertySyntax(ObjectTypePropertySyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = GetDeclaredTypeAndValidateDecorators(syntax, syntax.Value, diagnostics);
                diagnostics.WriteMultiple(declaredType.GetDiagnostics());

                base.VisitObjectTypePropertySyntax(syntax);

                return declaredType;
            });

        public override void VisitObjectTypeAdditionalPropertiesSyntax(ObjectTypeAdditionalPropertiesSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = GetDeclaredTypeAndValidateDecorators(syntax, syntax.Value, diagnostics);
                diagnostics.WriteMultiple(declaredType.GetDiagnostics());

                base.VisitObjectTypeAdditionalPropertiesSyntax(syntax);

                return declaredType;
            });

        public override void VisitTupleTypeSyntax(TupleTypeSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax);

                base.VisitTupleTypeSyntax(syntax);

                return declaredType ?? ErrorType.Empty();
            });

        public override void VisitTupleTypeItemSyntax(TupleTypeItemSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = GetDeclaredTypeAndValidateDecorators(syntax, syntax.Value, diagnostics);
                diagnostics.WriteMultiple(declaredType.GetDiagnostics());

                base.VisitTupleTypeItemSyntax(syntax);

                return declaredType;
            });

        public override void VisitArrayTypeSyntax(ArrayTypeSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax);

                base.VisitArrayTypeSyntax(syntax);

                return declaredType ?? ErrorType.Empty();
            });

        public override void VisitArrayTypeMemberSyntax(ArrayTypeMemberSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax) ?? ErrorType.Empty();
                diagnostics.WriteMultiple(declaredType.GetDiagnostics());

                base.VisitArrayTypeMemberSyntax(syntax);

                return declaredType;
            });

        public override void VisitUnionTypeSyntax(UnionTypeSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax) ?? ErrorType.Empty();
                diagnostics.WriteMultiple(declaredType.GetDiagnostics());

                base.VisitUnionTypeSyntax(syntax);

                if (declaredType is DiscriminatedObjectType or ErrorType
                    || (declaredType is UnionType unionType && TypeHelper.TryRemoveNullability(unionType) is DiscriminatedObjectType))
                {
                    return declaredType;
                }

                var memberTypes = syntax.Members.Select(memberSyntax => (GetTypeInfo(memberSyntax), memberSyntax))
                    .SelectMany(t => FlattenUnionMemberType(t.Item1, t.memberSyntax))
                    .ToImmutableArray();

                switch (TypeHelper.CreateTypeUnion(memberTypes.Select(t => GetNonLiteralType(t.memberType)).WhereNotNull()))
                {
                    case UnionType union when TypeHelper.TryRemoveNullability(union) is {} nonNullable && LanguageConstants.DeclarationTypes.ContainsValue(nonNullable):
                        ValidateAllowedValuesUnion(union, memberTypes, diagnostics);
                        break;

                    case UnionType union when MightBeArrayAny(syntax) &&
                        union.Members.Any() &&
                        !union.Members.Any(m => ReferenceEquals(m.Type, LanguageConstants.Array)):
                        ValidateAllowedValuesSubsetUnion(memberTypes, diagnostics);
                        break;

                    case TypeSymbol ts when LanguageConstants.DeclarationTypes.ContainsValue(ts):
                        ValidateAllowedValuesUnion(ts, memberTypes, diagnostics);
                        break;

                    default:
                        diagnostics.Write(DiagnosticBuilder.ForPosition(syntax).InvalidTypeUnion());
                        break;
                }

                return declaredType;
            });

        private static IEnumerable<(TypeSymbol memberType, UnionTypeMemberSyntax memberSyntax)>
        FlattenUnionMemberType(ITypeReference memberType, UnionTypeMemberSyntax memberSyntax) => memberType.Type switch
        {
            UnionType union => union.Members.SelectMany(m => FlattenUnionMemberType(m, memberSyntax)),
            TypeSymbol otherwise => (otherwise, memberSyntax).AsEnumerable(),
        };

        private static TypeSymbol? GetNonLiteralType(TypeSymbol? type) => type switch {
            StringLiteralType => LanguageConstants.String,
            IntegerLiteralType => LanguageConstants.Int,
            BooleanLiteralType => LanguageConstants.Bool,
            ObjectType => LanguageConstants.Object,
            TupleType => LanguageConstants.Array,
            NullType => LanguageConstants.Null,
            _ => null,
        };

        private bool MightBeArrayAny(SyntaxBase syntax) => binder.GetParent(syntax) switch
        {
            ParenthesizedExpressionSyntax parenthesized => MightBeArrayAny(parenthesized),
            ArrayTypeMemberSyntax arrayTypeMember => MightBeArrayAny(arrayTypeMember),
            ArrayTypeSyntax => true,
            _ => false,
        };

        private static void ValidateAllowedValuesSubsetUnion(ImmutableArray<(TypeSymbol, UnionTypeMemberSyntax)> memberTypes, IDiagnosticWriter diagnostics)
        {
            foreach (var (memberType, memberSyntax) in memberTypes)
            {
                if (GetNonLiteralUnionMemberDiagnostic(memberType, memberSyntax) is {} diagnostic)
                {
                    diagnostics.Write(diagnostic);
                }
            }
        }

        private static void ValidateAllowedValuesUnion(TypeSymbol keystoneType, ImmutableArray<(TypeSymbol, UnionTypeMemberSyntax)> memberTypes, IDiagnosticWriter diagnostics)
        {
            foreach (var (memberType, memberSyntax) in memberTypes)
            {
                if (GetNonLiteralUnionMemberDiagnostic(memberType, memberSyntax) is {} diagnostic)
                {
                    diagnostics.Write(diagnostic);
                }
                else if (!TypeValidator.AreTypesAssignable(memberType, keystoneType))
                {
                    diagnostics.Write(DiagnosticBuilder.ForPosition(memberSyntax).InvalidUnionTypeMember(keystoneType.Name));
                }
            }
        }

        private static ErrorDiagnostic? GetNonLiteralUnionMemberDiagnostic(TypeSymbol memberType, UnionTypeMemberSyntax memberSyntax)
            => TypeHelper.IsLiteralType(memberType) ? null : DiagnosticBuilder.ForPosition(memberSyntax).NonLiteralUnionMember();

        public override void VisitUnionTypeMemberSyntax(UnionTypeMemberSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax) ?? ErrorType.Empty();

                base.VisitUnionTypeMemberSyntax(syntax);

                return declaredType;
            });

        public override void VisitResourceTypeSyntax(ResourceTypeSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax);
                if (declaredType is null)
                {
                    return ErrorType.Empty();
                }

                // If the resource type was explicitly specified, emit a warning if no types can be found
                if (syntax.Type is {} explicitResourceType && declaredType is ResourceType resourceType && !resourceType.DeclaringNamespace.ResourceTypeProvider.HasDefinedType(resourceType.TypeReference))
                {
                    diagnostics.Write(DiagnosticBuilder.ForPosition(explicitResourceType).ResourceTypesUnavailable(resourceType.TypeReference));
                }

                return declaredType;
            });

        private TypeSymbol GetDeclaredTypeAndValidateDecorators(DecorableSyntax targetSyntax, SyntaxBase typeSyntax, IDiagnosticWriter diagnostics)
        {
            var declaredType = typeManager.GetDeclaredType(targetSyntax);
            if (declaredType is null)
            {
                return ErrorType.Empty();
            }

            this.ValidateDecorators(targetSyntax.Decorators, declaredType, diagnostics);

            return declaredType;
        }

        public override void VisitProviderDeclarationSyntax(ProviderDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                if (binder.GetSymbolInfo(syntax) is not ProviderNamespaceSymbol namespaceSymbol)
                {
                    // We have syntax or binding errors, which should have already been handled.
                    return ErrorType.Empty();
                }

                if (namespaceSymbol.DeclaredType is not NamespaceType namespaceType)
                {
                    // We should have an error type here - return it directly.
                    return namespaceSymbol.DeclaredType as ErrorType ?? ErrorType.Empty();
                }

                this.ValidateDecorators(syntax.Decorators, namespaceType, diagnostics);

                if (syntax.Config is not null)
                {
                    if (namespaceType.ConfigurationType is null)
                    {
                        diagnostics.Write(syntax.Config, x => x.ImportProviderDoesNotSupportConfiguration(namespaceType.ProviderName));
                    }
                    else
                    {
                        // Collect diagnostics for the configuration type assignment.
                        TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, this.parsingErrorLookup, diagnostics, syntax.Config, namespaceType.ConfigurationType.Type, false);
                    }
                }
                else
                {
                    if (syntax.WithClause.IsSkipped &&
                        namespaceType.ConfigurationType is not null &&
                        namespaceType.ConfigurationType.Properties.Values.Any(x => x.Flags.HasFlag(TypePropertyFlags.Required)))
                    {
                        diagnostics.Write(syntax, x => x.ImportProviderRequiresConfiguration(namespaceType.ProviderName));
                    }
                }

                return namespaceType;
            });

        private void ValidateDecorators(IEnumerable<DecoratorSyntax> decoratorSyntaxes, TypeSymbol targetType, IDiagnosticWriter diagnostics)
        {
            var decoratorSyntaxesByMatchingDecorator = new Dictionary<Decorator, List<DecoratorSyntax>>();

            foreach (var decoratorSyntax in decoratorSyntaxes)
            {
                var decoratorType = this.typeManager.GetTypeInfo(decoratorSyntax.Expression);

                if (decoratorType is ErrorType)
                {
                    diagnostics.WriteMultiple(decoratorType.GetDiagnostics());
                    continue;
                }

                foreach (var argumentSyntax in decoratorSyntax.Arguments)
                {
                    TypeValidator.GetCompileTimeConstantViolation(argumentSyntax, diagnostics);
                }

                var symbol = this.binder.GetSymbolInfo(decoratorSyntax.Expression);

                if (symbol is DeclaredSymbol or INamespaceSymbol)
                {
                    diagnostics.Write(DiagnosticBuilder.ForPosition(decoratorSyntax.Expression).ExpressionNotCallable());
                    continue;
                }

                if (this.binder.GetSymbolInfo(decoratorSyntax.Expression) is FunctionSymbol functionSymbol &&
                    functionSymbol.DeclaringObject is NamespaceType namespaceType)
                {
                    var argumentTypes = this.GetRecoveredArgumentTypes(decoratorSyntax.Arguments).ToArray();

                    // There should exist exact one matching decorator if there's no argument mismatches,
                    // since each argument must be a compile-time constant which cannot be of Any type.
                    var decorator = namespaceType.DecoratorResolver.GetMatches(functionSymbol, argumentTypes)
                        .SingleOrDefault();

                    if (decorator is not null)
                    {
                        if (decoratorSyntaxesByMatchingDecorator.TryGetValue(decorator, out var duplicateDecoratorSyntaxes))
                        {
                            duplicateDecoratorSyntaxes.Add(decoratorSyntax);
                        }
                        else
                        {
                            decoratorSyntaxesByMatchingDecorator[decorator] = new List<DecoratorSyntax> { decoratorSyntax };
                        }

                        decorator.Validate(decoratorSyntax, targetType, this.typeManager, this.binder, this.parsingErrorLookup, diagnostics);
                    }
                }
            }

            foreach (var (decorator, duplicateDecoratorSyntaxes) in decoratorSyntaxesByMatchingDecorator.Where(x => x.Value.Count > 1))
            {
                foreach (var decoratorSyntax in duplicateDecoratorSyntaxes)
                {
                    diagnostics.Write(DiagnosticBuilder.ForPosition(decoratorSyntax).DuplicateDecorator(decorator.Overload.Name));
                }
            }
        }

        public override void VisitMetadataDeclarationSyntax(MetadataDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var errors = new List<ErrorDiagnostic>();

                var valueType = typeManager.GetTypeInfo(syntax.Value);
                CollectErrors(errors, valueType);

                if (PropagateErrorType(errors, valueType))
                {
                    return ErrorType.Create(errors);
                }

                if (TypeValidator.AreTypesAssignable(valueType, LanguageConstants.Any) != true)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Value).VariableTypeAssignmentDisallowed(valueType));
                }

                ValidateDecorators(syntax.Decorators, valueType, diagnostics);
                TypeValidator.GetCompileTimeConstantViolation(syntax.Value, diagnostics);

                return valueType;
            });

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var errors = new List<ErrorDiagnostic>();

                var valueType = typeManager.GetTypeInfo(syntax.Value);
                CollectErrors(errors, valueType);

                if (PropagateErrorType(errors, valueType))
                {
                    return ErrorType.Create(errors);
                }

                if (TypeValidator.AreTypesAssignable(valueType, LanguageConstants.Any) != true)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Value).VariableTypeAssignmentDisallowed(valueType));
                }

                this.ValidateDecorators(syntax.Decorators, valueType, diagnostics);

                return valueType;
            });

        public override void VisitFunctionDeclarationSyntax(FunctionDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var errors = new List<ErrorDiagnostic>();

                var lambdaType = typeManager.GetTypeInfo(syntax.Lambda);
                CollectErrors(errors, lambdaType);

                if (PropagateErrorType(errors, lambdaType))
                {
                    return ErrorType.Create(errors);
                }

                this.ValidateDecorators(syntax.Decorators, lambdaType, diagnostics);

                return lambdaType;
            });

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = GetDeclaredTypeAndValidateDecorators(syntax, syntax.Type, diagnostics);
                diagnostics.WriteMultiple(GetOutputDeclarationDiagnostics(declaredType, syntax));

                base.VisitOutputDeclarationSyntax(syntax);

                return declaredType;
            });

        public override void VisitAssertDeclarationSyntax(AssertDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var errors = new List<ErrorDiagnostic>();

                var valueType = typeManager.GetTypeInfo(syntax.Value);
                CollectErrors(errors, valueType);

                if (PropagateErrorType(errors, valueType))
                {
                    return ErrorType.Create(errors);
                }

                if (!TypeValidator.AreTypesAssignable(valueType, LanguageConstants.Bool))
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Value).InvalidAssertAssignment(valueType));
                }

                this.ValidateDecorators(syntax.Decorators, valueType, diagnostics);

                base.VisitAssertDeclarationSyntax(syntax);

                return LanguageConstants.Bool;
            });

        public override void VisitCompileTimeImportDeclarationSyntax(CompileTimeImportDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                if (!features.CompileTimeImportsEnabled)
                {
                    diagnostics.Write(DiagnosticBuilder.ForPosition(syntax).CompileTimeImportsNotSupported());
                    return ErrorType.Empty();
                }

                base.VisitCompileTimeImportDeclarationSyntax(syntax);

                return GetTypeInfo(syntax.ImportExpression);
            });

        private ImmutableDictionary<string, TypeTypeProperty>? GetImportableTypesForDeclaration(CompileTimeImportDeclarationSyntax syntax, IDiagnosticWriter diagnostics)
        {
            if (!SemanticModelHelper.TryGetSemanticModelForForeignTemplateReference(sourceFileLookup,
                syntax,
                b => b.CompileTimeImportDeclarationMustReferenceTemplate(),
                semanticModelLookup,
                out var semanticModel,
                out var failureDiagnostic))
            {
                diagnostics.Write(failureDiagnostic);
                return null;
            }

            if (semanticModel.HasErrors())
            {
                diagnostics.Write(semanticModel is ArmTemplateSemanticModel
                    ? DiagnosticBuilder.ForPosition(syntax.FromClause).ReferencedArmTemplateHasErrors()
                    : DiagnosticBuilder.ForPosition(syntax.FromClause).ReferencedModuleHasErrors());
            }

            return semanticModel.ExportedTypes.Select(kvp => new TypeTypeProperty(kvp.Key,
                kvp.Value.TypeReference.Type switch
                {
                    TypeType tt => tt,
                    TypeSymbol otherwise => new TypeType(otherwise),
                },
                TypePropertyFlags.ReadOnly | TypePropertyFlags.Required,
                kvp.Value.Description))
                .ToImmutableDictionary(ttp => ttp.Name);
        }

        public override void VisitWildcardImportSyntax(WildcardImportSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                base.VisitWildcardImportSyntax(syntax);

                if (binder.GetParent(syntax) is not CompileTimeImportDeclarationSyntax importDeclarationSyntax ||
                    importableTypesByCompileTimeImportDeclaration.GetOrAdd(importDeclarationSyntax, d => GetImportableTypesForDeclaration(d, diagnostics)) is not {} importableTypes)
                {
                    return ErrorType.Empty();
                }

                return CreateNamespace(syntax.Name.IdentifierName, importableTypes.Values);
            });

        private static NamespaceType CreateNamespace(string namespaceName, IEnumerable<TypeTypeProperty> typesInNamespace)
        {
            NamespaceSettings settings = new(
                IsSingleton: true,
                BicepProviderName: namespaceName,
                ConfigurationType: null,
                ArmTemplateProviderName: namespaceName,
                ArmTemplateProviderVersion: "1.0.0");

            return new NamespaceType(namespaceName,
                settings,
                typesInNamespace,
                ImmutableArray<FunctionOverload>.Empty,
                ImmutableArray<BannedFunction>.Empty,
                ImmutableArray<Decorator>.Empty,
                new EmptyResourceTypeProvider());
        }

        public override void VisitImportedSymbolsListSyntax(ImportedSymbolsListSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                base.VisitImportedSymbolsListSyntax(syntax);

                if (binder.GetParent(syntax) is not CompileTimeImportDeclarationSyntax importDeclarationSyntax ||
                    importableTypesByCompileTimeImportDeclaration.GetOrAdd(importDeclarationSyntax, d => GetImportableTypesForDeclaration(d, diagnostics)) is not {} importableTypes)
                {
                    return ErrorType.Empty();
                }

                // Unlike a wildcard import, the intermediate syntax node surrounding the symbols declared by a statement like `import {foo, bar, baz} from 'main.bicep'`
                // doesn't declare a single symbol with a single type. For simplicity's sake, we'll assign a NamespaceType (albeit an unreferenceable one)
                return CreateNamespace(Guid.NewGuid().ToString(), importableTypes.Values);
            });

        public override void VisitImportedSymbolsListItemSyntax(ImportedSymbolsListItemSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                base.VisitImportedSymbolsListItemSyntax(syntax);

                if (binder.GetParent(syntax) is not {} parentSyntax ||
                    binder.GetParent(parentSyntax) is not CompileTimeImportDeclarationSyntax importDeclarationSyntax ||
                    importableTypesByCompileTimeImportDeclaration.GetOrAdd(importDeclarationSyntax, d => GetImportableTypesForDeclaration(d, diagnostics)) is not {} importableTypes)
                {
                    return ErrorType.Empty();
                }

                if (importableTypes.TryGetValue(syntax.OriginalSymbolName.IdentifierName) is not {} exportedType)
                {
                    diagnostics.Write(DiagnosticBuilder.ForPosition(syntax.OriginalSymbolName).ImportedSymbolNotFound(syntax.OriginalSymbolName.IdentifierName));
                    return ErrorType.Empty();
                }

                return exportedType.TypeReference;
            });

        public override void VisitBooleanLiteralSyntax(BooleanLiteralSyntax syntax)
            => AssignType(syntax, () => syntax.Value ? LanguageConstants.True : LanguageConstants.False);

        public override void VisitStringSyntax(StringSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                if (syntax.TryGetLiteralValue() is string literalValue)
                {
                    // uninterpolated strings have a known type
                    return TypeFactory.CreateStringLiteralType(literalValue);
                }

                var errors = new List<ErrorDiagnostic>();
                var expressionTypes = new List<TypeSymbol>();
                long minLength = syntax.SegmentValues.Sum(s => s.Length);
                long? maxLength = minLength;

                foreach (var interpolatedExpression in syntax.Expressions)
                {
                    var expressionType = typeManager.GetTypeInfo(interpolatedExpression);
                    CollectErrors(errors, expressionType);
                    expressionTypes.Add(expressionType);

                    (long expressionMinLength, long? expressionMaxLength) = TypeHelper.GetMinAndMaxLengthOfStringified(expressionType);
                    minLength += expressionMinLength;
                    if (maxLength.HasValue && expressionMaxLength.HasValue)
                    {
                        maxLength += expressionMaxLength.Value;
                    }
                    else
                    {
                        maxLength = null;
                    }
                }

                if (PropagateErrorType(errors, expressionTypes))
                {
                    return ErrorType.Create(errors);
                }

                // if the value of this string expression can be determined at compile time, use that
                if (ArmFunctionReturnTypeEvaluator.TryEvaluate("format", out _, TypeFactory.CreateStringLiteralType(StringFormatConverter.BuildFormatString(syntax.SegmentValues)).AsEnumerable().Concat(expressionTypes)) is {} folded)
                {
                    return folded;
                }

                // normally we would also do an assignability check, but we allow "any" type in string interpolation expressions
                // so the assignability check cannot possibly fail (we already collected type errors from the inner expressions at this point)
                return TypeFactory.CreateStringType(maxLength: maxLength, minLength: minLength switch
                {
                    <= 0 => null,
                    _ => minLength,
                });
            });

        public override void VisitIntegerLiteralSyntax(IntegerLiteralSyntax syntax)
            => AssignType(syntax, () => syntax.Value switch {
                <= long.MaxValue => TypeFactory.CreateIntegerLiteralType((long)syntax.Value),
                _ => LanguageConstants.Int,
            });

        public override void VisitNullLiteralSyntax(NullLiteralSyntax syntax)
            => AssignType(syntax, () => LanguageConstants.Null);

        public override void VisitSkippedTriviaSyntax(SkippedTriviaSyntax syntax)
            => AssignType(syntax, () =>
            {
                // error should have already been raised by the ParseDiagnosticsVisitor - no need to add another
                return ErrorType.Create(Enumerable.Empty<ErrorDiagnostic>());
            });

        public override void VisitObjectSyntax(ObjectSyntax syntax)
            => AssignType(syntax, () =>
            {
                var errors = new List<ErrorDiagnostic>();

                var duplicatedProperties = syntax.Properties
                    .GroupByExcludingNull(prop => prop.TryGetKeyText(), LanguageConstants.IdentifierComparer)
                    .Where(group => group.Count() > 1);

                foreach (var group in duplicatedProperties)
                {
                    foreach (ObjectPropertySyntax duplicatedProperty in group)
                    {
                        errors.Add(DiagnosticBuilder.ForPosition(duplicatedProperty.Key).PropertyMultipleDeclarations(group.Key));
                    }
                }

                var propertyTypes = new List<TypeSymbol>();
                foreach (var objectProperty in syntax.Properties)
                {
                    var propertyType = typeManager.GetTypeInfo(objectProperty);
                    CollectErrors(errors, propertyType);
                    propertyTypes.Add(propertyType);
                }

                if (PropagateErrorType(errors, propertyTypes))
                {
                    return ErrorType.Create(errors);
                }

                // Discriminated objects should have been resolved by the declared type manager.
                var declaredType = typeManager.GetDeclaredType(syntax);

                // type results are cached
                var namedProperties = syntax.Properties
                    .GroupByExcludingNull(p => p.TryGetKeyText(), LanguageConstants.IdentifierComparer)
                    .Select(group =>
                    {
                        var resolvedType = TypeHelper.CreateTypeUnion(group.Select(p => typeManager.GetTypeInfo(p)));

                        if (declaredType is ObjectType objectType && objectType.Properties.TryGetValue(group.Key, out var property))
                        {
                            // we've found a declared object type for the containing object, with a matching property name definition.
                            // preserve the type property details (name, descriptions etc.), and update the assigned type.
                            return new TypeProperty(property.Name, resolvedType, property.Flags, property.Description);
                        }

                        // we've not been able to find a declared object type for the containing object, or it doesn't contain a property matching this one.
                        // best we can do is to simply generate a property for the assigned type.
                        return new TypeProperty(group.Key, resolvedType);
                    });

                var additionalProperties = syntax.Properties
                    .Where(p => p.TryGetKeyText() is null)
                    .Select(p => typeManager.GetTypeInfo(p));

                var additionalPropertiesType = additionalProperties.Any() ? TypeHelper.CreateTypeUnion(additionalProperties) : null;

                // TODO: Add structural naming?
                return new ObjectType(LanguageConstants.Object.Name, TypeSymbolValidationFlags.Default, namedProperties, additionalPropertiesType);
            });

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
            => AssignType(syntax, () =>
            {
                var errors = new List<ErrorDiagnostic>();
                var types = new List<TypeSymbol>();

                if (syntax.Key is StringSyntax stringSyntax && stringSyntax.IsInterpolated())
                {
                    // if the key is an interpolated string, we need to check the expressions referenced by it
                    var keyType = typeManager.GetTypeInfo(syntax.Key);
                    CollectErrors(errors, keyType);
                    types.Add(keyType);
                }

                var valueType = typeManager.GetTypeInfo(syntax.Value);

                CollectErrors(errors, valueType);

                if (PropagateErrorType(errors, types.Concat(valueType)))
                {
                    valueType = ErrorType.Create(errors);
                }
                return valueType;
            });

        public override void VisitArrayItemSyntax(ArrayItemSyntax syntax)
            => AssignType(syntax, () => typeManager.GetTypeInfo(syntax.Value));

        public override void VisitParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax)
            => AssignType(syntax, () => typeManager.GetTypeInfo(syntax.Expression));

        public override void VisitFunctionArgumentSyntax(FunctionArgumentSyntax syntax)
            => AssignType(syntax, () => typeManager.GetTypeInfo(syntax.Expression));

        public override void VisitArraySyntax(ArraySyntax syntax)
            => AssignType(syntax, () =>
            {
                var errors = new List<ErrorDiagnostic>();

                var itemTypes = new List<TypeSymbol>(syntax.Children.Length);
                TupleTypeNameBuilder typeName = new();
                foreach (var arrayItem in syntax.Items)
                {
                    var itemType = typeManager.GetTypeInfo(arrayItem);
                    itemTypes.Add(itemType);
                    typeName.AppendItem(itemType.Name);
                    CollectErrors(errors, itemType);
                }

                if (PropagateErrorType(errors, itemTypes))
                {
                    return ErrorType.Create(errors);
                }

                return new TupleType(typeName.ToString(), itemTypes.ToImmutableArray<ITypeReference>(), TypeSymbolValidationFlags.Default);
            });

        public override void VisitTernaryOperationSyntax(TernaryOperationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var errors = new List<ErrorDiagnostic>();

                // ternary operator requires the condition to be of bool type
                var conditionType = typeManager.GetTypeInfo(syntax.ConditionExpression);
                CollectErrors(errors, conditionType);

                var trueType = typeManager.GetTypeInfo(syntax.TrueExpression);
                CollectErrors(errors, trueType);

                var falseType = typeManager.GetTypeInfo(syntax.FalseExpression);
                CollectErrors(errors, falseType);

                if (PropagateErrorType(errors, conditionType, trueType, falseType))
                {
                    return ErrorType.Create(errors);
                }

                var expectedConditionType = LanguageConstants.Bool;
                // if the condition is nullable, emit a fixable warning and proceed as if it had been non-nullable
                if (TypeHelper.WouldBeAssignableIfNonNullable(conditionType, expectedConditionType, out var nonNullableConditionType))
                {
                    diagnostics.Write(DiagnosticBuilder.ForPosition(syntax.ConditionExpression)
                        .PossibleNullReferenceAssignment(expectedConditionType, conditionType, syntax.ConditionExpression));
                    conditionType = nonNullableConditionType;
                }

                if (TypeValidator.AreTypesAssignable(conditionType, expectedConditionType) != true)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.ConditionExpression).ValueTypeMismatch(expectedConditionType));
                }

                // TODO if the condition is of a boolean literal type, return either `trueType` or `falseType`, not the union of both

                // the return type is the union of true and false expression types
                return TypeHelper.CreateTypeUnion(trueType, falseType);
            });

        public override void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var errors = new List<ErrorDiagnostic>();

                var operandType1 = typeManager.GetTypeInfo(syntax.LeftExpression);
                CollectErrors(errors, operandType1);

                var operandType2 = typeManager.GetTypeInfo(syntax.RightExpression);
                CollectErrors(errors, operandType2);

                if (PropagateErrorType(errors, operandType1, operandType2))
                {
                    return ErrorType.Create(errors);
                }

                // operands don't appear to have errors
                // let's fold the expression so that an operation with two literal typed operands will have a literal return type
                if (OperationReturnTypeEvaluator.TryFoldBinaryExpression(syntax, operandType1, operandType2, diagnostics) is {} result)
                {
                    return result;
                }

                string? additionalInfo = null;
                if (TypeValidator.AreTypesAssignable(operandType1, LanguageConstants.String) &&
                    TypeValidator.AreTypesAssignable(operandType2, LanguageConstants.String) &&
                    syntax.Operator is BinaryOperator.Add)
                {
                    additionalInfo = DiagnosticBuilder.UseStringInterpolationInsteadClause;
                }

                // we do not have a match
                // operand types didn't match available operators
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).BinaryOperatorInvalidType(syntax.OperatorToken.Text, operandType1, operandType2, additionalInfo: additionalInfo));
            });

        public override void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var errors = new List<ErrorDiagnostic>();

                var operandType = typeManager.GetTypeInfo(syntax.Expression);
                CollectErrors(errors, operandType);

                if (PropagateErrorType(errors, operandType))
                {
                    return ErrorType.Create(errors);
                }

                // operand doesn't appear to have errors
                // let's fold the expression so that an operation with a literal typed operand will have a literal return type
                if (OperationReturnTypeEvaluator.TryFoldUnaryExpression(syntax, operandType, diagnostics) is {} result)
                {
                    return result;
                }

                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).UnaryOperatorInvalidType(Operators.UnaryOperatorToText[syntax.Operator], operandType));
            });

        private static bool PropagateErrorType(IEnumerable<ErrorDiagnostic> errors, params TypeSymbol[] types)
            => PropagateErrorType(errors, types as IEnumerable<TypeSymbol>);

        private static bool PropagateErrorType(IEnumerable<ErrorDiagnostic> errors, IEnumerable<TypeSymbol> types)
        {
            if (errors.Any())
            {
                return true;
            }

            return types.Any(x => x.TypeKind == TypeKind.Error);
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics => GetAccessedType(syntax, diagnostics));

        private static TypeSymbol GetArrayItemType(ArrayAccessSyntax syntax, IDiagnosticWriter diagnostics, TypeSymbol baseType, TypeSymbol indexType)
        {
            var errors = new List<ErrorDiagnostic>();
            CollectErrors(errors, baseType);
            CollectErrors(errors, indexType);

            if (PropagateErrorType(errors, baseType, indexType))
            {
                return ErrorType.Create(errors);
            }

            baseType = UnwrapType(baseType);

            // if the index type is nullable but otherwise valid, emit a fixable warning
            if (TypeHelper.TryRemoveNullability(indexType) is {} nonNullableIndex)
            {
                var withNonNullableIndex = GetArrayItemType(syntax, diagnostics, baseType, nonNullableIndex);

                if (withNonNullableIndex is not ErrorType)
                {
                    diagnostics.Write(DiagnosticBuilder.ForPosition(syntax.IndexExpression).PossibleNullReferenceAssignment(nonNullableIndex, indexType, syntax.IndexExpression));
                }

                return withNonNullableIndex;
            }

            static TypeSymbol GetTypeAtIndex(TupleType baseType, IntegerLiteralType indexType, SyntaxBase indexSyntax) => indexType.Value switch
            {
                < 0 => ErrorType.Create(DiagnosticBuilder.ForPosition(indexSyntax).IndexOutOfBounds(baseType.Name, baseType.Items.Length, indexType.Value)),
                long value when value >= baseType.Items.Length => ErrorType.Create(DiagnosticBuilder.ForPosition(indexSyntax).IndexOutOfBounds(baseType.Name, baseType.Items.Length, value)),
                // unlikely to hit this given that we've established that the tuple has a item at the given position
                > int.MaxValue => ErrorType.Create(DiagnosticBuilder.ForPosition(indexSyntax).IndexOutOfBounds(baseType.Name, baseType.Items.Length, indexType.Value)),
                long otherwise => baseType.Items[(int) otherwise].Type,
            };

            switch (baseType)
            {
                case TypeSymbol when TypeHelper.TryRemoveNullability(baseType) is TypeSymbol nonNullableBaseType:
                    diagnostics.Write(DiagnosticBuilder.ForPosition(TextSpan.Between(syntax.OpenSquare, syntax.CloseSquare)).DereferenceOfPossiblyNullReference(baseType.Name, syntax));

                    return GetArrayItemType(syntax, diagnostics, nonNullableBaseType, indexType);

                case AnyType:
                    // base expression is of type any
                    if (indexType.TypeKind == TypeKind.Any)
                    {
                        // index is also of type any
                        return LanguageConstants.Any;
                    }

                    if (TypeValidator.AreTypesAssignable(indexType, LanguageConstants.Int) ||
                        TypeValidator.AreTypesAssignable(indexType, LanguageConstants.String))
                    {
                        // index expression is string | int but base is any
                        return LanguageConstants.Any;
                    }

                    // index was of the wrong type
                    return InvalidAccessExpression(DiagnosticBuilder.ForPosition(syntax.IndexExpression).StringOrIntegerIndexerRequired(indexType), diagnostics, syntax.IsSafeAccess);

                case TupleType baseTuple when indexType is IntegerLiteralType integerLiteralIndex:
                    return GetTypeAtIndex(baseTuple, integerLiteralIndex, syntax.IndexExpression);

                case TupleType baseTuple when indexType is UnionType indexUnion && indexUnion.Members.All(t => t.Type is IntegerLiteralType):
                    var possibilities = indexUnion.Members.Select(t => t.Type)
                        .OfType<IntegerLiteralType>()
                        .Select(index => GetTypeAtIndex(baseTuple, index, syntax.IndexExpression))
                        .ToImmutableArray();

                    if (possibilities.OfType<ErrorType>().Any())
                    {
                        return ErrorType.Create(possibilities.SelectMany(p => p.GetDiagnostics()));
                    }

                    return TypeHelper.CreateTypeUnion(possibilities);

                case ArrayType baseArray:
                    if (indexType is IntegerLiteralType integerLiteralArrayIndex && integerLiteralArrayIndex.Value < 0)
                    {
                        return ErrorType.Create(
                            DiagnosticBuilder
                                .ForPosition(syntax.IndexExpression)
                                .ArrayIndexOutOfBounds(integerLiteralArrayIndex.Value));
                    }

                    // we are indexing over an array
                    if (TypeValidator.AreTypesAssignable(indexType, LanguageConstants.Int))
                    {
                        // the index is of "any" type or integer type
                        // return the item type
                        return baseArray.Item.Type;
                    }

                    return InvalidAccessExpression(DiagnosticBuilder.ForPosition(syntax.IndexExpression).ArraysRequireIntegerIndex(indexType), diagnostics, syntax.IsSafeAccess);

                case ObjectType baseObject:
                    {
                        // we are indexing over an object
                        if (indexType.TypeKind == TypeKind.Any)
                        {
                            // index is of type "any"
                            return GetExpressionedPropertyType(baseObject, syntax.IndexExpression);
                        }

                        if (TypeValidator.AreTypesAssignable(indexType, LanguageConstants.String))
                        {
                            if (indexType is StringLiteralType literalIndex)
                            {
                                // indexing using a string literal so we know the name of the property
                                return TypeHelper.GetNamedPropertyType(baseObject, syntax.IndexExpression, literalIndex.RawStringValue, syntax.IsSafeAccess || TypeValidator.ShouldWarn(baseObject), diagnostics);
                            }

                            // the property name is itself an expression
                            return GetExpressionedPropertyType(baseObject, syntax.IndexExpression);
                        }

                        return InvalidAccessExpression(DiagnosticBuilder.ForPosition(syntax.IndexExpression).ObjectsRequireStringIndex(indexType), diagnostics, syntax.IsSafeAccess);
                    }

                case DiscriminatedObjectType:
                    if (TypeValidator.AreTypesAssignable(indexType, LanguageConstants.String))
                    {
                        // index is assignable to string
                        // since we're not resolving the discriminator currently, we can just return the "any" type
                        // TODO: resolve the discriminator
                        return LanguageConstants.Any;
                    }

                    return InvalidAccessExpression(DiagnosticBuilder.ForPosition(syntax.IndexExpression).ObjectsRequireStringIndex(indexType), diagnostics, syntax.IsSafeAccess);

                case UnionType unionType:
                    {
                        // ensure we enumerate only once since some paths include a side effect that writes a diagnostic
                        var arrayItemTypes = unionType.Members
                            .Select(baseMemberType => GetArrayItemType(syntax, diagnostics, baseMemberType.Type, indexType))
                            .ToList();

                        if (arrayItemTypes.OfType<ErrorType>().Any())
                        {
                            // some of the union members are not assignable
                            // base expression was of the wrong type
                            return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.BaseExpression).IndexerRequiresObjectOrArray(baseType));
                        }

                        // all of the union members are assignable - create the resulting item type
                        return TypeHelper.CreateTypeUnion(arrayItemTypes);
                    }

                default:
                    // base expression was of the wrong type
                    return InvalidAccessExpression(DiagnosticBuilder.ForPosition(syntax.BaseExpression).IndexerRequiresObjectOrArray(baseType), diagnostics, syntax.IsSafeAccess);
            }
        }

        private static TypeSymbol InvalidAccessExpression(ErrorDiagnostic error, IDiagnosticWriter diagnostics, bool safeAccess)
        {
            if (safeAccess)
            {
                diagnostics.Write(new Diagnostic(error.Span, DiagnosticLevel.Warning, error.Code, error.Message, error.Uri, error.Styling, error.Source));
                return LanguageConstants.Null;
            }
            return ErrorType.Create(error);
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics => GetAccessedType(syntax, diagnostics));

        private TypeSymbol GetAccessedType(AccessExpressionSyntax syntax, IDiagnosticWriter diagnostics)
        {
            Stack<AccessExpressionSyntax> chainedAccesses = syntax.ToAccessExpressionStack();

            var baseType = typeManager.GetTypeInfo(chainedAccesses.Peek().BaseExpression);

            var nullVariantRemoved = false;
            AccessExpressionSyntax? prevAccess = null;
            while (chainedAccesses.TryPop(out var nextAccess))
            {
                if (prevAccess?.IsSafeAccess is true || nextAccess.IsSafeAccess)
                {
                    // if the first access definitely returns null, short-circuit the whole chain
                    if (ReferenceEquals(baseType, LanguageConstants.Null))
                    {
                        return baseType;
                    }

                    // if the first access might return null, evaluate the rest of the chain as if it does not return null, the create a union of the result and null
                    if (TypeHelper.TryRemoveNullability(baseType) is TypeSymbol nonNullable)
                    {
                        nullVariantRemoved = true;
                        baseType = nonNullable;
                    }
                }

                baseType = nextAccess switch
                {
                    ArrayAccessSyntax arrayAccess => GetArrayItemType(arrayAccess, diagnostics, baseType, typeManager.GetTypeInfo(arrayAccess.IndexExpression)),
                    PropertyAccessSyntax propertyAccess => GetNamedPropertyType(propertyAccess, baseType, diagnostics),
                    _ => throw new InvalidOperationException("Unrecognized access syntax"),
                };

                prevAccess = nextAccess;
            }

            return nullVariantRemoved
                ? TypeHelper.CreateTypeUnion(baseType, LanguageConstants.Null)
                : baseType;
        }

        private static TypeSymbol GetNamedPropertyType(PropertyAccessSyntax syntax, TypeSymbol baseType, IDiagnosticWriter diagnostics) => UnwrapType(baseType) switch
        {
            ErrorType error => error,
            TypeSymbol withErrors when withErrors.GetDiagnostics().Any() => ErrorType.Create(withErrors.GetDiagnostics()),

            TypeSymbol original when TypeHelper.TryRemoveNullability(original) is TypeSymbol nonNullable => EmitNullablePropertyAccessDiagnosticAndEraseNullability(syntax, original, nonNullable, diagnostics),

            // the property is not valid
            // there's already a parse error for it, so we don't need to add a type error as well
            ObjectType when !syntax.PropertyName.IsValid => ErrorType.Empty(),

            ObjectType objectType => TypeHelper.GetNamedPropertyType(objectType, syntax.PropertyName, syntax.PropertyName.IdentifierName, syntax.IsSafeAccess || TypeValidator.ShouldWarn(objectType), diagnostics),

            // TODO: We might be able use the declared type here to resolve discriminator to improve the assigned type
            DiscriminatedObjectType => LanguageConstants.Any,

            // We can assign to an object, but we don't have a type for that object.
            // The best we can do is allow it and return the 'any' type.
            TypeSymbol maybeObject when TypeValidator.AreTypesAssignable(maybeObject, LanguageConstants.Object) => LanguageConstants.Any,

            // can only access properties of objects
            TypeSymbol otherwise => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.PropertyName).ObjectRequiredForPropertyAccess(otherwise)),
        };

        private static TypeSymbol EmitNullablePropertyAccessDiagnosticAndEraseNullability(PropertyAccessSyntax syntax, TypeSymbol originalBaseType, TypeSymbol nonNullableBaseType, IDiagnosticWriter diagnostics)
        {
            diagnostics.Write(DiagnosticBuilder.ForPosition(TextSpan.Between(syntax.Dot, syntax.PropertyName)).DereferenceOfPossiblyNullReference(originalBaseType.Name, syntax));

            return GetNamedPropertyType(syntax, nonNullableBaseType, diagnostics);
        }

        public override void VisitResourceAccessSyntax(ResourceAccessSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var errors = new List<ErrorDiagnostic>();

                var baseType = typeManager.GetTypeInfo(syntax.BaseExpression);
                CollectErrors(errors, baseType);

                if (PropagateErrorType(errors, baseType))
                {
                    return ErrorType.Create(errors);
                }

                if (baseType is not ResourceType)
                {
                    // can only access children of resources
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.ResourceName).ResourceRequiredForResourceAccess(baseType.Name));
                }

                if (!syntax.ResourceName.IsValid)
                {
                    // the resource name is not valid
                    // there's already a parse error for it, so we don't need to add a type error as well
                    return ErrorType.Empty();
                }

                // Should have a symbol from name binding.
                var symbol = binder.GetSymbolInfo(syntax);
                if (symbol == null)
                {
                    throw new InvalidOperationException("ResourceAccessSyntax was not assigned a symbol during name binding.");
                }

                if (symbol is ErrorSymbol error)
                {
                    return ErrorType.Create(error.GetDiagnostics());
                }
                else if (symbol is not ResourceSymbol)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.ResourceName).ResourceRequiredForResourceAccess(baseType.Kind.ToString()));
                }

                // This is a valid nested resource. Return its type.
                return typeManager.GetTypeInfo(((ResourceSymbol)symbol).DeclaringResource);
            });

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var errors = new List<ErrorDiagnostic>();

                foreach (TypeSymbol argumentType in GetArgumentTypes(syntax.Arguments).ToArray())
                {
                    CollectErrors(errors, argumentType);
                }

                switch (binder.GetSymbolInfo(syntax))
                {
                    case ErrorSymbol errorSymbol:
                        // function bind failure - pass the error along
                        return ErrorType.Create(errors.Concat(errorSymbol.GetDiagnostics()));

                    case FunctionSymbol function:
                        return GetFunctionSymbolType(function, syntax, errors, diagnostics);

                    case DeclaredFunctionSymbol declaredFunction:
                        return GetFunctionSymbolType(declaredFunction, syntax, errors, diagnostics);

                    case Symbol symbolInfo when binder.NamespaceResolver.GetKnownFunctions(symbolInfo.Name).FirstOrDefault() is {} knownFunction:
                        // A function exists, but it's being shadowed by another symbol in the file
                        return ErrorType.Create(
                            errors.Append(
                                DiagnosticBuilder.ForPosition(syntax.Name.Span)
                                .SymbolicNameShadowsAKnownFunction(syntax.Name.IdentifierName, knownFunction.DeclaringObject.Name, knownFunction.Name)));

                    default:
                        return ErrorType.Create(errors.Append(DiagnosticBuilder.ForPosition(syntax.Name.Span).SymbolicNameIsNotAFunction(syntax.Name.IdentifierName)));
                }
            });

        public override void VisitLambdaSyntax(LambdaSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var argumentTypes = syntax.GetLocalVariables().Select(x => typeManager.GetTypeInfo(x));
                var returnType = TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, this.parsingErrorLookup, diagnostics, syntax.Body, LanguageConstants.Any);

                return new LambdaType(argumentTypes.ToImmutableArray<ITypeReference>(), returnType);
            });

        public override void VisitTypedLambdaSyntax(TypedLambdaSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax);
                if (declaredType is not LambdaType declaredLambdaType)
                {
                    return declaredType ?? ErrorType.Empty();
                }

                var errors = new List<ErrorDiagnostic>();
                var argumentTypes = new List<TypeSymbol>();
                foreach (var argumentType in declaredLambdaType.ArgumentTypes)
                {
                    CollectErrors(errors, argumentType.Type);
                }

                var returnType = TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, this.parsingErrorLookup, diagnostics, syntax.Body, declaredLambdaType.ReturnType.Type);
                CollectErrors(errors, returnType);

                if (PropagateErrorType(errors, argumentTypes))
                {
                    return ErrorType.Create(errors);
                }

                return new LambdaType(declaredLambdaType.ArgumentTypes, returnType);
            });

        private Symbol? GetSymbolForDecorator(DecoratorSyntax decorator)
        {
            if (binder.GetSymbolInfo(decorator.Expression) is { } symbol)
            {
                return symbol;
            }

            if (decorator.Expression is not InstanceFunctionCallSyntax ifc ||
                typeManager.GetTypeInfo(ifc.BaseExpression) is not NamespaceType namespaceType)
            {
                return null;
            }

            if (!ifc.Name.IsValid)
            {
                // the parser produced an instance function calls with an invalid name
                // all instance function calls must be bound to a symbol, so let's
                // bind to a symbol without any errors (there's already a parse error)
                return null;
            }

            var functionFlags = binder.GetParent(decorator) switch
            {
                MetadataDeclarationSyntax _ => FunctionFlags.MetadataDecorator,
                ResourceDeclarationSyntax _ => FunctionFlags.ResourceDecorator,
                ModuleDeclarationSyntax _ => FunctionFlags.ModuleDecorator,
                ParameterDeclarationSyntax _ => FunctionFlags.ParameterDecorator,
                TypeDeclarationSyntax _ => FunctionFlags.TypeDecorator,
                VariableDeclarationSyntax _ => FunctionFlags.VariableDecorator,
                OutputDeclarationSyntax _ => FunctionFlags.OutputDecorator,
                _ => FunctionFlags.AnyDecorator,
            };

            var resolvedSymbol = functionFlags.HasAnyDecoratorFlag()
                // Decorator functions are only valid when HasDecoratorFlag() is true which means
                // the instance function call is the top level expression of a DecoratorSyntax node.
                ? namespaceType.MethodResolver.TryGetSymbol(ifc.Name) ?? namespaceType.DecoratorResolver.TryGetSymbol(ifc.Name)
                : namespaceType.MethodResolver.TryGetSymbol(ifc.Name);

            return SymbolValidator.ResolveNamespaceQualifiedFunction(functionFlags, resolvedSymbol, ifc.Name, namespaceType);
        }

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var errors = new List<ErrorDiagnostic>();

                var baseType = typeManager.GetTypeInfo(syntax.BaseExpression);
                CollectErrors(errors, baseType);

                if (PropagateErrorType(errors, baseType))
                {
                    return ErrorType.Create(errors);
                }

                baseType = UnwrapType(baseType);

                if (baseType is not ObjectType objectType)
                {
                    // can only access methods on objects
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Name).ObjectRequiredForMethodAccess(baseType));
                }

                foreach (TypeSymbol argumentType in this.GetArgumentTypes(syntax.Arguments).ToArray())
                {
                    CollectErrors(errors, argumentType);
                }

                if (!syntax.Name.IsValid)
                {
                    // the parser produced an instance function calls with an invalid name
                    // all instance function calls must be bound to a symbol, so let's
                    // bind to a symbol without any errors (there's already a parse error)
                    return ErrorType.Empty();
                }

                Symbol? foundSymbol;
                if (binder.GetParent(syntax) is DecoratorSyntax decorator)
                {
                    foundSymbol = GetSymbolForDecorator(decorator);
                }
                else
                {
                    var resolvedSymbol = objectType.MethodResolver.TryGetSymbol(syntax.Name);
                    foundSymbol = SymbolValidator.ResolveObjectQualifiedFunctionWithoutValidatingFlags(resolvedSymbol, syntax.Name, objectType);
                }

                switch (foundSymbol)
                {
                    case ErrorSymbol errorSymbol:
                        // bind bind failure - pass the error along
                        return ErrorType.Create(errors.Concat(errorSymbol.GetDiagnostics()));

                    case FunctionSymbol functionSymbol:
                        return GetFunctionSymbolType(functionSymbol, syntax, errors, diagnostics);

                    default:
                        return ErrorType.Create(errors.Append(DiagnosticBuilder.ForPosition(syntax.Name.Span).SymbolicNameIsNotAFunction(syntax.Name.IdentifierName)));
                }
            });

        public override void VisitTargetScopeSyntax(TargetScopeSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                if (syntax.Decorators.Any())
                {
                    diagnostics.Write(DiagnosticBuilder.ForPosition(syntax.Decorators.First().At).DecoratorsNotAllowed());
                }

                var declaredType = syntax.GetDeclaredType();
                if (declaredType is ErrorType)
                {
                    return declaredType;
                }

                TypeValidator.GetCompileTimeConstantViolation(syntax.Value, diagnostics);

                return TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, parsingErrorLookup, diagnostics, syntax.Value, declaredType);
            });

        public override void VisitMissingDeclarationSyntax(MissingDeclarationSyntax syntax) => AssignTypeWithDiagnostics(syntax, diagnostics =>
        {
            if (this.parsingErrorLookup.Contains(syntax))
            {
                // Skip adding semantic errors if there are parsing errors, as it might be a bit overwhelming.
                return LanguageConstants.Any;
            }

            List<ErrorDiagnostic> errors = new();

            foreach (var decoratorSyntax in syntax.Decorators)
            {
                var expressionType = this.typeManager.GetTypeInfo(decoratorSyntax.Expression);
                CollectErrors(errors, expressionType);
            }

            diagnostics.WriteMultiple(errors);

            // There must exist at least one decorator for MissingDeclarationSyntax.
            var lastDecoratorSyntax = syntax.Decorators.Last();

            diagnostics.Write(lastDecoratorSyntax, builder =>
            {
                var functionSymbol = this.GetSymbolForDecorator(lastDecoratorSyntax) as FunctionSymbol;

                return functionSymbol?.FunctionFlags switch
                {
                    FunctionFlags.MetadataDecorator => builder.ExpectedMetadataDeclarationAfterDecorator(),
                    FunctionFlags.ParameterDecorator => builder.ExpectedParameterDeclarationAfterDecorator(),
                    FunctionFlags.ParameterOutputOrTypeDecorator => features.UserDefinedTypesEnabled
                        ? builder.ExpectedParameterOutputOrTypeDeclarationAfterDecorator()
                        : builder.ExpectedParameterOrOutputDeclarationAfterDecorator(),
                    FunctionFlags.ParameterOrTypeDecorator => features.UserDefinedTypesEnabled
                        ? builder.ExpectedParameterOrTypeDeclarationAfterDecorator()
                        : builder.ExpectedParameterDeclarationAfterDecorator(),
                    FunctionFlags.VariableDecorator => builder.ExpectedVariableDeclarationAfterDecorator(),
                    FunctionFlags.ResourceDecorator => builder.ExpectedResourceDeclarationAfterDecorator(),
                    FunctionFlags.ModuleDecorator => builder.ExpectedModuleDeclarationAfterDecorator(),
                    FunctionFlags.OutputDecorator => builder.ExpectedOutputDeclarationAfterDecorator(),
                    FunctionFlags.ResourceOrModuleDecorator => builder.ExpectedResourceOrModuleDeclarationAfterDecorator(),
                    _ => builder.ExpectedDeclarationAfterDecorator(),
                };
            });

            return LanguageConstants.Any;
        });

        private TypeSymbol VisitDeclaredSymbol(VariableAccessSyntax syntax, DeclaredSymbol declaredSymbol)
        {
            var declaringType = typeManager.GetTypeInfo(declaredSymbol.DeclaringSyntax);

            // symbols are responsible for doing their own type checking
            // the error from that should not be propagated to expressions that have type errors
            // unless we're dealing with a cyclic expression error, then propagate away!
            if (declaringType is ErrorType errorType)
            {
                // replace the original error with a different one
                // we may consider suppressing this error in the future as well
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).ReferencedSymbolHasErrors(syntax.Name.IdentifierName));
            }

            return declaringType;
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
            => AssignType(syntax, () =>
            {
                switch (this.binder.GetSymbolInfo(syntax))
                {
                    case ErrorSymbol errorSymbol:
                        // variable bind failure - pass the error along
                        return errorSymbol.ToErrorType();

                    case ResourceSymbol resource:
                        // resource bodies can participate in cycles
                        // need to explicitly force a type check on the body
                        return new DeferredTypeReference(() => VisitDeclaredSymbol(syntax, resource));

                    case ModuleSymbol module:
                        return new DeferredTypeReference(() => VisitDeclaredSymbol(syntax, module));

                    case ParameterSymbol parameter:
                        return new DeferredTypeReference(() => VisitDeclaredSymbol(syntax, parameter));

                    case ParameterAssignmentSymbol parameter:
                        return new DeferredTypeReference(() => VisitDeclaredSymbol(syntax, parameter));

                    case VariableSymbol variable:
                        return new DeferredTypeReference(() => VisitDeclaredSymbol(syntax, variable));

                    case LocalVariableSymbol local:
                        return new DeferredTypeReference(() => VisitDeclaredSymbol(syntax, local));

                    case ProviderNamespaceSymbol provider:
                        return new DeferredTypeReference(() => VisitDeclaredSymbol(syntax, provider));

                    case BuiltInNamespaceSymbol @namespace:
                        return @namespace.Type;

                    case WildcardImportSymbol wildcardImport:
                        return wildcardImport.Type;

                    case TypeAliasSymbol:
                    case AmbientTypeSymbol:
                    case ImportedTypeSymbol:
                        return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Name.Span).TypeSymbolUsedAsValue(syntax.Name.IdentifierName));

                    default:
                        return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Name.Span).SymbolicNameIsNotAVariableOrParameter(syntax.Name.IdentifierName));
                }
            });

        public override void VisitNonNullAssertionSyntax(NonNullAssertionSyntax syntax)
            => AssignType(syntax, () =>
            {
                var baseType = typeManager.GetTypeInfo(syntax.BaseExpression);
                return TypeHelper.TryRemoveNullability(baseType) ?? baseType;
            });

        private static void CollectErrors(List<ErrorDiagnostic> errors, ITypeReference reference)
        {
            errors.AddRange(reference.Type.GetDiagnostics());
        }

        private TypeSymbol GetFunctionSymbolType(
            IFunctionSymbol function,
            FunctionCallSyntaxBase syntax,
            IList<ErrorDiagnostic> errors,
            IDiagnosticWriter diagnosticWriter) => GetFunctionSymbolType(function,
                syntax,
                // Recover argument type errors so we can continue type checking for the parent function call.
                GetRecoveredArgumentTypes(syntax.Arguments).ToImmutableArray(),
                errors,
                diagnosticWriter);

        private TypeSymbol GetFunctionSymbolType(
            IFunctionSymbol function,
            FunctionCallSyntaxBase syntax,
            ImmutableArray<TypeSymbol> argumentTypes,
            IList<ErrorDiagnostic> errors,
            IDiagnosticWriter diagnosticWriter)
        {
            var matches = FunctionResolver.GetMatches(
                function,
                argumentTypes,
                out IList<ArgumentCountMismatch> countMismatches,
                out IList<ArgumentTypeMismatch> typeMismatches).ToList();

            if (matches.Count == 0)
            {
                if (typeMismatches.Any())
                {
                    // if the type mismatch is because a nullably-typed argument was supplied for a non-nullable value, try again with a non-nullable arg type
                    foreach (var tm in typeMismatches)
                    {
                        if (TypeHelper.WouldBeAssignableIfNonNullable(tm.ArgumentType, tm.ParameterType, out var nonNullableArgType))
                        {
                            // at least one of our type mismatches is purely due to nullability. Recur, passing in a non-nullable version of the arg type.
                            // If *multiple* type mismatches are due to nullability, this function will recur for each mismatch, with each invocation supplying
                            // a diagnostic. Only the last invocation will generate a return type.
                            var resultSansNullability = GetFunctionSymbolType(function,
                                syntax,
                                Enumerable.Range(0, argumentTypes.Length).Select(i => tm.ArgumentIndex == i ? nonNullableArgType : argumentTypes[i]).ToImmutableArray(),
                                errors,
                                diagnosticWriter);

                            // if we couldn't find a match even after tweaking argument nullability, don't add any nullability warnings
                            if (resultSansNullability is ErrorType error)
                            {
                                return error;
                            }

                            var offendingArgSyntax = syntax.Arguments[tm.ArgumentIndex];
                            diagnosticWriter.Write(DiagnosticBuilder.ForPosition(offendingArgSyntax).PossibleNullReferenceAssignment(tm.ParameterType, tm.ArgumentType, offendingArgSyntax));
                            return resultSansNullability;
                        }
                    }

                    if (typeMismatches.Count > 1 && typeMismatches.Skip(1).All(tm => tm.ArgumentIndex == typeMismatches[0].ArgumentIndex))
                    {
                        // All type mismatches are equally good (or bad).
                        var parameterTypes = typeMismatches.Select(tm => tm.ParameterType).ToList();
                        var argumentType = typeMismatches[0].ArgumentType;
                        var signatures = typeMismatches.Select(tm => tm.Source.TypeSignature).ToList();
                        var argumentSyntax = syntax.GetArgumentByPosition(typeMismatches[0].ArgumentIndex);

                        errors.Add(DiagnosticBuilder.ForPosition(argumentSyntax).CannotResolveFunctionOverload(signatures, argumentType, parameterTypes));
                    }
                    else
                    {
                        // Choose the type mismatch that has the largest index as the best one.
                        var (_, argumentIndex, argumentType, parameterType) = typeMismatches.OrderBy(tm => tm.ArgumentIndex).Last();

                        errors.Add(DiagnosticBuilder.ForPosition(syntax.GetArgumentByPosition(argumentIndex)).ArgumentTypeMismatch(argumentType, parameterType));
                    }
                }
                else if (countMismatches.Any())
                {
                    // Argument type mismatch wins over count mismatch. Handle count mismatch only when there's no type mismatch.
                    var (actualCount, mininumArgumentCount, maximumArgumentCount) = countMismatches.Aggregate(ArgumentCountMismatch.Reduce);
                    var argumentsSpan = TextSpan.Between(syntax.OpenParen, syntax.CloseParen);

                    errors.Add(DiagnosticBuilder.ForPosition(argumentsSpan).ArgumentCountMismatch(actualCount, mininumArgumentCount, maximumArgumentCount));
                }
            }

            if (PropagateErrorType(errors))
            {
                return ErrorType.Create(errors);
            }

            if (matches.Count == 1)
            {
                // we have an exact match or a single ambiguous match
                var matchedOverload = matches.Single();
                matchedFunctionOverloads.TryAdd(syntax, matchedOverload);

                // return its type
                var result = matchedOverload.ResultBuilder(binder, fileResolver, diagnosticWriter, syntax, argumentTypes);
                if (result.Value is not null)
                {
                    matchedFunctionResultValues.TryAdd(syntax, result.Value);
                }
                return result.Type;
            }

            // function arguments are ambiguous (due to "any" type)
            // technically, the correct behavior would be to return a union of all possible types
            // unfortunately our language lacks a good type checking construct
            // and we also don't want users to have to use the converter functions to work around it
            // instead, we will return the "any" type to short circuit the type checking for those cases
            return LanguageConstants.Any;
        }

        /// <summary>
        /// Gets the type of the property whose name is an expression.
        /// </summary>
        /// <param name="baseType">The base object type</param>
        /// <param name="propertyExpressionPositionable">The position of the property name expression</param>
        private static TypeSymbol GetExpressionedPropertyType(ObjectType baseType, IPositionable propertyExpressionPositionable)
        {
            if (baseType.TypeKind == TypeKind.Any)
            {
                // all properties of "any" type are of type "any"
                return LanguageConstants.Any;
            }

            if (baseType.Properties.Any() || baseType.AdditionalPropertiesType != null)
            {
                // the object type allows properties
                return LanguageConstants.Any;
            }

            return ErrorType.Create(DiagnosticBuilder.ForPosition(propertyExpressionPositionable).NoPropertiesAllowed(baseType));
        }

        private IEnumerable<TypeSymbol> GetArgumentTypes(IEnumerable<FunctionArgumentSyntax> argumentSyntaxes) =>
            argumentSyntaxes.Select(syntax => this.typeManager.GetTypeInfo(syntax));

        private IEnumerable<TypeSymbol> GetRecoveredArgumentTypes(IEnumerable<FunctionArgumentSyntax> argumentSyntaxes) =>
            this.GetArgumentTypes(argumentSyntaxes).Select(argumentType => argumentType.TypeKind == TypeKind.Error ? LanguageConstants.Any : argumentType);

        private IEnumerable<IDiagnostic> GetOutputDeclarationDiagnostics(TypeSymbol assignedType, OutputDeclarationSyntax syntax)
        {
            var valueType = typeManager.GetTypeInfo(syntax.Value);

            // this type is not a property in a symbol so the semantic error visitor won't collect the errors automatically
            if (valueType is ErrorType)
            {
                return valueType.GetDiagnostics();
            }

            if (assignedType is ErrorType)
            {
                // no point in checking that the value is assignable to the declared output type if no valid declared type could be discerned
                return Enumerable.Empty<IDiagnostic>();
            }

            var diagnosticWriter = ToListDiagnosticWriter.Create();

            TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, parsingErrorLookup, diagnosticWriter, syntax.Value, assignedType);

            return diagnosticWriter.GetDiagnostics();
        }

        private IEnumerable<IDiagnostic> ValidateDefaultValue(ParameterDefaultValueSyntax defaultValueSyntax, TypeSymbol assignedType)
        {
            // figure out type of the default value
            var defaultValueType = typeManager.GetTypeInfo(defaultValueSyntax.DefaultValue);

            // this type is not a property in a symbol so the semantic error visitor won't collect the errors automatically
            if (defaultValueType is ErrorType)
            {
                return defaultValueType.GetDiagnostics();
            }

            var diagnosticWriter = ToListDiagnosticWriter.Create();

            TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, parsingErrorLookup, diagnosticWriter, defaultValueSyntax.DefaultValue, assignedType);

            return diagnosticWriter.GetDiagnostics();
        }

        private IEnumerable<IDiagnostic> ValidateIdentifierAccess(SyntaxBase syntax)
        {
            return SyntaxAggregator.Aggregate(syntax, new List<IDiagnostic>(), (accumulated, current) =>
                {
                    if (current is VariableAccessSyntax)
                    {
                        var symbol = this.binder.GetSymbolInfo(current);

                        // Error: already has error info attached, no need to add more
                        // Parameter: references are permitted in other parameters' default values as long as there is not a cycle (BCP080)
                        // Function: we already validate that a function cannot be used as a variable (BCP063)
                        // Output & Metadata: we already validate that output & metadata cannot be referenced in expressions (BCP057)
                        if (symbol != null &&
                            symbol.Kind != SymbolKind.Error &&
                            symbol.Kind != SymbolKind.Parameter &&
                            symbol.Kind != SymbolKind.Function &&
                            symbol.Kind != SymbolKind.Output &&
                            symbol.Kind != SymbolKind.Metadata &&
                            symbol.Kind != SymbolKind.Namespace &&
                            symbol.Kind != SymbolKind.ImportedNamespace)
                        {
                            accumulated.Add(DiagnosticBuilder.ForPosition(current).CannotReferenceSymbolInParamDefaultValue());
                        }
                    }

                    return accumulated;
                },
                accumulated => accumulated);
        }

        private IEnumerable<IDiagnostic> ValidateIfCondition(IfConditionSyntax syntax)
        {
            var conditionType = typeManager.GetTypeInfo(syntax.ConditionExpression);

            if (conditionType is ErrorType)
            {
                return conditionType.GetDiagnostics();
            }

            if (!TypeValidator.AreTypesAssignable(conditionType, LanguageConstants.Bool))
            {
                var builder = DiagnosticBuilder.ForPosition(syntax.ConditionExpression);
                return TypeHelper.WouldBeAssignableIfNonNullable(conditionType, LanguageConstants.Bool, out var nonNullable)
                    ? builder.PossibleNullReferenceAssignment(LanguageConstants.Bool,
                        conditionType,
                        // syntax.ConditionExpression includes the parentheses surrounding the condition
                        syntax.ConditionExpression is ParenthesizedExpressionSyntax parenthesized
                            ? parenthesized.Expression
                            : syntax.ConditionExpression).AsEnumerable()
                    : builder.ValueTypeMismatch(LanguageConstants.Bool).AsEnumerable();
            }

            return Enumerable.Empty<IDiagnostic>();
        }

        public static TypeSymbol UnwrapType(TypeSymbol baseType) =>
            baseType switch
            {
                ResourceType resourceType =>
                    // We're accessing a property on the resource body.
                    resourceType.Body.Type,

                ModuleType moduleType =>
                    // We're accessing a property on the module body.
                    moduleType.Body.Type,

                _ => baseType
            };

        private DecoratorSyntax? GetNamedDecorator(DecorableSyntax syntax, string decoratorName)
        {
            /*
            * Calling FirstOrDefault here because when there are duplicate decorators,
            * the top most one takes precedence.
            */
            return syntax.Decorators.FirstOrDefault(decoratorSyntax =>
            {
                if (decoratorSyntax.Expression is not FunctionCallSyntaxBase functionCall ||
                    !LanguageConstants.IdentifierComparer.Equals(functionCall.Name.IdentifierName, decoratorName))
                {
                    return false;
                }

                if (SymbolHelper.TryGetSymbolInfo(binder, typeManager.GetDeclaredType, functionCall) is not FunctionSymbol functionSymbol ||
                    functionSymbol.DeclaringObject is not NamespaceType declaringNamespace)
                {
                    return false;
                }

                var argumentTypes = this.GetRecoveredArgumentTypes(functionCall.Arguments).ToArray();
                var decorator = declaringNamespace.DecoratorResolver
                    .GetMatches(functionSymbol, argumentTypes)
                    .SingleOrDefault();

                return decorator is not null;
            });
        }
    }
}
