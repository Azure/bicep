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
        public ResourceType GetType(ResourceTypeReference reference, bool isExistingResource)
        {
            var resourceType = new ResourceType(reference, new NamedObjectType(reference.FormatName(), TypeSymbolValidationFlags.Default, LanguageConstants.CreateResourceProperties(reference), null));

            return AzResourceTypeProvider.SetBicepResourceProperties(resourceType, Azure.Bicep.Types.Concrete.ScopeType.Unknown, isExistingResource);
        }

        public bool HasType(ResourceTypeReference typeReference)
            => true;

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => Enumerable.Empty<ResourceTypeReference>();

        public static IResourceTypeProvider Create()
            => new TestResourceTypeProvider();
    }
}