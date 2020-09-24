// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parser;
using Bicep.Core.Resources;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public sealed class TypeAssignmentVisitor : SyntaxVisitor
    {
        private readonly IReadOnlyDictionary<SyntaxBase, Symbol> bindings;

        private readonly IReadOnlyDictionary<SyntaxBase, ImmutableArray<DeclaredSymbol>> cyclesBySyntax;

        private readonly IDictionary<SyntaxBase, TypeSymbol> assignedTypes;

        public TypeAssignmentVisitor(IReadOnlyDictionary<SyntaxBase, Symbol> bindings, IReadOnlyDictionary<SyntaxBase, ImmutableArray<DeclaredSymbol>> cyclesBySyntax)
        {
            // bindings will be modified by name binding after this object is created
            // so we can't make an immutable copy here
            // (using the IReadOnlyDictionary to prevent accidental mutation)
            this.bindings = bindings; 
            this.cyclesBySyntax = cyclesBySyntax;
            this.assignedTypes = new Dictionary<SyntaxBase, TypeSymbol>();
        }

        public TypeSymbol GetTypeSymbol(SyntaxBase syntax) => VisitAndReturnType(syntax);

        private TypeSymbol? CheckForCyclicError(SyntaxBase syntax)
        {
            if (cyclesBySyntax.TryGetValue(syntax, out var cycle))
            {
                // there's a cycle. stop visiting now or we never will!
                if (cycle.Length == 1)
                {
                    return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax).CyclicSelfReference());
                }

                var syntaxBinding = bindings[syntax];
                
                // show the cycle as originating from the current syntax symbol
                var cycleSuffix = cycle.TakeWhile(x => x != syntaxBinding);
                var cyclePrefix = cycle.Skip(cycleSuffix.Count());
                var orderedCycle = cyclePrefix.Concat(cycleSuffix);

                return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax).CyclicExpression(orderedCycle.Select(x => x.Name)));
            }

            return null;
        }

        private void AssignTypeWithCaching(SyntaxBase syntax, Func<TypeSymbol> assignFunc)
        {
            if (assignedTypes.ContainsKey(syntax))
            {
                return;
            }

            var cyclicErrorType = CheckForCyclicError(syntax);
            if (cyclicErrorType != null)
            {
                assignedTypes[syntax] = cyclicErrorType;
                return;
            }

            assignedTypes[syntax] = assignFunc();
        }

        private TypeSymbol VisitAndReturnType(SyntaxBase syntax)
        {
            Visit(syntax);

            if (!assignedTypes.TryGetValue(syntax, out var typeSymbol))
            {
                return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax).InvalidExpression());
            }

            return typeSymbol;
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
            => AssignTypeWithCaching(syntax, () => {
                var stringSyntax = syntax.TryGetType();

                if (stringSyntax != null && stringSyntax.IsInterpolated())
                {
                    // TODO: in the future, we can relax this check to allow interpolation with compile-time constants.
                    // right now, codegen will still generate a format string however, which will cause problems for the type.
                    return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.Type).ResourceTypeInterpolationUnsupported());
                }

                var stringContent = stringSyntax?.GetLiteralValue();
                if (stringContent == null)
                {
                    return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.Type).InvalidResourceType());
                }

                // TODO: This needs proper namespace, type, and version resolution logic in the future
                var typeReference = ResourceTypeReference.TryParse(stringContent);
                if (typeReference == null)
                {
                    return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.Type).InvalidResourceType());
                }

                // TODO: Construct/lookup type information based on JSON schema or swagger
                // for now assuming very basic resource schema
                return new ResourceType(stringContent, LanguageConstants.CreateResourceProperties(typeReference), additionalPropertiesType: null, typeReference);
            });

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
            => AssignTypeWithCaching(syntax, () => {
                var primitiveType = GetPrimitiveTypeByName(syntax.Type.TypeName);

                if (primitiveType == null)
                {
                    return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.Type).InvalidParameterType());
                }

                // TODO if this is a string parameter with 'allowed' set, convert it to a union of string literal types
                return primitiveType;
            });

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
            => AssignTypeWithCaching(syntax, () => {
                return VisitAndReturnType(syntax.Value);
            });

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
            => AssignTypeWithCaching(syntax, () => {
                var primitiveType = GetPrimitiveTypeByName(syntax.Type.TypeName);

                if (primitiveType == null)
                {
                    return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.Type).InvalidOutputType());
                }

                return primitiveType;
            });

        public override void VisitBooleanLiteralSyntax(BooleanLiteralSyntax syntax)
            => AssignTypeWithCaching(syntax, () => LanguageConstants.Bool);

        public override void VisitStringSyntax(StringSyntax syntax)
            => AssignTypeWithCaching(syntax, () => {
                if (!syntax.IsInterpolated())
                {
                    // uninterpolated strings have a known type
                    return new StringLiteralType(syntax.GetLiteralValue());
                }

                var errors = new List<ErrorDiagnostic>();

                foreach (var interpolatedExpression in syntax.Expressions)
                {
                    var expressionType = VisitAndReturnType(interpolatedExpression);
                    CollectErrors(errors, expressionType);
                }

                if (errors.Any())
                {
                    return new ErrorTypeSymbol(errors);
                }

                // normally we would also do an assignability check, but we allow "any" type in string interpolation expressions
                // so the assignability check cannot possibly fail (we already collected type errors from the inner expressions at this point)
                return LanguageConstants.String;
            });

        public override void VisitNumericLiteralSyntax(NumericLiteralSyntax syntax)
            => AssignTypeWithCaching(syntax, () => LanguageConstants.Int);

        public override void VisitNullLiteralSyntax(NullLiteralSyntax syntax)
            => AssignTypeWithCaching(syntax, () => LanguageConstants.Null);

        public override void VisitSkippedTriviaSyntax(SkippedTriviaSyntax syntax)
            => AssignTypeWithCaching(syntax, () => {
                // error should have already been raised by the ParseDiagnosticsVisitor - no need to add another
                return new ErrorTypeSymbol(Enumerable.Empty<ErrorDiagnostic>());
            });

        public override void VisitObjectSyntax(ObjectSyntax syntax)
            => AssignTypeWithCaching(syntax, () => {
                var errors = new List<ErrorDiagnostic>();

                foreach (var objectProperty in syntax.Properties)
                {
                    var propertyType = VisitAndReturnType(objectProperty);
                    CollectErrors(errors, propertyType);
                }

                if (errors.Any())
                {
                    return new ErrorTypeSymbol(errors);
                }

                // type results are cached
                var properties = syntax.Properties
                    .GroupBy(p => p.GetKeyText(), LanguageConstants.IdentifierComparer)
                    .Select(group => new TypeProperty(group.Key, UnionType.Create(group.Select(p => assignedTypes[p]))));

                // TODO: Add structural naming?
                return new NamedObjectType(LanguageConstants.Object.Name, properties, additionalPropertiesType: null);
            });

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
            => AssignTypeWithCaching(syntax, () => {
                Visit(syntax.Value);

                return assignedTypes[syntax.Value];
            });

        public override void VisitArrayItemSyntax(ArrayItemSyntax syntax)
            => AssignTypeWithCaching(syntax, () => {
                Visit(syntax.Value);

                return assignedTypes[syntax.Value];
            });

        public override void VisitParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax)
            => AssignTypeWithCaching(syntax, () => {
                Visit(syntax.Expression);

                return assignedTypes[syntax.Expression];
            });

        public override void VisitFunctionArgumentSyntax(FunctionArgumentSyntax syntax)
            => AssignTypeWithCaching(syntax, () => {
                Visit(syntax.Expression);

                return assignedTypes[syntax.Expression];
            });

        public override void VisitArraySyntax(ArraySyntax syntax)
            => AssignTypeWithCaching(syntax, () => {
                var errors = new List<ErrorDiagnostic>();

                var itemTypes = new List<TypeSymbol>(syntax.Children.Length);
                foreach (var arrayItem in syntax.Children)
                {
                    var itemType = VisitAndReturnType(arrayItem);
                    itemTypes.Add(itemType);
                    CollectErrors(errors, itemType);
                }

                if (errors.Any())
                {
                    return new ErrorTypeSymbol(errors);
                }

                var aggregatedItemType = UnionType.Create(itemTypes);
                if (aggregatedItemType.TypeKind == TypeKind.Union || aggregatedItemType.TypeKind == TypeKind.Never)
                {
                    // array contains a mix of item types or is empty
                    // assume array of any for now
                    return LanguageConstants.Array;
                }

                return new TypedArrayType(aggregatedItemType);
            });

        public override void VisitTernaryOperationSyntax(TernaryOperationSyntax syntax)
            => AssignTypeWithCaching(syntax, () => {
                var errors = new List<ErrorDiagnostic>();

                // ternary operator requires the condition to be of bool type
                var conditionType = VisitAndReturnType(syntax.ConditionExpression);
                CollectErrors(errors, conditionType);

                var trueType = VisitAndReturnType(syntax.TrueExpression);
                CollectErrors(errors, trueType);

                var falseType = VisitAndReturnType(syntax.FalseExpression);
                CollectErrors(errors, falseType);

                if (errors.Any())
                {
                    return new ErrorTypeSymbol(errors);
                }

                var expectedConditionType = LanguageConstants.Bool;
                if (TypeValidator.AreTypesAssignable(conditionType, expectedConditionType) != true)
                {
                    return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.ConditionExpression).ValueTypeMismatch(expectedConditionType.Name));
                }
                
                // the return type is the union of true and false expression types
                return UnionType.Create(trueType, falseType);
            });

        public override void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax)
            => AssignTypeWithCaching(syntax, () => {
                var errors = new List<ErrorDiagnostic>();

                var operandType1 = VisitAndReturnType(syntax.LeftExpression);
                CollectErrors(errors, operandType1);

                var operandType2 = VisitAndReturnType(syntax.RightExpression);
                CollectErrors(errors, operandType2);

                if (errors.Any())
                {
                    return new ErrorTypeSymbol(errors);
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
                return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax).BinaryOperatorInvalidType(Operators.BinaryOperatorToText[syntax.Operator], operandType1.Name, operandType2.Name));
            });

        public override void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax)
            => AssignTypeWithCaching(syntax, () => {
                var errors = new List<ErrorDiagnostic>();

                // TODO: When we add number type, this will have to be adjusted
                var expectedOperandType = syntax.Operator switch
                {
                    UnaryOperator.Not => LanguageConstants.Bool,
                    UnaryOperator.Minus => LanguageConstants.Int,
                    _ => throw new NotImplementedException()
                };

                var operandType = VisitAndReturnType(syntax.Expression);
                CollectErrors(errors, operandType);

                if (errors.Any())
                {
                    return new ErrorTypeSymbol(errors);
                }

                if (TypeValidator.AreTypesAssignable(operandType, expectedOperandType) != true)
                {
                    return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax).UnaryOperatorInvalidType(Operators.UnaryOperatorToText[syntax.Operator], operandType.Name));
                }

                return expectedOperandType;
            });

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
            => AssignTypeWithCaching(syntax, () => {
                var errors = new List<ErrorDiagnostic>();

                var baseType = VisitAndReturnType(syntax.BaseExpression);
                CollectErrors(errors, baseType);

                var indexType = VisitAndReturnType(syntax.IndexExpression);
                CollectErrors(errors, indexType);

                if (errors.Any() || indexType.TypeKind == TypeKind.Error)
                {
                    return new ErrorTypeSymbol(errors);
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
                    return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.IndexExpression).StringOrIntegerIndexerRequired(indexType));
                }

                if (baseType is ArrayType baseArray)
                {
                    // we are indexing over an array
                    if (TypeValidator.AreTypesAssignable(indexType, LanguageConstants.Int) == true)
                    {
                        // the index is of "any" type or integer type
                        // return the item type
                        return baseArray.ItemType;
                    }

                    return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.IndexExpression).ArraysRequireIntegerIndex(indexType));
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
                            case StringSyntax @string when @string.IsInterpolated() == false:
                                var propertyName = @string.GetLiteralValue();
                                
                                return GetNamedPropertyType(baseObject, syntax.IndexExpression, propertyName);

                            default:
                                // the property name is itself an expression
                                return GetExpressionedPropertyType(baseObject, syntax.IndexExpression);
                        }
                    }

                    return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.IndexExpression).ObjectsRequireStringIndex(indexType));
                }

                // index was of the wrong type
                return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.BaseExpression).IndexerRequiresObjectOrArray(baseType));
            });

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
            => AssignTypeWithCaching(syntax, () => {
                var errors = new List<ErrorDiagnostic>();

                var baseType = VisitAndReturnType(syntax.BaseExpression);
                CollectErrors(errors, baseType);

                if (errors.Any())
                {
                    return new ErrorTypeSymbol(errors);
                }

                if (TypeValidator.AreTypesAssignable(baseType, LanguageConstants.Object) != true)
                {
                    // can only access properties of objects
                    return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.PropertyName).ObjectRequiredForPropertyAccess(baseType));
                }

                if (baseType.TypeKind == TypeKind.Any || !(baseType is ObjectType objectType))
                {
                    return LanguageConstants.Any;
                }

                return GetNamedPropertyType(objectType, syntax.PropertyName, syntax.PropertyName.IdentifierName);
            });

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
            => AssignTypeWithCaching(syntax, () => {
                var errors = new List<ErrorDiagnostic>();
                var argumentTypes = syntax.Arguments.Select(syntax1 => VisitAndReturnType(syntax1)).ToList();

                foreach (TypeSymbol argumentType in argumentTypes)
                {
                    CollectErrors(errors, argumentType);
                }

                switch (bindings[syntax])
                {
                    case ErrorSymbol errorSymbol:
                        // function bind failure - pass the error along
                        return new ErrorTypeSymbol(errors.Concat(errorSymbol.GetDiagnostics()));

                    case FunctionSymbol function:
                        return GetFunctionSymbolType(syntax, function, syntax.Arguments, argumentTypes, errors);

                    default:
                        return new ErrorTypeSymbol(errors.Append(DiagnosticBuilder.ForPosition(syntax.Name.Span).SymbolicNameIsNotAFunction(syntax.Name.IdentifierName)));
                }
            });

        private TypeSymbol VisitDeclaredSymbol(VariableAccessSyntax syntax, DeclaredSymbol declaredSymbol)
        {
            var declaringType = VisitAndReturnType(declaredSymbol.DeclaringSyntax);

            // symbols are responsible for doing their own type checking
            // the error from that should not be propagated to expressions that have type errors
            // unless we're dealing with a cyclic expression error, then propagate away!
            if (declaringType is ErrorTypeSymbol errorType)
            {
                // replace the original error with a different one
                // we may consider suppressing this error in the future as well
                return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax).ReferencedSymbolHasErrors(syntax.Name.IdentifierName));
            }

            return declaringType;
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
            => AssignTypeWithCaching(syntax, () => {
                switch (bindings[syntax])
                {
                    case ErrorSymbol errorSymbol:
                        // variable bind failure - pass the error along
                        return errorSymbol.ToErrorType();

                    case ResourceSymbol resource:
                        // resource bodies can participate in cycles
                        // need to explicitly force a type check on the body
                        return VisitDeclaredSymbol(syntax, resource);

                    case ParameterSymbol parameter:
                        return VisitDeclaredSymbol(syntax, parameter);

                    case VariableSymbol variable:
                        return VisitDeclaredSymbol(syntax, variable);

                    case NamespaceSymbol @namespace:
                        return @namespace!;

                    case OutputSymbol _:
                        return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.Name.Span).OutputReferenceNotSupported(syntax.Name.IdentifierName));

                    default:
                        return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.Name.Span).SymbolicNameIsNotAVariableOrParameter(syntax.Name.IdentifierName));
                }
            });

        private static TypeSymbol? GetPrimitiveTypeByName(string typeName)
        {
            if (LanguageConstants.DeclarationTypes.TryGetValue(typeName, out TypeSymbol primitiveType))
            {
                return primitiveType;
            }

            return null;
        }

        private static void CollectErrors(List<ErrorDiagnostic> errors, TypeSymbol type)
        {
            errors.AddRange(type.GetDiagnostics());
        }

        private static TypeSymbol GetFunctionSymbolType(
            FunctionCallSyntax syntax,
            FunctionSymbol function,
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
                    var argumentsSpan = TextSpan.Between(syntax.OpenParen, syntax.CloseParen);

                    errors.Add(DiagnosticBuilder.ForPosition(argumentsSpan).ArgumentCountMismatch(actualCount, mininumArgumentCount, maximumArgumentCount));
                }
            }

            if (errors.Any())
            {
                return new ErrorTypeSymbol(errors);
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

            return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(propertyExpressionPositionable).NoPropertiesAllowed(baseType));
        }

        /// <summary>
        /// Gets the type of the property whose name we can obtain at compile-time.
        /// </summary>
        /// <param name="baseType">The base object type</param>
        /// <param name="propertyExpressionPositionable">The position of the property name expression</param>
        /// <param name="propertyName">The resolved property name</param>
        private static TypeSymbol GetNamedPropertyType(ObjectType baseType, IPositionable propertyExpressionPositionable, string propertyName)
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
                    return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(propertyExpressionPositionable).WriteOnlyProperty(baseType, propertyName));
                }

                // there is - return its type
                return declaredProperty.Type;
            }

            // the property is not declared
            // check additional properties
            if (baseType.AdditionalPropertiesType != null)
            {
                // yes - return the additional property type
                return baseType.AdditionalPropertiesType;
            }

            return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(propertyExpressionPositionable).UnknownProperty(baseType, propertyName));
        }
    }
}