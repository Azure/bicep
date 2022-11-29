// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem;

internal static class OperationReturnTypeEvaluator
{
    private static readonly ImmutableArray<UnaryEvaluator> unaryEvaluators = new List<UnaryEvaluator>()
    {
        new(UnaryOperator.Not, LanguageConstants.Bool, "not", LanguageConstants.Bool),
        new(UnaryOperator.Minus, LanguageConstants.Int, "sub", LanguageConstants.Int, new FunctionArgument(0)),
    }.ToImmutableArray();

    private static readonly ImmutableArray<BinaryEvaluator> binaryEvaluators = new List<BinaryEvaluator>()
    {
        // logical
        new(BinaryOperator.LogicalOr, LanguageConstants.Bool, LanguageConstants.Bool, "or", LanguageConstants.Bool),
        new(BinaryOperator.LogicalAnd, LanguageConstants.Bool, LanguageConstants.Bool, "and", LanguageConstants.Bool),

        // equality
        new(BinaryOperator.Equals, LanguageConstants.Any, LanguageConstants.Any, "equals", LanguageConstants.Bool),
        new NotEqualsEvaluator(),
        new InsensitiveEqualityEvaluator(BinaryOperator.EqualsInsensitive),
        new InsensitiveEqualityEvaluator(BinaryOperator.NotEqualsInsensitive, negated: true),

        // relational (int)
        new(BinaryOperator.LessThan, LanguageConstants.Int, LanguageConstants.Int, "less", LanguageConstants.Bool),
        new(BinaryOperator.LessThanOrEqual, LanguageConstants.Int, LanguageConstants.Int, "lessOrEquals", LanguageConstants.Bool),
        new(BinaryOperator.GreaterThan, LanguageConstants.Int, LanguageConstants.Int, "greater", LanguageConstants.Bool),
        new(BinaryOperator.GreaterThanOrEqual, LanguageConstants.Int, LanguageConstants.Int, "greaterOrEquals", LanguageConstants.Bool),

        // relational (string)
        new(BinaryOperator.LessThan, LanguageConstants.String, LanguageConstants.String, "less", LanguageConstants.Bool),
        new(BinaryOperator.LessThanOrEqual, LanguageConstants.String, LanguageConstants.String, "lessOrEquals", LanguageConstants.Bool),
        new(BinaryOperator.GreaterThan, LanguageConstants.String, LanguageConstants.String, "greater", LanguageConstants.Bool),
        new(BinaryOperator.GreaterThanOrEqual, LanguageConstants.String, LanguageConstants.String, "greaterOrEquals", LanguageConstants.Bool),

        // arithmetic
        new(BinaryOperator.Add, LanguageConstants.Int, LanguageConstants.Int, "add", LanguageConstants.Int),
        new(BinaryOperator.Subtract, LanguageConstants.Int, LanguageConstants.Int, "sub", LanguageConstants.Int),
        new(BinaryOperator.Multiply, LanguageConstants.Int, LanguageConstants.Int, "mul", LanguageConstants.Int),
        new(BinaryOperator.Divide, LanguageConstants.Int, LanguageConstants.Int, "mul", LanguageConstants.Int),
        new(BinaryOperator.Modulo, LanguageConstants.Int, LanguageConstants.Int, "mod", LanguageConstants.Int),

        // coalesce
        new(BinaryOperator.Coalesce, LanguageConstants.Any, LanguageConstants.Any, "coalesce", LanguageConstants.Any),
    }.ToImmutableArray();

    internal static TypeSymbol? FoldUnaryExpression(UnaryOperationSyntax expressionSyntax, TypeSymbol operandType, out IEnumerable<IDiagnostic> foldDiagnostics)
    {
        if (unaryEvaluators.Where(e => e.IsMatch(expressionSyntax.Operator, operandType)).FirstOrDefault() is {} evaluator)
        {
            return evaluator.Evaluate(expressionSyntax, out foldDiagnostics, operandType);
        }

        foldDiagnostics = Enumerable.Empty<IDiagnostic>();
        return null;
    }

    internal static TypeSymbol? FoldBinaryExpression(BinaryOperationSyntax expressionSyntax, TypeSymbol leftOperandType, TypeSymbol rightOperandType, out IEnumerable<IDiagnostic> foldDiagnostics)
    {
        if (binaryEvaluators.Where(e => e.IsMatch(expressionSyntax.Operator, leftOperandType, rightOperandType)).FirstOrDefault() is {} evaluator)
        {
            return evaluator.Evaluate(expressionSyntax, out foldDiagnostics, leftOperandType, rightOperandType);
        }

        foldDiagnostics = Enumerable.Empty<IDiagnostic>();
        return null;
    }

    private abstract class Evaluator
    {
        private readonly string armFunctionName;
        private readonly TypeSymbol nonLiteralReturnType;
        private readonly ImmutableArray<FunctionArgument>? prefixArgs;

        protected Evaluator(string armFunctionName, TypeSymbol nonLiteralReturnType, params FunctionArgument[] prefixArgs)
        {
            this.armFunctionName = armFunctionName;
            this.nonLiteralReturnType = nonLiteralReturnType;
            this.prefixArgs = prefixArgs.Any() ? prefixArgs.ToImmutableArray() : null;
        }

