// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
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

    public IBicepConfigurationChain GetConfigurationChain(IOUri sourceFileUri) => inner.GetConfigurationChain(sourceFileUri);
    public IBicepConfiguration GetMergedConfiguration(IOUri sourceFileUri) => patchFunc(inner.GetMergedConfiguration(sourceFileUri));
    public void PurgeCacheForAffectedChains(IOUri changedFileUri) => inner.PurgeCacheForAffectedChains(changedFileUri);
    public void PurgeAllCaches() => inner.PurgeAllCaches();
    public void PurgeChainCache() => inner.PurgeChainCache();
}
