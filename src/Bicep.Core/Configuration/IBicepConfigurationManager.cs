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
    /// The leaf configuration is the explicit configuration of the current operation
    /// (see <see cref="ExplicitConfigurationContext"/>) if one is set; otherwise it is the nearest bicepconfig.json.
    /// Walks the "extends" chain starting from the leaf configuration,
    /// detects cycles, enforces depth limits, and merges all layers.
    /// Falls back to the built-in configuration if no bicepconfig.json is found.
    /// Returns the same chain until a file it was loaded from changes, checked as described on <see cref="FileChangeEpoch"/>.
    /// </summary>
    IBicepConfigurationChain GetConfigurationChain(IOUri sourceFileUri);
}
