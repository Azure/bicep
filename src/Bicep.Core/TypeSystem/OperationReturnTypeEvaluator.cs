// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Numerics;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem;

public static class OperationReturnTypeEvaluator
{
    private static readonly ImmutableArray<IUnaryEvaluator> unaryEvaluators = [new NotEvaluator(), new MinusEvaluator()];

    private static readonly IBinaryEvaluator equalsEvaluator
        = new BinaryEvaluator(BinaryOperator.Equals, (LanguageConstants.Any, LanguageConstants.Any), "equals", LanguageConstants.Bool);

    private static readonly ImmutableArray<IBinaryEvaluator> binaries =
    [
        new LogicalOrEvaluator(),
        new LogicalAndEvaluator(),
        equalsEvaluator,
        new NotEqualsEvaluator(),
        new InsensitiveEqualityEvaluator(BinaryOperator.EqualsInsensitive),
        new InsensitiveEqualityEvaluator(BinaryOperator.NotEqualsInsensitive, negated: true),
        new LessThanEvaluator(),
        new LessThanOrEqualToEvaluator(),
        new GreaterThanEvaluator(),
        new GreaterThanOrEqualToEvaluator(),
        new BinaryEvaluator(BinaryOperator.LessThan, (LanguageConstants.String, LanguageConstants.String), "less", LanguageConstants.Bool),
        new BinaryEvaluator(BinaryOperator.LessThanOrEqual, (LanguageConstants.String, LanguageConstants.String), "lessOrEquals", LanguageConstants.Bool),
        new BinaryEvaluator(BinaryOperator.GreaterThan, (LanguageConstants.String, LanguageConstants.String), "greater", LanguageConstants.Bool),
        new BinaryEvaluator(BinaryOperator.GreaterThanOrEqual, (LanguageConstants.String, LanguageConstants.String), "greaterOrEquals", LanguageConstants.Bool),
        new AdditionEvaluator(),
        new SubtractionEvaluator(),
        new BinaryEvaluator(BinaryOperator.Multiply, (LanguageConstants.Int, LanguageConstants.Int), "mul", LanguageConstants.Int),
        new BinaryEvaluator(BinaryOperator.Divide, (LanguageConstants.Int, LanguageConstants.Int), "div", LanguageConstants.Int),
        new ModuloEvaluator(),
        new CoalesceEvaluator(),
    ];

    public static TypeSymbol? TryFoldUnaryExpression(UnaryOperator unaryOperator, TypeSymbol operandType, IDiagnosticWriter diagnosticWriter)
        => unaryEvaluators.Where(e => e.IsMatch(unaryOperator, operandType)).FirstOrDefault()?.Evaluate(operandType);

    public static TypeSymbol? TryFoldBinaryExpression(BinaryOperationSyntax expressionSyntax, TypeSymbol leftOperandType, TypeSymbol rightOperandType, IDiagnosticWriter diagnosticWriter)
    {
        if (binaries.Where(e => e.IsMatch(expressionSyntax.Operator, leftOperandType, rightOperandType)).FirstOrDefault() is { } evaluator)
        {
            return evaluator.Evaluate(expressionSyntax, leftOperandType, rightOperandType, diagnosticWriter);
        }

        return null;
    }

    private interface IUnaryEvaluator
    {
        UnaryOperator Operator { get; }

        TypeSymbol OperandType { get; }

        TypeSymbol Evaluate(TypeSymbol operandType);

        bool IsMatch(UnaryOperator @operator, TypeSymbol operandType)
            => Operator == @operator && TypeValidator.AreTypesAssignable(operandType, OperandType);
    }

    private class NotEvaluator : IUnaryEvaluator
    {
        public UnaryOperator Operator => UnaryOperator.Not;

        public TypeSymbol OperandType => LanguageConstants.Bool;

        public TypeSymbol Evaluate(TypeSymbol operandType) => operandType switch
        {
            UnionType union => TypeHelper.CreateTypeUnion(union.Members.Select(t => Evaluate(t.Type))),
            BooleanLiteralType booleanLiteral => TypeFactory.CreateBooleanLiteralType(!booleanLiteral.Value, booleanLiteral.ValidationFlags),
            BooleanType @bool => @bool,
            _ => TypeFactory.CreateBooleanType(operandType.ValidationFlags),
        };
    }

