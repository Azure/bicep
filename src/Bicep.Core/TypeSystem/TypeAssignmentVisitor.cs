// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;

namespace Bicep.Core.TypeSystem
{
    public sealed class TypeAssignmentVisitor : SyntaxVisitor
    {
        private class TypeAssignment
        {
            public TypeAssignment(ITypeReference reference)
                : this(reference, Enumerable.Empty<IDiagnostic>())
            {
            }

            public TypeAssignment(ITypeReference reference, IEnumerable<IDiagnostic> diagnostics)
            {
                Reference = reference;
                Diagnostics = diagnostics;
            }

            public ITypeReference Reference { get; }

            public IEnumerable<IDiagnostic> Diagnostics { get; }
        }

        private readonly IResourceTypeProvider resourceTypeProvider;
        private readonly TypeManager typeManager;
        private readonly IBinder binder;
        private readonly IDictionary<SyntaxBase, TypeAssignment> assignedTypes;

        public TypeAssignmentVisitor(IResourceTypeProvider resourceTypeProvider, TypeManager typeManager, IBinder binder)
        {
            this.resourceTypeProvider = resourceTypeProvider;
            this.typeManager = typeManager;
            this.binder = binder;
            this.assignedTypes = new Dictionary<SyntaxBase, TypeAssignment>();
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

        private void AssignTypeWithCaching(SyntaxBase syntax, Func<TypeAssignment> assignFunc)
        {
            if (assignedTypes.ContainsKey(syntax))
            {
                return;
            }

            var cyclicErrorType = CheckForCyclicError(syntax);
            if (cyclicErrorType != null)
            {
                assignedTypes[syntax] = new TypeAssignment(cyclicErrorType);
                return;
            }

            assignedTypes[syntax] = assignFunc();
        }

        private void AssignType(SyntaxBase syntax, Func<ITypeReference> assignFunc)
            => AssignTypeWithCaching(syntax, () => new TypeAssignment(assignFunc()));

        private void AssignTypeWithDiagnostics(SyntaxBase syntax, Func<IDiagnosticWriter, ITypeReference> assignFunc)
            => AssignTypeWithCaching(syntax, () => {
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
                if(symbol is not LocalVariableSymbol localVariableSymbol)
                {
                    throw new InvalidOperationException($"{syntax.GetType().Name} is bound to unexpected type '{symbol?.GetType().Name}'.");
                }

                var parent = this.binder.GetParent(syntax);
                var @for = parent switch
                {
                    ForSyntax forParent => forParent,
                    ForVariableBlockSyntax block when this.binder.GetParent(block) is ForSyntax forParent => forParent,
                    _ => throw new InvalidOperationException($"{syntax.GetType().Name} at {syntax.Span} has an unexpected parent of type {parent?.GetType().Name}")
                };

                return localVariableSymbol.LocalKind switch
                {
                    // this local variable is a loop item variable
                    // we should return item type of the array (if feasible)
                    LocalKind.ForExpressionItemVariable => GetItemType(@for),

                    // the local variable is an index variable
                    // index variables are always of type int
                    LocalKind.ForExpressionIndexVariable => LanguageConstants.Int,

                    _ => throw new InvalidOperationException($"Unexpected local kind '{localVariableSymbol.LocalKind}'.")
                };
            });

        public override void VisitForSyntax(ForSyntax syntax)
            => AssignType(syntax, () =>
            {
                var errors = new List<ErrorDiagnostic>();

                if(syntax.ItemVariable is null)
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
                var singleResourceDeclaredType = syntax.GetDeclaredType(binder, resourceTypeProvider);
                var singleOrCollectionDeclaredType = syntax.Value is ForSyntax
                    ? new TypedArrayType(singleResourceDeclaredType, TypeSymbolValidationFlags.Default)
                    : singleResourceDeclaredType;

                this.ValidateDecorators(syntax.Decorators, singleOrCollectionDeclaredType, diagnostics);

                if (singleResourceDeclaredType is ErrorType)
                {
                    return singleResourceDeclaredType;
                }

                if (singleResourceDeclaredType is ResourceType resourceType && !resourceTypeProvider.HasType(resourceType.TypeReference))
                {
                    diagnostics.Write(DiagnosticBuilder.ForPosition(syntax.Type).ResourceTypesUnavailable(resourceType.TypeReference));
                }

                return TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, syntax.Value, singleOrCollectionDeclaredType, diagnostics);
            });

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics => {
                if (this.binder.GetSymbolInfo(syntax) is not ModuleSymbol moduleSymbol)
                {
                    // TODO: Ideally we'd still be able to return a type here, but we'd need access to the compilation to get it.
                    return ErrorType.Empty();
                }

                if (!moduleSymbol.TryGetSemanticModel(out var moduleSemanticModel, out var failureDiagnostic))
                {
                    return ErrorType.Create(failureDiagnostic);
                }

                var singleModuleDeclaredType = syntax.GetDeclaredType(this.binder.TargetScope, moduleSemanticModel);
                var singleOrCollectionDeclaredType = syntax.Value is ForSyntax
                    ? new TypedArrayType(singleModuleDeclaredType, TypeSymbolValidationFlags.Default)
                    : singleModuleDeclaredType;

                this.ValidateDecorators(syntax.Decorators, singleOrCollectionDeclaredType, diagnostics);

                if (moduleSemanticModel.HasErrors())
                {
                    diagnostics.Write(DiagnosticBuilder.ForPosition(syntax.Path).ReferencedModuleHasErrors());
                }

                return TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, syntax.Value, singleOrCollectionDeclaredType, diagnostics);
            });

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics => {
                var declaredType = syntax.GetDeclaredType();

                this.ValidateDecorators(syntax.Decorators, declaredType, diagnostics);

                if (syntax.Modifier != null)
                {
                    if (syntax.Modifier is ObjectSyntax modifierSyntax)
                    {
                        diagnostics.Write(syntax.Decorators.Any()
                            ? DiagnosticBuilder.ForPosition(modifierSyntax.OpenBrace).CannotUseParameterDecoratorsAndModifiersTogether()
                            : DiagnosticBuilder.ForPosition(modifierSyntax).ParameterModifiersDeprecated());
                    }

                    diagnostics.WriteMultiple(this.ValidateIdentifierAccess(syntax.Modifier));
                }

                if (declaredType is ErrorType)
                {
                    return declaredType;
                }

                /*
                 * Calling FirstOrDefault here because when there are duplicate decorators,
                 * the top most one takes precedence.
                 */
                var allowedDecoratorSyntax = syntax.Decorators.FirstOrDefault(decoratorSyntax =>
                {
                    if (this.binder.GetSymbolInfo(decoratorSyntax.Expression) is FunctionSymbol functionSymbol)
                    {
                        var argumentTypes = this.GetRecoveredArgumentTypes(decoratorSyntax.Arguments).ToArray();
                        Decorator? decorator = this.binder.FileSymbol.ImportedNamespaces["sys"].Type.DecoratorResolver
                            .GetMatches(functionSymbol, argumentTypes)
                            .SingleOrDefault();

                        if (decorator?.Overload.Name == "allowed")
                        {
                            return true;
                        }
                    }

                    return false;
                });


                var assignedType = allowedDecoratorSyntax?.Arguments.Single().Expression is ArraySyntax allowedArrraySyntax
                    ? syntax.GetAssignedType(this.typeManager, allowedArrraySyntax)
                    : syntax.GetAssignedType(this.typeManager, null);

                switch (syntax.Modifier)
                {
                    case ParameterDefaultValueSyntax defaultValueSyntax:
                        diagnostics.WriteMultiple(ValidateDefaultValue(defaultValueSyntax, assignedType));
                        break;

                    case ObjectSyntax modifierSyntax:
                        var modifierType = LanguageConstants.CreateParameterModifierType(declaredType, assignedType);
                        // we don't need to actually use the narrowed type; just need to use this to collect assignment diagnostics
                        TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, modifierSyntax, modifierType, diagnostics);
                        break;
                }

                return assignedType;
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

                if (symbol is DeclaredSymbol or NamespaceSymbol)
                {
                    diagnostics.Write(DiagnosticBuilder.ForPosition(decoratorSyntax.Expression).ExpressionNotCallable());
                    continue;
                }

                if (this.binder.GetSymbolInfo(decoratorSyntax.Expression) is FunctionSymbol functionSymbol)
                {
                    var argumentTypes = this.GetRecoveredArgumentTypes(decoratorSyntax.Arguments).ToArray();

                    // There should exist exact one matching decorator if there's no argument mismatches,
                    // since each argument must be a compile-time constant which cannot be of Any type.
                    var decorator = this.binder.FileSymbol.ImportedNamespaces.Values
                        .SelectMany(ns => ns.Type.DecoratorResolver.GetMatches(functionSymbol, argumentTypes))
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

                        decorator.Validate(decoratorSyntax, targetType, this.typeManager, diagnostics);
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

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics => {
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
                // assume "any" type if the output type has parse errors (either missing or skipped)
                var primitiveType = syntax.OutputType == null
                    ? LanguageConstants.Any
                    : LanguageConstants.TryGetDeclarationType(syntax.OutputType.TypeName);

                if (primitiveType == null)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Type).InvalidOutputType());
                }

                this.ValidateDecorators(syntax.Decorators, primitiveType, diagnostics);

                var currentDiagnostics = GetOutputDeclarationDiagnostics(primitiveType, syntax);

                diagnostics.WriteMultiple(currentDiagnostics);

                return primitiveType;
            });

        public override void VisitBooleanLiteralSyntax(BooleanLiteralSyntax syntax)
            => AssignType(syntax, () => LanguageConstants.Bool);

        public override void VisitStringSyntax(StringSyntax syntax)
            => AssignType(syntax, () => {
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
            => AssignType(syntax, () => LanguageConstants.Int);

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

                // type results are cached
                var namedProperties = syntax.Properties
                    .GroupByExcludingNull(p => p.TryGetKeyText(), LanguageConstants.IdentifierComparer)
                    .Select(group => new TypeProperty(group.Key, UnionType.Create(group.Select(p => typeManager.GetTypeInfo(p)))));

                var additionalProperties = syntax.Properties
                    .Where(p => p.TryGetKeyText() == null)
                    .Select(p => typeManager.GetTypeInfo(p));

                var additionalPropertiesType = additionalProperties.Any() ? UnionType.Create(additionalProperties) : null;

                // TODO: Add structural naming?
                return new NamedObjectType(LanguageConstants.Object.Name, TypeSymbolValidationFlags.Default, namedProperties, additionalPropertiesType);
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
                    return ErrorType.Create(errors);
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

                var aggregatedItemType = UnionType.Create(itemTypes);
                if (aggregatedItemType.TypeKind == TypeKind.Union || aggregatedItemType.TypeKind == TypeKind.Never)
                {
                    // array contains a mix of item types or is empty
                    // assume array of any for now
                    return LanguageConstants.Array;
                }

                return new TypedArrayType(aggregatedItemType, TypeSymbolValidationFlags.Default);
            });

        public override void VisitTernaryOperationSyntax(TernaryOperationSyntax syntax)
            => AssignType(syntax, () => {
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
                return UnionType.Create(trueType, falseType);
            });

        public override void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax)
            => AssignType(syntax, () => {
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
                // let's match the operator now
                var operatorInfo = BinaryOperationResolver.TryMatchExact(syntax.Operator, operandType1, operandType2);
                if (operatorInfo != null)
                {
                    // we found a match - use its return type
                    return operatorInfo.ReturnType;
                }

                // we do not have a match
                // operand types didn't match available operators
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).BinaryOperatorInvalidType(Operators.BinaryOperatorToText[syntax.Operator], operandType1, operandType2));
            });

        public override void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax)
            => AssignType(syntax, () => {
                var errors = new List<ErrorDiagnostic>();

                // TODO: When we add number type, this will have to be adjusted
                var expectedOperandType = syntax.Operator switch
                {
                    UnaryOperator.Not => LanguageConstants.Bool,
                    UnaryOperator.Minus => LanguageConstants.Int,
                    _ => throw new NotImplementedException()
                };

                var operandType = typeManager.GetTypeInfo(syntax.Expression);
                CollectErrors(errors, operandType);

                if (PropagateErrorType(errors, operandType))
                {
                    return ErrorType.Create(errors);
                }

                if (TypeValidator.AreTypesAssignable(operandType, expectedOperandType) != true)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax).UnaryOperatorInvalidType(Operators.UnaryOperatorToText[syntax.Operator], operandType));
                }

                return expectedOperandType;
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
            => AssignTypeWithDiagnostics(syntax, diagnostics => {
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

                if (baseType.TypeKind == TypeKind.Any)
                {
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
                }

                if (baseType is ArrayType baseArray)
                {
                    // we are indexing over an array
                    if (TypeValidator.AreTypesAssignable(indexType, LanguageConstants.Int))
                    {
                        // the index is of "any" type or integer type
                        // return the item type
                        return baseArray.Item.Type;
                    }

                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.IndexExpression).ArraysRequireIntegerIndex(indexType));
                }

                if (baseType is ObjectType baseObject)
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
                                return GetNamedPropertyType(baseObject, syntax.IndexExpression, literalValue, diagnostics);

                            default:
                                // the property name is itself an expression
                                return GetExpressionedPropertyType(baseObject, syntax.IndexExpression);
                        }
                    }

                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.IndexExpression).ObjectsRequireStringIndex(indexType));
                }

                if (baseType is DiscriminatedObjectType)
                {
                    if (TypeValidator.AreTypesAssignable(indexType, LanguageConstants.String))
                    {
                        // index is assignable to string
                        // since we're not resolving the discriminator currently, we can just return the "any" type
                        // TODO: resolve the discriminator
                        return LanguageConstants.Any;
                    }

                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.IndexExpression).ObjectsRequireStringIndex(indexType));
                }

                // index was of the wrong type
                return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.BaseExpression).IndexerRequiresObjectOrArray(baseType));
            });

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics => {
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

                        return GetNamedPropertyType(objectType, syntax.PropertyName, syntax.PropertyName.IdentifierName, diagnostics);

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
            => AssignTypeWithDiagnostics(syntax, diagnostics => {
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
            => AssignType(syntax, () => {
                var errors = new List<ErrorDiagnostic>();

                foreach (TypeSymbol argumentType in this.GetArgumentTypes(syntax.Arguments).ToArray())
                {
                    CollectErrors(errors, argumentType);
                }

                switch (this.binder.GetSymbolInfo(syntax))
                {
                    case ErrorSymbol errorSymbol:
                        // function bind failure - pass the error along
                        return ErrorType.Create(errors.Concat(errorSymbol.GetDiagnostics()));

                    case FunctionSymbol function:
                        return this.GetFunctionSymbolType(function, syntax.OpenParen, syntax.CloseParen, syntax.Arguments, errors);

                    default:
                        return ErrorType.Create(errors.Append(DiagnosticBuilder.ForPosition(syntax.Name.Span).SymbolicNameIsNotAFunction(syntax.Name.IdentifierName)));
                }
            });

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
            => AssignType(syntax, () => {
                var errors = new List<ErrorDiagnostic>();

                var baseType = typeManager.GetTypeInfo(syntax.BaseExpression);
                CollectErrors(errors, baseType);

                if (PropagateErrorType(errors, baseType))
                {
                    return ErrorType.Create(errors);
                }

                baseType = UnwrapType(baseType);

                if (!(baseType is ObjectType objectType))
                {
                    // can only access methods on objects
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Name).ObjectRequiredForMethodAccess(baseType));
                }

                foreach (TypeSymbol argumentType in this.GetArgumentTypes(syntax.Arguments).ToArray())
                {
                    CollectErrors(errors, argumentType);
                }

                if (this.binder.GetSymbolInfo(syntax) is not Symbol foundSymbol)
                {
                    // namespace methods will have already been bound. Everything else will not have been.
                    var resolvedSymbol = objectType.MethodResolver.TryGetSymbol(syntax.Name);

                    foundSymbol = SymbolValidator.ResolveObjectQualifiedFunction(resolvedSymbol, syntax.Name, objectType);
                }

                switch (foundSymbol)
                {
                    case ErrorSymbol errorSymbol:
                        // bind bind failure - pass the error along
                        return ErrorType.Create(errors.Concat(errorSymbol.GetDiagnostics()));

                    case FunctionSymbol functionSymbol:
                        return this.GetFunctionSymbolType(functionSymbol, syntax.OpenParen, syntax.CloseParen, syntax.Arguments, errors);

                    default:
                        return ErrorType.Create(errors.Append(DiagnosticBuilder.ForPosition(syntax.Name.Span).SymbolicNameIsNotAFunction(syntax.Name.IdentifierName)));
                }
            });

        public override void VisitTargetScopeSyntax(TargetScopeSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics => {
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

                return TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, syntax.Value, declaredType, diagnostics);
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

            if (this.binder.GetSymbolInfo(lastDecoratorSyntax.Expression) is FunctionSymbol functionSymbol)
            {
                var diagnosticBuilder = DiagnosticBuilder.ForPosition(lastDecoratorSyntax);
                var diagnostic = functionSymbol.FunctionFlags switch
                {
                    FunctionFlags.ParameterDecorator => diagnosticBuilder.ExpectedParameterDeclarationAfterDecorator(),
                    FunctionFlags.VariableDecorator => diagnosticBuilder.ExpectedVariableDeclarationAfterDecorator(),
                    FunctionFlags.ResourceDecorator => diagnosticBuilder.ExpectedResourceDeclarationAfterDecorator(),
                    FunctionFlags.ModuleDecorator => diagnosticBuilder.ExpectedModuleDeclarationAfterDecorator(),
                    FunctionFlags.OutputDecorator => diagnosticBuilder.ExpectedOutputDeclarationAfterDecorator(),
                    FunctionFlags.ResourceOrModuleDecorator => diagnosticBuilder.ExpectedResourceOrModuleDeclarationAfterDecorator(),
                    _ => null,
                };

                if (diagnostic is not null)
                {
                    diagnostics.Write(diagnostic);
                }
            }

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
            => AssignType(syntax, () => {
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
                    
                    case NamespaceSymbol @namespace:
                        return @namespace.Type;

                    case OutputSymbol _:
                        return ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.Name.Span).OutputReferenceNotSupported(syntax.Name.IdentifierName));

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
            Token openParen,
            Token closeParen,
            ImmutableArray<FunctionArgumentSyntax> argumentSyntaxes,
            IList<ErrorDiagnostic> errors)
        {
            // Recover argument type errors so we can continue type checking for the parent function call.
            var argumentTypes = this.GetRecoveredArgumentTypes(argumentSyntaxes).ToArray();
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
                        var argumentSyntax = argumentSyntaxes[typeMismatches[0].ArgumentIndex];

                        errors.Add(DiagnosticBuilder.ForPosition(argumentSyntax).CannotResolveFunctionOverload(signatures, argumentType, parameterTypes));
                    }
                    else
                    {
                        // Choose the type mismatch that has the largest index as the best one.
                        var (_, argumentIndex, argumentType, parameterType) = typeMismatches.OrderBy(tm => tm.ArgumentIndex).Last();

                        errors.Add(DiagnosticBuilder.ForPosition(argumentSyntaxes[argumentIndex]).ArgumentTypeMismatch(argumentType, parameterType));
                    }
                }
                else
                {
                    // Argument type mismatch wins over count mismatch. Handle count mismatch only when there's no type mismatch.
                    var (actualCount, mininumArgumentCount, maximumArgumentCount) = countMismatches.Aggregate(ArgumentCountMismatch.Reduce);
                    var argumentsSpan = TextSpan.Between(openParen, closeParen);

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
                // return its type
                return matches.Single().ReturnTypeBuilder(argumentSyntaxes);
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

        /// <summary>
        /// Gets the type of the property whose name we can obtain at compile-time.
        /// </summary>
        /// <param name="baseType">The base object type</param>
        /// <param name="propertyExpressionPositionable">The position of the property name expression</param>
        /// <param name="propertyName">The resolved property name</param>
        /// <param name="diagnostics">List of diagnostics to append diagnostics</param>
        private static TypeSymbol GetNamedPropertyType(ObjectType baseType, IPositionable propertyExpressionPositionable, string propertyName, IDiagnosticWriter diagnostics)
        {
            if (baseType.TypeKind == TypeKind.Any)
            {
                // all properties of "any" type are of type "any"
                return LanguageConstants.Any;
            }

            // is there a declared property with this name
            var declaredProperty = baseType.Properties.TryGetValue(propertyName);
            if (declaredProperty != null)
            {
                if (declaredProperty.Flags.HasFlag(TypePropertyFlags.WriteOnly))
                {
                    var writeOnlyDiagnostic = DiagnosticBuilder.ForPosition(propertyExpressionPositionable).WriteOnlyProperty(TypeValidator.ShouldWarn(baseType), baseType, propertyName);
                    diagnostics.Write(writeOnlyDiagnostic);

                    if (writeOnlyDiagnostic.Level == DiagnosticLevel.Error)
                    {
                        return ErrorType.Create(Enumerable.Empty<ErrorDiagnostic>());
                    }
                }

                // there is - return its type
                return declaredProperty.TypeReference.Type;
            }

            // the property is not declared
            // check additional properties
            if (baseType.AdditionalPropertiesType != null)
            {
                // yes - return the additional property type
                return baseType.AdditionalPropertiesType.Type;
            }

            var availableProperties = baseType.Properties.Values
                .Where(p => !p.Flags.HasFlag(TypePropertyFlags.WriteOnly))
                .Select(p => p.Name)
                .OrderBy(x => x);

            var diagnosticBuilder = DiagnosticBuilder.ForPosition(propertyExpressionPositionable);
            var shouldWarn = TypeValidator.ShouldWarn(baseType);

            var unknownPropertyDiagnostic = availableProperties.Any() switch
            {
                true => SpellChecker.GetSpellingSuggestion(propertyName, availableProperties) switch
                {
                    string suggestedPropertyName when suggestedPropertyName != null =>
                        diagnosticBuilder.UnknownPropertyWithSuggestion(shouldWarn, baseType, propertyName, suggestedPropertyName),
                    _ => diagnosticBuilder.UnknownPropertyWithAvailableProperties(shouldWarn, baseType, propertyName, availableProperties),
                },
                _ => diagnosticBuilder.UnknownProperty(shouldWarn, baseType, propertyName)
            };

            diagnostics.Write(unknownPropertyDiagnostic);

            return (unknownPropertyDiagnostic.Level == DiagnosticLevel.Error) ? ErrorType.Create(Enumerable.Empty<ErrorDiagnostic>()) : LanguageConstants.Any;
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

            if (TypeValidator.AreTypesAssignable(valueType, assignedType) == false)
            {
                return DiagnosticBuilder.ForPosition(syntax.Value).OutputTypeMismatch(assignedType, valueType).AsEnumerable();
            }

            return Enumerable.Empty<IDiagnostic>();
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

            if (TypeValidator.AreTypesAssignable(defaultValueType, assignedType) == false)
            {
                return DiagnosticBuilder.ForPosition(defaultValueSyntax.DefaultValue).ParameterTypeMismatch(assignedType, defaultValueType).AsEnumerable();
            }

            return Enumerable.Empty<IDiagnostic>();
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
                            symbol.Kind != SymbolKind.Namespace)
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
    }
}
