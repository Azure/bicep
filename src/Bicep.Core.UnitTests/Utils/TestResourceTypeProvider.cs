// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.UnitTests.Utils
{
    public class TestResourceTypeProvider : IResourceTypeProvider
    {
        public ResourceType GetType(ResourceScopeType scopeType, ResourceTypeReference reference)
            => new ResourceType(reference, new NamedObjectType(reference.FormatName(), TypeSymbolValidationFlags.Default, LanguageConstants.CreateResourceProperties(reference), null));

        public bool HasType(ResourceScopeType scopeType, ResourceTypeReference typeReference)
            => true;

        public IEnumerable<ResourceTypeReference> GetAvailableTypes(ResourceScopeType scopeType)
            => Enumerable.Empty<ResourceTypeReference>();

        public static IResourceTypeProvider Create()
            => new TestResourceTypeProvider();
    }
}