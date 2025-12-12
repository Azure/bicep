// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics.Metadata;

// Represents a logical resource, regardless of how it was declared.
public record ResourceMetadata(
    ResourceType Type,
    bool IsExistingResource,
    bool IsNullableExistingResource = false)
{
    public ResourceTypeReference TypeReference => Type.TypeReference;

    public bool IsAzResource => Type.IsAzResource();
}
