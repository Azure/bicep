// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;

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
}
