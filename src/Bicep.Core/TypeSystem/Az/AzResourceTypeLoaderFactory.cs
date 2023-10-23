// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using Bicep.Core.Features;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics.Namespaces;
using Newtonsoft.Json;

namespace Bicep.Core.TypeSystem.Az
{
    public class AzResourceTypeLoaderFactory : IResourceTypeLoaderFactory
    {
        private const string typesArtifactFilename = "types.tgz";
        private const string BuiltInLoaderKey = "builtin";
        private Dictionary<string, IResourceTypeLoader> resourceTypeLoaders;

        public AzResourceTypeLoaderFactory(IResourceTypeLoader defaultAzResourceTypeLoader)
        {
            this.resourceTypeLoaders = new() {
                {BuiltInLoaderKey, defaultAzResourceTypeLoader},
            };
        }

        public IResourceTypeLoader GetBuiltInTypeLoader()
        {
            return resourceTypeLoaders[BuiltInLoaderKey];
        }

        public IResourceTypeLoader? GetResourceTypeLoader(TypesProviderDescriptor providerDescriptor, IFeatureProvider features)
        {
            if (!features.DynamicTypeLoadingEnabled || providerDescriptor.Version is null)
            {
                return resourceTypeLoaders[BuiltInLoaderKey];
            }

            // compose the path to the OCI manifest based on the cache root directory and provider version
            var azProviderDir = providerDescriptor.Path!;
            var ociManifestPath = Path.Combine(azProviderDir, "manifest");
            if (!File.Exists(ociManifestPath))
            {
                return null;
            }

            // Read the OCI manifest
            var manifestFileContents = File.ReadAllText(ociManifestPath);
            OciManifest? ociManifest = JsonConvert.DeserializeObject<OciManifest>(manifestFileContents);

            if (ociManifest is null)
            {
                return null;
            }

            // Read the OCI type definitions
            var typesDefinitionPath = Path.Combine(azProviderDir, typesArtifactFilename);
            if (!resourceTypeLoaders.ContainsKey(typesDefinitionPath))
            {
                resourceTypeLoaders[typesDefinitionPath] = new AzResourceTypeLoader(OciAzTypeLoader.FromTgz(typesDefinitionPath));
            }
            return resourceTypeLoaders[typesDefinitionPath];
        }
    }
}
