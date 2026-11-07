// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Artifacts.LegacyModuleAliasMocks;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Artifacts.Resolution;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts.Redirects;

/// <summary>
/// Resolves what the compiler loads for an artifact reference: redirect rules are applied first, then the text is
/// resolved normally. Only the compiler uses this; publish targets and stored references are never redirected.
/// </summary>
public sealed class RedirectingArtifactResolver
{
    private readonly IArtifactReferenceResolver resolver;
    private readonly ImmutableArray<ArtifactRedirectRule> rules;

    public RedirectingArtifactResolver(IArtifactReferenceResolver resolver, IEnumerable<ArtifactRedirectRule> rules)
    {
        this.resolver = resolver;
        this.rules = [.. rules];
    }

    public static RedirectingArtifactResolver Create(
        IArtifactReferenceResolver resolver,
        IFileExplorer fileExplorer,
        IBicepConfigurationManager configurationManager,
        IFeatureProviderFactory featureProviderFactory) =>
        new(resolver, [new ModuleAliasMockRule(fileExplorer, configurationManager, featureProviderFactory)]);

    public ResultWithDiagnosticBuilder<ArtifactReference> TryResolve(IOUri referencingUri, ArtifactType artifactType, string referenceText)
    {
        var address = ArtifactAddress.Parse(referenceText);
        foreach (var rule in this.rules)
        {
            if (rule.TryRedirect(referencingUri, artifactType, address) is { } result)
            {
                return result;
            }
        }

        return this.resolver.TryResolve(referencingUri, artifactType, referenceText);
    }
}
