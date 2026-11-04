// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Artifacts.LegacyModuleAliasMocks;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts.Redirects;

/// <summary>
/// The ordered redirect rules applied by the compiler. The first rule that applies wins.
/// </summary>
public sealed class ArtifactRedirectTable
{
    private readonly ImmutableArray<ArtifactRedirectRule> rules;

    public ArtifactRedirectTable(IEnumerable<ArtifactRedirectRule> rules)
    {
        this.rules = [.. rules];
    }

    public static ArtifactRedirectTable Create(IFileExplorer fileExplorer, IBicepConfigurationManager configurationManager, IFeatureProviderFactory featureProviderFactory) =>
        new([new ModuleAliasMockRule(fileExplorer, configurationManager, featureProviderFactory)]);

    public ResultWithDiagnosticBuilder<ArtifactReference>? TryRedirect(IOUri referencingUri, ArtifactType artifactType, ArtifactAddress address)
    {
        foreach (var rule in this.rules)
        {
            if (rule.TryRedirect(referencingUri, artifactType, address) is { } result)
            {
                return result;
            }
        }

        return null;
    }
}
