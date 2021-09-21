// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem
{
    public class EmptyResourceTypeProvider : IResourceTypeProvider
    {
        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => Enumerable.Empty<ResourceTypeReference>();

        public ResourceType? TryGetType(ResourceTypeReference reference, ResourceTypeGenerationFlags flags)
            => null;

        public bool HasType(ResourceTypeReference typeReference)
            => false;
    }
}
