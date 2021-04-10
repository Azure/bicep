// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;

namespace Bicep.Core.UnitTests.Utils
{
    public class TestResourceTypeProvider : IResourceTypeProvider
    {
        public ResourceType GetType(ResourceTypeReference reference, ResourceTypeGenerationFlags flags)
        {
            var bodyType = new ObjectType(reference.FormatName(), TypeSymbolValidationFlags.Default, LanguageConstants.CreateResourceProperties(reference), null);
            var resourceType = new ResourceType(reference, ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup | ResourceScope.Resource, bodyType);

            return AzResourceTypeProvider.SetBicepResourceProperties(resourceType, flags);
        }

        public bool HasType(ResourceTypeReference typeReference)
            => true;

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => Enumerable.Empty<ResourceTypeReference>();

        public static IResourceTypeProvider Create()
            => new TestResourceTypeProvider();
    }
}