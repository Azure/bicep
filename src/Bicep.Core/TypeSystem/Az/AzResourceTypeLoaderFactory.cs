// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Linq;
using Azure.Bicep.Types.Az;
using Bicep.Core.Syntax;
using System.IO;
using Bicep.Core.Registry.Oci;
using Newtonsoft.Json;
using Bicep.Core.Features;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.TypeSystem.Az
{
    public class AzResourceTypeLoaderFactory : IAzResourceTypeLoaderFactory
    {
        private readonly IFeatureProviderFactory featureProviderFactory;
        public AzResourceTypeLoaderFactory(IFeatureProviderFactory featureProviderFactory)
        {
            this.featureProviderFactory = featureProviderFactory;
        }

        public IAzResourceTypeLoader? GetResourceTypeLoader(ImportDeclarationSyntax? ids, IFeatureProvider features)
        {
            if (!features.DynamicTypeLoadingEnabled || ids is null)
            {
                return new AzResourceTypeLoader(new AzTypeLoader());
            }

            var azProviderDir = Path.Combine(features.CacheRootDirectory, "br", "mcr.microsoft.com", @"bicep$providers$az", ids.Specification.Version);
            var ociManifestPath = Path.Combine(azProviderDir, "manifest.json");
            if (!File.Exists(ociManifestPath))
            {
                return null;
            }
            var manifestFileContents = File.ReadAllText(ociManifestPath);
            OciManifest? ociManifest = JsonConvert.DeserializeObject<OciManifest>(manifestFileContents);
            if (ociManifest is null)
            {
                return null;
            }
            var typesDefinitionFilename = ociManifest.Layers.SingleOrDefault()?.Annotations["org.opencontainers.image.title"];
            if (typesDefinitionFilename is null)
            {
                return null;
            }
            var typesDefinitionPath = Path.Combine(azProviderDir, typesDefinitionFilename);
            return new AzResourceTypeLoader(new AzTypeLoaderOci(typesDefinitionPath));
        }
    }
}
