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
using Bicep.IO.Abstraction;
using JetBrains.Annotations;

namespace Bicep.Core.TypeSystem.Providers
{
    public class ResourceTypeProviderFactory : IResourceTypeProviderFactory
    {
        private readonly ConcurrentDictionary<IFileHandle, ResultWithDiagnosticBuilder<IResourceTypeProvider>> cachedResourceTypeLoaders = new();

        public ResultWithDiagnosticBuilder<IResourceTypeProvider> GetResourceTypeProvider(IFileHandle typesTgzFileHandle)
        {
            // TODO invalidate this cache on module force restore
            return cachedResourceTypeLoaders.GetOrAdd(typesTgzFileHandle, _ =>
            {
                try
                {
                    var typeLoader = ArchivedTypeLoader.FromFileHandle(typesTgzFileHandle);
                    var typeIndex = typeLoader.LoadTypeIndex();
                    var useAzLoader = typeIndex.Settings?.Name == AzNamespaceType.Settings.TemplateExtensionName;

                    if (useAzLoader)
                    {
                        return new(new AzResourceTypeProvider(new AzResourceTypeLoader(typeLoader, typeIndex)));
                    }

                    return new(new ExtensionResourceTypeProvider(new ExtensionResourceTypeLoader(typeLoader)));
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"Failed to deserialize provider package from {typesTgzFileHandle}: {ex}");
                    return new(x => x.InvalidTypesTgzPackage_DeserializationFailed());
                }
            });
        }

        public IResourceTypeProvider GetBuiltInAzResourceTypesProvider()
           => AzNamespaceType.BuiltInTypeProvider;
    }
}
