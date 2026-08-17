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

    /// <summary>
    /// Returns the fully merged <see cref="RootConfiguration"/> for the given source file.
    /// Equivalent to walking the "extends" chain and returning the effective merged result.
    /// This allows <see cref="IConfigurationManager"/> to delegate to this manager
    /// without changing the return type expected by existing consumers.
    /// </summary>
    RootConfiguration GetMergedConfiguration(IOUri sourceFileUri);

    /// <summary>
    /// Invalidates only the chains that include <paramref name="changedFileUri"/> as a dependency.
    /// </summary>
    void PurgeCacheForAffectedChains(IOUri changedFileUri);

    /// <summary>
    /// Purges the chain cache. Call this when any config file in the workspace changes
    /// so stale chains are not reused.
    /// </summary>
    void PurgeChainCache();
}
