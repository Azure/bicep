// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.TextFixtures.Fakes.TypeSystem
{
    public class FakeResourceTypeLoader : IResourceTypeLoader
    {
        private readonly FrozenDictionary<ResourceTypeReference, ResourceTypeComponents> resourceTypesByReference;

        public FakeResourceTypeLoader(IEnumerable<ResourceTypeComponents> resourceTypes)
        {
            this.resourceTypesByReference = resourceTypes.ToFrozenDictionary(x => x.TypeReference);
        }

        public IEnumerable<ResourceTypeReference> GetAvailableTypes() => this.resourceTypesByReference.Keys;

        public ResourceTypeComponents LoadType(ResourceTypeReference reference) => this.resourceTypesByReference[reference];
    }
}