        internal virtual TypeSymbol Evaluate(SyntaxBase expressionSyntax, out IEnumerable<IDiagnostic> evaluationDiagnostics, params TypeSymbol[] operandTypes)
        {
            var type = ArmFunctionReturnTypeEvaluator.Evaluate(armFunctionName, out var builders, operandTypes, prefixArgs) ?? nonLiteralReturnType;
            evaluationDiagnostics = builders.Select(b => b(DiagnosticBuilder.ForPosition(expressionSyntax)));

            return type;
        }
    }

    private class UnaryEvaluator : Evaluator
    {
        private readonly UnaryOperator unaryOperator;
        private readonly TypeSymbol expectedOperandType;

        internal UnaryEvaluator(UnaryOperator unaryOperator,
            TypeSymbol expectedOperandType,
            string armFunctionName,
            TypeSymbol nonLiteralReturnType,
            params FunctionArgument[] prefixArgs) : base(armFunctionName, nonLiteralReturnType, prefixArgs)
        {
            this.unaryOperator = unaryOperator;
            this.expectedOperandType = expectedOperandType;
        }

        internal bool IsMatch(UnaryOperator @operator, TypeSymbol operandType)
            => unaryOperator == @operator && TypeValidator.AreTypesAssignable(operandType, expectedOperandType);
    }

    private class BinaryEvaluator : Evaluator
    {
        private readonly BinaryOperator binaryOperator;
        private readonly TypeSymbol expectedLeftOperandType;
        private readonly TypeSymbol expectedRightOperandType;

        internal BinaryEvaluator(BinaryOperator binaryOperator,
            TypeSymbol expectedLeftOperandType,
            TypeSymbol expectedRightOperandType,
            string armFunctionName,
            TypeSymbol nonLiteralReturnType,
            params FunctionArgument[] prefixArgs) : base(armFunctionName, nonLiteralReturnType, prefixArgs)
        {
            this.binaryOperator = binaryOperator;
            this.expectedLeftOperandType = expectedLeftOperandType;
            this.expectedRightOperandType = expectedRightOperandType;
        }

        internal bool IsMatch(BinaryOperator @operator, TypeSymbol leftOperandType, TypeSymbol rightOperandType)
            => binaryOperator == @operator &&
                TypeValidator.AreTypesAssignable(leftOperandType, expectedLeftOperandType) &&
                TypeValidator.AreTypesAssignable(rightOperandType, expectedRightOperandType);
    }

    private class NotEqualsEvaluator : BinaryEvaluator
    {
        internal NotEqualsEvaluator() : base(BinaryOperator.NotEquals, LanguageConstants.Any, LanguageConstants.Any, Operators.BinaryOperatorToText[BinaryOperator.NotEquals], LanguageConstants.Bool) {}

        internal override TypeSymbol Evaluate(SyntaxBase expressionSyntax, out IEnumerable<IDiagnostic> evaluationDiagnostics, params TypeSymbol[] operandTypes)
        {
            var returnType = ArmFunctionReturnTypeEvaluator.Evaluate("equals", out var builders, operandTypes);
            evaluationDiagnostics = builders.Select(builder => builder(DiagnosticBuilder.ForPosition(expressionSyntax)));

            if (returnType is BooleanLiteralType booleanLiteral)
            {
                return new BooleanLiteralType(!booleanLiteral.Value);
            }

            return LanguageConstants.Bool;
        }
    }

    private class InsensitiveEqualityEvaluator : BinaryEvaluator
    {
        private readonly bool negated;

        internal InsensitiveEqualityEvaluator(BinaryOperator op, bool negated = false) : base(op, LanguageConstants.String, LanguageConstants.String, Operators.BinaryOperatorToText[op], LanguageConstants.Bool)
        {
            this.negated = negated;
        }

        internal override TypeSymbol Evaluate(SyntaxBase expressionSyntax, out IEnumerable<IDiagnostic> evaluationDiagnostics, params TypeSymbol[] operandTypes)
        {
            List<IDiagnostic> diags = new();
            evaluationDiagnostics = diags;
            var transformedArgTypes = new TypeSymbol[operandTypes.Length];
            for (int i = 0; i < operandTypes.Length; i++)
            {
                transformedArgTypes[i] = ArmFunctionReturnTypeEvaluator.Evaluate("toLower", out var builderDelegates, new [] { operandTypes[i] }) ?? LanguageConstants.String;
                diags.AddRange(builderDelegates.Select(b => b(DiagnosticBuilder.ForPosition(expressionSyntax))));
            }

            var result = ArmFunctionReturnTypeEvaluator.Evaluate("equals", out var builders, transformedArgTypes);
            diags.AddRange(builders.Select(b => b(DiagnosticBuilder.ForPosition(expressionSyntax))));

            if (result is BooleanLiteralType booleanLiteral)
            {
                return negated ? new BooleanLiteralType(!booleanLiteral.Value) : booleanLiteral;
            }

            return LanguageConstants.Bool;
        }
    }
}
