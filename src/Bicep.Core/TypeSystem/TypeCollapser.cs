// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Extensions;

namespace Bicep.Core.TypeSystem;

internal static class TypeCollapser
{
    internal static TypeSymbol? TryCollapse(TypeSymbol type) => type switch
    {
        // it doesn't really make sense to collapse 'never' or 'any'
        _ when type.TypeKind == TypeKind.Never => null,
        AnyType => null,
        UnionType unionType => TryCollapse(unionType),
        _ => type,
    };

    /// <remarks>
    /// How and whether multiple types can be collapsed varies by type, so a finite state machine is used so that each
    /// type can define its collapsing rules separately and we can choose which ruleset to use based on the first 1-2
    /// union members.
    /// </remarks>
    private static TypeSymbol? TryCollapse(UnionType unionType)
    {
        var collapseState = UnionCollapseState.Initialize();
        foreach (var member in unionType.Members)
        {
            collapseState = collapseState.Push(member);
        }

        return collapseState.Collapse();
    }

    private interface UnionCollapseState
    {
        UnionCollapseState Push(ITypeReference memberType);

        TypeSymbol? Collapse();

        public static UnionCollapseState Initialize() => new InitialState();

        private static TypeSymbol CreateTypeUnion(IEnumerable<TypeSymbol> toUnion, bool nullable)
            => TypeHelper.CreateTypeUnion(nullable ? toUnion.Append(LanguageConstants.Null) : toUnion);

        private class InitialState : UnionCollapseState
        {
            private bool nullable = false;

            public TypeSymbol? Collapse() => LanguageConstants.Never;

            public UnionCollapseState Push(ITypeReference memberType) => memberType.Type switch
            {
                NullType => WithNullability(),
                StringLiteralType stringLiteral => new StringCollapse(stringLiteral, nullable),
                StringType @string => new StringCollapse(@string, nullable),
                IntegerLiteralType intLiteral => new IntCollapse(intLiteral, nullable),
                IntegerType @int => new IntCollapse(@int, nullable),
                BooleanLiteralType boolLiteral => new BoolCollapse(boolLiteral, nullable),
                BooleanType @bool => new BoolCollapse(@bool, nullable),
                TupleType tuple => new ArrayCollapse(tuple, nullable),
                ArrayType array => new ArrayCollapse(array, nullable),
                AnyType => CollapsesToAny.Instance,
                _ => Uncollapsable.Instance,
            };

            private UnionCollapseState WithNullability()
            {
                nullable = true;
                return this;
            }
        }

        private class StringCollapse : UnionCollapseState
        {
            private readonly RefinementSpanCollapser spanCollapser = new();
            private readonly HashSet<StringLiteralType> stringLiterals = new();
            private bool nullable;

            internal StringCollapse(StringLiteralType stringLiteral, bool nullable)
            {
                stringLiterals.Add(stringLiteral);
                this.nullable = nullable;
            }

            internal StringCollapse(StringType @string, bool nullable)
            {
                spanCollapser.PushSpan(RefinementSpan.For(@string));
                this.nullable = nullable;
            }

            public TypeSymbol? Collapse() => CreateTypeUnion(
                // only keep string literals that are not valid in any of the discrete spans
                stringLiterals.Where(literal => !spanCollapser.Spans.Any(s => s.Contains(literal.RawStringValue.Length)))
                    // create a refined string type for each span
                    .Concat(spanCollapser.Spans.Select(span => TypeFactory.CreateStringType(
                        span.Min switch
                        {
                            <= 0 => null,
                            long otherwise => otherwise,
                        },
                        span.Max switch
                        {
                            long.MaxValue => null,
                            long otherwise => otherwise,
                        },
                        span.Flags))),
                nullable);

            public UnionCollapseState Push(ITypeReference memberType)
            {
                switch (memberType.Type)
                {
                    case StringLiteralType literal:
                        stringLiterals.Add(literal);
                        return this;
                    case StringType @string:
                        spanCollapser.PushSpan(RefinementSpan.For(@string));
                        return this;
                    case NullType:
                        nullable = true;
                        return this;
                    case AnyType:
                        return CollapsesToAny.Instance;
                    default:
                        return Uncollapsable.Instance;
                }
            }
        }