    private static TypeSymbol Negate(TypeSymbol toNegate)
    {
        switch (toNegate)
        {
            case UnionType union:
                List<TypeSymbol> transformed = new();
                List<ErrorType> errors = new();

                foreach (var member in union.Members)
                {
                    var evaluatedMember = Negate(member.Type);

                    if (evaluatedMember is ErrorType error)
                    {
                        errors.Add(error);
                    }
                    else
                    {
                        transformed.Add(evaluatedMember);
                    }
                }

                if (errors.Any())
                {
                    return ErrorType.Create(errors.SelectMany(e => e.GetDiagnostics()));
                }

                return TypeHelper.CollapseOrCreateTypeUnion(transformed);
            case IntegerLiteralType integerLiteral when Negate(integerLiteral.Value) is long negated:
                return TypeFactory.CreateIntegerLiteralType(negated, integerLiteral.ValidationFlags);
            case IntegerType @int:
                return TypeFactory.CreateIntegerType(minValue: Negate(@int.MaxValue), maxValue: Negate(@int.MinValue), @int.ValidationFlags);
            default:
                return LanguageConstants.Int;
        }
    }

    private static long? Negate(long? toNegate) => toNegate switch
    {
        long nonNull => Negate(nonNull),
        _ => null,
    };

    private static long? Negate(long toNegate) => toNegate switch
    {
        // -long.MinValue would overflow by 1
        long.MinValue => null,
        _ => -toNegate,
    };

    private class MinusEvaluator : IUnaryEvaluator
    {
        public UnaryOperator Operator => UnaryOperator.Minus;

        public TypeSymbol OperandType => LanguageConstants.Int;

        public TypeSymbol Evaluate(TypeSymbol operandType) => Negate(operandType);
    }

    private interface IBinaryEvaluator
    {
        BinaryOperator Operator { get; }

        (TypeSymbol Left, TypeSymbol Right) OperandTypes { get; }

        TypeSymbol Evaluate(SyntaxBase expressionSyntax, TypeSymbol leftOperandType, TypeSymbol rightOperandType, IDiagnosticWriter diagnosticWriter);

        bool IsMatch(BinaryOperator @operator, TypeSymbol leftOperandType, TypeSymbol rightOperandType)
            => Operator == @operator &&
                TypeValidator.AreTypesAssignable(leftOperandType, OperandTypes.Left) &&
                TypeValidator.AreTypesAssignable(rightOperandType, OperandTypes.Right);
    }

    private class BinaryEvaluator : IBinaryEvaluator
    {
        private readonly BinaryOperator @operator;
        private readonly (TypeSymbol, TypeSymbol) operandTypes;
        private readonly string armFunctionName;
        private readonly TypeSymbol genericReturnType;

        public BinaryEvaluator(BinaryOperator @operator, (TypeSymbol, TypeSymbol) operandTypes, string armFunctionName, TypeSymbol genericReturnType)
        {
            this.@operator = @operator;
            this.operandTypes = operandTypes;
            this.armFunctionName = armFunctionName;
            this.genericReturnType = genericReturnType;
        }

        public BinaryOperator Operator => @operator;

        public (TypeSymbol, TypeSymbol) OperandTypes => operandTypes;

        public TypeSymbol Evaluate(SyntaxBase expressionSyntax, TypeSymbol leftOperandType, TypeSymbol rightOperandType, IDiagnosticWriter diagnosticWriter)
        {
            var literal = ArmFunctionReturnTypeEvaluator.TryEvaluate(armFunctionName, out var builders, new[] { leftOperandType, rightOperandType });
            diagnosticWriter.WriteMultiple(builders.Select(b => b(DiagnosticBuilder.ForPosition(expressionSyntax))));

            return literal ?? TryDeriveNonLiteralType(expressionSyntax, leftOperandType, rightOperandType) ?? genericReturnType;
        }

        protected virtual TypeSymbol? TryDeriveNonLiteralType(SyntaxBase expressionSyntax, TypeSymbol leftOperandType, TypeSymbol rightOperandType)
            => null;
    }

    private class LogicalOrEvaluator : BinaryEvaluator
    {
        public LogicalOrEvaluator() : base(BinaryOperator.LogicalOr, (LanguageConstants.Bool, LanguageConstants.Bool), "or", LanguageConstants.Bool) { }

        protected override TypeSymbol? TryDeriveNonLiteralType(SyntaxBase expressionSyntax, TypeSymbol leftOperandType, TypeSymbol rightOperandType) => (leftOperandType, rightOperandType) switch
        {
            (BooleanLiteralType leftLiteral, _) when leftLiteral.Value => LanguageConstants.True,
            (_, BooleanLiteralType rightLiteral) when rightLiteral.Value => LanguageConstants.True,
            (BooleanLiteralType, BooleanLiteralType) => LanguageConstants.False,
            _ => null,
        };
    }

