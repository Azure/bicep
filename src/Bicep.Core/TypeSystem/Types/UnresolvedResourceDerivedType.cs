// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Types;

public class UnresolvedResourceDerivedType(ResourceTypeReference typeReference, ImmutableArray<string> pointerSegments, TypeSymbol fallbackType) : TypeSymbol(typeReference.FormatType()), IUnresolvedResourceDerivedType
{
    public ResourceTypeReference TypeReference { get; } = typeReference;

    public ImmutableArray<string> PointerSegments { get; } = pointerSegments;

    public TypeSymbol FallbackType { get; } = fallbackType;

    public override TypeKind TypeKind => TypeKind.UnboundResourceDerivedType;
}
