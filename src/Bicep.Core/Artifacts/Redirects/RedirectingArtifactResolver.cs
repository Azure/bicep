// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Registry;
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

    public static RedirectingArtifactResolver Create(
        IArtifactReferenceResolver resolver,
        IFileExplorer fileExplorer,
        IBicepConfigurationManager configurationManager,
        IFeatureProviderFactory featureProviderFactory) =>
        new(resolver, ArtifactRedirectTable.Create(fileExplorer, configurationManager, featureProviderFactory));

    public ResultWithDiagnosticBuilder<ArtifactReference> TryResolve(IOUri referencingUri, ArtifactType artifactType, string referenceText) =>
        this.redirectTable.TryRedirect(referencingUri, artifactType, ArtifactAddress.Parse(referenceText)) ??
        this.resolver.TryResolve(referencingUri, artifactType, referenceText);
}
