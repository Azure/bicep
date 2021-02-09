// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public static class BinaryOperationResolver
    {
        private static readonly ILookup<BinaryOperator, BinaryOperatorInfo> OperatorLookup = new[]
        {
            // logical
            new BinaryOperatorInfo(BinaryOperator.LogicalOr, LanguageConstants.Bool, LanguageConstants.Bool),
            new BinaryOperatorInfo(BinaryOperator.LogicalAnd, LanguageConstants.Bool, LanguageConstants.Bool),

            // equality
            new BinaryOperatorInfo(BinaryOperator.Equals, LanguageConstants.Any, LanguageConstants.Bool),
            new BinaryOperatorInfo(BinaryOperator.NotEquals, LanguageConstants.Any, LanguageConstants.Bool),
            new BinaryOperatorInfo(BinaryOperator.EqualsInsensitive, LanguageConstants.String, LanguageConstants.Bool),
            new BinaryOperatorInfo(BinaryOperator.NotEqualsInsensitive, LanguageConstants.String, LanguageConstants.Bool),

            // relational (int)
            // TODO: Needs number type
            new BinaryOperatorInfo(BinaryOperator.LessThan, LanguageConstants.Int, LanguageConstants.Bool),
            new BinaryOperatorInfo(BinaryOperator.LessThanOrEqual, LanguageConstants.Int, LanguageConstants.Bool),
            new BinaryOperatorInfo(BinaryOperator.GreaterThan, LanguageConstants.Int, LanguageConstants.Bool),
            new BinaryOperatorInfo(BinaryOperator.GreaterThanOrEqual, LanguageConstants.Int, LanguageConstants.Bool),

            // relational (string)
            new BinaryOperatorInfo(BinaryOperator.LessThan, LanguageConstants.String, LanguageConstants.Bool),
            new BinaryOperatorInfo(BinaryOperator.LessThanOrEqual, LanguageConstants.String, LanguageConstants.Bool),
            new BinaryOperatorInfo(BinaryOperator.GreaterThan, LanguageConstants.String, LanguageConstants.Bool),
            new BinaryOperatorInfo(BinaryOperator.GreaterThanOrEqual, LanguageConstants.String, LanguageConstants.Bool),

            // add
            new BinaryOperatorInfo(BinaryOperator.Add, LanguageConstants.Int, LanguageConstants.Int),
            
            // subtract
            new BinaryOperatorInfo(BinaryOperator.Subtract, LanguageConstants.Int, LanguageConstants.Int),

            // multiplicative
            new BinaryOperatorInfo(BinaryOperator.Multiply, LanguageConstants.Int, LanguageConstants.Int),
            new BinaryOperatorInfo(BinaryOperator.Divide, LanguageConstants.Int, LanguageConstants.Int),
            new BinaryOperatorInfo(BinaryOperator.Modulo, LanguageConstants.Int, LanguageConstants.Int),

            //coalesce
            new BinaryOperatorInfo(BinaryOperator.Coalesce, LanguageConstants.Any, LanguageConstants.Any)            
        }.ToLookup(info => info.Operator);

        public static BinaryOperatorInfo? TryMatchExact(BinaryOperator @operator, TypeSymbol operandType1, TypeSymbol operandType2)
        {
            return OperatorLookup[@operator]
                .SingleOrDefault(info => TypeValidator.AreTypesAssignable(operandType1, info.OperandType) == true &&
                                         TypeValidator.AreTypesAssignable(operandType2, info.OperandType) == true);
        }

        public static IEnumerable<BinaryOperatorInfo> GetMatches(BinaryOperator @operator, TypeSymbol operandType1, TypeSymbol operandType2)
        {
            // error types will cause this to return multiple operator matches in some cases
            return OperatorLookup[@operator]
                .Where(info => TypeValidator.AreTypesAssignable(operandType1, info.OperandType) != false &&
                               TypeValidator.AreTypesAssignable(operandType2, info.OperandType) != false);
        }

        public static IEnumerable<BinaryOperatorInfo> GetMatches(BinaryOperator @operator) => OperatorLookup[@operator];
    }
}

