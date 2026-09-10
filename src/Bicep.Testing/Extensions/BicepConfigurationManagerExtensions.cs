// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.Testing.Extensions;

public static class BicepConfigurationManagerExtensions
{
    /// <summary>Wraps this configuration in an <see cref="IBicepConfigurationManager"/> that always returns it.</summary>
    public static IBicepConfigurationManager WithStaticConfiguration(this IBicepConfiguration configuration)
        => new ConstantBicepConfigurationManager(configuration);

    private sealed class ConstantBicepConfigurationManager : IBicepConfigurationManager
    {
        private readonly IBicepConfiguration configuration;

        internal ConstantBicepConfigurationManager(IBicepConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IBicepConfigurationChain GetConfigurationChain(IOUri sourceFileUri) => new ConstantChain(this.configuration);
        public void PurgeCacheForAffectedChains(IOUri changedFileUri) { }
        public void PurgeAllCaches() { }
        public void PurgeChainCache() { }
    }

    private sealed class ConstantChain(IBicepConfiguration configuration) : IBicepConfigurationChain
    {
        public IBicepConfiguration GetEffectiveConfiguration() => configuration;
        public IEnumerable<KeyValuePair<IOUri, ImmutableArray<IDiagnostic>>> EnumerateDiagnosticsPerFile() => [];
        public int LayerCount => 1;
    }
}
