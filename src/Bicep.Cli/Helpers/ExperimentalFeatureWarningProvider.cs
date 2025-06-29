// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Features;
using Bicep.Core.SourceGraph;

namespace Bicep.Cli.Helpers;

public static class ExperimentalFeatureWarningProvider
{
    public static string? TryGetEnabledExperimentalFeatureWarningMessage(IEnumerable<ISourceFile> sourceFiles)
    {
        var experimentalFeaturesEnabled = sourceFiles
            .OfType<BicepSourceFile>()
            .Select(file => file.Features)
            .SelectMany(static features => features.EnabledFeatureMetadata.Where(f => f.impactsCompilation).Select(f => f.name))
            .Distinct()
            .ToImmutableArray();

        return experimentalFeaturesEnabled.Any()
            ? string.Format(CliResources.ExperimentalFeaturesDisclaimerMessage, string.Join(", ", experimentalFeaturesEnabled))
            : null;
    }
}
