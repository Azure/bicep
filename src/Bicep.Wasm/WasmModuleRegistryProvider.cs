// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry;

namespace Bicep.Wasm
{
    public class WasmModuleRegistryProvider : ArtifactRegistryProvider
    {
        public WasmModuleRegistryProvider()
            : base([new LocalModuleRegistry()])
        {
        }
    }
}
