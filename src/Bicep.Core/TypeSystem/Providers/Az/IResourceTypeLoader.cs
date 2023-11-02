// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Providers.Az
{
    public interface IResourceTypeLoader
    {
        ResourceTypeComponents LoadType(ResourceTypeReference reference);

        IEnumerable<ResourceTypeReference> GetAvailableTypes();
    }
}
