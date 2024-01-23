// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.TypeSystem.Types;

/// <summary>
/// Represents an array with a fixed number of items, each of which has an independently defined type.
/// </summary>
public class TupleType : ArrayType
{
    public TupleType(ImmutableArray<ITypeReference> items, TypeSymbolValidationFlags validationFlags)
        : this(DeriveTupleName(items), items, validationFlags) { }

    public TupleType(string name, ImmutableArray<ITypeReference> items, TypeSymbolValidationFlags validationFlags)
        : base(name, new DeferredTypeReference(() => TypeHelper.CreateTypeUnion(items)), validationFlags, minLength: items.Length, maxLength: items.Length)
    {
        Items = items;
    }

    public ImmutableArray<ITypeReference> Items { get; }

    /// <summary>
    /// Recharacterize this type as a TypedArrayType containing a union of the types of the tuple members. Unlike a tuple, a typed array makes no assertions about which indices contain a more specific type.
    /// Intended to be used when a tuple's contents are reordered (e.g., via `sys.sort`) or when some members may have been removed (e.g., via `sys.filter`) but no new members have been added and no existing members have been transformed.
    /// </summary>
    public TypedArrayType ToTypedArray() => ToTypedArray(MinLength, MaxLength);

    public TypedArrayType ToTypedArray(long? minLength, long? maxLength) => new(Item, ValidationFlags, minLength, maxLength);

    public override bool Equals(object? other) => other is TupleType otherTuple && ValidationFlags == otherTuple.ValidationFlags && Items.SequenceEqual(otherTuple.Items);

    public override int GetHashCode()
    {
        HashCode hashCode = new();

        hashCode.Add(TypeKind);
        hashCode.Add(ValidationFlags);
        foreach (var item in Items)
        {
            hashCode.Add(item);
        }

        return hashCode.ToHashCode();
    }

    private static string DeriveTupleName(ImmutableArray<ITypeReference> items)
    {
        TupleTypeNameBuilder builder = new();
        foreach (var item in items)
        {
            builder.AppendItem(item.Type.Name);
        }

        return builder.ToString();
    }
}
