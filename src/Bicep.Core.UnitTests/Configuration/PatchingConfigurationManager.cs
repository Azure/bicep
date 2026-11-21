// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.Core.UnitTests.Configuration;

public class PatchingConfigurationManager : IBicepConfigurationManager
{
    private readonly IBicepConfigurationManager inner;
    private readonly Func<IBicepConfiguration, IBicepConfiguration> patchFunc;

    // Patches each configuration once, so this returns the same instance until the inner manager's configuration changes,
    // as IBicepConfigurationManager callers expect.
    private readonly ConditionalWeakTable<IBicepConfiguration, IBicepConfiguration> patchedConfigurations = new();

    public PatchingConfigurationManager(IBicepConfigurationManager inner, Func<IBicepConfiguration, IBicepConfiguration> patchFunc)
    {
        this.inner = inner;
        this.patchFunc = patchFunc;
    }

    public IBicepConfigurationChain GetConfigurationChain(IOUri sourceFileUri)
        => new PatchingChain(inner.GetConfigurationChain(sourceFileUri), this);

    private sealed class PatchingChain(IBicepConfigurationChain inner, PatchingConfigurationManager manager) : IBicepConfigurationChain
    {
        public IBicepConfiguration GetEffectiveConfiguration() =>
            manager.patchedConfigurations.GetValue(inner.GetEffectiveConfiguration(), configuration => manager.patchFunc(configuration));
        public IEnumerable<KeyValuePair<IOUri, ImmutableArray<IDiagnostic>>> EnumerateDiagnosticsPerFile() => inner.EnumerateDiagnosticsPerFile();
        public int LayerCount => inner.LayerCount;
    }
}
