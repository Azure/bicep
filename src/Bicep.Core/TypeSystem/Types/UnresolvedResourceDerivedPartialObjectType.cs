// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Types;

/// <summary>
/// An IUnresolvedResourceDerivedType to use as a branch of a DiscriminatedObjectType
/// </summary>
public class UnresolvedResourceDerivedPartialObjectType(ResourceTypeReference typeReference, ImmutableArray<string> pointerSegments, string discriminatorName, string discriminatorValue) : ObjectType(typeReference.FormatType(),
        TypeSymbolValidationFlags.Default,
        new TypeProperty(discriminatorName, TypeFactory.CreateStringLiteralType(discriminatorValue)).AsEnumerable(),
        LanguageConstants.Any,
        TypePropertyFlags.FallbackProperty), IUnresolvedResourceDerivedType
{
    public ResourceTypeReference TypeReference { get; } = typeReference;

    public ImmutableArray<string> PointerSegments { get; } = pointerSegments;

    public TypeSymbol FallbackType => this;

    public override TypeKind TypeKind => TypeKind.UnboundResourceDerivedType;
}
