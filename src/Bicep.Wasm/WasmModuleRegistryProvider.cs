// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry;
using Bicep.IO.Abstraction;

namespace Bicep.Wasm
{
    public class WasmModuleRegistryProvider : ArtifactRegistryProvider
    {
        public WasmModuleRegistryProvider(IFileExplorer fileExplorer)
            : base([new LocalModuleRegistry(fileExplorer)])
        {
        }
    }
}
