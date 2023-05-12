// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Azure.Bicep.Types.Az;
using Bicep.Core.Syntax;
using System.IO;
using Bicep.Core.Registry.Oci;
using Newtonsoft.Json;
using Bicep.Core.Features;

namespace Bicep.Core.TypeSystem.Az
{
    public class AzResourceTypeLoaderFactory : IAzResourceTypeLoaderFactory
    {
        private readonly IFeatureProviderFactory featureProviderFactory;

        private Dictionary<string, IAzResourceTypeLoader> resourceTypeLoaders = new(){
            {"builtin", new AzResourceTypeLoader(new AzTypeLoader())},
        };

        public AzResourceTypeLoaderFactory(IFeatureProviderFactory featureProviderFactory)
        {
            this.featureProviderFactory = featureProviderFactory;
        }

        public IAzResourceTypeLoader? GetResourceTypeLoader(ImportDeclarationSyntax? ids, IFeatureProvider features)
        {
            if (!features.DynamicTypeLoadingEnabled || ids is null)
            {
                return resourceTypeLoaders["builtin"];
            }

            var azProviderDir = Path.Combine(features.CacheRootDirectory, "br", "mcr.microsoft.com", @"bicep$providers$az", ids.Specification.Version);
            var ociManifestPath = Path.Combine(azProviderDir, "manifest.json");
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

            // Get the filename of the OCI type definitions artifact
            var typesDefinitionFilename = ociManifest.Layers.SingleOrDefault()?.Annotations["org.opencontainers.image.title"];

            if (typesDefinitionFilename is null)
            {
                return null;
            }

            // Read the OCI type definitions
            var typesDefinitionPath = Path.Combine(azProviderDir, typesDefinitionFilename);
            if (!resourceTypeLoaders.ContainsKey(typesDefinitionPath))
            {
                resourceTypeLoaders[typesDefinitionPath] = new AzResourceTypeLoader(new AzTypeLoaderOci(typesDefinitionPath));
            }
            return resourceTypeLoaders[typesDefinitionPath];
        }
    }
}
