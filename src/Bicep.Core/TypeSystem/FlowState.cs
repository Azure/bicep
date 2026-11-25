// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Numerics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem;

public record FlowState
{
    internal ImmutableDictionary<FlowPredicate, bool> PredicateValues { get; init; } = [];

    internal ImmutableHashSet<FlowReference> NonNullReferences { get; init; } = [];

    public static FlowState Empty { get; } = new();
}

/// <summary>
/// A variable, parameter or local, optionally followed by a chain of literal property names.
/// </summary>
public abstract record FlowReference;

public sealed record SymbolFlowReference(Symbol Symbol) : FlowReference;

public sealed record PropertyFlowReference(FlowReference Parent, string PropertyName) : FlowReference;

internal abstract record FlowPredicate;

internal sealed record BooleanSymbolPredicate(Symbol Symbol) : FlowPredicate;

internal sealed record BooleanLiteralPredicate(bool Value) : FlowPredicate;

internal sealed record LiteralComparisonPredicate(
    Symbol Symbol,
    BinaryOperator Operator,
    FlowLiteral Literal) : FlowPredicate;

internal abstract record FlowLiteral;

internal sealed record NullFlowLiteral : FlowLiteral
{
    public static NullFlowLiteral Instance { get; } = new();
}

internal sealed record BooleanFlowLiteral(bool Value) : FlowLiteral;

internal sealed record IntegerFlowLiteral(BigInteger Value) : FlowLiteral;

internal sealed record StringFlowLiteral(string Value) : FlowLiteral;

internal sealed record LogicalPredicate(BinaryOperator Operator, FlowPredicate Left, FlowPredicate Right) : FlowPredicate;

internal sealed record NegatedPredicate(FlowPredicate Operand) : FlowPredicate;
