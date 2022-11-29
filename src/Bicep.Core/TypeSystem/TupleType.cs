// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.TypeSystem;

/// <summary>
/// Represents an array with a fixed number of items, each of which has an independently defined type.
/// </summary>
public class TupleType : ArrayType
{
    private readonly Lazy<ITypeReference> lazyItem;

    public TupleType(string name, ImmutableArray<ITypeReference> items, TypeSymbolValidationFlags validationFlags) : base(name)
    {
        Items = items;
        ValidationFlags = validationFlags;
        // Items may contain DeferredTypeReferences, so only calculate Item on request
        lazyItem = new(() => TypeHelper.CreateTypeUnion(this.Items));
    }

    public override TypeSymbolValidationFlags ValidationFlags { get; }

    public ImmutableArray<ITypeReference> Items { get; }

    public override ITypeReference Item => lazyItem.Value;

    public override bool Equals(object? other) =>
        other is TupleType otherTuple ? otherTuple == this : false;

    public override int GetHashCode() => (GetType(), Items, ValidationFlags).GetHashCode();

    public static bool operator ==(TupleType? a, TupleType? b) => a is not null
        ? (b is not null ? nonNullEquals(a, b) : false)
        : b is null;

    public static bool operator !=(TupleType? a, TupleType? b) => !(a == b);

    private static bool nonNullEquals(TupleType a, TupleType b) => a.ValidationFlags == b.ValidationFlags &&
        a.Items.Length == b.Items.Length &&
        Enumerable.Range(0, a.Items.Length).All(idx => a.Items[idx].Equals(b.Items[idx]));
}
