// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

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
        lazyItem = new(() => TypeHelper.CreateTypeUnion(this.Items), LazyThreadSafetyMode.PublicationOnly);
    }

    public override TypeSymbolValidationFlags ValidationFlags { get; }

    public ImmutableArray<ITypeReference> Items { get; }

    public override ITypeReference Item => lazyItem.Value;

    /// <summary>
    /// Recharacterize this type as a TypedArrayType containing a union of the types of the tuple members. Unlike a tuple, a typed array makes no assertions about the associated array value's length or about which indices contain a more specific type.
    /// Intended to be used when a tuple's contents are reordered (e.g., via `sys.sort`) or when some members may have been removed (e.g., via `sys.filter`) but no new members have been added and no existing members have been transformed.
    /// </summary>
    public TypedArrayType ToTypedArray() => new TypedArrayType(Item, ValidationFlags);

    public override bool Equals(object? other) => other is TupleType otherTuple && ValidationFlags == otherTuple.ValidationFlags && Items.SequenceEqual(otherTuple.Items);

    public override int GetHashCode() => HashCode.Combine(TypeKind, Items, ValidationFlags);
}
