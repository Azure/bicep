// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Bicep.Core.Features
{
    public class FeatureProvider : IFeatureProvider
    {
        private readonly IEnumerable<IFeatureProviderSource> providerChain;
        private readonly IFeatureProvider defaultsProvider;
        private readonly Lazy<string> cacheRootDirectoryLazy;
        private readonly Lazy<string> assemblyVersionLazy;
        private readonly Lazy<bool> registryEnabledLazy;
        private readonly Lazy<bool> symbolicNameCodegenEnabledLazy;
        private readonly Lazy<bool> importsEnabledLazy;
        private readonly Lazy<bool> resourceTypedParamsAndOutputsEnabledLazy;
        private readonly Lazy<bool> sourceMappingEnabledLazy;
        private readonly Lazy<bool> paramsFilesEnabledLazy;

        public FeatureProvider(IFeatureProvider defaultsProvider, IEnumerable<IFeatureProviderSource> sources)
        {
            this.defaultsProvider = defaultsProvider;
            providerChain = sources.OrderBy(s => s.Priority).ToList();

            cacheRootDirectoryLazy = new(() => LookupFeature(s => s.CacheRootDirectory, d => d.CacheRootDirectory), LazyThreadSafetyMode.PublicationOnly);
            assemblyVersionLazy = new(() => LookupFeature(s => s.AssemblyVersion, d => d.AssemblyVersion), LazyThreadSafetyMode.PublicationOnly);
            registryEnabledLazy = new(() => LookupFeature(s => s.RegistryEnabled, d => d.RegistryEnabled), LazyThreadSafetyMode.PublicationOnly);
            symbolicNameCodegenEnabledLazy = new(() => LookupFeature(s => s.SymbolicNameCodegenEnabled, d => d.SymbolicNameCodegenEnabled), LazyThreadSafetyMode.PublicationOnly);
            importsEnabledLazy = new(() => LookupFeature(s => s.ImportsEnabled, d => d.ImportsEnabled), LazyThreadSafetyMode.PublicationOnly);
            resourceTypedParamsAndOutputsEnabledLazy = new(() => LookupFeature(s => s.ResourceTypedParamsAndOutputsEnabled, d => d.ResourceTypedParamsAndOutputsEnabled), LazyThreadSafetyMode.PublicationOnly);
            sourceMappingEnabledLazy = new(() => LookupFeature(s => s.SourceMappingEnabled, d => d.SourceMappingEnabled), LazyThreadSafetyMode.PublicationOnly);
            paramsFilesEnabledLazy = new(() => LookupFeature(s => s.ParamsFilesEnabled, d => d.ParamsFilesEnabled), LazyThreadSafetyMode.PublicationOnly);
        }

        public string CacheRootDirectory => cacheRootDirectoryLazy.Value;

        public bool RegistryEnabled => registryEnabledLazy.Value;

        public bool SymbolicNameCodegenEnabled => symbolicNameCodegenEnabledLazy.Value;

        public bool ImportsEnabled => importsEnabledLazy.Value;

        public bool ResourceTypedParamsAndOutputsEnabled => resourceTypedParamsAndOutputsEnabledLazy.Value;

        public string AssemblyVersion => assemblyVersionLazy.Value;

        public bool SourceMappingEnabled => sourceMappingEnabledLazy.Value;

        public bool ParamsFilesEnabled => paramsFilesEnabledLazy.Value;

        private T LookupFeature<T>(Func<IFeatureProviderSource, T?> sourceLookup, Func<IFeatureProvider, T> defaultLookup)
        {
            foreach (var link in providerChain)
            {
                if (sourceLookup.Invoke(link) is T resolved)
                {
                    return resolved;
                }
            }

            return defaultLookup.Invoke(defaultsProvider);
        }

        private T LookupFeature<T>(Func<IFeatureProviderSource, Nullable<T>> sourceLookup, Func<IFeatureProvider, T> defaultLookup) where T : struct
        {
            foreach (var link in providerChain)
            {
                if (sourceLookup.Invoke(link) is T resolved)
                {
                    return resolved;
                }
            }

            return defaultLookup.Invoke(defaultsProvider);
        }
    }
}
