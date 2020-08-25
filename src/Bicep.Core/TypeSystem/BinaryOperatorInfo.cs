// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class BinaryOperatorInfo
    {
        public BinaryOperatorInfo(BinaryOperator @operator, TypeSymbol operandType, TypeSymbol returnType)
        {
            this.Operator = @operator;
            this.OperandType = operandType;
            this.ReturnType = returnType;
        }

        public BinaryOperator Operator { get; }

        public TypeSymbol OperandType { get; }

        public TypeSymbol ReturnType { get; }
    }
}
