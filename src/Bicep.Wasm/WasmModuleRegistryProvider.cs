// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Catalog;
using Bicep.Core.Registry.Oci;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bicep.Wasm
{
    public class WasmModuleRegistryProvider : ArtifactRegistryProvider
    {
        public WasmModuleRegistryProvider(
            RegistryConfiguration registryConfiguration,
            IPublicModuleMetadataProvider publicModuleMetadataProvider,
            IOciRegistryTransportFactory transportFactory,
            ILogger<OciArtifactRegistry>? logger = null)
            : base([
                new LocalModuleRegistry(),
                new OciArtifactRegistry(registryConfiguration, transportFactory, publicModuleMetadataProvider, logger ?? NullLogger<OciArtifactRegistry>.Instance),
            ])
        {
        }
    }
}