    private class LogicalAndEvaluator : BinaryEvaluator
    {
        public LogicalAndEvaluator() : base(BinaryOperator.LogicalAnd, (LanguageConstants.Bool, LanguageConstants.Bool), "and", LanguageConstants.Bool) { }

        protected override TypeSymbol? TryDeriveNonLiteralType(SyntaxBase expressionSyntax, TypeSymbol leftOperandType, TypeSymbol rightOperandType) => (leftOperandType, rightOperandType) switch
        {
            (BooleanLiteralType leftLiteral, _) when !leftLiteral.Value => LanguageConstants.False,
            (_, BooleanLiteralType rightLiteral) when !rightLiteral.Value => LanguageConstants.False,
            (BooleanLiteralType, BooleanLiteralType) => LanguageConstants.True,
            _ => null,
        };
    }

    private class NotEqualsEvaluator : IBinaryEvaluator
    {
        public BinaryOperator Operator => BinaryOperator.NotEquals;

        public (TypeSymbol, TypeSymbol) OperandTypes => equalsEvaluator.OperandTypes;

        public TypeSymbol Evaluate(SyntaxBase expressionSyntax, TypeSymbol leftOperandType, TypeSymbol rightOperandType, IDiagnosticWriter diagnosticWriter)
            => equalsEvaluator.Evaluate(expressionSyntax, leftOperandType, rightOperandType, diagnosticWriter) switch
            {
                BooleanLiteralType literalReturn => TypeFactory.CreateBooleanLiteralType(!literalReturn.Value),
                _ => LanguageConstants.Bool,
            };
    }

    private class InsensitiveEqualityEvaluator : IBinaryEvaluator
    {
        private readonly BinaryOperator @operator;
        private readonly bool negated;

        internal InsensitiveEqualityEvaluator(BinaryOperator @operator, bool negated = false)
        {
            this.@operator = @operator;
            this.negated = negated;
        }

        public BinaryOperator Operator => @operator;

        public (TypeSymbol, TypeSymbol) OperandTypes => (LanguageConstants.String, LanguageConstants.String);

        public TypeSymbol Evaluate(SyntaxBase expressionSyntax, TypeSymbol leftOperandType, TypeSymbol rightOperandType, IDiagnosticWriter diagnosticWriter)
        {
            var transformedArgTypes = new TypeSymbol[2];
            for (int i = 0; i < 2; i++)
            {
                transformedArgTypes[i] = ArmFunctionReturnTypeEvaluator.TryEvaluate("toLower", out var builderDelegates, new[] { i % 2 == 0 ? leftOperandType : rightOperandType }) ?? LanguageConstants.String;
                diagnosticWriter.WriteMultiple(builderDelegates.Select(b => b(DiagnosticBuilder.ForPosition(expressionSyntax))));
            }

            var result = ArmFunctionReturnTypeEvaluator.TryEvaluate("equals", out var builders, transformedArgTypes);
            diagnosticWriter.WriteMultiple(builders.Select(b => b(DiagnosticBuilder.ForPosition(expressionSyntax))));

            if (result is BooleanLiteralType booleanLiteral)
            {
                return negated ? TypeFactory.CreateBooleanLiteralType(!booleanLiteral.Value) : booleanLiteral;
            }

            return LanguageConstants.Bool;
        }
    }

    private abstract class NumericComparisonEvaluator : BinaryEvaluator
    {
        protected NumericComparisonEvaluator(BinaryOperator @operator, string armFunctionName)
            : base(@operator, (LanguageConstants.Int, LanguageConstants.Int), armFunctionName, LanguageConstants.Bool) { }
    }

    private static long? MinValue(TypeSymbol operandType) => operandType switch
    {
        IntegerLiteralType literal => literal.Value,
        IntegerType @int => @int.MinValue,
        UnionType union => union.Members.Select(m => MinValue(m.Type)).Min(),
        _ => null,
    };

    private static long? MaxValue(TypeSymbol operandType) => operandType switch
    {
        IntegerLiteralType literal => literal.Value,
        IntegerType @int => @int.MaxValue,
        UnionType union => union.Members.Select(m => MaxValue(m.Type)).Max(),
        _ => null,
    };

    private class LessThanEvaluator : NumericComparisonEvaluator
    {
        internal LessThanEvaluator() : base(BinaryOperator.LessThan, "less") { }

        protected override TypeSymbol? TryDeriveNonLiteralType(SyntaxBase expressionSyntax, TypeSymbol leftOperandType, TypeSymbol rightOperandType)
        {
            if (MaxValue(leftOperandType) is long leftMax && MinValue(rightOperandType) is long rightMin && leftMax < rightMin)
            {
                return LanguageConstants.True;
            }

            if (MinValue(leftOperandType) is long leftMin && MaxValue(rightOperandType) is long rightMax && leftMin >= rightMax)
            {
                return LanguageConstants.False;
            }

            return null;
        }
    }

