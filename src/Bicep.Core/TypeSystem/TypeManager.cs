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
    public class TypeManager : ITypeManager
    {
        private readonly IReadOnlyDictionary<SyntaxBase,Symbol> bindings;

        // stores results of type checks
        private readonly ConcurrentDictionary<SyntaxBase, TypeSymbol> typeCheckCache = new ConcurrentDictionary<SyntaxBase, TypeSymbol>();

        public TypeManager(IReadOnlyDictionary<SyntaxBase, Symbol> bindings)
        {
            // bindings will be modified by name binding after this object is created
            // so we can't make an immutable copy here
            // (using the IReadOnlyDictionary to prevent accidental mutation)
            this.bindings = bindings; 
        }

        public TypeSymbol GetTypeInfo(SyntaxBase syntax, TypeManagerContext context) => GetTypeInfoInternal(context, syntax);

        // TODO: This does not recognize non-resource named objects yet
        public TypeSymbol? GetTypeByName(string? typeName)
        {
            /*
             * Obtaining the type by name currently does not involve processing outgoing edges in the expression graph (function or variable refs)
             * This means that we do not need to check for cycles. However, if that ever changes, ensure that proper cycle detection is added here.
             */

            if (typeName == null)
            {
                return null;
            }

            if (LanguageConstants.DeclarationTypes.TryGetValue(typeName, out TypeSymbol primitiveType))
            {
                return primitiveType;
            }

            // TODO: This needs proper namespace, type, and version resolution logic in the future
            ResourceTypeReference? typeReference = ResourceTypeReference.TryParse(typeName);
            if (typeReference == null)
            {
                return null;
            }

            // TODO: Construct/lookup type information based on JSON schema or swagger
            // for now assuming very basic resource schema
            return new ResourceType(typeName, LanguageConstants.CreateResourceProperties(typeReference), additionalPropertiesType: null, typeReference);
        }

        private TypeSymbol GetTypeInfoInternal(TypeManagerContext context, SyntaxBase syntax)
        {
            // local function because I don't want this called directly
            TypeSymbol GetTypeInfoWithoutCache()
            {
                if (context.TryMarkVisited(syntax) == false)
                {
                    // we have already visited this node, which means we have a cycle
                    // all the nodes that were visited are involved in the cycle
                    return new ErrorTypeSymbol(context.GetVisitedNodes().Select(node => DiagnosticBuilder.ForPosition(node.Span).CyclicExpression()));
                }

                switch (syntax)
                {
                    case BooleanLiteralSyntax _:
                        return LanguageConstants.Bool;

                    case NumericLiteralSyntax _:
                        return LanguageConstants.Int;

                    case StringSyntax @string:
                        return GetStringType(context, @string);

                    case ObjectSyntax @object:
                        return GetObjectType(context, @object);

                    case ObjectPropertySyntax objectProperty:
                        return GetTypeInfoInternal(context, objectProperty.Value);

                    case ArraySyntax array:
                        return GetArrayType(context, array);

                    case ArrayItemSyntax arrayItem:
                        return GetTypeInfoInternal(context, arrayItem.Value);

                    case BinaryOperationSyntax binary:
                        return GetBinaryOperationType(context, binary);

                    case FunctionArgumentSyntax functionArgument:
                        return GetTypeInfoInternal(context, functionArgument.Expression);

                    case FunctionCallSyntax functionCall:
                        return GetFunctionCallType(context, functionCall);

                    case NullLiteralSyntax _:
                        // null is its own type
                        return LanguageConstants.Null;

                    case ParenthesizedExpressionSyntax parenthesized:
                        // parentheses don't change the type of the parenthesized expression
                        return GetTypeInfoInternal(context, parenthesized.Expression);

                    case PropertyAccessSyntax propertyAccess:
                        return GetPropertyAccessType(context, propertyAccess);

                    case ArrayAccessSyntax arrayAccess:
                        return GetArrayAccessType(context, arrayAccess);

                    case TernaryOperationSyntax ternary:
                        return GetTernaryOperationType(context, ternary);

                    case UnaryOperationSyntax unary:
                        return GetUnaryOperationType(context, unary);

                    case VariableAccessSyntax variableAccess:
                        return GetVariableAccessType(context, variableAccess);

                    case SkippedTriviaSyntax _:
                        // error should have already been raised by the ParseDiagnosticsVisitor - no need to add another
                        return new ErrorTypeSymbol(Enumerable.Empty<ErrorDiagnostic>());

                    default:
                        return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax).InvalidExpression());
                }
            }

            if (this.typeCheckCache.TryGetValue(syntax, out var cachedType))
            {
                // the result was already in our cache
                return cachedType;
            }

            var type = GetTypeInfoWithoutCache();
            this.typeCheckCache.TryAdd(syntax, type);

            return type;
        }

        private TypeSymbol GetStringType(TypeManagerContext context, StringSyntax @string)
        {
            if (@string.IsInterpolated() == false)
            {
                // uninterpolated strings have a known type
                return new StringLiteralType(@string.GetLiteralValue());
            }

            var errors = new List<ErrorDiagnostic>();

            foreach (var interpolatedExpression in @string.Expressions)
            {
                var expressionType = this.GetTypeInfoInternal(context, interpolatedExpression);
                CollectErrors(errors, expressionType);
            }

            if (errors.Any())
            {
                return new ErrorTypeSymbol(errors);
            }

            // normally we would also do an assignability check, but we allow "any" type in string interpolation expressions
            // so the assignability check cannot possibly fail (we already collected type errors from the inner expressions at this point)
            return LanguageConstants.String;
        }

        private TypeSymbol GetObjectType(TypeManagerContext context, ObjectSyntax @object)
        {
            var errors = new List<ErrorDiagnostic>();

            foreach (ObjectPropertySyntax objectProperty in @object.Properties)
            {
                var propertyType = this.GetTypeInfoInternal(context, objectProperty);
                CollectErrors(errors, propertyType);
            }

            if (errors.Any())
            {
                return new ErrorTypeSymbol(errors);
            }

            // type results are cached
            var properties = @object.Properties
                .GroupBy(p => p.GetKeyText(), LanguageConstants.IdentifierComparer)
                .Select(group => new TypeProperty(group.Key, UnionType.Create(group.Select(p => this.GetTypeInfoInternal(context, p.Value)))));

            // TODO: Add structural naming?
            return new NamedObjectType(LanguageConstants.Object.Name, properties, additionalPropertiesType: null);
        }

        private TypeSymbol GetArrayType(TypeManagerContext context, ArraySyntax array)
        {
            var errors = new List<ErrorDiagnostic>();

            var itemTypes = new List<TypeSymbol>(array.Children.Length);
            foreach (SyntaxBase arrayItem in array.Children)
            {
                var itemType = this.GetTypeInfoInternal(context, arrayItem);
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
        }

        private TypeSymbol GetPropertyAccessType(TypeManagerContext context, PropertyAccessSyntax syntax)
        {
            var errors = new List<ErrorDiagnostic>();

            var baseType = this.GetTypeInfoInternal(context, syntax.BaseExpression);
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

            return this.GetNamedPropertyType(objectType, syntax.PropertyName, syntax.PropertyName.IdentifierName);
        }

        /// <summary>
        /// Gets the type of the property whose name is an expression.
        /// </summary>
        /// <param name="baseType">The base object type</param>
        /// <param name="propertyExpressionPositionable">The position of the property name expression</param>
        private TypeSymbol GetExpressionedPropertyType(ObjectType baseType, IPositionable propertyExpressionPositionable)
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
        private TypeSymbol GetNamedPropertyType(ObjectType baseType, IPositionable propertyExpressionPositionable, string propertyName)
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

        private TypeSymbol GetArrayAccessType(TypeManagerContext context, ArrayAccessSyntax syntax)
        {
            var errors = new List<ErrorDiagnostic>();

            var baseType = this.GetTypeInfoInternal(context, syntax.BaseExpression);
            CollectErrors(errors, baseType);

            var indexType = this.GetTypeInfoInternal(context, syntax.IndexExpression);
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
                            
                            return this.GetNamedPropertyType(baseObject, syntax.IndexExpression, propertyName);

                        default:
                            // the property name is itself an expression
                            return this.GetExpressionedPropertyType(baseObject, syntax.IndexExpression);
                    }
                }

                return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.IndexExpression).ObjectsRequireStringIndex(indexType));
            }

            // index was of the wrong type
            return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.BaseExpression).IndexerRequiresObjectOrArray(baseType));
        }

        private TypeSymbol GetVariableAccessType(TypeManagerContext context, VariableAccessSyntax syntax)
        {
            var symbol = this.ResolveSymbol(syntax);

            switch (symbol)
            {
                case ErrorSymbol errorSymbol:
                    // variable bind failure - pass the error along
                    return errorSymbol.ToErrorType();

                case ResourceSymbol resource:
                    return GetResourceType(context, syntax, resource);

                case ParameterSymbol parameter:
                    return GetParameterType(context, syntax, parameter);

                case VariableSymbol variable:
                    return HandleSymbolType(syntax.Name.IdentifierName, syntax.Name.Span, variable.GetVariableType(context));

                case OutputSymbol _:
                    return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.Name.Span).OutputReferenceNotSupported(syntax.Name.IdentifierName));

                default:
                    return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.Name.Span).SymbolicNameIsNotAVariableOrParameter(syntax.Name.IdentifierName));
            }
        }

        private TypeSymbol GetResourceType(TypeManagerContext context, VariableAccessSyntax syntax, ResourceSymbol resource)
        {
            // resource bodies can participate in cycles
            // need to explicitly force a type check on the body
            if (ContainsCyclicExpressionError(this.GetTypeInfoInternal(context, resource.Body)))
            {
                return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.Name.Span).CyclicExpression());
            }

            return HandleSymbolType(syntax.Name.IdentifierName, syntax.Name.Span, resource.Type);
        }

        private TypeSymbol GetParameterType(TypeManagerContext context, VariableAccessSyntax syntax, ParameterSymbol parameter)
        {
            // parameter default values can participate in cycles with their own parameters or other symbols
            // need to explicitly force a type check to detect that
            SyntaxBase? expressionToTypeCheck;
            switch (parameter.Modifier)
            {
                case ParameterDefaultValueSyntax defaultValueSyntax:
                    expressionToTypeCheck = defaultValueSyntax.DefaultValue;
                    break;

                case ObjectSyntax modifierSyntax:
                    // technically it's redundant to type check the entire object because we have existing
                    // compile-time constant checks, but it shouldn't harm anything
                    expressionToTypeCheck = modifierSyntax;
                    break;

                case null:
                    // there's no default value or modifier - this parameter cannot participate in a cycle
                    expressionToTypeCheck = null;
                    break;

                default:
                    throw new NotImplementedException($"Unexpected parameter modifier type '{parameter.Modifier.GetType()}");
            }

            if (expressionToTypeCheck != null && ContainsCyclicExpressionError(this.GetTypeInfoInternal(context, expressionToTypeCheck)))
            {
                return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.Name.Span).CyclicExpression());
            }

            return HandleSymbolType(syntax.Name.IdentifierName, syntax.Name.Span, parameter.Type);
        }
        
        private TypeSymbol GetFunctionCallType(TypeManagerContext context, FunctionCallSyntax syntax)
        {
            var errors = new List<ErrorDiagnostic>();
            var argumentTypes = syntax.Arguments.Select(syntax1 => GetTypeInfoInternal(context, syntax1)).ToList();

            foreach (TypeSymbol argumentType in argumentTypes)
            {
                CollectErrors(errors, argumentType);
            }

            switch (this.ResolveSymbol(syntax))
            {
                case ErrorSymbol errorSymbol:
                    // function bind failure - pass the error along
                    return new ErrorTypeSymbol(errors.Concat(errorSymbol.GetDiagnostics()));

                case FunctionSymbol function:
                    return GetFunctionSymbolType(syntax, function, syntax.Arguments, argumentTypes, errors);

                default:
                    return new ErrorTypeSymbol(errors.Append(DiagnosticBuilder.ForPosition(syntax.Name.Span).SymbolicNameIsNotAFunction(syntax.Name.IdentifierName)));
            }
        }

        private TypeSymbol GetFunctionSymbolType(
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

        private TypeSymbol GetUnaryOperationType(TypeManagerContext context, UnaryOperationSyntax syntax)
        {
            var errors = new List<ErrorDiagnostic>();

            // TODO: When we add number type, this will have to be adjusted
            var expectedOperandType = syntax.Operator switch
            {
                UnaryOperator.Not => LanguageConstants.Bool,
                UnaryOperator.Minus => LanguageConstants.Int,
                _ => throw new NotImplementedException()
            };

            var operandType = this.GetTypeInfoInternal(context, syntax.Expression);
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
        }

        private TypeSymbol GetBinaryOperationType(TypeManagerContext context, BinaryOperationSyntax syntax)
        {
            var errors = new List<ErrorDiagnostic>();

            var operandType1 = this.GetTypeInfoInternal(context, syntax.LeftExpression);
            CollectErrors(errors, operandType1);

            var operandType2 = this.GetTypeInfoInternal(context, syntax.RightExpression);
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
        }

        private TypeSymbol GetTernaryOperationType(TypeManagerContext context, TernaryOperationSyntax syntax)
        {
            var errors = new List<ErrorDiagnostic>();

            // ternary operator requires the condition to be of bool type
            var conditionType = this.GetTypeInfoInternal(context, syntax.ConditionExpression);
            CollectErrors(errors, conditionType);

            var trueType = this.GetTypeInfoInternal(context, syntax.TrueExpression);
            CollectErrors(errors, trueType);

            var falseType = this.GetTypeInfoInternal(context, syntax.FalseExpression);
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
        }

        private static void CollectErrors(List<ErrorDiagnostic> errors, TypeSymbol type)
        {
            errors.AddRange(type.GetDiagnostics());
        }

        private TypeSymbol HandleSymbolType(string symbolName, TextSpan symbolNameSpan, TypeSymbol symbolType)
        {
            // symbols are responsible for doing their own type checking
            // the error from that should not be propagated to expressions that have type errors
            // unless we're dealing with a cyclic expression error, then propagate away!
            if (symbolType is ErrorTypeSymbol errorType)
            {
                if (ContainsCyclicExpressionError(errorType))
                {
                    return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(symbolNameSpan).CyclicExpression());
                }

                // replace the original error with a different one
                // we may consider suppressing this error in the future as well
                return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(symbolNameSpan).ReferencedSymbolHasErrors(symbolName));
            }

            return symbolType;
        }

        private static bool ContainsCyclicExpressionError(Symbol errorType) => errorType.GetDiagnostics().Any(ErrorDiagnostic.IsCyclicExpressionError);

        private Symbol ResolveSymbol(SyntaxBase syntax)
        {
            // the binder guarantees that all bindable syntax nodes have a symbol attached to them even if it's the error symbol
            // if we see a null here, we have a code defect somewhere
            return this.bindings.TryGetValue(syntax) ?? throw new InvalidOperationException($"Name binding failed to assign a symbol to syntax node of type '{syntax.GetType().Name}'.");
        }
    }
}

