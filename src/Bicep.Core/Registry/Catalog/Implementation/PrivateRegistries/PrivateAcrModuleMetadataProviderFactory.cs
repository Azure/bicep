// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Microsoft.Win32;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Registry.Catalog.Implementation.PrivateRegistries;

/// <summary>
/// Creates IRegistryModuleMetadataProvider instances for private registries
/// </summary>
public class PrivateAcrModuleMetadataProviderFactory : IPrivateAcrModuleMetadataProviderFactory
{
    public IRegistryModuleMetadataProvider Create(CloudConfiguration cloud, string registry, IContainerRegistryClientFactory containerRegistryClientFactory)
    {
        return new PrivateAcrModuleMetadataProvider(cloud, registry, containerRegistryClientFactory);
    }
}
