// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem
{
    public interface IResourceTypeProvider
    {
        ResourceType GetType(ResourceScope scopeType, ResourceTypeReference reference);

        bool HasType(ResourceScope scopeType, ResourceTypeReference typeReference);

        IEnumerable<ResourceTypeReference> GetAvailableTypes(ResourceScope scopeType);
    }
}