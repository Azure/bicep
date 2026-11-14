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
    /// </summary>
    IBicepConfigurationChain GetConfigurationChain(IOUri sourceFileUri);

    /// <summary>
    /// Drops cached state affected by a change to <paramref name="fileUri"/> on disk (created, changed or deleted).
    /// The URI may also be a folder, which covers everything under it, because clients may report only the folder when one is deleted.
    /// Every cached chain that read an affected file is invalidated, including chains that failed to load because of it.
    /// If the file is a bicepconfig.json, configuration file discovery is also reset, because creating or deleting one
    /// can change which configuration file a source file uses.
    /// </summary>
    /// <returns>True if any cached state was dropped, meaning configurations returned before the call may be stale.</returns>
    bool InvalidateFile(IOUri fileUri);

    /// <summary>
    /// Drops all cached configuration state.
    /// </summary>
    void InvalidateAll();
}