        private class IntCollapse : UnionCollapseState
        {
            private readonly RefinementSpanCollapser spanCollapser = new();
            private bool nullable;

            internal IntCollapse(IntegerLiteralType integerLiteral, bool nullable)
            {
                spanCollapser.PushSpan(RefinementSpan.For(integerLiteral));
                this.nullable = nullable;
            }

            internal IntCollapse(IntegerType @int, bool nullable)
            {
                spanCollapser.PushSpan(RefinementSpan.For(@int));
                this.nullable = nullable;
            }

            public TypeSymbol? Collapse() => CreateTypeUnion(
                spanCollapser.Spans.Select(span => span.Min == span.Max
                    ? TypeFactory.CreateIntegerLiteralType(span.Min, span.Flags)
                    : TypeFactory.CreateIntegerType(
                        span.Min switch
                        {
                            long.MinValue => null,
                            long otherwise => otherwise,
                        },
                        span.Max switch
                        {
                            long.MaxValue => null,
                            long otherwise => otherwise,
                        },
                        span.Flags)),
                nullable);

            public UnionCollapseState Push(ITypeReference memberType)
            {
                switch (memberType.Type)
                {
                    case IntegerLiteralType literal:
                        spanCollapser.PushSpan(RefinementSpan.For(literal));
                        return this;
                    case IntegerType @int:
                        spanCollapser.PushSpan(RefinementSpan.For(@int));
                        return this;
                    case NullType:
                        nullable = true;
                        return this;
                    case AnyType:
                        return CollapsesToAny.Instance;
                    default:
                        return Uncollapsable.Instance;
                }
            }
        }

        private class BoolCollapse : UnionCollapseState
        {
            private bool includesTrue;
            private bool includesFalse;
            private TypeSymbolValidationFlags flags;
            private bool nullable;

            internal BoolCollapse(BooleanLiteralType literal, bool nullable)
            {
                includesTrue = literal.Value;
                includesFalse = !literal.Value;
                flags = literal.ValidationFlags;
                this.nullable = nullable;
            }

            internal BoolCollapse(BooleanType @bool, bool nullable)
            {
                includesTrue = includesFalse = true;
                flags = @bool.ValidationFlags;
                this.nullable = nullable;
            }

            public TypeSymbol? Collapse()
            {
                TypeSymbol collapsed = includesTrue ^ includesFalse
                    ? TypeFactory.CreateBooleanLiteralType(includesTrue, flags)
                    : TypeFactory.CreateBooleanType(flags);

                return nullable ? TypeHelper.CreateTypeUnion(new[] { collapsed, LanguageConstants.Null }) : collapsed;
            }

            public UnionCollapseState Push(ITypeReference memberType)
            {
                switch (memberType.Type)
                {
                    case BooleanLiteralType literal:
                        if (literal.Value)
                        {
                            includesTrue = true;
                        }
                        else
                        {
                            includesFalse = true;
                        }
                        flags |= literal.ValidationFlags;
                        return this;
                    case BooleanType @bool:
                        includesTrue = true;
                        includesFalse = true;
                        flags |= @bool.ValidationFlags;
                        return this;
                    case NullType:
                        nullable = true;
                        return this;
                    case AnyType:
                        return CollapsesToAny.Instance;
                    default:
                        return Uncollapsable.Instance;
                }
            }
        }

        private class ArrayCollapse : UnionCollapseState
        {
            private readonly ConcurrentDictionary<TypeSymbol, RefinementSpanCollapser> spanCollapsersByItemType = new();
            private readonly HashSet<TupleType> tuples = new();
            private bool nullable;

            internal ArrayCollapse(TupleType tuple, bool nullable)
            {
                PushTuple(tuple);
                this.nullable = nullable;
            }

            internal ArrayCollapse(ArrayType array, bool nullable)
            {
                PushArraySpan(array);
                this.nullable = nullable;
            }

