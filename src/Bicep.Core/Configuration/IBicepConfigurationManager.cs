// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration;

/// <summary>
/// Manages loading and caching of Bicep configuration chains.
/// A chain is the ordered sequence of configuration files linked via "extends",
/// from the leaf (nearest bicepconfig.json) up to the built-in defaults.
/// </summary>
public interface IBicepConfigurationManager
{
    /// <summary>
    /// Returns the configuration chain for the given source file.
    /// Walks the "extends" chain starting from the nearest bicepconfig.json,
    /// detects cycles, enforces depth limits, and merges all layers.
    /// Falls back to the built-in configuration if no bicepconfig.json is found.
    /// </summary>
    IBicepConfigurationChain GetConfigurationChain(IOUri sourceFileUri);
}
