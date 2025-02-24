// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry.Catalog.Implementation;

public static class IRegistryModuleMetadataProviderExtensions
{
    public static async Task<IRegistryModuleMetadata?> TryGetModuleAsync(this IRegistryModuleMetadataProvider provider, string modulePath)
    {
        return (await provider.TryGetModulesAsync())
            .FirstOrDefault(x => x.ModuleName.Equals(modulePath, StringComparison.Ordinal));
    }
}
