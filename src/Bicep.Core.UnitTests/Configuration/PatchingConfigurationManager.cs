// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.Core.UnitTests.Configuration;

public class PatchingConfigurationManager : IBicepConfigurationManager
{
    private readonly IBicepConfigurationManager inner;
    private readonly Func<IBicepConfiguration, IBicepConfiguration> patchFunc;

    public PatchingConfigurationManager(IBicepConfigurationManager inner, Func<IBicepConfiguration, IBicepConfiguration> patchFunc)
    {
        this.inner = inner;
        this.patchFunc = patchFunc;
    }

    public IBicepConfigurationChain GetConfigurationChain(IOUri sourceFileUri)
        => new PatchingChain(inner.GetConfigurationChain(sourceFileUri), patchFunc);

    public void PurgeCacheForAffectedChains(IOUri changedFileUri) => inner.PurgeCacheForAffectedChains(changedFileUri);
    public void PurgeAllCaches() => inner.PurgeAllCaches();
    public void PurgeChainCache() => inner.PurgeChainCache();

    private sealed class PatchingChain(IBicepConfigurationChain inner, Func<IBicepConfiguration, IBicepConfiguration> patchFunc) : IBicepConfigurationChain
    {
        public IBicepConfiguration GetEffectiveConfiguration() => patchFunc(inner.GetEffectiveConfiguration());
        public IEnumerable<KeyValuePair<IOUri, ImmutableArray<IDiagnostic>>> EnumerateDiagnosticsPerFile() => inner.EnumerateDiagnosticsPerFile();
        public int LayerCount => inner.LayerCount;
    }
}

