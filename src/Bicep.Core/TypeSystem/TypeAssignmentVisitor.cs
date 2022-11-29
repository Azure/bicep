// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;
using Bicep.Core.Workspaces;

namespace Bicep.Core.TypeSystem
{
    public sealed class TypeAssignmentVisitor : AstVisitor
    {
        private readonly IFeatureProvider features;
        private readonly ITypeManager typeManager;
        private readonly IBinder binder;
        private readonly IFileResolver fileResolver;
        private readonly ConcurrentDictionary<SyntaxBase, TypeAssignment> assignedTypes;
        private readonly ConcurrentDictionary<FunctionCallSyntaxBase, FunctionOverload> matchedFunctionOverloads;
        private readonly ConcurrentDictionary<FunctionCallSyntaxBase, object> matchedFunctionResultValues;
        private readonly BicepSourceFileKind fileKind;

        public TypeAssignmentVisitor(ITypeManager typeManager, IFeatureProvider features, IBinder binder, IFileResolver fileResolver, Workspaces.BicepSourceFileKind fileKind)
        {
            this.typeManager = typeManager;
            this.features = features;
            this.binder = binder;
            this.fileResolver = fileResolver;
            assignedTypes = new();
            matchedFunctionOverloads = new();
            matchedFunctionResultValues = new();
            this.fileKind = fileKind;
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
        public object? GetMatchedFunctionResultValue(FunctionCallSyntaxBase syntax)
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

        public override void VisitForSyntax(ForSyntax syntax)
            => AssignType(syntax, () =>
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
                    // the array expression isn't actually an array
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Expression).LoopArrayExpressionTypeMismatch(arrayExpressionType));
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

                return TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, diagnostics, syntax.Value, declaredType, true);
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


