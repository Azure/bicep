// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;
using Bicep.Core.Workspaces;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Cli.Helpers;

public static class ExperimentalFeatureWarningProvider
{
    public static string? TryGetEnabledExperimentalFeatureWarningMessage(SourceFileGrouping sourceFileGrouping, IFeatureProviderFactory featureProviderFactory)
    {
        var experimentalFeaturesEnabled = sourceFileGrouping.SourceFiles
            .Select(file => featureProviderFactory.GetFeatureProvider(file.FileUri))
            .SelectMany(static features => features.EnabledFeatureMetadata.Where(f => f.impactsCompilation).Select(f => f.name))
            .Distinct()
            .ToImmutableArray();

        return experimentalFeaturesEnabled.Any()
            ? string.Format(CliResources.ExperimentalFeaturesDisclaimerMessage, string.Join(", ", experimentalFeaturesEnabled))
            : null;
    }
}
