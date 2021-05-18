// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter;

namespace Bicep.Core.Configuration
{
    internal static class ConfigHelperExtensions
    {
        public static ConfigHelper GetDisabledLinterCofig(this ConfigHelper configHelper) =>
            configHelper.OverrideSetting(LinterAnalyzer.LinterEnabledSetting, false)
                        .OverrideSetting(LinterAnalyzer.LinterVerboseSetting, false);

    }
}
