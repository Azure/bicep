// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem
{
    public class ResourceTypeRegistrar
    {
        private readonly IResourceTypeProvider resourceTypeProvider;

        public ResourceTypeRegistrar(IResourceTypeProvider resourceTypeProvider)
        {
            this.resourceTypeProvider = resourceTypeProvider;
        }

        public ResourceType GetType(ResourceTypeReference typeReference)
            => resourceTypeProvider.GetType(typeReference);

        public bool HasType(ResourceTypeReference typeReference)
            => resourceTypeProvider.HasType(typeReference);
    }
}