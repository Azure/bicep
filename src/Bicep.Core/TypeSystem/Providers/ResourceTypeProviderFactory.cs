// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using System.Text.Json;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Az;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Providers.ThirdParty;

namespace Bicep.Core.TypeSystem.Providers
{
    public class ResourceTypeProviderFactory : IResourceTypeProviderFactory
    {
        private static readonly Lazy<IResourceTypeProvider> azResourceTypeProviderLazy
            = new(() => new AzResourceTypeProvider(new AzResourceTypeLoader(new AzTypeLoader()), AzNamespaceType.Settings.ArmTemplateProviderVersion));

        private record ResourceTypeLoaderKey(string Name, string Version);
        private readonly Dictionary<ResourceTypeLoaderKey, IResourceTypeProvider> cachedResourceTypeLoaders = new();
        private readonly IFileSystem fileSystem;

        public ResourceTypeProviderFactory(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public ResultWithDiagnostic<IResourceTypeProvider> GetResourceTypeProviderFromFilePath(ResourceTypesProviderDescriptor providerDescriptor)
        {
            var key = new ResourceTypeLoaderKey(providerDescriptor.Name, providerDescriptor.Version);

            if (cachedResourceTypeLoaders.ContainsKey(key))
            {
                return new(cachedResourceTypeLoaders[key]);
            }

            // is never null since provider restore success is validated prior.
            var typesTgzPath = Uri.UnescapeDataString(providerDescriptor.TypesBaseUri?.AbsolutePath ?? throw new UnreachableException("the provider directory doesn't exist"));
            var typesParentPath = Path.GetDirectoryName(typesTgzPath) ?? throw new UnreachableException("the provider directory doesn't exist");

            // compose the path to the OCI manifest based on the cache root directory and provider version
            var ociManifestPath = Path.Combine(typesParentPath, "manifest");
            if (!fileSystem.File.Exists(ociManifestPath))
            {
                // always exists since provider restore was successful
                throw new UnreachableException("the provider manifest path doesn't exist");
            }

            // Read the OCI manifest
            var manifestFileContents = fileSystem.File.OpenRead(ociManifestPath);
            var ociManifest = JsonSerializer.Deserialize(manifestFileContents, OciManifestSerializationContext.Default.OciManifest);

            if (ociManifest is null)
            {
                return new(x => x.ErrorOccurredReadingFile(ociManifestPath)); ;
            }

            using var fileStream = fileSystem.File.OpenRead(Path.Combine(typesParentPath, OciTypeLoader.TypesArtifactFilename));

            IResourceTypeProvider? newResourceTypeLoader = providerDescriptor.Name switch
            {
                AzNamespaceType.BuiltInName => new AzResourceTypeProvider(new AzResourceTypeLoader(OciTypeLoader.FromTgz(fileStream)), providerDescriptor.Version),
                _ => new ThirdPartyResourceTypeProvider(new ThirdPartyResourceTypeLoader(OciTypeLoader.FromTgz(fileStream)), providerDescriptor.Version),
            };
            return new(cachedResourceTypeLoaders[key] = newResourceTypeLoader);
        }

        public IResourceTypeProvider GetBuiltInAzResourceTypesProvider()
            => azResourceTypeProviderLazy.Value;
    }
}
