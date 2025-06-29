// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Types;

public class UnresolvedResourceDerivedType : TypeSymbol, IUnresolvedResourceDerivedType
{
    public UnresolvedResourceDerivedType(
        ResourceTypeReference typeReference,
        ImmutableArray<string> pointerSegments,
        TypeSymbol fallbackType,
        ResourceDerivedTypeVariant variant) : base(typeReference.FormatType())
    {
        TypeReference = typeReference;
        PointerSegments = pointerSegments;
        FallbackType = fallbackType;
        Variant = variant;
    }

    public ResourceTypeReference TypeReference { get; }

    public ImmutableArray<string> PointerSegments { get; }

    public TypeSymbol FallbackType { get; }

    public ResourceDerivedTypeVariant Variant { get; }

    public override TypeKind TypeKind => TypeKind.UnboundResourceDerivedType;
}
