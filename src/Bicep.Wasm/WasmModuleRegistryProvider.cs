// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Catalog;
using Bicep.IO.Abstraction;

namespace Bicep.Wasm
{
    public class WasmModuleRegistryProvider : ArtifactRegistryProvider
    {
        public WasmModuleRegistryProvider(
            RegistryConfiguration registryConfiguration,
            IPublicModuleMetadataProvider publicModuleMetadataProvider,
            IContainerRegistryClientFactory clientFactory,
            IFileExplorer fileExplorer)
            : base([
                new LocalModuleRegistry(),
                new OciArtifactRegistry(registryConfiguration, clientFactory, publicModuleMetadataProvider, fileExplorer),
            ])
        {
        }
    }
}
