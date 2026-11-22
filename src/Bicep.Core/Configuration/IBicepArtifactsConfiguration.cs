// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;

namespace Bicep.Core.Configuration;

/// <summary>
/// The <c>artifacts</c> configuration section.
/// </summary>
public interface IBicepArtifactsConfiguration
{
    /// <summary>
    /// True if a configuration file sets the <c>artifacts</c> property.
    /// </summary>
    bool IsDeclared { get; }

    /// <summary>
    /// Returns the artifact alias named <paramref name="aliasName"/>: an entry of <c>artifacts.aliases</c>, or a predefined alias.
    /// Fails if the alias doesn't exist or its definition is invalid.
    /// </summary>
    ResultWithDiagnosticBuilder<ArtifactAlias> TryGetAlias(string aliasName);
}
