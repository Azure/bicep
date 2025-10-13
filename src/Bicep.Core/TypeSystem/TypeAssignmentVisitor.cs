// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem
{
    public sealed class TypeAssignmentVisitor : AstVisitor
    {
        private readonly IFeatureProvider features;
        private readonly ITypeManager typeManager;
        private readonly IBinder binder;
        private readonly SemanticModel model;
        private readonly IDiagnosticLookup parsingErrorLookup;
        private readonly ResourceDerivedTypeResolver resourceDerivedTypeResolver;
        private readonly ResourceDerivedTypeDiagnosticReporter resourceDerivedTypeDiagnosticReporter;
        private readonly ConcurrentDictionary<SyntaxBase, TypeAssignment> assignedTypes;
        private readonly ConcurrentDictionary<FunctionCallSyntaxBase, FunctionOverload> matchedFunctionOverloads;
        private readonly ConcurrentDictionary<FunctionCallSyntaxBase, Expression> matchedFunctionResultValues;

        public TypeAssignmentVisitor(ITypeManager typeManager, SemanticModel model)
        {
            this.typeManager = typeManager;
            this.model = model;
            this.features = model.Features;
            this.binder = model.Binder;
            this.parsingErrorLookup = model.ParsingErrorLookup;
            resourceDerivedTypeResolver = new(binder);
            resourceDerivedTypeDiagnosticReporter = new(features, binder);
            assignedTypes = new();
            matchedFunctionOverloads = new();
            matchedFunctionResultValues = new();
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

        private ErrorType? CheckForCyclicError(SyntaxBase syntax)
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
                // local functions
                ArrayType? GetIterationTargetType(ForSyntax @for)
                {
                    // get type of the loop array expression
                    // (this shouldn't cause a stack overflow because it's a peer node of this one)
                    var arrayExpressionType = this.typeManager.GetTypeInfo(@for.Expression);

                    if (arrayExpressionType.TypeKind == TypeKind.Any || arrayExpressionType is not ArrayType arrayType)
                    {
                        // the array is of "any" type or the loop array expression isn't actually an array
                        // in the former case, there isn't much we can do
                        // in the latter case, we will let the ForSyntax type check rules produce the error for it
                        return null;
                    }

                    // the array expression is actually an array
                    return arrayType;
                }

                ITypeReference GetItemType(ForSyntax @for)
                    => GetIterationTargetType(@for)?.Item ?? LanguageConstants.Any;

                long GetEnumerationTargetLength(ForSyntax @for) => GetIterationTargetType(@for)?.MaxLength switch
                {
                    0 or null => LanguageConstants.MaxResourceCopyIndexValue,
                    long maxLength => maxLength - 1,
                };

                var symbol = this.binder.GetSymbolInfo(syntax);
                if (symbol is not LocalVariableSymbol localVariableSymbol)
                {
                    throw new InvalidOperationException($"{syntax.GetType().Name} is bound to unexpected type '{symbol?.GetType().Name}'.");
                }

                switch (localVariableSymbol.LocalKind)
                {
                    case LocalKind.ForExpressionItemVariable:
                        // this local variable is a loop item variable
                        // we should return item type of the array (if feasible)

                        return GetItemType(GetParentFor(syntax));

                    case LocalKind.ForExpressionIndexVariable:
                        // the local variable is an index variable
                        // index variables are always of type int
                        return TypeFactory.CreateIntegerType(
                            minValue: 0,
                            maxValue: GetEnumerationTargetLength(GetParentFor(syntax)));

                    case LocalKind.LambdaItemVariable:
                        var (lambda, argumentIndex) = binder.GetParent(syntax) switch
                        {
                            LambdaSyntax lambdaSyntax => (lambdaSyntax, 0),
                            VariableBlockSyntax block when this.binder.GetParent(block) is LambdaSyntax lambdaSyntax => (lambdaSyntax, block.Arguments.IndexOf(syntax)),
                            var parent => throw new InvalidOperationException($"{syntax.GetType().Name} at {syntax.Span} has an unexpected parent of type {parent?.GetType().Name}"),
                        };

                        if (binder.GetParent(lambda) is { } lambdaParent &&
                            typeManager.GetDeclaredType(lambdaParent) is LambdaType lambdaType &&
                            argumentIndex < lambdaType.MaximumArgCount)
                        {
                            return lambdaType.GetArgumentType(argumentIndex);
                        }

                        return LanguageConstants.Any;

                    default:
                        throw new InvalidOperationException($"Unexpected local kind '{localVariableSymbol.LocalKind}'.");
                }
            });

        private ForSyntax GetParentFor(SyntaxBase syntax) => binder.GetParent(syntax) switch
        {
            ForSyntax forParent => forParent,
            VariableBlockSyntax block when this.binder.GetParent(block) is ForSyntax forParent => forParent,
            var parent => throw new InvalidOperationException($"{syntax.GetType().Name} at {syntax.Span} has an unexpected parent of type {parent?.GetType().Name}"),
        };

        public override void VisitTypedLocalVariableSyntax(TypedLocalVariableSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                if (typeManager.GetDeclaredType(syntax) is not { } declaredType)
                {
                    return ErrorType.Empty();
                }

                base.VisitTypedLocalVariableSyntax(syntax);

                return declaredType;
            });

        public override void VisitForSyntax(ForSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var errors = new List<IDiagnostic>();

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

                        if (resourceType.DeclaringNamespace.ExtensionName == AzNamespaceType.BuiltInName &&
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

                    if (syntax.IsExistingResource() && resourceType.Flags.HasFlag(ResourceFlags.WriteOnly))
                    {
                        diagnostics.Write(syntax.Type, x => x.CannotUseExistingWithWriteOnlyResource(resourceType.TypeReference));
                    }
                }

                return TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, this.parsingErrorLookup, diagnostics, syntax.Value, declaredType, true);
            });

        public override void VisitTestDeclarationSyntax(TestDeclarationSyntax syntax)
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

                if (this.binder.GetSymbolInfo(syntax) is TestSymbol testSymbol &&
                   testSymbol.TryGetSemanticModel().IsSuccess(out var testSemanticModel, out var _) &&
                   testSemanticModel.HasErrors())
                {
                    diagnostics.Write(testSemanticModel is ArmTemplateSemanticModel
                        ? DiagnosticBuilder.ForPosition(syntax.Path).ReferencedArmTemplateHasErrors()
                        : DiagnosticBuilder.ForPosition(syntax.Path).ReferencedModuleHasErrors());
                }

                return TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, this.parsingErrorLookup, diagnostics, syntax.Value, declaredType);

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

                if (this.binder.GetSymbolInfo(syntax) is ModuleSymbol moduleSymbol && moduleSymbol.TryGetSemanticModel().IsSuccess(out var moduleSemanticModel, out var _))
                {
                    if (moduleSemanticModel.HasErrors())
                    {
                        diagnostics.Write(moduleSemanticModel is ArmTemplateSemanticModel
                            ? DiagnosticBuilder.ForPosition(syntax.Path).ReferencedArmTemplateHasErrors()
                            : DiagnosticBuilder.ForPosition(syntax.Path).ReferencedModuleHasErrors());
                    }

                    diagnostics.WriteMultiple(moduleSemanticModel.Parameters.Values.Select(md => md.TypeReference)
                        .Concat(moduleSemanticModel.Outputs.Select(md => md.TypeReference))
                        .SelectMany(resourceDerivedTypeDiagnosticReporter.ReportResourceDerivedTypeDiagnostics)
                        .Select(builder => builder(DiagnosticBuilder.ForPosition(syntax.Path))));
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

                    if (TypeHelper.IsNullable(declaredType))
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
                var errors = new List<IDiagnostic>();

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

                diagnostics.WriteMultiple(declaredType?.GetDiagnostics() ?? []);

                if (declaredType is not null)
                {
                    var unwrapped = declaredType is TypeType wrapped ? wrapped.Unwrapped : declaredType;
                    ValidateDecorators(syntax.Decorators, unwrapped, diagnostics);
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

                if (declaredType is DiscriminatedObjectType or ErrorType)
                {
                    return declaredType;
                }

                var memberTypes = syntax.Members.Select(memberSyntax => (GetTypeInfo(memberSyntax), memberSyntax))
                    .SelectMany(t => FlattenUnionMemberType(t.Item1, t.memberSyntax))
                    .ToImmutableArray();

                switch (TypeHelper.CreateTypeUnion(memberTypes.Select(t => GetNonLiteralType(t.memberType)).WhereNotNull()))
                {
                    case UnionType union when TypeHelper.TryRemoveNullability(union) is { } nonNullable && LanguageConstants.DeclarationTypes.ContainsValue(nonNullable):
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

        private static TypeSymbol? GetNonLiteralType(TypeSymbol? type) => type switch
        {
            StringLiteralType or StringType => LanguageConstants.String,
            IntegerLiteralType or IntegerType => LanguageConstants.Int,
            BooleanLiteralType or BooleanType => LanguageConstants.Bool,
            ObjectType => LanguageConstants.Object,
            TupleType => LanguageConstants.Array,
            NullType => LanguageConstants.Null,
            _ => null,
        };

        private bool MightBeArrayAny(SyntaxBase syntax) => binder.GetParent(syntax) switch
        {
            ParenthesizedTypeSyntax parenthesized => MightBeArrayAny(parenthesized),
            ArrayTypeMemberSyntax arrayTypeMember => MightBeArrayAny(arrayTypeMember),
            ArrayTypeSyntax => true,
            _ => false,
        };

        private static void ValidateAllowedValuesSubsetUnion(ImmutableArray<(TypeSymbol, UnionTypeMemberSyntax)> memberTypes, IDiagnosticWriter diagnostics)
        {
            foreach (var (memberType, memberSyntax) in memberTypes)
            {
                if (GetNonLiteralUnionMemberDiagnostic(memberType, memberSyntax) is { } diagnostic)
                {
                    diagnostics.Write(diagnostic);
                }
            }
        }

        private static void ValidateAllowedValuesUnion(TypeSymbol keystoneType, ImmutableArray<(TypeSymbol, UnionTypeMemberSyntax)> memberTypes, IDiagnosticWriter diagnostics)
        {
            foreach (var (memberType, memberSyntax) in memberTypes)
            {
                if (GetNonLiteralUnionMemberDiagnostic(memberType, memberSyntax) is { } diagnostic)
                {
                    diagnostics.Write(diagnostic);
                }
                else if (!TypeValidator.AreTypesAssignable(memberType, keystoneType))
                {
                    diagnostics.Write(DiagnosticBuilder.ForPosition(memberSyntax).InvalidUnionTypeMember(keystoneType.Name));
                }
            }
        }

        private static Diagnostic? GetNonLiteralUnionMemberDiagnostic(TypeSymbol memberType, UnionTypeMemberSyntax memberSyntax)
            => TypeHelper.IsLiteralType(memberType) ? null : DiagnosticBuilder.ForPosition(memberSyntax).NonLiteralUnionMember();

        public override void VisitUnionTypeMemberSyntax(UnionTypeMemberSyntax syntax)
            => AssignType(syntax, () =>
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
                if (syntax.Type is { } explicitResourceType && declaredType is ResourceType resourceType && !resourceType.DeclaringNamespace.ResourceTypeProvider.HasDefinedType(resourceType.TypeReference))
                {
                    diagnostics.Write(DiagnosticBuilder.ForPosition(explicitResourceType).ResourceTypesUnavailable(resourceType.TypeReference));
                }

                return declaredType;
            });

        public override void VisitParameterizedTypeInstantiationSyntax(ParameterizedTypeInstantiationSyntax syntax)
            => VisitParameterizedTypeInstantiationSyntaxBase(syntax);

        public override void VisitInstanceParameterizedTypeInstantiationSyntax(InstanceParameterizedTypeInstantiationSyntax syntax)
            => VisitParameterizedTypeInstantiationSyntaxBase(syntax);

        private void VisitParameterizedTypeInstantiationSyntaxBase(ParameterizedTypeInstantiationSyntaxBase syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = typeManager.TryGetReifiedType(syntax)?.ExpressedType;
                if (declaredType is null)
                {
                    return ErrorType.Empty();
                }

                diagnostics.WriteMultiple(declaredType.GetDiagnostics());

                // resource<> is deprecated
                if (syntax.Name.IdentifierName.Equals(LanguageConstants.TypeNameResource))
                {
                    diagnostics.Write(DiagnosticBuilder.ForPosition(syntax.Name).ResourceParameterizedTypeIsDeprecated(syntax));
                }

                return declaredType;
            });

        public override void VisitTypePropertyAccessSyntax(TypePropertyAccessSyntax syntax)
            => AssignType(syntax, () =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax) ?? ErrorType.Empty();

                base.VisitTypePropertyAccessSyntax(syntax);

                return declaredType;
            });

        public override void VisitTypeAdditionalPropertiesAccessSyntax(TypeAdditionalPropertiesAccessSyntax syntax)
            => AssignType(syntax, () =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax) ?? ErrorType.Empty();

                base.VisitTypeAdditionalPropertiesAccessSyntax(syntax);

                return declaredType;
            });

        public override void VisitTypeArrayAccessSyntax(TypeArrayAccessSyntax syntax)
            => AssignType(syntax, () =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax) ?? ErrorType.Empty();

                base.VisitTypeArrayAccessSyntax(syntax);

                return declaredType;
            });

        public override void VisitTypeItemsAccessSyntax(TypeItemsAccessSyntax syntax)
            => AssignType(syntax, () =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax) ?? ErrorType.Empty();

                base.VisitTypeItemsAccessSyntax(syntax);

                return declaredType;
            });

        public override void VisitTypeVariableAccessSyntax(TypeVariableAccessSyntax syntax)
            => AssignType(syntax, () =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax) ?? ErrorType.Empty();

                base.VisitTypeVariableAccessSyntax(syntax);

                return declaredType;
            });

        public override void VisitStringTypeLiteralSyntax(StringTypeLiteralSyntax syntax)
            => AssignType(syntax, () =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax) ?? ErrorType.Empty();

                base.VisitStringTypeLiteralSyntax(syntax);

                return declaredType;
            });

        public override void VisitIntegerTypeLiteralSyntax(IntegerTypeLiteralSyntax syntax)
            => AssignType(syntax, () =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax) ?? ErrorType.Empty();

                base.VisitIntegerTypeLiteralSyntax(syntax);

                return declaredType;
            });

        public override void VisitBooleanTypeLiteralSyntax(BooleanTypeLiteralSyntax syntax)
            => AssignType(syntax, () =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax) ?? ErrorType.Empty();

                base.VisitBooleanTypeLiteralSyntax(syntax);

                return declaredType;
            });

        public override void VisitNullTypeLiteralSyntax(NullTypeLiteralSyntax syntax)
            => AssignType(syntax, () => LanguageConstants.Null);

        public override void VisitUnaryTypeOperationSyntax(UnaryTypeOperationSyntax syntax)
            => AssignType(syntax, () =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax) ?? ErrorType.Empty();

                base.VisitUnaryTypeOperationSyntax(syntax);

                return declaredType;
            });

        public override void VisitNonNullableTypeSyntax(NonNullableTypeSyntax syntax)
            => AssignType(syntax, () =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax) ?? ErrorType.Empty();

                base.VisitNonNullableTypeSyntax(syntax);

                return declaredType;
            });

        public override void VisitParenthesizedTypeSyntax(ParenthesizedTypeSyntax syntax)
            => AssignType(syntax, () =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax) ?? ErrorType.Empty();

                base.VisitParenthesizedTypeSyntax(syntax);

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

        public override void VisitExtensionDeclarationSyntax(ExtensionDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                if (binder.GetSymbolInfo(syntax) is not ExtensionNamespaceSymbol namespaceSymbol)
                {
                    // We have syntax or binding errors, which should have already been handled.
                    return ErrorType.Empty();
                }

                if (namespaceSymbol.DeclaredType is not NamespaceType namespaceType)
                {
                    // We should have an error type here - return it directly.
                    return namespaceSymbol.DeclaredType as ErrorType ?? ErrorType.Empty();
                }

                if (features.ModuleExtensionConfigsEnabled && syntax.Path is not null && syntax.TryGetAliasFromAsClause() is null)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).ExtensionAliasMustBeDefinedForInlinedRegistryExtensionDeclaration());
                }

                this.ValidateDecorators(syntax.Decorators, namespaceType, diagnostics);

                if (syntax.Config is not null)
                {
                    if (namespaceType.ConfigurationType is null)
                    {
                        diagnostics.Write(syntax.Config, x => x.ExtensionDoesNotSupportConfiguration(namespaceType.ExtensionName));
                    }
                    else
                    {
                        // When module extension configs are used, exclude required property checks because those properties can be provided on the deployment properties and on the backend, will be merged in and validated.
                        var assignmentTargetType = namespaceType.ConfigurationType;

                        if (features.ModuleExtensionConfigsEnabled)
                        {
                            assignmentTargetType = assignmentTargetType switch
                            {
                                ObjectType assignmentTargetObjType => assignmentTargetObjType
                                    .WithModifiedProperties(p => p.WithoutFlags(TypePropertyFlags.Required)),
                                DiscriminatedObjectType assignmentTargetDiscrimObjType => assignmentTargetDiscrimObjType
                                    .WithModifiedMembers(memberType =>
                                        memberType.WithModifiedProperties(p =>
                                            LanguageConstants.IdentifierComparer.Equals(p.Name, assignmentTargetDiscrimObjType.DiscriminatorKey)
                                                ? p
                                                : p.WithoutFlags(TypePropertyFlags.Required))),
                                _ => assignmentTargetType
                            };
                        }

                        // Collect diagnostics for the configuration type assignment.
                        TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, this.parsingErrorLookup, diagnostics, syntax.Config, assignmentTargetType.Type, false);
                    }
                }
                else
                {
                    // When module extension configs are used, exclude required property checks because those properties can be provided on the deployment properties and on the backend, will be merged in and validated.
                    if (!features.ModuleExtensionConfigsEnabled && syntax.WithClause.IsSkipped && namespaceType.ConfigurationType is not null)
                    {
                        var isConfigurationRequired = namespaceType.ConfigurationType switch
                        {
                            ObjectType extConfigObjType => extConfigObjType.Properties.Values.Any(p => p.Flags.HasFlag(TypePropertyFlags.Required)),
                            _ => true
                        };

                        if (isConfigurationRequired)
                        {
                            diagnostics.Write(syntax, x => x.ExtensionRequiresConfiguration(namespaceType.ExtensionName));
                        }
                    }
                }

                return namespaceType; // Return the namespace type as this represents the extension as a whole.
            });

        public override void VisitExtensionConfigAssignmentSyntax(ExtensionConfigAssignmentSyntax syntax)
            => AssignTypeWithDiagnostics(
                syntax, diagnostics =>
                {
                    if (!features.ModuleExtensionConfigsEnabled)
                    {
                        return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).UnrecognizedParamsFileDeclaration(false));
                    }

                    if (binder.GetSymbolInfo(syntax) is not ExtensionConfigAssignmentSymbol configAssignmentSymbol)
                    {
                        // We have syntax or binding errors, which should have already been handled.
                        return ErrorType.Empty();
                    }

                    // The declared type is the module-aware configuration type when moduleConfigsEnabled is true (defaulted config properties in the template are considered optional from this perspective).
                    var moduleAwareExtConfigType = typeManager.GetDeclaredType(syntax);

                    base.VisitExtensionConfigAssignmentSyntax(syntax);

                    if (moduleAwareExtConfigType is null or ErrorType) // Ext does not support configuration
                    {
                        diagnostics.Write(syntax.Alias, x => x.ExtensionDoesNotSupportConfiguration(configAssignmentSymbol.Name));

                        return ErrorType.Empty();
                    }

                    if (syntax.Config is not null)
                    {
                        TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, this.parsingErrorLookup, diagnostics, syntax.Config, moduleAwareExtConfigType, false);
                    }
                    else if (syntax.WithClause.IsSkipped)
                    {
                        var isConfigurationRequired = moduleAwareExtConfigType switch
                        {
                            ObjectType moduleAwareExtConfigObjType => moduleAwareExtConfigObjType.Properties.Values.Any(p => p.Flags.HasFlag(TypePropertyFlags.Required)),
                            _ => true
                        };

                        if (isConfigurationRequired)
                        {
                            diagnostics.Write(syntax, x => x.ExtensionRequiresConfiguration(configAssignmentSymbol.Name));
                        }
                    }

                    return moduleAwareExtConfigType;
                });

        public override void VisitUsingWithClauseSyntax(UsingWithClauseSyntax syntax)
            => AssignTypeWithDiagnostics(
                syntax, diagnostics =>
                {
                    if (typeManager.GetDeclaredType(syntax.Config) is not { } configType)
                    {
                        return ErrorType.Empty();
                    }

                    return TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, this.parsingErrorLookup, diagnostics, syntax.Config, configType, false);
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
                var errors = new List<IDiagnostic>();

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
                if (syntax.Type is { })
                {
                    var declaredType = GetDeclaredTypeAndValidateDecorators(syntax, syntax.Type, diagnostics);
                    diagnostics.WriteMultiple(GetDeclarationAssignmentDiagnostics(declaredType, syntax.Value));

                    return declaredType;
                }

                var errors = new List<IDiagnostic>();

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
                var errors = new List<IDiagnostic>();

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
                diagnostics.WriteMultiple(GetDeclarationAssignmentDiagnostics(declaredType, syntax.Value));

                base.VisitOutputDeclarationSyntax(syntax);

                return declaredType;
            });

        public override void VisitAssertDeclarationSyntax(AssertDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var errors = new List<IDiagnostic>();

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
                base.VisitCompileTimeImportDeclarationSyntax(syntax);

                return GetTypeInfo(syntax.ImportExpression);
            });

        public override void VisitWildcardImportSyntax(WildcardImportSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                base.VisitWildcardImportSyntax(syntax);

                if (binder.GetParent(syntax) is not CompileTimeImportDeclarationSyntax importDeclarationSyntax ||
                    !this.model.TryGetReferencedModel(importDeclarationSyntax).IsSuccess(out var importedModel))
                {
                    return ErrorType.Empty();
                }

                List<NamedTypeProperty> nsProperties = new();
                List<FunctionOverload> nsFunctions = new();
                foreach (var export in importedModel.Exports.Values)
                {
                    diagnostics.WriteMultiple(resourceDerivedTypeDiagnosticReporter.ReportResourceDerivedTypeDiagnostics(export.TypeReference)
                        .Select(builder => builder(DiagnosticBuilder.ForPosition(syntax.Wildcard))));

                    if (export is ExportedFunctionMetadata exportedFunction)
                    {
                        nsFunctions.Add(TypeHelper.OverloadWithResolvedTypes(resourceDerivedTypeResolver, exportedFunction));
                    }
                    else
                    {
                        nsProperties.Add(new(export.Name,
                            resourceDerivedTypeResolver.ResolveResourceDerivedTypes(export.TypeReference.Type),
                            TypePropertyFlags.ReadOnly | TypePropertyFlags.Required,
                            export.Description));
                    }
                }

                return new NamespaceType(syntax.Name.IdentifierName,
                    new(IsSingleton: true,
                        BicepExtensionName: syntax.Name.IdentifierName,
                        ConfigurationType: null,
                        TemplateExtensionName: syntax.Name.IdentifierName,
                        TemplateExtensionVersion: "1.0.0"),
                    nsProperties,
                    nsFunctions,
                    ImmutableArray<BannedFunction>.Empty,
                    ImmutableArray<Decorator>.Empty,
                    new EmptyResourceTypeProvider());
            });

        public override void VisitImportedSymbolsListSyntax(ImportedSymbolsListSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                base.VisitImportedSymbolsListSyntax(syntax);

                // Unlike a wildcard import, the intermediate syntax node surrounding the symbols declared by a statement like `import {foo, bar, baz} from 'main.bicep'`
                // doesn't declare a single dereferenceable symbol.
                return LanguageConstants.Never;
            });

        public override void VisitImportedSymbolsListItemSyntax(ImportedSymbolsListItemSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                base.VisitImportedSymbolsListItemSyntax(syntax);

                if (binder.GetParent(syntax) is not { } parentSyntax ||
                    binder.GetParent(parentSyntax) is not CompileTimeImportDeclarationSyntax importDeclarationSyntax ||
                    !model.TryGetReferencedModel(importDeclarationSyntax).IsSuccess(out var importedModel) ||
                    syntax.TryGetOriginalSymbolNameText() is not string importTarget ||
                    importedModel.Exports.TryGetValue(importTarget) is not { } exported)
                {
                    return ErrorType.Empty();
                }

                diagnostics.WriteMultiple(resourceDerivedTypeDiagnosticReporter.ReportResourceDerivedTypeDiagnostics(exported.TypeReference)
                    .Select(builder => builder(DiagnosticBuilder.ForPosition(syntax))));
                return resourceDerivedTypeResolver.ResolveResourceDerivedTypes(exported.TypeReference.Type);
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

                var errors = new List<IDiagnostic>();
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
                if (ArmFunctionReturnTypeEvaluator.TryEvaluate("format", out _, TypeFactory.CreateStringLiteralType(StringFormatConverter.BuildFormatString(syntax.SegmentValues)).AsEnumerable().Concat(expressionTypes)) is { } folded)
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
            => AssignType(syntax, () => syntax.Value switch
            {
                <= long.MaxValue => TypeFactory.CreateIntegerLiteralType((long)syntax.Value),
                _ => LanguageConstants.Int,
            });

        public override void VisitNullLiteralSyntax(NullLiteralSyntax syntax)
            => AssignType(syntax, () => LanguageConstants.Null);

        public override void VisitSkippedTriviaSyntax(SkippedTriviaSyntax syntax)
            => AssignType(syntax, () =>
            {
                // error should have already been raised by the ParseDiagnosticsVisitor - no need to add another
                return ErrorType.Create([]);
            });

        public override void VisitObjectSyntax(ObjectSyntax syntax)
            => AssignType(syntax, () =>
            {
                foreach (var missingDeclarationSyntax in syntax.Children.OfType<MissingDeclarationSyntax>())
                {
                    VisitMissingDeclarationSyntax(missingDeclarationSyntax);
                }

                var errors = new List<IDiagnostic>();

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

                var childTypes = new List<TypeSymbol>();
                foreach (var child in syntax.Children)
                {
                    var childType = child switch
                    {
                        ObjectPropertySyntax x => typeManager.GetTypeInfo(x),
                        SpreadExpressionSyntax x => typeManager.GetTypeInfo(x.Expression),
                        _ => null,
                    };

                    if (childType is null)
                    {
                        continue;
                    }

                    if (child is SpreadExpressionSyntax spread &&
                        childType is not ErrorType &&
                        !TypeValidator.AreTypesAssignable(childType, LanguageConstants.Object))
                    {
                        childType = ErrorType.Create(DiagnosticBuilder.ForPosition(child).SpreadOperatorRequiresAssignableValue(spread, LanguageConstants.Object));
                    }

                    CollectErrors(errors, childType);
                    childTypes.Add(childType);
                }

                if (PropagateErrorType(errors, childTypes))
                {
                    return ErrorType.Create(errors);
                }

                // Discriminated objects should have been resolved by the declared type manager.
                var declaredType = typeManager.GetDeclaredType(syntax);

                var namedProperties = new Dictionary<string, NamedTypeProperty>(LanguageConstants.IdentifierComparer);
                var additionalProperties = new List<TypeSymbol>();
                foreach (var child in syntax.Children)
                {
                    if (child is ObjectPropertySyntax propertySyntax)
                    {
                        var resolvedType = typeManager.GetTypeInfo(propertySyntax);

                        if (propertySyntax.TryGetKeyText() is { } name)
                        {
                            if (declaredType is ObjectType objectType && objectType.Properties.TryGetValue(name, out var property))
                            {
                                // we've found a declared object type for the containing object, with a matching property name definition.
                                // preserve the type property details (name, descriptions etc.), and update the assigned type.
                                // Since this type corresponds to a value that is being supplied, make sure it has the `Required` flag and does not have the `.ReadOnly` flag
                                namedProperties[name] = new NamedTypeProperty(
                                    property.Name,
                                    resolvedType,
                                    (property.Flags | TypePropertyFlags.Required) & ~TypePropertyFlags.ReadOnly,
                                    property.Description);
                            }
                            else
                            {
                                // we've not been able to find a declared object type for the containing object, or it doesn't contain a property matching this one.
                                // best we can do is to simply generate a property for the assigned type.
                                namedProperties[name] = new NamedTypeProperty(name, resolvedType, TypePropertyFlags.Required);
                            }
                        }
                        else
                        {
                            additionalProperties.Add(resolvedType);
                        }
                    }

                    if (child is SpreadExpressionSyntax spreadSyntax)
                    {
                        var type = typeManager.GetTypeInfo(spreadSyntax.Expression);
                        if (type is ObjectType spreadType)
                        {
                            foreach (var (name, property) in spreadType.Properties)
                            {
                                namedProperties[name] = property;
                            }

                            if (spreadType.AdditionalProperties is { } spreadTypeAdditionalProperties)
                            {
                                additionalProperties.Add(spreadTypeAdditionalProperties.TypeReference.Type);
                            }
                        }
                        else
                        {
                            additionalProperties.Add(LanguageConstants.Any);
                        }
                    }
                }

                var additionalPropertiesType = additionalProperties.Any() ? TypeHelper.CreateTypeUnion(additionalProperties) : null;

                // TODO: Add structural naming?
                return new ObjectType(LanguageConstants.Object.Name, TypeSymbolValidationFlags.Default, namedProperties.Values, additionalPropertiesType is null ? null : new(additionalPropertiesType));
            });

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
            => AssignType(syntax, () =>
            {
                var errors = new List<IDiagnostic>();
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
                var errors = new List<IDiagnostic>();

                var childTypes = new List<TypeSymbol>(syntax.Children.Length);

                foreach (var child in syntax.Children)
                {
                    var childType = child switch
                    {
                        ArrayItemSyntax x => typeManager.GetTypeInfo(x.Value),
                        SpreadExpressionSyntax x => typeManager.GetTypeInfo(x.Expression),
                        _ => null,
                    };

                    if (childType is null)
                    {
                        continue;
                    }

                    if (child is SpreadExpressionSyntax spread &&
                        childType is not ErrorType &&
                        !TypeValidator.AreTypesAssignable(childType, LanguageConstants.Array))
                    {
                        childType = ErrorType.Create(DiagnosticBuilder.ForPosition(child).SpreadOperatorRequiresAssignableValue(spread, LanguageConstants.Array));
                    }

                    CollectErrors(errors, childType);
                    childTypes.Add(childType);
                }

                if (PropagateErrorType(errors, childTypes))
                {
                    return ErrorType.Create(errors);
                }

                var tupleEntries = new List<TypeSymbol>();
                var itemEntries = new List<TypeSymbol>();
                foreach (var child in syntax.Children)
                {
                    if (child is ArrayItemSyntax arrayItemSyntax)
                    {
                        tupleEntries.Add(typeManager.GetTypeInfo(child));
                    }
                    else if (child is SpreadExpressionSyntax spreadSyntax)
                    {
                        var type = typeManager.GetTypeInfo(spreadSyntax.Expression);
                        if (type is TupleType spreadType)
                        {
                            tupleEntries.AddRange(spreadType.Items.Select(x => x.Type));
                        }
                        else if (type is ArrayType arrayType)
                        {
                            itemEntries.Add(arrayType.Item.Type);
                        }
                        else
                        {
                            itemEntries.Add(LanguageConstants.Any);
                        }
                    }
                }

                if (itemEntries.Any())
                {
                    var itemType = TypeHelper.CreateTypeUnion(tupleEntries.Concat(itemEntries));
                    return new TypedArrayType(itemType, TypeSymbolValidationFlags.Default);
                }

                return new TupleType([.. tupleEntries], TypeSymbolValidationFlags.Default);
            });

        public override void VisitTernaryOperationSyntax(TernaryOperationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var errors = new List<IDiagnostic>();

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

                return conditionType switch
                {
                    BooleanLiteralType { Value: true } => trueType,
                    BooleanLiteralType => falseType,
                    // the return type is the union of true and false expression types
                    _ => TypeHelper.CollapseOrCreateTypeUnion(trueType, falseType),
                };
            });

        public override void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var errors = new List<IDiagnostic>();

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
                if (OperationReturnTypeEvaluator.TryFoldBinaryExpression(syntax, operandType1, operandType2, diagnostics) is { } result)
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
                var errors = new List<IDiagnostic>();

                var operandType = typeManager.GetTypeInfo(syntax.Expression);
                CollectErrors(errors, operandType);

                if (PropagateErrorType(errors, operandType))
                {
                    return ErrorType.Create(errors);
                }

                // operand doesn't appear to have errors
                // let's fold the expression so that an operation with a literal typed operand will have a literal return type
                if (OperationReturnTypeEvaluator.TryFoldUnaryExpression(syntax.Operator, operandType, diagnostics) is { } result)
                {
                    return result;
                }

                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).UnaryOperatorInvalidType(Operators.UnaryOperatorToText[syntax.Operator], operandType));
            });

        private static bool PropagateErrorType(IEnumerable<IDiagnostic> diagnostics, params TypeSymbol[] types)
            => PropagateErrorType(diagnostics, types as IEnumerable<TypeSymbol>);

        private static bool PropagateErrorType(IEnumerable<IDiagnostic> diagnostics, IEnumerable<TypeSymbol> types)
        {
            if (diagnostics.Any(x => x.IsError()))
            {
                return true;
            }

            return types.Any(x => x.TypeKind == TypeKind.Error);
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics => GetAccessedType(syntax, diagnostics));

        private TypeSymbol GetArrayItemType(
            ArrayAccessSyntax syntax,
            IDiagnosticWriter diagnostics,
            TypeSymbol baseType,
            Symbol? baseSymbol,
            TypeSymbol indexType)
        {
            var errors = new List<IDiagnostic>();
            CollectErrors(errors, baseType);
            CollectErrors(errors, indexType);

            if (PropagateErrorType(errors, baseType, indexType))
            {
                return ErrorType.Create(errors);
            }

            baseType = UnwrapType(baseType);

            // if the index type is nullable but otherwise valid, emit a fixable warning
            if (TypeHelper.TryRemoveNullability(indexType) is { } nonNullableIndex)
            {
                var withNonNullableIndex = GetArrayItemType(syntax, diagnostics, baseType, baseSymbol, nonNullableIndex);

                if (withNonNullableIndex is not ErrorType)
                {
                    diagnostics.Write(DiagnosticBuilder.ForPosition(syntax.IndexExpression).PossibleNullReferenceAssignment(nonNullableIndex, indexType, syntax.IndexExpression));
                }

                return withNonNullableIndex;
            }

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

            switch (baseType)
            {
                case TypeSymbol when TypeHelper.TryRemoveNullability(baseType) is TypeSymbol nonNullableBaseType:
                    diagnostics.Write(DiagnosticBuilder.ForPosition(TextSpan.Between(syntax.OpenSquare, syntax.CloseSquare))
                        .DereferenceOfPossiblyNullReference(baseType.Name, syntax));

                    return GetArrayItemType(syntax, diagnostics, nonNullableBaseType, baseSymbol, indexType);

                case TypeSymbol when IsPotentiallyDisabledResourceOrModule(baseSymbol):
                    diagnostics.Write(DiagnosticBuilder.ForPosition(TextSpan.Between(syntax.OpenSquare, syntax.CloseSquare))
                        .DereferenceOfPossiblyNullReference(TypeHelper.CreateTypeUnion(baseType, LanguageConstants.Null).Name, syntax));

                    return GetArrayItemType(syntax, diagnostics, baseType, baseSymbol: null, indexType);

                case AnyType:
                    // base expression is of type any
                    if (indexType.TypeKind == TypeKind.Any)
                    {
                        // index is also of type any
                        return LanguageConstants.Any;
                    }

                    if (TypeValidator.AreTypesAssignable(indexType, LanguageConstants.String) &&
                        syntax.FromEndMarker is not null)
                    {
                        return InvalidAccessExpression(
                            DiagnosticBuilder.ForPosition(syntax.FromEndMarker)
                                .FromEndArrayAccessNotSupportedWithIndexType(indexType),
                            diagnostics,
                            syntax.IsSafeAccess);
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
                    return GetTypeAtIndex(baseTuple, integerLiteralIndex, syntax.IndexExpression, syntax.FromEndMarker is not null);

                case TupleType baseTuple when indexType is UnionType indexUnion && indexUnion.Members.All(t => t.Type is IntegerLiteralType):
                    var possibilities = indexUnion.Members.Select(t => t.Type)
                        .OfType<IntegerLiteralType>()
                        .Select(index => GetTypeAtIndex(baseTuple, index, syntax.IndexExpression, syntax.FromEndMarker is not null))
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

                case ObjectType or DiscriminatedObjectType when syntax.FromEndMarker is not null:
                    return InvalidAccessExpression(
                        DiagnosticBuilder.ForPosition(syntax.FromEndMarker)
                            .FromEndArrayAccessNotSupportedOnBaseType(baseType),
                        diagnostics,
                        syntax.IsSafeAccess);

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
                                return TypeHelper.GetNamedPropertyType(baseObject,
                                    syntax.IndexExpression,
                                    literalIndex.RawStringValue,
                                    syntax.IsSafeAccess,
                                    shouldWarn: syntax.IsSafeAccess || TypeValidator.ShouldWarnForPropertyMismatch(baseObject),
                                    diagnostics);
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
                            .Select(baseMemberType => GetArrayItemType(syntax, diagnostics, baseMemberType.Type, baseSymbol, indexType))
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

        private static TypeSymbol InvalidAccessExpression(Diagnostic error, IDiagnosticWriter diagnostics, bool safeAccess)
        {
            if (safeAccess)
            {
                diagnostics.Write(error with { Level = DiagnosticLevel.Warning });
                return LanguageConstants.Null;
            }
            return ErrorType.Create(error);
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics => GetAccessedType(syntax, diagnostics));

        private static (SyntaxBase unwrapped, bool nonNullAsserted) UnwrapParenthesesAndAssertions(SyntaxBase syntax)
            => syntax switch
            {
                NonNullAssertionSyntax nonNullAssertion
                    => (UnwrapParenthesesAndAssertions(nonNullAssertion.BaseExpression).unwrapped, true),
                ParenthesizedExpressionSyntax parenthesized
                    => UnwrapParenthesesAndAssertions(parenthesized.Expression),
                _ => (syntax, false),
            };

        private TypeSymbol GetAccessedType(AccessExpressionSyntax syntax, IDiagnosticWriter diagnostics)
        {
            Stack<AccessExpressionSyntax> chainedAccesses = syntax.ToAccessExpressionStack();

            var baseType = typeManager.GetTypeInfo(chainedAccesses.Peek().BaseExpression);

            var nullVariantRemoved = false;
            AccessExpressionSyntax? prevAccess = null;
            Symbol? prevBaseSymbol = null;
            while (chainedAccesses.TryPop(out var nextAccess))
            {
                var (unwrappedBase, nonNullAsserted) = UnwrapParenthesesAndAssertions(nextAccess.BaseExpression);
                var baseSymbol = nonNullAsserted ? null : binder.GetSymbolInfo(unwrappedBase);

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
                    else if (nextAccess.IsSafeAccess && (
                        // this access expression is a safe dereference of a resource or module property (`res.?properties` or `mod.?outputs`)
                        IsPotentiallyDisabledResourceOrModule(baseSymbol) ||
                        // this access expression is a safe dereference of a property of an element of a resource or module collection (`res[0].?properties` or `mod[0].?outputs`)
                        (IsPotentiallyDisabledResourceOrModule(prevBaseSymbol) && prevAccess is ArrayAccessSyntax)))
                    {
                        nullVariantRemoved = true;
                        baseSymbol = null;
                    }
                }

                DeclaredSymbol? baseSymbolToUse = prevBaseSymbol switch
                {
                    DeclaredSymbol ds when binder.TryGetCycle(ds).HasValue ||
                        syntax.Span.IsNil ||
                        binder.IsDescendant(syntax, ds.DeclaringSyntax) => null,
                    ResourceSymbol rs when rs.IsCollection => rs,
                    ModuleSymbol ms when ms.IsCollection => ms,
                    _ => baseSymbol switch
                    {
                        DeclaredSymbol ds when binder.TryGetCycle(ds).HasValue ||
                            syntax.Span.IsNil ||
                            binder.IsDescendant(syntax, ds.DeclaringSyntax) => null,
                        ResourceSymbol rs when !rs.IsCollection => rs,
                        ModuleSymbol ms when !ms.IsCollection => ms,
                        _ => null,
                    },
                };

                baseType = nextAccess switch
                {
                    ArrayAccessSyntax arrayAccess => GetArrayItemType(arrayAccess, diagnostics, baseType, baseSymbolToUse, typeManager.GetTypeInfo(arrayAccess.IndexExpression)),
                    PropertyAccessSyntax propertyAccess => GetNamedPropertyType(propertyAccess, baseType, baseSymbolToUse, diagnostics),
                    _ => throw new InvalidOperationException("Unrecognized access syntax"),
                };

                prevAccess = nextAccess;
                prevBaseSymbol = baseSymbol;
            }

            return nullVariantRemoved
                ? TypeHelper.CreateTypeUnion(baseType, LanguageConstants.Null)
                : baseType;
        }

        private TypeSymbol GetNamedPropertyType(PropertyAccessSyntax syntax, TypeSymbol baseType, Symbol? baseSymbol, IDiagnosticWriter diagnostics)
            => UnwrapType(baseType) switch
            {
                ErrorType error => error,
                TypeSymbol withErrors when withErrors.GetDiagnostics().Any(d => d.Level == DiagnosticLevel.Error)
                    => ErrorType.Create(withErrors.GetDiagnostics()),

                TypeSymbol original when TypeHelper.TryRemoveNullability(original) is TypeSymbol nonNullable
                    => EmitNullablePropertyAccessDiagnosticAndEraseNullability(syntax, original, nonNullable, diagnostics),

                TypeSymbol original when IsPotentiallyDisabledResourceOrModule(baseSymbol) && !IsResourceInfoProperty(baseSymbol, syntax.PropertyName.IdentifierName)
                    => EmitNullablePropertyAccessDiagnosticAndEraseNullability(syntax, TypeHelper.CreateTypeUnion(original, LanguageConstants.Null), original, diagnostics),

                // the property is not valid
                // there's already a parse error for it, so we don't need to add a type error as well
                ObjectType when !syntax.PropertyName.IsValid => ErrorType.Empty(),

                ObjectType objectType => TypeHelper.GetNamedPropertyType(objectType,
                    syntax.PropertyName,
                    syntax.PropertyName.IdentifierName,
                    syntax.IsSafeAccess,
                    syntax.IsSafeAccess || TypeValidator.ShouldWarnForPropertyMismatch(objectType),
                    diagnostics),

                UnionType unionType when syntax.PropertyName.IsValid => TypeHelper.GetNamedPropertyType(unionType,
                    syntax.PropertyName,
                    syntax.PropertyName.IdentifierName,
                    syntax.IsSafeAccess,
                    syntax.IsSafeAccess || TypeValidator.ShouldWarnForPropertyMismatch(unionType),
                    diagnostics),

                // TODO: We might be able use the declared type here to resolve discriminator to improve the assigned type
                DiscriminatedObjectType => LanguageConstants.Any,

                // We can assign to an object, but we don't have a type for that object.
                // The best we can do is allow it and return the 'any' type.
                TypeSymbol maybeObject when TypeValidator.AreTypesAssignable(maybeObject, LanguageConstants.Object) => LanguageConstants.Any,

                // can only access properties of objects
                TypeSymbol otherwise => ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.PropertyName).ObjectRequiredForPropertyAccess(otherwise)),
            };

        private TypeSymbol EmitNullablePropertyAccessDiagnosticAndEraseNullability(PropertyAccessSyntax syntax, TypeSymbol originalBaseType, TypeSymbol nonNullableBaseType, IDiagnosticWriter diagnostics)
        {
            diagnostics.Write(DiagnosticBuilder.ForPosition(TextSpan.Between(syntax.Dot, syntax.PropertyName)).DereferenceOfPossiblyNullReference(originalBaseType.Name, syntax));

            return GetNamedPropertyType(syntax, nonNullableBaseType, baseSymbol: null, diagnostics);
        }

        private bool IsPotentiallyDisabledResourceOrModule(Symbol? symbol) => symbol switch
        {
            ResourceSymbol resourceSymbol => IsResourceEnabled(resourceSymbol) is not true,
            ModuleSymbol moduleSymbol => moduleSymbol.DeclaringModule.TryGetCondition() is { } condition &&
                GetTypeInfo(condition) is not BooleanLiteralType { Value: true },
            _ => false,
        };

        private bool? IsResourceEnabled(ResourceSymbol resource)
        {
            if (resource.DeclaringResource.TryGetCondition() is { } condition)
            {
                switch (GetTypeInfo(condition))
                {
                    case BooleanLiteralType { Value: false }:
                        // if the resource condition is false, that's definitive
                        return false;
                    case BooleanType:
                        // we can't resolve the resource condition at compile time
                        return null;
                }
            }

            if (TryGetEnclosingResource(resource.DeclaringResource) is ResourceSymbol syntacticAncestor)
            {
                // nested resource conditions stack. This resource either doesn't have a condition or has a condition
                // that is definitely `true`, so check its parent
                return IsResourceEnabled(syntacticAncestor);
            }

            return true;
        }

        private ResourceSymbol? TryGetEnclosingResource(SyntaxBase syntax) => binder.GetParent(syntax) switch
        {
            ResourceDeclarationSyntax rds => binder.GetSymbolInfo(rds) as ResourceSymbol,
            SyntaxBase otherwise => TryGetEnclosingResource(otherwise),
            _ => null,
        };

        private bool IsResourceInfoProperty(Symbol? symbol, string propertyName) => symbol switch
        {
            ResourceSymbol resource when resource.TryGetResourceType()?.IsAzResource() is true
                => EmitConstants.ResourceInfoProperties.Contains(propertyName),
            ModuleSymbol => propertyName == LanguageConstants.ModuleNamePropertyName,
            _ => false,
        };

        public override void VisitResourceAccessSyntax(ResourceAccessSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var errors = new List<IDiagnostic>();

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
                var errors = new List<IDiagnostic>();

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

                    case ImportedFunctionSymbol importedFunction:
                        return GetFunctionSymbolType(importedFunction, syntax, errors, diagnostics);

                    case Symbol symbolInfo when binder.NamespaceResolver
                            .GetKnownFunctions(symbolInfo.Name, binder.GetParent(syntax) is DecoratorSyntax)
                            .FirstOrDefault() is { } knownFunction:
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
                var returnType = this.GetTypeInfo(syntax.Body);

                return new LambdaType([.. argumentTypes], [], returnType);
            });

        public override void VisitTypedLambdaSyntax(TypedLambdaSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax);
                if (declaredType is not LambdaType declaredLambdaType)
                {
                    return declaredType ?? ErrorType.Empty();
                }

                base.VisitTypedLambdaSyntax(syntax);

                var errors = new List<IDiagnostic>();
                foreach (var argumentType in declaredLambdaType.ArgumentTypes.Concat(declaredLambdaType.OptionalArgumentTypes))
                {
                    CollectErrors(errors, argumentType.Type);
                }

                var returnType = TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, this.parsingErrorLookup, diagnostics, syntax.Body, declaredLambdaType.ReturnType.Type);
                CollectErrors(errors, returnType);

                return new LambdaType(declaredLambdaType.ArgumentTypes, declaredLambdaType.OptionalArgumentTypes, returnType);
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
                var errors = new List<IDiagnostic>();

                var baseType = typeManager.GetTypeInfo(syntax.BaseExpression);
                CollectErrors(errors, baseType);

                if (PropagateErrorType(errors, baseType))
                {
                    return ErrorType.Create(errors);
                }

                baseType = UnwrapType(baseType);
                var (unwrapped, nonNullAsserted) = UnwrapParenthesesAndAssertions(syntax.BaseExpression);

                if (!nonNullAsserted &&
                    IsPotentiallyDisabledResourceOrModule(binder.GetSymbolInfo(unwrapped)))
                {
                    diagnostics.Write(DiagnosticBuilder.ForPosition(syntax.Name)
                        .InstanceFunctionCallOnPossiblyNullBase(TypeHelper.CreateTypeUnion(baseType, LanguageConstants.Null), syntax.Name));
                }

                if (baseType is not ObjectType objectType)
                {
                    // can only access methods on objects
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Name).ObjectRequiredForMethodAccess(baseType));
                }

                foreach (TypeSymbol argumentType in this.GetArgumentTypes(syntax.Arguments))
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

                var declaredType = TargetScopeSyntax.GetDeclaredType(features);
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

            var errors = new List<IDiagnostic>();

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
                    FunctionFlags.ParameterOutputOrTypeDecorator => builder.ExpectedParameterOutputOrTypeDeclarationAfterDecorator(),
                    FunctionFlags.ParameterOrTypeDecorator => builder.ExpectedParameterOrTypeDeclarationAfterDecorator(),
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

                    case TypeAliasSymbol:
                    case AmbientTypeSymbol:
                    case ImportedTypeSymbol:
                        return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Name.Span).TypeSymbolUsedAsValue(syntax.Name.IdentifierName));

                    case ResourceSymbol resource:
                        // resource bodies can participate in cycles
                        // need to explicitly force a type check on the body
                        return new DeferredTypeReference(() => VisitDeclaredSymbol(syntax, resource));

                    case ModuleSymbol module:
                        return new DeferredTypeReference(() => VisitDeclaredSymbol(syntax, module));

                    case TestSymbol test:
                        return new DeferredTypeReference(() => VisitDeclaredSymbol(syntax, test));

                    case ParameterSymbol parameter:
                        return new DeferredTypeReference(() => VisitDeclaredSymbol(syntax, parameter));

                    case ParameterAssignmentSymbol parameter:
                        return new DeferredTypeReference(() => VisitDeclaredSymbol(syntax, parameter));

                    case VariableSymbol variable:
                        return new DeferredTypeReference(() => VisitDeclaredSymbol(syntax, variable));

                    case LocalVariableSymbol local:
                        return new DeferredTypeReference(() => VisitDeclaredSymbol(syntax, local));

                    case ImportedSymbol imported:
                        return imported.Type;

                    case ExtensionNamespaceSymbol provider:
                        return new DeferredTypeReference(() => VisitDeclaredSymbol(syntax, provider));

                    case BuiltInNamespaceSymbol @namespace:
                        return @namespace.Type;

                    case WildcardImportSymbol wildcardImport:
                        return wildcardImport.Type;

                    case BaseParametersSymbol baseParameters:
                        return new DeferredTypeReference(() => VisitDeclaredSymbol(syntax, baseParameters));

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

        private static void CollectErrors(List<IDiagnostic> errors, ITypeReference reference)
        {
            errors.AddRange(reference.Type.GetDiagnostics());
        }

        private TypeSymbol GetFunctionSymbolType(
            IFunctionSymbol function,
            FunctionCallSyntaxBase syntax,
            IList<IDiagnostic> diagnostics,
            IDiagnosticWriter diagnosticWriter) => GetFunctionSymbolType(function,
                syntax,
                // Recover argument type errors so we can continue type checking for the parent function call.
                [.. GetRecoveredArgumentTypes(syntax.Arguments)],
                diagnostics,
                diagnosticWriter);

        private TypeSymbol GetFunctionSymbolType(
            IFunctionSymbol function,
            FunctionCallSyntaxBase syntax,
            ImmutableArray<TypeSymbol> argumentTypes,
            IList<IDiagnostic> diagnostics,
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
                                [.. Enumerable.Range(0, argumentTypes.Length).Select(i => tm.ArgumentIndex == i ? nonNullableArgType : argumentTypes[i])],
                                diagnostics,
                                diagnosticWriter);

                            // if we couldn't find a match even after tweaking argument nullability, don't add any nullability warnings
                            if (resultSansNullability is ErrorType error)
                            {
                                return error;
                            }

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

                        diagnostics.Add(DiagnosticBuilder.ForPosition(argumentSyntax).CannotResolveFunctionOverload(signatures, argumentType, parameterTypes));
                    }
                    else
                    {
                        // Choose the type mismatch that has the largest index as the best one.
                        var (_, argumentIndex, argumentType, parameterType) = typeMismatches.OrderBy(tm => tm.ArgumentIndex).Last();

                        diagnostics.Add(DiagnosticBuilder.ForPosition(syntax.GetArgumentByPosition(argumentIndex)).ArgumentTypeMismatch(argumentType, parameterType));
                    }
                }
                else if (countMismatches.Any())
                {
                    // Argument type mismatch wins over count mismatch. Handle count mismatch only when there's no type mismatch.
                    var (actualCount, minimumArgumentCount, maximumArgumentCount) = countMismatches.Aggregate(ArgumentCountMismatch.Reduce);
                    var argumentsSpan = TextSpan.Between(syntax.OpenParen, syntax.CloseParen);

                    diagnostics.Add(DiagnosticBuilder.ForPosition(argumentsSpan).ArgumentCountMismatch(actualCount, minimumArgumentCount, maximumArgumentCount));
                }
            }

            if (PropagateErrorType(diagnostics))
            {
                return ErrorType.Create(diagnostics);
            }

            if (matches.Count == 1)
            {
                // we have an exact match or a single ambiguous match
                var matchedOverload = matches.Single();
                matchedFunctionOverloads.TryAdd(syntax, matchedOverload);

                // do detailed type validation now we have a match
                for (var i = 0; i < syntax.Arguments.Length; i++)
                {
                    var argumentSyntax = syntax.Arguments[i];

                    var parameterFlags = matchedOverload.FixedParameters.Length > i
                        ? matchedOverload.FixedParameters[i].Flags
                        : matchedOverload.VariableParameter?.Flags ?? FunctionParameterFlags.None;

                    if (parameterFlags.HasFlag(FunctionParameterFlags.Constant))
                    {
                        TypeValidator.GetCompileTimeConstantViolation(argumentSyntax, diagnosticWriter);
                    }

                    var targetType = matchedOverload.GetArgumentType(
                        index: i,
                        getFunctionArgumentType: i => GetTypeInfo(syntax.Arguments[i]),
                        getAttachedType: () => binder.GetParent(syntax) is DecoratorSyntax decorator && binder.GetParent(decorator) is DecorableSyntax target
                            ? typeManager.GetDeclaredType(target) ?? ErrorType.Empty()
                            : throw new InvalidOperationException("Cannot get attached type of function that is not used as a decorator"));

                    TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, parsingErrorLookup, diagnosticWriter, argumentSyntax, targetType);
                }

                // return its type
                var result = matchedOverload.ResultBuilder(model, diagnosticWriter, syntax, argumentTypes);
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

            if (baseType.Properties.Any() || baseType.AdditionalProperties != null)
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

        private IEnumerable<IDiagnostic> GetDeclarationAssignmentDiagnostics(TypeSymbol declaredType, SyntaxBase value)
        {
            var valueType = typeManager.GetTypeInfo(value);

            // this type is not a property in a symbol so the semantic error visitor won't collect the errors automatically
            if (valueType is ErrorType)
            {
                return valueType.GetDiagnostics();
            }

            if (declaredType is ErrorType)
            {
                // no point in checking that the value is assignable to the declared output type if no valid declared type could be discerned
                return [];
            }

            var diagnosticWriter = ToListDiagnosticWriter.Create();

            TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, parsingErrorLookup, diagnosticWriter, value, declaredType);

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

        private static bool IsExplicitUnion(SyntaxBase syntax) => syntax switch
        {
            UnionTypeSyntax => true,
            ParenthesizedTypeSyntax parenthesized => IsExplicitUnion(parenthesized.Expression),
            NonNullableTypeSyntax nonNullable => IsExplicitUnion(nonNullable.Base),
            NullableTypeSyntax nullable => IsExplicitUnion(nullable.Base),
            _ => false,
        };

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

            return [];
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
