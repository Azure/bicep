// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.UnitTests.Utils
{
    public class TestResourceTypeProvider : IResourceTypeProvider
    {
        public ResourceType GetType(ResourceTypeReference reference)
            => new ResourceType(reference, new NamedObjectType(reference.FormatName(), TypeSymbolValidationFlags.Default, LanguageConstants.CreateResourceProperties(reference), null), TypeSymbolValidationFlags.Default);

        public bool HasType(ResourceTypeReference typeReference)
            => true;

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => throw new NotImplementedException();

        public static IResourceTypeProvider Create()
            => new TestResourceTypeProvider();
    }
}