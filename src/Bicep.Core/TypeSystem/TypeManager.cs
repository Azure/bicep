﻿using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Errors;
using Bicep.Core.Parser;
using Bicep.Core.Resources;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class TypeManager : ISemanticContext
    {
        public TypeManager()
        {
        }

        public TypeSymbol GetTypeInfo(SyntaxBase syntax)
        {
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
                    return new ErrorTypeSymbol(ErrorBuilder.ForPosition(functionArgument.Expression).NotImplementedFunctionArgs());

                case FunctionCallSyntax functionCall:
                    return new ErrorTypeSymbol(ErrorBuilder.ForPosition(functionCall.FunctionName).NotImplementedFunctionCalls());

                case NullLiteralSyntax _:
                    // null is its own type
                    return LanguageConstants.Null;

                case ParenthesizedExpressionSyntax parenthesized:
                    // parentheses don't change the type of the parenthesized expression
                    return GetTypeInfo(parenthesized.Expression);

                case PropertyAccessSyntax propertyAccess:
                    return new ErrorTypeSymbol(ErrorBuilder.ForPosition(propertyAccess.Dot).NotImplementedPropertyAccess());

                case ArrayAccessSyntax arrayAccess:
                    return new ErrorTypeSymbol(ErrorBuilder.ForPosition(arrayAccess.OpenSquare).NotImplementedArrayAccess());

                case TernaryOperationSyntax ternary:
                    return GetTernaryOperationType(ternary);

                case UnaryOperationSyntax unary:
                    return GetUnaryOperationType(unary);

                case VariableAccessSyntax variableAccess:
                    // TODO: Variable access is not currently supported
                    return new ErrorTypeSymbol(ErrorBuilder.ForPosition(variableAccess.Name).NotImplementedVariableAccess());

                default:
                    return new ErrorTypeSymbol(ErrorBuilder.ForPosition(syntax).InvalidExpression());
            }
        }


        // TODO: This does not recognize non-resource named objects yet

        public TypeSymbol? GetTypeByName(string? typeName)
        {
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
            var errors = new List<Error>();

            foreach (ObjectPropertySyntax objectProperty in @object.Properties)
            {
                var propertyType = this.GetTypeInfo(objectProperty);
                CollectErrors(errors, propertyType);
            }

            if (errors.Any())
            {
                return new ErrorTypeSymbol(errors);
            }

            return LanguageConstants.Object;
        }

        private TypeSymbol GetArrayType(ArraySyntax array)
        {
            var errors = new List<Error>();

            foreach (SyntaxBase arrayItem in array.Children)
            {
                var itemType = this.GetTypeInfo(arrayItem);
                CollectErrors(errors, itemType);
            }

            if (errors.Any())
            {
                return new ErrorTypeSymbol(errors);
            }

            // TODO: In the future we should consider narrowing down the type of the array
            return LanguageConstants.Array;
        }

        private TypeSymbol GetUnaryOperationType(UnaryOperationSyntax syntax)
        {
            var errors = new List<Error>();

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
                return new ErrorTypeSymbol(ErrorBuilder.ForPosition(syntax).UnaryOperatorInvalidType(Operators.UnaryOperatorToText[syntax.Operator], operandType.Name));
            }

            return expectedOperandType;
        }

        private TypeSymbol GetBinaryOperationType(BinaryOperationSyntax syntax)
        {
            var errors = new List<Error>();

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
            return new ErrorTypeSymbol(ErrorBuilder.ForPosition(syntax).BinaryOperatorInvalidType(Operators.BinaryOperatorToText[syntax.Operator], operandType1.Name, operandType2.Name));
        }

        private TypeSymbol GetTernaryOperationType(TernaryOperationSyntax syntax)
        {
            var errors = new List<Error>();

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
                return new ErrorTypeSymbol(ErrorBuilder.ForPosition(syntax.ConditionExpression).ValueTypeMismatch(expectedConditionType.Name));
            }
            
            // the return type is the union of true and false expression types
            return UnionType.Create(trueType, falseType);
        }

        private static void CollectErrors(List<Error> errors, TypeSymbol type)
        {
            errors.AddRange(type.GetDiagnostics());
        }
    }
}
