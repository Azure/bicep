// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Parser;
using Bicep.Core.Resources;
using Bicep.Core.SemanticModel;
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
                : this(reference, Enumerable.Empty<Diagnostic>())
            {
            }

            public TypeAssignment(ITypeReference reference, IEnumerable<Diagnostic> diagnostics)
            {
                Reference = reference;
                Diagnostics = diagnostics;
            }

            public ITypeReference Reference { get; }

            public IEnumerable<Diagnostic> Diagnostics { get; }
        }

        private readonly IResourceTypeProvider resourceTypeProvider;
        private readonly TypeManager typeManager;
        private readonly IReadOnlyDictionary<SyntaxBase, Symbol> bindings;
        private readonly IReadOnlyDictionary<DeclaredSymbol, ImmutableArray<DeclaredSymbol>> cyclesBySymbol;
        private readonly IDictionary<SyntaxBase, TypeAssignment> assignedTypes;
        private readonly IDictionary<SyntaxBase, TypeAssignment> declaredTypes;
        private readonly SyntaxHierarchy hierarchy;

        public TypeAssignmentVisitor(
            IResourceTypeProvider resourceTypeProvider,
            TypeManager typeManager,
            IReadOnlyDictionary<SyntaxBase, Symbol> bindings,
            IReadOnlyDictionary<DeclaredSymbol, ImmutableArray<DeclaredSymbol>> cyclesBySymbol,
            SyntaxHierarchy hierarchy)
        {
            this.resourceTypeProvider = resourceTypeProvider;
            this.typeManager = typeManager;
            // bindings will be modified by name binding after this object is created
            // so we can't make an immutable copy here
            // (using the IReadOnlyDictionary to prevent accidental mutation)
            this.bindings = bindings;
            this.cyclesBySymbol = cyclesBySymbol;
            this.assignedTypes = new Dictionary<SyntaxBase, TypeAssignment>();
            this.declaredTypes = new Dictionary<SyntaxBase, TypeAssignment>();
            this.hierarchy = hierarchy;
        }

        private TypeAssignment GetTypeAssignment(SyntaxBase syntax)
        {
            Visit(syntax);

            if (!assignedTypes.TryGetValue(syntax, out var typeAssignment))
            {
                return new TypeAssignment(UnassignableTypeSymbol.CreateErrors(DiagnosticBuilder.ForPosition(syntax).InvalidExpression()));
            }

            return typeAssignment;
        }

        private TypeAssignment? GetDeclaredTypeAssignment(SyntaxBase syntax)
        {
            // causes stack overflow Visit(syntax);

            if (declaredTypes.TryGetValue(syntax, out var typeAssignment))
            {
                return typeAssignment;
            }

            return null;
        }

        public TypeSymbol GetTypeInfo(SyntaxBase syntax)
            => GetTypeAssignment(syntax).Reference.Type;

        public TypeSymbol? GetDeclaredType(SyntaxBase syntax)
            => GetDeclaredTypeAssignment(syntax)?.Reference.Type;

        public IEnumerable<Diagnostic> GetAllDiagnostics()
            => assignedTypes.Values.SelectMany(x => x.Diagnostics);

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

        private void AssignDeclaredType(SyntaxBase syntax, ITypeReference? typeReference)
        {
            if (typeReference != null)
            {
                this.declaredTypes[syntax] = new TypeAssignment(typeReference);
            }
        }

        private void AssignType(SyntaxBase syntax, Func<ITypeReference> assignFunc)
            => AssignTypeWithCaching(syntax, () => new TypeAssignment(assignFunc()));

        private void AssignTypeWithDiagnostics(SyntaxBase syntax, Func<IList<Diagnostic>, ITypeReference> assignFunc)
            => AssignTypeWithCaching(syntax, () => {
                var diagnostics = new List<Diagnostic>();
                var reference = assignFunc(diagnostics);

                return new TypeAssignment(reference, diagnostics);
            });

        private TypeSymbol? CheckForCyclicError(SyntaxBase syntax)
        {
            if (!(bindings.TryGetValue(syntax, out var symbol) && (symbol is DeclaredSymbol declaredSymbol)))
            {
                return null;
            }

            if (declaredSymbol.DeclaringSyntax == syntax)
            {
                // Report cycle errors on accesses to cyclic symbols, not on the declaration itself
                return null;
            }

            if (cyclesBySymbol.TryGetValue(declaredSymbol, out var cycle))
            {
                // there's a cycle. stop visiting now or we never will!
                if (cycle.Length == 1)
                {
                    return UnassignableTypeSymbol.CreateErrors(DiagnosticBuilder.ForPosition(syntax).CyclicExpressionSelfReference());
                }

                return UnassignableTypeSymbol.CreateErrors(DiagnosticBuilder.ForPosition(syntax).CyclicExpression(cycle.Select(x => x.Name)));
            }

            return null;
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics => {
                var stringSyntax = syntax.TryGetType();

                if (stringSyntax != null && stringSyntax.IsInterpolated())
                {
                    // TODO: in the future, we can relax this check to allow interpolation with compile-time constants.
                    // right now, codegen will still generate a format string however, which will cause problems for the type.
                    return UnassignableTypeSymbol.CreateErrors(DiagnosticBuilder.ForPosition(syntax.Type).ResourceTypeInterpolationUnsupported());
                }

                var stringContent = stringSyntax?.TryGetLiteralValue();
                if (stringContent == null)
                {
                    return UnassignableTypeSymbol.CreateErrors(DiagnosticBuilder.ForPosition(syntax.Type).InvalidResourceType());
                }

                // TODO: This needs proper namespace, type, and version resolution logic in the future
                var typeReference = ResourceTypeReference.TryParse(stringContent);
                if (typeReference == null)
                {
                    return UnassignableTypeSymbol.CreateErrors(DiagnosticBuilder.ForPosition(syntax.Type).InvalidResourceType());
                }

                var declaredType = resourceTypeProvider.GetType(typeReference);
                
                // just established the declared type - assign it!
                AssignDeclaredType(syntax, declaredType);

                if (declaredType is ResourceType resourceType && !resourceTypeProvider.HasType(resourceType.TypeReference))
                {
                    diagnostics.Add(DiagnosticBuilder.ForPosition(syntax.Type).ResourceTypesUnavailable(resourceType.TypeReference));
                }
            
                return TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, syntax.Body, declaredType, diagnostics);
            });

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics => {
                if (!(bindings.TryGetValue(syntax, out var symbol) && symbol is ModuleSymbol moduleSymbol))
                {
                    // TODO: Ideally we'd still be able to return a type here, but we'd need access to the compilation to get it.
                    return UnassignableTypeSymbol.CreateErrors(Enumerable.Empty<ErrorDiagnostic>());
                }

                var moduleSemanticModel = moduleSymbol.TryGetSemanticModel(out var failureDiagnostic);
                if (moduleSemanticModel == null)
                {
                    // TODO: If we upgrade to netstandard2.1, we should be able to use the following to hint to the compiler that failureDiagnostic is non-null:
                    // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis
                    var concreteFailureDiagnostic = failureDiagnostic ?? throw new InvalidOperationException($"Expected {nameof(moduleSymbol.TryGetSemanticModel)} to provide failure diagnostics");
                    return UnassignableTypeSymbol.CreateErrors(concreteFailureDiagnostic);
                }

                var typeProperties = new List<TypeProperty>();
                foreach (var param in moduleSemanticModel.Root.ParameterDeclarations)
                {
                    var typePropertyFlags = TypePropertyFlags.WriteOnly;
                    if (SyntaxHelper.TryGetDefaultValue(param.DeclaringParameter) == null)
                    {
                        // if there's no default value, it must be specified
                        typePropertyFlags |= TypePropertyFlags.Required;
                    }

                    typeProperties.Add(new TypeProperty(param.Name, param.Type, typePropertyFlags));
                }

                foreach (var output in moduleSemanticModel.Root.OutputDeclarations)
                {
                    typeProperties.Add(new TypeProperty(output.Name, output.Type, TypePropertyFlags.ReadOnly));
                }

                var expectedType = new NamedObjectType(
                    syntax.Name.IdentifierName,
                    TypeSymbolValidationFlags.Default,
                    typeProperties,
                    null);

                return TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, syntax.Body, expectedType, diagnostics);
            });

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics => {
                diagnostics.AddRange(this.ValidateIdentifierAccess(syntax));

                // assume "any" type when the parameter has parse errors (either missing or was skipped)
                var declaredType = syntax.ParameterType == null
                    ? LanguageConstants.Any
                    : LanguageConstants.TryGetDeclarationType(syntax.ParameterType.TypeName);

                if (declaredType == null)
                {
                    return UnassignableTypeSymbol.CreateErrors(DiagnosticBuilder.ForPosition(syntax.Type).InvalidParameterType());
                }

                // just established the declared type
                AssignDeclaredType(syntax, declaredType);

                var assignedType = GetParameterAssignedType(syntax, declaredType);

                switch (syntax.Modifier)
                {
                    case ParameterDefaultValueSyntax defaultValueSyntax:
                        diagnostics.AddRange(ValidateDefaultValue(defaultValueSyntax, assignedType));
                        break;

                    case ObjectSyntax modifierSyntax:
                        var modifierType = LanguageConstants.CreateParameterModifierType(declaredType, assignedType);
                        // we don't need to actually use the narrowed type; just need to use this to collect assignment diagnostics
                        TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, modifierSyntax, modifierType, diagnostics);
                        break;
                }

                return assignedType;
            });

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
            => AssignType(syntax, () => {
                return typeManager.GetTypeInfo(syntax.Value);
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
                    return UnassignableTypeSymbol.CreateErrors(DiagnosticBuilder.ForPosition(syntax.Type).InvalidOutputType());
                }

                var currentDiagnostics = GetOutputDeclarationDiagnostics(primitiveType, syntax);

                diagnostics.AddRange(currentDiagnostics);

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
                    return UnassignableTypeSymbol.CreateErrors(errors);
                }

                // normally we would also do an assignability check, but we allow "any" type in string interpolation expressions
                // so the assignability check cannot possibly fail (we already collected type errors from the inner expressions at this point)
                return LanguageConstants.String;
            });

        public override void VisitNumericLiteralSyntax(NumericLiteralSyntax syntax)
            => AssignType(syntax, () => LanguageConstants.Int);

        public override void VisitNullLiteralSyntax(NullLiteralSyntax syntax)
            => AssignType(syntax, () => LanguageConstants.Null);

        public override void VisitSkippedTriviaSyntax(SkippedTriviaSyntax syntax)
            => AssignType(syntax, () => {
                // error should have already been raised by the ParseDiagnosticsVisitor - no need to add another
                return UnassignableTypeSymbol.CreateErrors(Enumerable.Empty<ErrorDiagnostic>());
            });

        public override void VisitObjectSyntax(ObjectSyntax syntax)
            => AssignType(syntax, () => {
                // assigning declared type depends on parent and nothing in the object itself
                // so do it immediately
                AssignDeclaredType(syntax, GetDeclaredTypeInternal(syntax));
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
                    return UnassignableTypeSymbol.CreateErrors(errors);
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
                 AssignDeclaredType(syntax, GetDeclaredTypeInternal(syntax));

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
                    return UnassignableTypeSymbol.CreateErrors(errors);
                }

                return valueType;
            });

        public override void VisitArrayItemSyntax(ArrayItemSyntax syntax)
            => AssignType(syntax, () =>
            {
                AssignDeclaredType(syntax, GetDeclaredTypeInternal(syntax));
                return typeManager.GetTypeInfo(syntax.Value);
            });

        public override void VisitParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax)
            => AssignType(syntax, () => typeManager.GetTypeInfo(syntax.Expression));

        public override void VisitFunctionArgumentSyntax(FunctionArgumentSyntax syntax)
            => AssignType(syntax, () => typeManager.GetTypeInfo(syntax.Expression));

        public override void VisitArraySyntax(ArraySyntax syntax)
            => AssignType(syntax, () =>
            {
                AssignDeclaredType(syntax, GetDeclaredTypeInternal(syntax));
                
                var errors = new List<ErrorDiagnostic>();

                var itemTypes = new List<TypeSymbol>(syntax.Children.Length);
                foreach (var arrayItem in syntax.Children)
                {
                    var itemType = typeManager.GetTypeInfo(arrayItem);
                    itemTypes.Add(itemType);
                    CollectErrors(errors, itemType);
                }

                if (PropagateErrorType(errors, itemTypes))
                {
                    return UnassignableTypeSymbol.CreateErrors(errors);
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
                    return UnassignableTypeSymbol.CreateErrors(errors);
                }

                var expectedConditionType = LanguageConstants.Bool;
                if (TypeValidator.AreTypesAssignable(conditionType, expectedConditionType) != true)
                {
                    return UnassignableTypeSymbol.CreateErrors(DiagnosticBuilder.ForPosition(syntax.ConditionExpression).ValueTypeMismatch(expectedConditionType));
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
                    return UnassignableTypeSymbol.CreateErrors(errors);
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
                return UnassignableTypeSymbol.CreateErrors(DiagnosticBuilder.ForPosition(syntax).BinaryOperatorInvalidType(Operators.BinaryOperatorToText[syntax.Operator], operandType1, operandType2));
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
                    return UnassignableTypeSymbol.CreateErrors(errors);
                }

                if (TypeValidator.AreTypesAssignable(operandType, expectedOperandType) != true)
                {
                    return UnassignableTypeSymbol.CreateErrors(DiagnosticBuilder.ForPosition(syntax).UnaryOperatorInvalidType(Operators.UnaryOperatorToText[syntax.Operator], operandType));
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
                    return UnassignableTypeSymbol.CreateErrors(errors);
                }

                if (baseType.TypeKind == TypeKind.Any)
                {
                    // base expression is of type any
                    if (indexType.TypeKind == TypeKind.Any)
                    {
                        // index is also of type any
                        return LanguageConstants.Any;
                    }

                    if (TypeValidator.AreTypesAssignable(indexType, LanguageConstants.Int) == true ||
                        TypeValidator.AreTypesAssignable(indexType, LanguageConstants.String) == true)
                    {
                        // index expression is string | int but base is any
                        return LanguageConstants.Any;
                    }

                    // index was of the wrong type
                    return UnassignableTypeSymbol.CreateErrors(DiagnosticBuilder.ForPosition(syntax.IndexExpression).StringOrIntegerIndexerRequired(indexType));
                }

                if (baseType is ArrayType baseArray)
                {
                    // we are indexing over an array
                    if (TypeValidator.AreTypesAssignable(indexType, LanguageConstants.Int) == true)
                    {
                        // the index is of "any" type or integer type
                        // return the item type
                        return baseArray.Item.Type;
                    }

                    return UnassignableTypeSymbol.CreateErrors(DiagnosticBuilder.ForPosition(syntax.IndexExpression).ArraysRequireIntegerIndex(indexType));
                }

                if (baseType is ObjectType baseObject)
                {
                    // we are indexing over an object
                    if (indexType.TypeKind == TypeKind.Any)
                    {
                        // index is of type "any"
                        return GetExpressionedPropertyType(baseObject, syntax.IndexExpression);
                    }

                    if (TypeValidator.AreTypesAssignable(indexType, LanguageConstants.String) == true)
                    {
                        switch (syntax.IndexExpression)
                        {
                            case StringSyntax @string when @string.TryGetLiteralValue() is string literalValue:
                                // indexing using a string literal so we know the name of the property
                                return GetNamedPropertyType(baseObject, syntax.IndexExpression, literalValue, diagnostics);

                            default:
                                // the property name is itself an expression
                                return GetExpressionedPropertyType(baseObject, syntax.IndexExpression);
                        }
                    }

                    return UnassignableTypeSymbol.CreateErrors(DiagnosticBuilder.ForPosition(syntax.IndexExpression).ObjectsRequireStringIndex(indexType));
                }

                // index was of the wrong type
                return UnassignableTypeSymbol.CreateErrors(DiagnosticBuilder.ForPosition(syntax.BaseExpression).IndexerRequiresObjectOrArray(baseType));
            });

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
            => AssignTypeWithDiagnostics(syntax, diagnostics => {
                var errors = new List<ErrorDiagnostic>();

                var baseType = typeManager.GetTypeInfo(syntax.BaseExpression);
                CollectErrors(errors, baseType);

                if (PropagateErrorType(errors, baseType))
                {
                    return UnassignableTypeSymbol.CreateErrors(errors);
                }

                if (baseType is ResourceType resourceType)
                {
                    // We're accessing a property on the resource body.
                    baseType = resourceType.Body.Type;
                }

                if (TypeValidator.AreTypesAssignable(baseType, LanguageConstants.Object) != true)
                {
                    // can only access properties of objects
                    return UnassignableTypeSymbol.CreateErrors(DiagnosticBuilder.ForPosition(syntax.PropertyName).ObjectRequiredForPropertyAccess(baseType));
                }

                if (baseType.TypeKind == TypeKind.Any || !(baseType is ObjectType objectType))
                {
                    return LanguageConstants.Any;
                }

                return GetNamedPropertyType(objectType, syntax.PropertyName, syntax.PropertyName.IdentifierName, diagnostics);
            });

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
            => AssignType(syntax, () => {
                var errors = new List<ErrorDiagnostic>();
                var argumentTypes = syntax.Arguments.Select(arg => typeManager.GetTypeInfo(arg)).ToList();

                foreach (TypeSymbol argumentType in argumentTypes)
                {
                    CollectErrors(errors, argumentType);
                }

                switch (bindings[syntax])
                {
                    case UnassignableSymbol errorSymbol:
                        // function bind failure - pass the error along
                        return UnassignableTypeSymbol.CreateErrors(errors.Concat(errorSymbol.GetDiagnostics()));

                    case FunctionSymbol function:
                        return GetFunctionSymbolType(function, syntax.OpenParen, syntax.CloseParen, syntax.Arguments, argumentTypes, errors);

                    default:
                        return UnassignableTypeSymbol.CreateErrors(errors.Append(DiagnosticBuilder.ForPosition(syntax.Name.Span).SymbolicNameIsNotAFunction(syntax.Name.IdentifierName)));
                }
            });

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
            => AssignType(syntax, () => {

                base.VisitInstanceFunctionCallSyntax(syntax);

                var errors = new List<ErrorDiagnostic>();
                
                var baseType = typeManager.GetTypeInfo(syntax.BaseExpression);
                CollectErrors(errors, baseType);

                if (errors.Any())
                {
                    return UnassignableTypeSymbol.CreateErrors(errors);
                }

                var argumentTypes = syntax.Arguments.Select(arg => typeManager.GetTypeInfo(arg)).ToList();

                foreach (TypeSymbol argumentType in argumentTypes)
                {
                    CollectErrors(errors, argumentType);
                }

                switch (bindings[syntax])
                {
                    case UnassignableSymbol errorSymbol:
                        // bind bind failure - pass the error along
                        return UnassignableTypeSymbol.CreateErrors(errors.Concat(errorSymbol.GetDiagnostics()));

                    case FunctionSymbol function:
                        return GetFunctionSymbolType(function, syntax.OpenParen, syntax.CloseParen, syntax.Arguments, argumentTypes, errors);

                    default:
                        return UnassignableTypeSymbol.CreateErrors(errors.Append(DiagnosticBuilder.ForPosition(syntax.Name.Span).SymbolicNameIsNotAFunction(syntax.Name.IdentifierName)));
                }
            });

        private TypeSymbol VisitDeclaredSymbol(VariableAccessSyntax syntax, DeclaredSymbol declaredSymbol)
        {
            var declaringType = typeManager.GetTypeInfo(declaredSymbol.DeclaringSyntax);

            // symbols are responsible for doing their own type checking
            // the error from that should not be propagated to expressions that have type errors
            // unless we're dealing with a cyclic expression error, then propagate away!
            if (declaringType is UnassignableTypeSymbol errorType)
            {
                // replace the original error with a different one
                // we may consider suppressing this error in the future as well
                return UnassignableTypeSymbol.CreateErrors(DiagnosticBuilder.ForPosition(syntax).ReferencedSymbolHasErrors(syntax.Name.IdentifierName));
            }

            return declaringType;
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
            => AssignType(syntax, () => {
                switch (bindings[syntax])
                {
                    case UnassignableSymbol errorSymbol:
                        // variable bind failure - pass the error along
                        return errorSymbol.ToUnassignableType();

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
                    
                    case NamespaceSymbol _:
                        return new UnassignableTypeSymbol("namespace", TypeKind.Never);

                    case OutputSymbol _:
                        return UnassignableTypeSymbol.CreateErrors(DiagnosticBuilder.ForPosition(syntax.Name.Span).OutputReferenceNotSupported(syntax.Name.IdentifierName));

                    default:
                        return UnassignableTypeSymbol.CreateErrors(DiagnosticBuilder.ForPosition(syntax.Name.Span).SymbolicNameIsNotAVariableOrParameter(syntax.Name.IdentifierName));
                }
            });

        private static void CollectErrors(List<ErrorDiagnostic> errors, ITypeReference reference)
        {

            errors.AddRange(reference.Type.GetDiagnostics());
        }

        private static TypeSymbol GetFunctionSymbolType(
            FunctionSymbol function,
            Token openParen,
            Token closeParen,
            ImmutableArray<FunctionArgumentSyntax> argumentSyntaxes,
            IList<TypeSymbol> argumentTypes,
            IList<ErrorDiagnostic> errors)
        {
            // Recover argument type errors so we can continue type checking for the parent function call.
            var recoveredArgumentTypes = argumentTypes
                .Select(t => t.TypeKind == TypeKind.Error ? LanguageConstants.Any : t)
                .ToList();

            var matches = FunctionResolver.GetMatches(
                function,
                recoveredArgumentTypes,
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
                return UnassignableTypeSymbol.CreateErrors(errors);
            }

            if (matches.Count == 1)
            {
                // we have an exact match or a single ambiguous match
                // return its type
                return matches.Single().ReturnType;
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

            return UnassignableTypeSymbol.CreateErrors(DiagnosticBuilder.ForPosition(propertyExpressionPositionable).NoPropertiesAllowed(baseType));
        }

        /// <summary>
        /// Gets the type of the property whose name we can obtain at compile-time.
        /// </summary>
        /// <param name="baseType">The base object type</param>
        /// <param name="propertyExpressionPositionable">The position of the property name expression</param>
        /// <param name="propertyName">The resolved property name</param>
        /// <param name="diagnostics">List of diagnostics to append diagnostics</param>
        private static TypeSymbol GetNamedPropertyType(ObjectType baseType, IPositionable propertyExpressionPositionable, string propertyName, IList<Diagnostic> diagnostics)
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
                    diagnostics.Add(writeOnlyDiagnostic);

                    if (writeOnlyDiagnostic.Level == DiagnosticLevel.Error)
                    {
                        return UnassignableTypeSymbol.CreateErrors(Enumerable.Empty<ErrorDiagnostic>());
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

            diagnostics.Add(unknownPropertyDiagnostic);

            return (unknownPropertyDiagnostic.Level == DiagnosticLevel.Error) ? UnassignableTypeSymbol.CreateErrors(Enumerable.Empty<ErrorDiagnostic>()) : LanguageConstants.Any;
        }

        private IEnumerable<Diagnostic> GetOutputDeclarationDiagnostics(TypeSymbol assignedType, OutputDeclarationSyntax syntax)
        {
            var valueType = typeManager.GetTypeInfo(syntax.Value);

            // this type is not a property in a symbol so the semantic error visitor won't collect the errors automatically
            if (valueType is UnassignableTypeSymbol)
            {
                return valueType.GetDiagnostics();
            }

            if (TypeValidator.AreTypesAssignable(valueType, assignedType) == false)
            {
                return DiagnosticBuilder.ForPosition(syntax.Value).OutputTypeMismatch(assignedType, valueType).AsEnumerable();
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> ValidateDefaultValue(ParameterDefaultValueSyntax defaultValueSyntax, TypeSymbol assignedType)
        {
            // figure out type of the default value
            var defaultValueType = typeManager.GetTypeInfo(defaultValueSyntax.DefaultValue);

            // this type is not a property in a symbol so the semantic error visitor won't collect the errors automatically
            if (defaultValueType is UnassignableTypeSymbol)
            {
                return defaultValueType.GetDiagnostics();
            }

            if (TypeValidator.AreTypesAssignable(defaultValueType, assignedType) == false)
            {
                return DiagnosticBuilder.ForPosition(defaultValueSyntax.DefaultValue).ParameterTypeMismatch(assignedType, defaultValueType).AsEnumerable();
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> ValidateIdentifierAccess(ParameterDeclarationSyntax syntax)
        {
            return SyntaxAggregator.Aggregate(syntax, new List<Diagnostic>(), (accumulated, current) =>
                {
                    if (current is VariableAccessSyntax)
                    {
                        var symbol = bindings[current];
                        
                        // Error: already has error info attached, no need to add more
                        // Parameter: references are permitted in other parameters' default values as long as there is not a cycle (BCP080)
                        // Function: we already validate that a function cannot be used as a variable (BCP063)
                        // Output: we already validate that outputs cannot be referenced in expressions (BCP058)
                        if (symbol.Kind != SymbolKind.Error &&
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

        private TypeSymbol GetParameterAssignedType(ParameterDeclarationSyntax syntax, TypeSymbol declaredType)
        {
            var assignedType = declaredType;
            if (object.ReferenceEquals(assignedType, LanguageConstants.String))
            {
                var allowedItemTypes = SyntaxHelper.TryGetAllowedItems(syntax)?
                    .Select(item => typeManager.GetTypeInfo(item));

                if (allowedItemTypes != null && allowedItemTypes.All(itemType => itemType is StringLiteralType))
                {
                    assignedType = UnionType.Create(allowedItemTypes);
                }
                else
                {
                    // In order to support assignment for a generic string to enum-typed properties (which generally is forbidden),
                    // we need to relax the validation for string parameters without 'allowed' values specified.
                    assignedType = LanguageConstants.LooseString;
                }
            }

            return assignedType;
        }

        private TypeSymbol? GetDeclaredTypeInternal(ArraySyntax syntax)
        {
            var parent = this.hierarchy.GetParent(syntax);
            switch (parent)
            {
                // we are only handling paths in the AST that are going to produce a declared type
                // arrays can exist under a variable declaration, but variables don't have declared types,
                // so we don't need to check that case
                case ObjectPropertySyntax _:
                    // this array is a value of the property
                    // the declared type should be the same as the array
                    return GetDeclaredTypeAssignment(parent)?.Reference.Type;
            }
            
            return null;
        }

        private TypeSymbol? GetDeclaredTypeInternal(ArrayItemSyntax syntax)
        {
            var parent = this.hierarchy.GetParent(syntax);
            switch (parent)
            {
                case ArraySyntax _:
                    // array items can only have array parents
                    // use the declared item type
                    var parentType = GetDeclaredTypeAssignment(parent)?.Reference.Type;
                    if (parentType is ArrayType arrayType)
                    {
                        return arrayType.Item.Type;
                    }

                    break;
            }

            return null;
        }

        private TypeSymbol? GetDeclaredTypeInternal(ObjectSyntax syntax)
        {
            var parent = this.hierarchy.GetParent(syntax);
            if (parent == null)
            {
                return null;
            }

            var parentType = GetDeclaredTypeAssignment(parent)?.Reference.Type;
            if (parentType == null)
            {
                return null;
            }

            switch (parent)
            {
                case ResourceDeclarationSyntax _ when parentType is ResourceType resourceType:
                    // the object literal's parent is a resource declaration, which makes this the body of the resource
                    // the declared type will be the same as the parent
                    return ResolveDiscriminatedObjects(resourceType.Body.Type, syntax);

                case ParameterDeclarationSyntax parameterDeclaration when ReferenceEquals(parameterDeclaration.Modifier, syntax):
                    // the object is a modifier of a parameter type
                    // the declared type should be the appropriate modifier type
                    // however we need the parameter's assigned type to determine the modifier type
                    var parameterAssignedType = GetParameterAssignedType(parameterDeclaration, parentType.Type.Type);
                    return LanguageConstants.CreateParameterModifierType(parentType, parameterAssignedType);

                case ObjectPropertySyntax _:
                    // the object is the value of a property of another object
                    // use the declared type of the property
                    return ResolveDiscriminatedObjects(parentType, syntax);

                case ArrayItemSyntax _:
                    // the object is an item in an array
                    // use the item's type
                    return ResolveDiscriminatedObjects(parentType, syntax);
            }

            return null;
        }

        private TypeSymbol? GetDeclaredTypeInternal(ObjectPropertySyntax syntax)
        {
            var propertyName = syntax.TryGetKeyText();
            var parent = this.hierarchy.GetParent(syntax);
            if (propertyName == null || parent == null)
            {
                // the property name is an interpolated string (expression) OR the parent is missing
                // cannot establish declared type
                // TODO: Improve this when we have constant folding
                return null;
            }

            var assignment = GetDeclaredTypeAssignment(parent);
            switch (assignment?.Reference.Type)
            {
                case ObjectType objectType:
                    // lookup declared property
                    if (objectType.Properties.TryGetValue(propertyName, out var property))
                    {
                        return property.TypeReference.Type;
                    }

                    // if there are additional properties, try those
                    if (objectType.AdditionalPropertiesType != null)
                    {
                        return objectType.AdditionalPropertiesType.Type;
                    }

                    break;

                case DiscriminatedObjectType discriminated:
                    if (string.Equals(propertyName, discriminated.DiscriminatorProperty.Name, LanguageConstants.IdentifierComparison))
                    {
                        // the property is the discriminator property - use its type
                        return discriminated.DiscriminatorProperty.TypeReference.Type;
                    }

                    break;
            }

            return null;
        }

        private TypeSymbol? ResolveDiscriminatedObjects(TypeSymbol type, ObjectSyntax syntax)
        {
            if (!(type is DiscriminatedObjectType discriminated))
            {
                // not a discriminated object type - return as-is
                return type;
            }

            var discriminatorProperties = syntax.Properties
                .Where(p => string.Equals(p.TryGetKeyText(), discriminated.DiscriminatorKey, LanguageConstants.IdentifierComparison))
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
            if (!(discriminatorProperty.Value is StringSyntax stringSyntax))
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
    }
}