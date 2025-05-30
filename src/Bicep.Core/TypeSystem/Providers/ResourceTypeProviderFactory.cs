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
using Bicep.Core.TypeSystem.Providers.Extensibility;
using Bicep.Core.TypeSystem.Providers.MicrosoftGraph;
using JetBrains.Annotations;

namespace Bicep.Core.TypeSystem.Providers
{
    public class ResourceTypeProviderFactory : IResourceTypeProviderFactory
    {
        private readonly ConcurrentDictionary<Uri, ResultWithDiagnosticBuilder<IResourceTypeProvider>> cachedResourceTypeLoaders = new();
        private readonly IFileSystem fileSystem;

        public ResourceTypeProviderFactory(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public ResultWithDiagnosticBuilder<IResourceTypeProvider> GetResourceTypeProvider(ArtifactReference? artifactReference, Uri typesTgzUri)
        {
            // TODO invalidate this cache on module force restore
            return cachedResourceTypeLoaders.GetOrAdd(typesTgzUri, _ =>
            {
                try
                {
                    using var fileStream = fileSystem.File.OpenRead(typesTgzUri.LocalPath);
                    var typesLoader = OciTypeLoader.FromStream(fileStream);

                    var typeIndex = typesLoader.LoadTypeIndex();
                    var useAzLoader = typeIndex.Settings?.Name == AzNamespaceType.Settings.TemplateExtensionName;
                    var useMsGraphLoader = MicrosoftGraphNamespaceType.ShouldUseLoader(typeIndex.Settings?.Name);

                    if (useAzLoader)
                    {
                        return new(new AzResourceTypeProvider(new AzResourceTypeLoader(typesLoader, typeIndex)));
                    }
                    else if (useMsGraphLoader)
                    {
                        return new(new MicrosoftGraphResourceTypeProvider(new MicrosoftGraphResourceTypeLoader(typesLoader)));
                    }

                    return new(new ExtensionResourceTypeProvider(new ExtensionResourceTypeLoader(typesLoader)));
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
