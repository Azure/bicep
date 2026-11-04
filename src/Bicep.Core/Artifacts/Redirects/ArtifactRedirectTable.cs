// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Artifacts.LegacyModuleAliasMocks;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;
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

    public static ArtifactRedirectTable Create(IFileExplorer fileExplorer) => new([new ModuleAliasMockRule(fileExplorer)]);

    public ResultWithDiagnosticBuilder<ArtifactReference>? TryRedirect(BicepSourceFile referencingFile, ArtifactType artifactType, ArtifactAddress address)
    {
        foreach (var rule in this.rules)
        {
            if (rule.TryRedirect(referencingFile, artifactType, address) is { } result)
            {
                return result;
            }
        }

        return null;
    }
}
