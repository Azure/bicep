// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
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
        public AzResourceTypeLoaderFactory(IFeatureProviderFactory featureProviderFactory)
        {
            this.featureProviderFactory = featureProviderFactory;
        }

        public IAzResourceTypeLoader GetResourceTypeLoader(ImportDeclarationSyntax? ids, IFeatureProvider features)
        {
            if (!features.DynamicTypeLoadingEnabled || ids is null)
            {
                return new AzResourceTypeLoader(new AzTypeLoader());
            }

            var azProviderDir = Path.Combine(features.CacheRootDirectory, "br", "mcr.microsoft.com", @"bicep$providers$az", ids.Specification.Version);
            var ociManifestPath = Path.Combine(azProviderDir, "manifest.json");
            if (!File.Exists(ociManifestPath))
            {
                throw new InvalidOperationException($"Failed to find OCI manifest at {ociManifestPath}");
            }
            var manifestFileContents = File.ReadAllText(ociManifestPath);
            OciManifest? ociManifest = JsonConvert.DeserializeObject<OciManifest>(manifestFileContents);
            if (ociManifest is null)
            {
                throw new InvalidOperationException($"Failed to deserialize OCI manifest from {ociManifestPath}: manifest contents are not following expected schema");
            }
            var typesDefinitionFilename = ociManifest.Layers.SingleOrDefault()?.Annotations["org.opencontainers.image.title"];
            if (typesDefinitionFilename is null)
            {
                throw new InvalidOperationException($"Failed to find OCI manifest layers from {ociManifestPath}: manifest contents are not following expected schema");
            }
            var typesDefinitionPath = Path.Combine(azProviderDir, typesDefinitionFilename);
            return new AzResourceTypeLoader(new AzTypeLoaderOci(typesDefinitionPath));
        }
    }
}
