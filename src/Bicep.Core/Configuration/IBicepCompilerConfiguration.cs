// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.SemanticVersioning;

namespace Bicep.Core.Configuration;

public interface IBicepCompilerConfiguration
{
    /// <summary>
    /// Gets the accepted Bicep compiler version range, or null if no constraint was specified.
    /// </summary>
    VersionRange? Version { get; }
}
