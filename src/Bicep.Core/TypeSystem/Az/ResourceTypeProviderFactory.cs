// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Az;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Az;
using Newtonsoft.Json;

namespace Bicep.Core.TypeSystem
{


    public class ResourceTypeProviderFactory : IResourceTypeProviderFactory
    {
        private record Key(string Name, string Version);

        private readonly Key BuiltInAzLoaderKey = new("az", IResourceTypeProvider.BuiltInVersion);
        private readonly Dictionary<Key, IResourceTypeProvider> resourceTypeLoaders;

        public ResourceTypeProviderFactory()
        {
            resourceTypeLoaders = new() {
                {BuiltInAzLoaderKey, new AzResourceTypeProvider(new AzResourceTypeLoader(new AzTypeLoader()),IResourceTypeProvider.BuiltInVersion)},
            };
        }

        public ResultWithDiagnostic<IResourceTypeProvider> GetResourceTypeProvider(ResourceTypesProviderDescriptor providerDescriptor, IFeatureProvider features)
        {
            if (!features.DynamicTypeLoadingEnabled)
            {
                return new(resourceTypeLoaders[BuiltInAzLoaderKey]);
            }
            var key = new Key(providerDescriptor.Alias, providerDescriptor.Version);

            if (resourceTypeLoaders.ContainsKey(key))
            {
                return new(resourceTypeLoaders[key]);
            }

            if (providerDescriptor.Path  == null)
            {
                // should never happen since builtin providers are handled prior and path is required for non-builtin providers
                throw new ArgumentNullException("Provider filepath is null");
            }

            // compose the path to the OCI manifest based on the cache root directory and provider version
            var ociManifestPath = Path.Combine(providerDescriptor.Path, "manifest");
            if (!File.Exists(ociManifestPath))
            {
                return new(x => x.ArtifactFilePathCouldNotBeResolved(ociManifestPath));
            }

            // Read the OCI manifest
            var manifestFileContents = File.ReadAllText(ociManifestPath);
            OciManifest? ociManifest = JsonConvert.DeserializeObject<OciManifest>(manifestFileContents);

            if (ociManifest is null)
            {
                return new(x => x.ErrorOccurredReadingFile(ociManifestPath)); ;
            }

            // Register a new types loader
            IResourceTypeProvider newResourceTypeLoader = providerDescriptor.Alias switch
            {
                AzNamespaceType.BuiltInName => new AzResourceTypeProvider(new AzResourceTypeLoader(OciTypeLoader.FromTgz(providerDescriptor.Path)), providerDescriptor.Version),
                _ => throw new NotImplementedException($"The provider {providerDescriptor.Alias} is not supported."),
            };

            return new(resourceTypeLoaders[key] = newResourceTypeLoader);

        }

        public IResourceTypeProvider GetBuiltInAzResourceTypesProvider()
        {
            return resourceTypeLoaders[BuiltInAzLoaderKey];
        }
    }
}
