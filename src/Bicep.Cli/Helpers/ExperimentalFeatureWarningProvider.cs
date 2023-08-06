// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Features;
using Bicep.Core.Workspaces;

namespace Bicep.Cli.Helpers;

public static class ExperimentalFeatureWarningProvider
{
    public static IEnumerable<string> GetEnabledExperimentalFeatureWarningMessages(SourceFileGrouping sourceFileGrouping, IFeatureProviderFactory featureProviderFactory)
        => sourceFileGrouping.SourceFiles
            .Select(file => featureProviderFactory.GetFeatureProvider(file.FileUri))
            .SelectMany(GetEnabledExperimentalFeatureWarningMessages)
            .Distinct();

    private static IEnumerable<string> GetEnabledExperimentalFeatureWarningMessages(IFeatureProvider featureProvider)
    {
        foreach (var (enabled, message) in new[]
        {
            (featureProvider.ExtensibilityEnabled, CliResources.ExtensibilityDisclaimerMessage),
            (featureProvider.ResourceTypedParamsAndOutputsEnabled, CliResources.ResourceTypesDisclaimerMessage),
            (featureProvider.SourceMappingEnabled, CliResources.SourceMappingDisclaimerMessage),
            (featureProvider.UserDefinedFunctionsEnabled, CliResources.UserDefinedFunctionsDisclaimerMessage),
            (featureProvider.DynamicTypeLoadingEnabled, CliResources.DynamicTypeLoadingDisclaimerMessage),
            (featureProvider.AssertsEnabled, CliResources.AssertsDisclaimerMessage),
        })
        {
            if (enabled)
            {
                yield return message;
            }
        }
    }
}
