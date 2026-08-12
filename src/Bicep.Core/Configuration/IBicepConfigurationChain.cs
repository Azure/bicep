// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Configuration;

/// <summary>
/// Represents a chain of Bicep configuration files linked via "extends".
/// The chain goes from the leaf (most specific) file up to the built-in defaults.
/// </summary>
public interface IBicepConfigurationChain
{
    /// <summary>
    /// Returns the effective configuration produced by merging all layers in the chain,
    /// with leaf settings winning over base settings.
    /// </summary>
    IBicepConfiguration GetEffectiveConfiguration();
}
