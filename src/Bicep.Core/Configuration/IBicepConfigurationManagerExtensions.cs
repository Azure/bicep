// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration;

public static class IBicepConfigurationManagerExtensions
{
    /// <summary>
    /// Returns the effective <see cref="IBicepConfiguration"/> for the given source file.
    /// Shorthand for <see cref="IBicepConfigurationManager.GetConfigurationChain"/> followed by
    /// <see cref="IBicepConfigurationChain.GetEffectiveConfiguration"/>.
    /// </summary>
    public static IBicepConfiguration GetEffectiveConfiguration(this IBicepConfigurationManager manager, IOUri sourceFileUri)
        => manager.GetConfigurationChain(sourceFileUri).GetEffectiveConfiguration();
}