            public TypeSymbol? Collapse()
            {
                foreach (var tuple in tuples.ToArray())
                {
                    if (spanCollapsersByItemType.Any(kvp => kvp.Value.Spans.Any(span => span.Min <= tuple.Items.Length && tuple.Items.Length <= span.Max) &&
                        TypeValidator.AreTypesAssignable(tuple.Item.Type, kvp.Key)))
                    {
                        tuples.Remove(tuple);
                    }
                }

                return CreateTypeUnion(
                    tuples.Concat(spanCollapsersByItemType.SelectMany(kvp => kvp.Value.Spans.Select(span => TypeFactory.CreateArrayType(kvp.Key,
                        span.Min switch
                        {
                            <= 0 => null,
                            long otherwise => otherwise,
                        },
                        span.Max switch
                        {
                            long.MaxValue => null,
                            long otherwise => otherwise,
                        },
                        span.Flags)))),
                    nullable);
            }

            public UnionCollapseState Push(ITypeReference memberType)
            {
                switch (memberType.Type)
                {
                    case TupleType tuple:
                        PushTuple(tuple);
                        return this;
                    case ArrayType array:
                        PushArraySpan(array);
                        return this;
                    case NullType:
                        nullable = true;
                        return this;
                    case AnyType:
                        return CollapsesToAny.Instance;
                    default:
                        return Uncollapsable.Instance;
                }
            }

            private void PushTuple(TupleType tuple)
            {
                if (tuple.Items.Select(i => i.Type).Distinct().Count() == 1)
                {
                    PushArraySpan(TypeFactory.CreateArrayType(tuple.Items[0], tuple.Items.Length, tuple.Items.Length, tuple.ValidationFlags));
                }
                else
                {
                    tuples.Add(tuple);
                }
            }

            private void PushArraySpan(ArrayType array)
                => spanCollapsersByItemType.GetOrAdd(array.Item.Type, _ => new RefinementSpanCollapser()).PushSpan(RefinementSpan.For(array));
        }

        private class CollapsesToAny : UnionCollapseState
        {
            private CollapsesToAny() { }

            internal static readonly CollapsesToAny Instance = new();

            public TypeSymbol? Collapse() => LanguageConstants.Any;

            public UnionCollapseState Push(ITypeReference _) => this;
        }

        private class Uncollapsable : UnionCollapseState
        {
            private Uncollapsable() { }

            internal static readonly Uncollapsable Instance = new();

            public TypeSymbol? Collapse() => null;

            public UnionCollapseState Push(ITypeReference _) => this;
        }

        private readonly record struct RefinementSpan(long Min, long Max, TypeSymbolValidationFlags Flags)
        {
            internal bool Contains(long number) => Min <= number && number <= Max;

            internal bool OverlapsOrAbuts(RefinementSpan other) => Min <= (other.Max == long.MaxValue ? long.MaxValue : other.Max + 1) &&
                Max >= (other.Min == long.MinValue ? long.MinValue : other.Min - 1);

            internal RefinementSpan Fuse(RefinementSpan other) => new(Math.Min(Min, other.Min), Math.Max(Max, other.Max), Flags | other.Flags);

            internal static RefinementSpan For(StringType @string) => new(@string.MinLength ?? 0, @string.MaxLength ?? long.MaxValue, @string.ValidationFlags);

            internal static RefinementSpan For(IntegerType @int) => new(@int.MinValue ?? long.MinValue, @int.MaxValue ?? long.MaxValue, @int.ValidationFlags);

            internal static RefinementSpan For(IntegerLiteralType literal) => new(literal.Value, literal.Value, literal.ValidationFlags);

            internal static RefinementSpan For(ArrayType array) => new(array.MinLength ?? 0, array.MaxLength ?? long.MaxValue, array.ValidationFlags);
        }

        private class RefinementSpanCollapser
        {
            private readonly List<RefinementSpan> discreteSpans = new();

            internal void PushSpan(RefinementSpan span)
            {
                for (int i = 0; i < discreteSpans.Count; i++)
                {
                    if (span.OverlapsOrAbuts(discreteSpans[i]))
                    {
                        discreteSpans[i] = discreteSpans[i].Fuse(span);
                        if (discreteSpans.Count > 1)
                        {
                            var fused = discreteSpans[i];
                            discreteSpans.RemoveAt(i);
                            PushSpan(fused);
                        }
                        return;
                    }
                }

                discreteSpans.Add(span);
            }

            internal IEnumerable<RefinementSpan> Spans => discreteSpans;
        }
    }
}
