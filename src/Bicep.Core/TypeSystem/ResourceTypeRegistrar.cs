// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Azrm;

namespace Bicep.Core.TypeSystem
{
    public class ResourceTypeRegistrar
    {
        public static ResourceTypeRegistrar Instance { get; } = new ResourceTypeRegistrar();

        private IResourceTypeProvider resourceTypeProvider;

        private ResourceTypeRegistrar()
        {
            resourceTypeProvider = new AzrmResourceTypeProvider();
        }

        public ResourceType LookupType(ResourceTypeReference typeReference)
            => resourceTypeProvider.LookupType(typeReference);

        public bool HasTypeDefined(ResourceTypeReference typeReference)
            => resourceTypeProvider.HasTypeDefined(typeReference);
    }
}