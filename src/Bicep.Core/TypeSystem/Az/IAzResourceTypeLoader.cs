// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Az
{
    public interface IAzResourceTypeLoader
    {
        ResourceTypeComponents LoadType(ResourceTypeReference reference);

        IEnumerable<ResourceTypeReference> GetAvailableTypes();
    }
}
