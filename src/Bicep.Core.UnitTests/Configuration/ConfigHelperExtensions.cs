// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.UnitTests.Configuration
{
    public static class ConfigHelperExtensions
    {
        public static ConfigHelper GetDisabledLinterConfig(this ConfigHelper configHelper) =>
            configHelper.OverrideSetting(LinterAnalyzer.LinterEnabledSetting, false)
                        .OverrideSetting(LinterAnalyzer.LinterVerboseSetting, false);
    }
}
