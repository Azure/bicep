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
using Bicep.Core.TypeSystem.Providers.Az;
using Newtonsoft.Json;

namespace Bicep.Core.TypeSystem.Providers.Az
{
    public class ResourceTypeProviderFactory : IResourceTypeProviderFactory
    {

        private readonly (string, string) BuiltInAzLoaderKey = ("az", IResourceTypeProvider.BuiltInVersion);
        private readonly Dictionary<(string, string), Lazy<IResourceTypeProvider>> resourceTypeLoaders;

        public ResourceTypeProviderFactory()
        {
            resourceTypeLoaders = new() {
                {BuiltInAzLoaderKey, new(new AzResourceTypeProvider(new AzResourceTypeLoader(new AzTypeLoader())))},
            };
        }

        public ResultWithDiagnostic<IResourceTypeProvider> GetResourceTypeProvider(ResourceTypesProviderDescriptor providerDescriptor, IFeatureProvider features)
        {
            if (!features.DynamicTypeLoadingEnabled)
            {
                return new(resourceTypeLoaders[BuiltInAzLoaderKey].Value);
            }
            var key = (providerDescriptor.Alias, providerDescriptor.Version);
            if (resourceTypeLoaders.ContainsKey(key))
            {
                return new(resourceTypeLoaders[key].Value);
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
            Lazy<IResourceTypeProvider> newResourceTypeLoader = providerDescriptor.Alias switch
            {
                AzNamespaceType.BuiltInName => new(new AzResourceTypeProvider(new AzResourceTypeLoader(OciTypeLoader.FromTgz(providerDescriptor.Path)), providerDescriptor.Version)),
                _ => throw new NotImplementedException($"The provider {providerDescriptor.Alias} is not supported."),
            };

            resourceTypeLoaders[key] = newResourceTypeLoader;
            return new(newResourceTypeLoader.Value);

        }

        public IResourceTypeProvider GetBuiltInAzResourceTypesProvider()
        {
            return resourceTypeLoaders[BuiltInAzLoaderKey].Value;
        }
    }
}
