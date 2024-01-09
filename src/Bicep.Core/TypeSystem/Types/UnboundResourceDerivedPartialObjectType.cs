// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Types;

/// <summary>
/// An IUnboundResourceDerivedType to use as a branch of a DiscriminatedObjectType
/// </summary>
public class UnboundResourceDerivedPartialObjectType : ObjectType, IUnboundResourceDerivedType
{
    public UnboundResourceDerivedPartialObjectType(ResourceTypeReference typeReference, string discriminatorName, string discriminatorValue)
        : base(typeReference.FormatType(),
            TypeSymbolValidationFlags.Default,
            new TypeProperty(discriminatorName, TypeFactory.CreateStringLiteralType(discriminatorValue)).AsEnumerable(),
            LanguageConstants.Any,
            TypePropertyFlags.FallbackProperty)
    {
        TypeReference = typeReference;
    }

    public ResourceTypeReference TypeReference { get; }

    public TypeSymbol FallbackType => this;

    public override TypeKind TypeKind => TypeKind.UnboundResourceDerivedType;
}
