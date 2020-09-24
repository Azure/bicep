// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.UnitTests.Utils
{
    public class TestResourceTypeProvider : IResourceTypeProvider
    {
        public ResourceType GetType(ResourceTypeReference reference)
            => new ResourceType(reference, new NamedObjectType(reference.FormatName(), LanguageConstants.CreateResourceProperties(reference), null));

        public bool HasType(ResourceTypeReference typeReference)
            => true;

        public static ResourceTypeRegistrar CreateRegistrar()
            => new ResourceTypeRegistrar(new TestResourceTypeProvider());
    }
}