// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Types;

public class UnboundResourceDerivedType : TypeSymbol, IUnboundResourceDerivedType
{
    public UnboundResourceDerivedType(ResourceTypeReference typeReference, TypeSymbol fallbackType)
        : base(typeReference.FormatType())
    {
        TypeReference = typeReference;
        FallbackType = fallbackType;
    }

    public ResourceTypeReference TypeReference { get; }

    public TypeSymbol FallbackType { get; }

    public override TypeKind TypeKind => TypeKind.UnboundResourceDerivedType;
}
