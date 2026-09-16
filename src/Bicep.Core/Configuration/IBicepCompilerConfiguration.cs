// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.SemanticVersioning;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration;

public interface IBicepCompilerConfiguration
{
    /// <summary>
    /// Gets the accepted Bicep compiler version range, or null if no constraint was specified.
    /// </summary>
    VersionRange? Version { get; }

    /// <summary>
    /// The URI of the configuration file that actually declared "bicep.version" (the layer nearest the leaf
    /// that sets it, walking the "extends" chain), or null if no layer declares it. Used to attribute
    /// version-constraint diagnostics to the exact file a user needs to edit, rather than always the leaf.
    /// </summary>
    IOUri? DeclaringConfigUri { get; }
}