    private class LessThanOrEqualToEvaluator : NumericComparisonEvaluator
    {
        internal LessThanOrEqualToEvaluator() : base(BinaryOperator.LessThanOrEqual, "lessOrEquals") { }

        protected override TypeSymbol? TryDeriveNonLiteralType(SyntaxBase expressionSyntax, TypeSymbol leftOperandType, TypeSymbol rightOperandType)
        {
            if (MaxValue(leftOperandType) is long leftMax && MinValue(rightOperandType) is long rightMin && leftMax <= rightMin)
            {
                return LanguageConstants.True;
            }

            if (MinValue(leftOperandType) is long leftMin && MaxValue(rightOperandType) is long rightMax && leftMin > rightMax)
            {
                return LanguageConstants.False;
            }

            return null;
        }
    }

    private class GreaterThanEvaluator : NumericComparisonEvaluator
    {
        internal GreaterThanEvaluator() : base(BinaryOperator.GreaterThan, "greater") { }

        protected override TypeSymbol? TryDeriveNonLiteralType(SyntaxBase expressionSyntax, TypeSymbol leftOperandType, TypeSymbol rightOperandType)
        {
            if (MinValue(leftOperandType) is long leftMin && MaxValue(rightOperandType) is long rightMax && leftMin > rightMax)
            {
                return LanguageConstants.True;
            }

            if (MaxValue(leftOperandType) is long leftMax && MinValue(rightOperandType) is long rightMin && leftMax <= rightMin)
            {
                return LanguageConstants.False;
            }

            return null;
        }
    }

    private class GreaterThanOrEqualToEvaluator : NumericComparisonEvaluator
    {
        internal GreaterThanOrEqualToEvaluator() : base(BinaryOperator.GreaterThanOrEqual, "greaterOrEquals") { }

        protected override TypeSymbol? TryDeriveNonLiteralType(SyntaxBase expressionSyntax, TypeSymbol leftOperandType, TypeSymbol rightOperandType)
        {
            if (MinValue(leftOperandType) is long leftMin && MaxValue(rightOperandType) is long rightMax && leftMin >= rightMax)
            {
                return LanguageConstants.True;
            }

            if (MaxValue(leftOperandType) is long leftMax && MinValue(rightOperandType) is long rightMin && leftMax < rightMin)
            {
                return LanguageConstants.False;
            }

            return null;
        }
    }

    private abstract class ArithmeticEvaluator : BinaryEvaluator
    {
        protected ArithmeticEvaluator(BinaryOperator @operator, string armFunctionName)
            : base(@operator, (LanguageConstants.Int, LanguageConstants.Int), armFunctionName, LanguageConstants.Int) { }

        protected override TypeSymbol? TryDeriveNonLiteralType(SyntaxBase expressionSyntax, TypeSymbol leftOperandType, TypeSymbol rightOperandType)
            => DeriveNonLiteralType(expressionSyntax, leftOperandType, rightOperandType);

        private TypeSymbol DeriveNonLiteralType(SyntaxBase expressionSyntax, TypeSymbol leftOperandType, TypeSymbol rightOperandType)
            => (leftOperandType, rightOperandType) switch
            {
                (UnionType leftUnion, _) => CollapseIfPossible(TypeHelper.CreateTypeUnion(
                    leftUnion.Members.Select(m => DeriveNonLiteralType(expressionSyntax, m.Type, rightOperandType)))),
                (_, UnionType rightUnion) => CollapseIfPossible(TypeHelper.CreateTypeUnion(
                    rightUnion.Members.Select(m => DeriveNonLiteralType(expressionSyntax, leftOperandType, m.Type)))),
                _ => PerformTypeArithmetic(expressionSyntax, leftOperandType, rightOperandType),
            };

        private static TypeSymbol CollapseIfPossible(TypeSymbol derived)
            => TypeCollapser.TryCollapse(derived) ?? derived;

        protected abstract TypeSymbol PerformTypeArithmetic(
            SyntaxBase expressionSyntax,
            TypeSymbol leftOperandType,
            TypeSymbol rightOperandType);
    }

