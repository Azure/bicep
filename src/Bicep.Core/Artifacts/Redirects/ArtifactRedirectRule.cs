// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;

namespace Bicep.Core.Artifacts.Redirects;

/// <summary>
/// A rule that changes what the compiler loads for an artifact reference.
/// </summary>
public abstract class ArtifactRedirectRule
{
    /// <summary>
    /// Returns <c>null</c> if the rule does not apply to <paramref name="address"/>; otherwise the reference to load instead, or an error.
    /// </summary>
    public abstract ResultWithDiagnosticBuilder<ArtifactReference>? TryRedirect(BicepSourceFile referencingFile, ArtifactType artifactType, ArtifactAddress address);
}
