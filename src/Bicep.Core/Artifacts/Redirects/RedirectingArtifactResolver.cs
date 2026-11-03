// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts.Redirects;

/// <summary>
/// Resolves what the compiler loads for an artifact reference: redirect rules are applied first, then the text is
/// resolved normally. Only the compiler uses this; publish targets and stored references are never redirected.
/// </summary>
public sealed class RedirectingArtifactResolver
{
    private readonly IArtifactReferenceResolver resolver;
    private readonly ArtifactRedirectTable redirectTable;

    public RedirectingArtifactResolver(IArtifactReferenceResolver resolver, ArtifactRedirectTable redirectTable)
    {
        this.resolver = resolver;
        this.redirectTable = redirectTable;
    }

    public RedirectingArtifactResolver(IArtifactReferenceResolver resolver, IFileExplorer fileExplorer)
        : this(resolver, ArtifactRedirectTable.Create(fileExplorer))
    {
    }

    public ResultWithDiagnosticBuilder<ArtifactReference> TryResolve(BicepSourceFile referencingFile, ArtifactType artifactType, string referenceText) =>
        this.redirectTable.TryRedirect(referencingFile, artifactType, ArtifactAddress.Parse(referenceText)) ??
        this.resolver.TryResolve(referencingFile, artifactType, referenceText);
}
