// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Abstractions;
using Azure.Bicep.Types.Az;
using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Providers.ThirdParty;
using JetBrains.Annotations;

namespace Bicep.Core.TypeSystem.Providers
{
    public class ResourceTypeProviderFactory : IResourceTypeProviderFactory
    {
        private record ResourceTypeLoaderKey(Uri TypesTgzUri, bool UseAzLoader);
        private readonly ConcurrentDictionary<ResourceTypeLoaderKey, ResultWithDiagnostic<IResourceTypeProvider>> cachedResourceTypeLoaders = new();
        private readonly IFileSystem fileSystem;

        public ResourceTypeProviderFactory(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public ResultWithDiagnostic<IResourceTypeProvider> GetResourceTypeProvider(ArtifactReference? artifactReference, Uri typesTgzUri, bool useAzLoader)
        {
            var key = new ResourceTypeLoaderKey(typesTgzUri, useAzLoader);
            // TODO invalidate this cache on module force restore
            return cachedResourceTypeLoaders.GetOrAdd(key, _ =>
            {
                try
                {
                    using var fileStream = fileSystem.File.OpenRead(typesTgzUri.LocalPath);
                    var typesLoader = OciTypeLoader.FromStream(fileStream);

                    if (key.UseAzLoader)
                    {
                        return new(new AzResourceTypeProvider(new AzResourceTypeLoader(typesLoader)));
                    }
                    return new(new ThirdPartyResourceTypeProvider(new ThirdPartyResourceTypeLoader(typesLoader)));
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"Failed to deserialize provider package from {typesTgzUri}: {ex}");
                    return new(x => x.InvalidTypesTgzPackage_DeserializationFailed());
                }
            });
        }

        public IResourceTypeProvider GetBuiltInAzResourceTypesProvider()
           => AzNamespaceType.BuiltInTypeProvider;
    }
}
