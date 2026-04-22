// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Oci;
using Microsoft.Win32;

namespace Bicep.Core.Registry.Catalog.Implementation.PrivateRegistries;

/// <summary>
/// Creates IRegistryModuleMetadataProvider instances
/// </summary>
public interface IPrivateAcrModuleMetadataProviderFactory
{
    public IRegistryModuleMetadataProvider Create(CloudConfiguration cloud, string registry, IOciRegistryTransportFactory transportFactory);
}