                return TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, diagnostics, syntax.Value, declaredType);
            });

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = GetDeclaredTypeAndValidateDecorators(syntax, syntax.Type, diagnostics);

                base.VisitParameterDeclarationSyntax(syntax);

                if (syntax.Modifier != null)
                {
                    diagnostics.WriteMultiple(this.ValidateIdentifierAccess(syntax.Modifier));
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

        public override void VisitObjectTypePropertySyntax(ObjectTypePropertySyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = GetDeclaredTypeAndValidateDecorators(syntax, syntax.Value, diagnostics);
                diagnostics.WriteMultiple(declaredType.GetDiagnostics());

                base.VisitObjectTypePropertySyntax(syntax);

                return declaredType;
            });

        public override void VisitTupleTypeItemSyntax(TupleTypeItemSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = GetDeclaredTypeAndValidateDecorators(syntax, syntax.Value, diagnostics);
                diagnostics.WriteMultiple(declaredType.GetDiagnostics());

                base.VisitTupleTypeItemSyntax(syntax);

                return declaredType;
            });

        public override void VisitArrayTypeMemberSyntax(ArrayTypeMemberSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = typeManager.GetDeclaredType(syntax) ?? ErrorType.Empty();
                diagnostics.WriteMultiple(declaredType.GetDiagnostics());

                base.VisitArrayTypeMemberSyntax(syntax);

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

        public override void VisitImportDeclarationSyntax(ImportDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                if (binder.GetSymbolInfo(syntax) is not ImportedNamespaceSymbol namespaceSymbol)
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
                        TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, diagnostics, syntax.Config, namespaceType.ConfigurationType.Type, false);
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

                        decorator.Validate(decoratorSyntax, targetType, this.typeManager, this.binder, diagnostics);
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

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var declaredType = GetDeclaredTypeAndValidateDecorators(syntax, syntax.Type, diagnostics);
                diagnostics.WriteMultiple(GetOutputDeclarationDiagnostics(declaredType, syntax));

                base.VisitOutputDeclarationSyntax(syntax);

                return declaredType;
            });

        public override void VisitBooleanLiteralSyntax(BooleanLiteralSyntax syntax)
            => AssignType(syntax, () => syntax.Value ? LanguageConstants.True : LanguageConstants.False);

        public override void VisitStringSyntax(StringSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                if (syntax.TryGetLiteralValue() is string literalValue)
                {
                    // uninterpolated strings have a known type
                    return new StringLiteralType(literalValue);
                }

                var errors = new List<ErrorDiagnostic>();
                var expressionTypes = new List<TypeSymbol>();

                foreach (var interpolatedExpression in syntax.Expressions)
                {
                    var expressionType = typeManager.GetTypeInfo(interpolatedExpression);
                    CollectErrors(errors, expressionType);
                    expressionTypes.Add(expressionType);
                }

                if (PropagateErrorType(errors, expressionTypes))
                {
                    return ErrorType.Create(errors);
                }

                // normally we would also do an assignability check, but we allow "any" type in string interpolation expressions
                // so the assignability check cannot possibly fail (we already collected type errors from the inner expressions at this point)
                return LanguageConstants.String;
            });

        public override void VisitIntegerLiteralSyntax(IntegerLiteralSyntax syntax)
            => AssignType(syntax, () => syntax.Value switch {
                <= long.MaxValue => new IntegerLiteralType((long)syntax.Value),
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
                foreach (var arrayItem in syntax.Items)
                {
                    var itemType = typeManager.GetTypeInfo(arrayItem);
                    itemTypes.Add(itemType);
                    CollectErrors(errors, itemType);
                }

                if (PropagateErrorType(errors, itemTypes))
                {
                    return ErrorType.Create(errors);
                }

                if (TypeHelper.TryCollapseTypes(itemTypes) is not { } collapsedItemType)
                {
                    return LanguageConstants.Array;
                }

                return new TypedArrayType(collapsedItemType, TypeSymbolValidationFlags.Default);
            });

        public override void VisitTernaryOperationSyntax(TernaryOperationSyntax syntax)
            => AssignType(syntax, () =>
            {
                if(this.fileKind == BicepSourceFileKind.ParamsFile)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).ParameterTernaryOperationNotSupported());
                }

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
                if (TypeValidator.AreTypesAssignable(conditionType, expectedConditionType) != true)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.ConditionExpression).ValueTypeMismatch(expectedConditionType));
                }

                // the return type is the union of true and false expression types
                return TypeHelper.CreateTypeUnion(trueType, falseType);
            });

        public override void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                if (this.fileKind == BicepSourceFileKind.ParamsFile)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).ParameterBinaryOperationNotSupported());
                }

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
                if (OperationReturnTypeEvaluator.FoldBinaryExpression(syntax, operandType1, operandType2, out var foldDiags) is {} result)
                {
                    diagnostics.WriteMultiple(foldDiags);

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
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).BinaryOperatorInvalidType(Operators.BinaryOperatorToText[syntax.Operator], operandType1, operandType2, additionalInfo: additionalInfo));
            });

        public override void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                if (this.fileKind == BicepSourceFileKind.ParamsFile)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).ParameterUnaryOperationNotSupported());
                }

                var errors = new List<ErrorDiagnostic>();

                var operandType = typeManager.GetTypeInfo(syntax.Expression);
                CollectErrors(errors, operandType);

                if (PropagateErrorType(errors, operandType))
                {
                    return ErrorType.Create(errors);
                }

                // operand doesn't appear to have errors
                // let's fold the expression so that an operation with a literal typed operand will have a literal return type
                if (OperationReturnTypeEvaluator.FoldUnaryExpression(syntax, operandType, out var foldDiags) is {} result)
                {
                    diagnostics.WriteMultiple(foldDiags);

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
            => AssignTypeWithDiagnostics(syntax, diagnostics =>
            {
                var errors = new List<ErrorDiagnostic>();

                var baseType = typeManager.GetTypeInfo(syntax.BaseExpression);
                CollectErrors(errors, baseType);

                var indexType = typeManager.GetTypeInfo(syntax.IndexExpression);
                CollectErrors(errors, indexType);

                if (PropagateErrorType(errors, baseType, indexType))
                {
                    return ErrorType.Create(errors);
                }

                baseType = UnwrapType(baseType);
                return GetArrayItemType(syntax, diagnostics, baseType, indexType);
            });

        private static ITypeReference GetArrayItemType(ArrayAccessSyntax syntax, IDiagnosticWriter diagnostics, TypeSymbol baseType, TypeSymbol indexType)
        {
            switch (baseType)
            {
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
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.IndexExpression).StringOrIntegerIndexerRequired(indexType));

                case ArrayType baseArray:
                    // we are indexing over an array
                    if (TypeValidator.AreTypesAssignable(indexType, LanguageConstants.Int))
                    {
                        // the index is of "any" type or integer type
                        // return the item type
                        return baseArray.Item.Type;
                    }

                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.IndexExpression).ArraysRequireIntegerIndex(indexType));

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
                            switch (syntax.IndexExpression)
                            {
                                case StringSyntax @string when @string.TryGetLiteralValue() is { } literalValue:
                                    // indexing using a string literal so we know the name of the property
                                    return TypeHelper.GetNamedPropertyType(baseObject, syntax.IndexExpression, literalValue, TypeValidator.ShouldWarn(baseObject), diagnostics);

                                default:
                                    // the property name is itself an expression
                                    return GetExpressionedPropertyType(baseObject, syntax.IndexExpression);
                            }
                        }

                        return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.IndexExpression).ObjectsRequireStringIndex(indexType));
                    }

                case DiscriminatedObjectType:
                    if (TypeValidator.AreTypesAssignable(indexType, LanguageConstants.String))
                    {
                        // index is assignable to string
                        // since we're not resolving the discriminator currently, we can just return the "any" type
                        // TODO: resolve the discriminator
                        return LanguageConstants.Any;
                    }

                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.IndexExpression).ObjectsRequireStringIndex(indexType));

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
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.BaseExpression).IndexerRequiresObjectOrArray(baseType));
            }
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
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

                switch (baseType)
                {
                    case ObjectType objectType:
                        if (!syntax.PropertyName.IsValid)
                        {
                            // the property is not valid
                            // there's already a parse error for it, so we don't need to add a type error as well
                            return ErrorType.Empty();
                        }

                        return TypeHelper.GetNamedPropertyType(objectType, syntax.PropertyName, syntax.PropertyName.IdentifierName, TypeValidator.ShouldWarn(objectType), diagnostics);

                    case DiscriminatedObjectType _:
                        // TODO: We might be able use the declared type here to resolve discriminator to improve the assigned type
                        return LanguageConstants.Any;

                    case TypeSymbol _ when TypeValidator.AreTypesAssignable(baseType, LanguageConstants.Object):
                        // We can assign to an object, but we don't have a type for that object.
                        // The best we can do is allow it and return the 'any' type.
                        return LanguageConstants.Any;

                    default:
                        // can only access properties of objects
                        return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.PropertyName).ObjectRequiredForPropertyAccess(baseType));
                }
            });

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
                if (this.fileKind == BicepSourceFileKind.ParamsFile)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).ParameterFunctionCallNotSupported());
                }

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
                if (this.fileKind == BicepSourceFileKind.ParamsFile)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).ParameterLambdaFunctionNotSupported());
                }

                var argumentTypes = syntax.GetLocalVariables().Select(x => typeManager.GetTypeInfo(x));
                var returnType = TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, diagnostics, syntax.Body, LanguageConstants.Any);

                return new LambdaType(argumentTypes.ToImmutableArray<ITypeReference>(), returnType);
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

                return TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, diagnostics, syntax.Value, declaredType);
            });

        public override void VisitMissingDeclarationSyntax(MissingDeclarationSyntax syntax) => AssignTypeWithDiagnostics(syntax, diagnostics =>
        {
            if (syntax.HasParseErrors())
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

                    case VariableSymbol variable:
                        return new DeferredTypeReference(() => VisitDeclaredSymbol(syntax, variable));

                    case LocalVariableSymbol local:
                        return new DeferredTypeReference(() => VisitDeclaredSymbol(syntax, local));

                    case ImportedNamespaceSymbol import:
                        return new DeferredTypeReference(() => VisitDeclaredSymbol(syntax, import));

                    case BuiltInNamespaceSymbol @namespace:
                        return @namespace.Type;

                    case OutputSymbol _:
                        return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Name.Span).OutputReferenceNotSupported(syntax.Name.IdentifierName));

                    case TypeAliasSymbol:
                    case AmbientTypeSymbol:
                        return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Name.Span).TypeSymbolUsedAsValue(syntax.Name.IdentifierName));

                    default:
                        return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Name.Span).SymbolicNameIsNotAVariableOrParameter(syntax.Name.IdentifierName));
                }
            });

        private static void CollectErrors(List<ErrorDiagnostic> errors, ITypeReference reference)
        {
            errors.AddRange(reference.Type.GetDiagnostics());
        }

        private TypeSymbol GetFunctionSymbolType(
            FunctionSymbol function,
            FunctionCallSyntaxBase syntax,
            IList<ErrorDiagnostic> errors,
            IDiagnosticWriter diagnosticWriter)
        {
            // Recover argument type errors so we can continue type checking for the parent function call.
            var argumentTypes = this.GetRecoveredArgumentTypes(syntax.Arguments).ToImmutableArray();
            var matches = FunctionResolver.GetMatches(
                function,
                argumentTypes,
                out IList<ArgumentCountMismatch> countMismatches,
                out IList<ArgumentTypeMismatch> typeMismatches).ToList();

            if (matches.Count == 0)
            {
                if (typeMismatches.Any())
                {
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
                else
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
            var diagnostics = new List<IDiagnostic>();
            diagnostics.AddRange(valueType.GetDiagnostics());

            // Avoid reporting an additional error if we failed to bind the output type.
            if (TypeValidator.AreTypesAssignable(valueType, assignedType) == false && valueType is not ErrorType && assignedType is not ErrorType)
            {
                return DiagnosticBuilder.ForPosition(syntax.Value).OutputTypeMismatch(assignedType, valueType).AsEnumerable();
            }

            return diagnostics;
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

            TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, diagnosticWriter, defaultValueSyntax.DefaultValue, assignedType);

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
                        // Output: we already validate that outputs cannot be referenced in expressions (BCP058)
                        if (symbol != null &&
                            symbol.Kind != SymbolKind.Error &&
                            symbol.Kind != SymbolKind.Parameter &&
                            symbol.Kind != SymbolKind.Function &&
                            symbol.Kind != SymbolKind.Output &&
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
                return DiagnosticBuilder.ForPosition(syntax.ConditionExpression).ValueTypeMismatch(LanguageConstants.Bool).AsEnumerable();
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
