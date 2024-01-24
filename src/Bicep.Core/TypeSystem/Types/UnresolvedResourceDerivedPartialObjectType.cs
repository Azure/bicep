// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Types;

/// <summary>
/// An IUnresolvedResourceDerivedType to use as a branch of a DiscriminatedObjectType
/// </summary>
public class UnresolvedResourceDerivedPartialObjectType : ObjectType, IUnresolvedResourceDerivedType
{
    public UnresolvedResourceDerivedPartialObjectType(ResourceTypeReference typeReference, ImmutableArray<string> pointerSegments, string discriminatorName, string discriminatorValue)
        : base(typeReference.FormatType(),
            TypeSymbolValidationFlags.Default,
            new TypeProperty(discriminatorName, TypeFactory.CreateStringLiteralType(discriminatorValue)).AsEnumerable(),
            LanguageConstants.Any,
            TypePropertyFlags.FallbackProperty)
    {
        TypeReference = typeReference;
        PointerSegments = pointerSegments;
    }

    public ResourceTypeReference TypeReference { get; }

    public ImmutableArray<string> PointerSegments { get; }

    public TypeSymbol FallbackType => this;

    public override TypeKind TypeKind => TypeKind.UnboundResourceDerivedType;
}
