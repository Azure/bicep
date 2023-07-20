// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using Bicep.Core.Registry.Oci;
using Newtonsoft.Json;
using Bicep.Core.Features;

namespace Bicep.Core.TypeSystem.Az
{
    public class AzResourceTypeLoaderFactory : IAzResourceTypeLoaderFactory
    {
        private const string typesArtifactFilename = "types.tgz";
        private const string BuiltInLoaderKey = "builtin";

        private readonly IFeatureProviderFactory featureProviderFactory;

        private Dictionary<string, IAzResourceTypeLoader> resourceTypeLoaders;

        public AzResourceTypeLoaderFactory(IFeatureProviderFactory featureProviderFactory, IAzResourceTypeLoader defaultAzResourceTypeLoader)
        {
            this.featureProviderFactory = featureProviderFactory;
            this.resourceTypeLoaders = new() {
                {BuiltInLoaderKey, defaultAzResourceTypeLoader},
            };
        }

        public IAzResourceTypeLoader GetBuiltInTypeLoader()
        {
            return resourceTypeLoaders[BuiltInLoaderKey];
        }

        public IAzResourceTypeLoader? GetResourceTypeLoader(string? version, IFeatureProvider features)
        {
            if (!features.DynamicTypeLoadingEnabled || version is null)
            {
                return resourceTypeLoaders[BuiltInLoaderKey];
            }

            //TODO(asilverman): The magic strings below are temporary and will be changed to use variables fetched at restore time
            var azProviderDir = Path.Combine(features.CacheRootDirectory, "br", "mcr.microsoft.com", @"bicep$providers$az", version);
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
