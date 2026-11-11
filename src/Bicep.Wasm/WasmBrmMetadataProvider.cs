// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core;
using Bicep.Core.Artifacts.Catalog;
using Bicep.Core.Artifacts.Catalog.Brm;

namespace Bicep.Wasm
{
    public sealed class WasmBrmMetadataProvider : IBrmMetadataProvider
    {
        public string Registry => LanguageConstants.BicepPublicMcrRegistry;

        public bool IsCached => true;

        public string? DownloadError => null;

        public void StartCache()
        {
        }

        public Task TryAwaitCache(bool forceUpdate = false) => Task.CompletedTask;

        public Task<ImmutableArray<IRegistryModuleMetadata>> TryGetModulesAsync() => Task.FromResult(ImmutableArray<IRegistryModuleMetadata>.Empty);

        public ImmutableArray<IRegistryModuleMetadata> GetCachedModules() => [];
    }
}
