// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem
{
    public interface IResourceTypeProvider
    {
        ResourceType? TryGetType(ResourceTypeReference reference, ResourceTypeGenerationFlags flags);

        bool HasType(ResourceTypeReference typeReference);

        IEnumerable<ResourceTypeReference> GetAvailableTypes();
    }
}