    private static TypeSymbol PerformAdditiveTypeArithmetic(TypeSymbol leftOperand, TypeSymbol rightOperand) => (leftOperand, rightOperand) switch
    {
        (IntegerLiteralType leftLiteral, IntegerLiteralType rightLiteral) when TryAdd(leftLiteral.Value, rightLiteral.Value) is long sum
            => TypeFactory.CreateIntegerLiteralType(sum),
        (IntegerLiteralType leftLiteral, IntegerType rightInt) => PerformAdditiveTypeArithmetic(leftLiteral, rightInt),
        (IntegerType leftInt, IntegerLiteralType rightLiteral) => PerformAdditiveTypeArithmetic(rightLiteral, leftInt),
        (IntegerType leftInt, IntegerType rightInt) => TypeFactory.CreateIntegerType(TryAdd(leftInt.MinValue, rightInt.MinValue), TryAdd(leftInt.MaxValue, rightInt.MaxValue)),
        _ => LanguageConstants.Int,

    };

    private static TypeSymbol PerformAdditiveTypeArithmetic(IntegerLiteralType integerLiteral, IntegerType @int)
        => TypeFactory.CreateIntegerType(TryAdd(integerLiteral.Value, @int.MinValue), TryAdd(integerLiteral.Value, @int.MaxValue));

    private static long? TryAdd(long? a, long? b) => (a, b) switch
    {
        (long nonNullA, long nonNullB) => BigInteger.Add(nonNullA, nonNullB) switch
        {
            var sum when sum >= long.MinValue || sum <= long.MaxValue => (long)sum,
            _ => null,
        },
        _ => null,
    };

    private class AdditionEvaluator : ArithmeticEvaluator
    {
        internal AdditionEvaluator() : base(BinaryOperator.Add, "add") { }

        protected override TypeSymbol PerformTypeArithmetic(SyntaxBase _, TypeSymbol leftOperandType, TypeSymbol rightOperandType)
            => PerformAdditiveTypeArithmetic(leftOperandType, rightOperandType);
    }

    private class SubtractionEvaluator : ArithmeticEvaluator
    {
        internal SubtractionEvaluator() : base(BinaryOperator.Subtract, "sub") { }

        protected override TypeSymbol PerformTypeArithmetic(SyntaxBase _, TypeSymbol leftOperandType, TypeSymbol rightOperandType)
            => PerformAdditiveTypeArithmetic(leftOperandType, Negate(rightOperandType));
    }

    private class ModuloEvaluator : ArithmeticEvaluator
    {
        internal ModuloEvaluator() : base(BinaryOperator.Modulo, "mod") { }

        protected override TypeSymbol PerformTypeArithmetic(SyntaxBase expressionSyntax, TypeSymbol leftOperandType, TypeSymbol rightOperandType)
            => (leftOperandType, rightOperandType) switch
            {
                (_, IntegerLiteralType literalDivisor) when literalDivisor.Value == 0
                    => ErrorType.Create(DiagnosticBuilder.ForPosition(expressionSyntax).AttemptToDivideByZero()),
                (IntegerLiteralType literalDividend, IntegerLiteralType literalDivisor)
                    => TypeFactory.CreateIntegerLiteralType(literalDividend.Value % literalDivisor.Value),
                (_, _) when TryGetMaxDivisor(rightOperandType) is long maxDivisor
                    => TypeFactory.CreateIntegerType(
                        minValue: MinValue(leftOperandType) >= 0 ? 0 : 1 - maxDivisor,
                        maxValue: MaxValue(leftOperandType) <= 0 ? 0 : maxDivisor - 1),
                _ => LanguageConstants.Int,
            };

        private static long? TryGetMaxDivisor(TypeSymbol divisorType) => divisorType switch
        {
            IntegerLiteralType literal => Math.Abs(literal.Value),
            IntegerType ranged when ranged.MaxValue.HasValue && ranged.MinValue > long.MinValue
                => Math.Max(Math.Abs(ranged.MinValue.Value), Math.Abs(ranged.MaxValue.Value)),
            _ => null,
        };
    }

    private class CoalesceEvaluator : BinaryEvaluator
    {
        public CoalesceEvaluator() : base(BinaryOperator.Coalesce, (LanguageConstants.Any, LanguageConstants.Any), "coalesce", LanguageConstants.Any) { }

        protected override TypeSymbol? TryDeriveNonLiteralType(SyntaxBase expressionSyntax, TypeSymbol leftOperandType, TypeSymbol rightOperandType) => leftOperandType switch
        {
            // if the left operand is definitely null, use the right operand's type
            NullType => rightOperandType,
            // if the left operand may be null, use a union of the type the left operand will be if not null plus the right operand's type
            _ when TypeHelper.TryRemoveNullability(leftOperandType) is TypeSymbol nonNullType => TypeHelper.CreateTypeUnion(nonNullType, rightOperandType),
            // if we got here, the left operand is not nullable, so use its type
            _ => leftOperandType,
        };
    }
}
