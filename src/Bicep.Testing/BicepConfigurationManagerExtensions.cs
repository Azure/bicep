// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.IO.Abstraction;

namespace Bicep.Testing;

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

        public IBicepConfigurationChain GetConfigurationChain(IOUri sourceFileUri)
            => throw new NotSupportedException("ConstantBicepConfigurationManager does not support GetConfigurationChain.");

        public IBicepConfiguration GetMergedConfiguration(IOUri sourceFileUri) => configuration;
        public void PurgeCacheForAffectedChains(IOUri changedFileUri) { }
        public void PurgeAllCaches() { }
        public void PurgeChainCache() { }
    }
}
