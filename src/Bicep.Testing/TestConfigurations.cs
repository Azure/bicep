// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Configuration;

namespace Bicep.Testing;

public static class TestConfigurations
{
    public static RootConfiguration BuiltIn => TestConfigurationBuilder.Create().Build();

    public static RootConfiguration BuiltInWithAllAnalyzersDisabled => TestConfigurationBuilder
        .Create()
        .WithAllAnalyzersDisabled()
        .Build();

    public static RootConfiguration BuiltInWithStableAnalyzers => TestConfigurationBuilder
        .Create()
        .WithAllAnalyzers()
        .WithAnalyzersDisabled(
            UseRecentApiVersionRule.Code,
            UseRecentModuleVersionsRule.Code,
            NoHardcodedOutputsRule.Code)
        .Build();
}
