using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parser;
using Bicep.Core.Resources;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class TypeManager : ITypeContext
    {
        private readonly IReadOnlyDictionary<SyntaxBase,Symbol> bindings;

        private bool unlocked = false;

        public TypeManager(IReadOnlyDictionary<SyntaxBase, Symbol> bindings)
        {
            // bindings will be modified by name binding after this object is created
            // so we can't make an immutable copy here
            // (using the IReadOnlyDictionary to prevent accidental mutation
            this.bindings = bindings; 
        }

        public void Unlock()
        {
            this.unlocked = true;
        }

        public TypeSymbol GetTypeInfo(SyntaxBase syntax)
        {
            this.CheckLock();

            // TODO: When we have expressions, this class will cache the result, so we don't have walk the three all the time.
            switch (syntax)
            {
                case BooleanLiteralSyntax _:
                    return LanguageConstants.Bool;

                case NumericLiteralSyntax _:
                    return LanguageConstants.Int;

                case StringSyntax _:
                    return LanguageConstants.String;

                case ObjectSyntax @object:
                    return GetObjectType(@object);

                case ObjectPropertySyntax objectProperty:
                    return GetTypeInfo(objectProperty.Value);

                case ArraySyntax array:
                    return GetArrayType(array);

                case ArrayItemSyntax arrayItem:
                    return GetTypeInfo(arrayItem.Value);

                case BinaryOperationSyntax binary:
                    return GetBinaryOperationType(binary);

                case FunctionArgumentSyntax functionArgument:
                    return GetTypeInfo(functionArgument.Expression);

                case FunctionCallSyntax functionCall:
                    return GetFunctionCallType(functionCall);

                case NullLiteralSyntax _:
                    // null is its own type
                    return LanguageConstants.Null;

                case ParenthesizedExpressionSyntax parenthesized:
                    // parentheses don't change the type of the parenthesized expression
                    return GetTypeInfo(parenthesized.Expression);

                case PropertyAccessSyntax propertyAccess:
                    return GetPropertyAccessType(propertyAccess);

                case ArrayAccessSyntax arrayAccess:
                    return GetArrayAccessType(arrayAccess);

                case TernaryOperationSyntax ternary:
                    return GetTernaryOperationType(ternary);

                case UnaryOperationSyntax unary:
                    return GetUnaryOperationType(unary);

                case VariableAccessSyntax variableAccess:
                    // TODO: Variable access is not currently supported
                    return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(variableAccess.Name).NotImplementedVariableAccess());

                default:
                    return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax).InvalidExpression());
            }
        }

        // TODO: This does not recognize non-resource named objects yet
        public TypeSymbol? GetTypeByName(string? typeName)
        {
            this.CheckLock();

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
            return new ResourceType(typeName, LanguageConstants.TopLevelResourceProperties, additionalPropertiesType: null);
        }

        private TypeSymbol GetObjectType(ObjectSyntax @object)
        {
            var errors = new List<ErrorDiagnostic>();

            foreach (ObjectPropertySyntax objectProperty in @object.Properties)
            {
                var propertyType = this.GetTypeInfo(objectProperty);
                CollectErrors(errors, propertyType);
            }

            if (errors.Any())
            {
                return new ErrorTypeSymbol(errors);
            }

            // TODO: Narrow down the object type
            return LanguageConstants.Object;
        }

        private TypeSymbol GetArrayType(ArraySyntax array)
        {
            var errors = new List<ErrorDiagnostic>();

            var itemTypes = new List<TypeSymbol>(array.Children.Length);
            foreach (SyntaxBase arrayItem in array.Children)
            {
                var itemType = this.GetTypeInfo(arrayItem);
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

        private TypeSymbol GetPropertyAccessType(PropertyAccessSyntax syntax)
        {
            var errors = new List<ErrorDiagnostic>();

            var baseType = this.GetTypeInfo(syntax.BaseExpression);
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

        private TypeSymbol GetArrayAccessType(ArrayAccessSyntax syntax)
        {
            var errors = new List<ErrorDiagnostic>();

            var baseType = this.GetTypeInfo(syntax.BaseExpression);
            CollectErrors(errors, baseType);

            var indexType = this.GetTypeInfo(syntax.IndexExpression);
            CollectErrors(errors, indexType);

            if (errors.Any())
            {
                return new ErrorTypeSymbol(errors);
            }

            if (indexType.TypeKind == TypeKind.Any)
            {
                // index type is "any"
                switch (baseType)
                {
                    case AnyType _:
                        return LanguageConstants.Any;

                    case ArrayType arrayType:
                        return arrayType.ItemType;

                    case ObjectType objectType:
                        return GetExpressionedPropertyType(objectType, syntax.IndexExpression);
                }
            }

            if (TypeValidator.AreTypesAssignable(indexType, LanguageConstants.Int) == true)
            {
                // int indexer

                if (TypeValidator.AreTypesAssignable(baseType, LanguageConstants.Array) != true)
                {
                    // can only index over arrays
                    return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.IndexExpression).ArrayRequiredForIntegerIndexer(baseType));
                }

                switch (baseType)
                {
                    case AnyType _:
                        return LanguageConstants.Any;

                    case ArrayType arrayType:
                        return arrayType.ItemType;

                    default:
                        throw new NotImplementedException($"Unexpected array access base expression type '{baseType}'.");
                }
            }

            if (TypeValidator.AreTypesAssignable(indexType, LanguageConstants.String) == true)
            {
                // string indexer

                if (TypeValidator.AreTypesAssignable(baseType, LanguageConstants.Object) != true)
                {
                    // string indexers only work on objects
                    return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.IndexExpression).ObjectRequiredForStringIndexer(baseType));
                }

                if (baseType.TypeKind == TypeKind.Any || !(baseType is ObjectType objectType))
                {
                    return LanguageConstants.Any;
                }

                switch (syntax.IndexExpression)
                {
                    case StringSyntax @string when @string.IsInterpolated() == false:
                        var propertyName = @string.TryGetFormatString();
                        if (propertyName == null)
                        {
                            return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(@string).MalformedPropertyNameString());
                        }

                        return this.GetNamedPropertyType(objectType, syntax.IndexExpression, propertyName);

                    default:
                        // the property name is itself an expression
                        return this.GetExpressionedPropertyType(objectType, syntax.IndexExpression);
                }
            }

            // index was of the wrong type
            return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax.IndexExpression).StringOrIntegerIndexerRequired(indexType));
        }

        private TypeSymbol GetFunctionCallType(FunctionCallSyntax syntax)
        {
            var errors = new List<ErrorDiagnostic>();

            var argumentTypes = syntax.Arguments.Select(GetTypeInfo).ToList();
            foreach (TypeSymbol argumentType in argumentTypes)
            {
                CollectErrors(errors, argumentType);
            }

            if (errors.Any())
            {
                return new ErrorTypeSymbol(errors);
            }

            var symbol = this.ResolveSymbol(syntax);
            switch (symbol)
            {
                case ErrorSymbol errorSymbol:
                    return errorSymbol.ToErrorType();

                case FunctionSymbol function:
                    return GetFunctionSymbolType(syntax, function, argumentTypes);

                default:
                    throw new NotImplementedException($"Unexpected symbol kind '{symbol.Kind}' on a function call binding.");
            }
        }

        private TypeSymbol GetFunctionSymbolType(FunctionCallSyntax syntax, FunctionSymbol function, List<TypeSymbol> argumentTypes)
        {
            var matches = FunctionResolver.GetMatches(function, argumentTypes).ToList();

            switch (matches.Count)
            {
                case 0:
                    // cannot find a function matching the types and number of arguments
                    return new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(syntax).CannotResolveFunction(syntax.FunctionName.IdentifierName, argumentTypes));

                case 1:
                    // we have an exact match or a single ambiguous match
                    // return its type
                    return matches.Single().ReturnType;

                default:
                    // function arguments are ambiguous (due to "any" type)
                    // technically, the correct behavior would be to return a union of all possible types
                    // unfortunately our language lacks a good type checking construct
                    // and we also don't want users to have to use the converter functions to work around it
                    // instead, we will return the "any" type to short circuit the type checking for those cases
                    return LanguageConstants.Any;
            }
        }

        private TypeSymbol GetUnaryOperationType(UnaryOperationSyntax syntax)
        {
            var errors = new List<ErrorDiagnostic>();

            // TODO: When we add number type, this will have to be adjusted
            var expectedOperandType = syntax.Operator switch
            {
                UnaryOperator.Not => LanguageConstants.Bool,
                UnaryOperator.Minus => LanguageConstants.Int,
                _ => throw new NotImplementedException()
            };

            var operandType = this.GetTypeInfo(syntax.Expression);
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

        private TypeSymbol GetBinaryOperationType(BinaryOperationSyntax syntax)
        {
            var errors = new List<ErrorDiagnostic>();

            var operandType1 = this.GetTypeInfo(syntax.LeftExpression);
            CollectErrors(errors, operandType1);

            var operandType2 = this.GetTypeInfo(syntax.RightExpression);
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

        private TypeSymbol GetTernaryOperationType(TernaryOperationSyntax syntax)
        {
            var errors = new List<ErrorDiagnostic>();

            // ternary operator requires the condition to be of bool type
            var conditionType = this.GetTypeInfo(syntax.ConditionExpression);
            CollectErrors(errors, conditionType);

            var trueType = this.GetTypeInfo(syntax.TrueExpression);
            CollectErrors(errors, trueType);

            var falseType = this.GetTypeInfo(syntax.FalseExpression);
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

        private void CheckLock()
        {
            if (this.unlocked == false)
            {
                throw new InvalidOperationException("Type checks are blocked until name binding is completed.");
            }
        }

        private Symbol ResolveSymbol(SyntaxBase syntax)
        {
            // the binder guarantees that all bindable syntax nodes have a symbol attached to them even if it's the error symbol
            // if we see a null here, we have a code defect somewhere
            return this.bindings.TryGetValue(syntax) ?? throw new InvalidOperationException($"Name binding failed to assign a symbol to syntax node of type '{syntax.GetType().Name}'.");
        }
    }
}
