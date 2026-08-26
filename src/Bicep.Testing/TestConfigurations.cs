// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Configuration;

namespace Bicep.Testing;

public static class TestConfigurations
{
    public static IBicepConfiguration BuiltIn => TestConfigurationBuilder.Create().Build();

    public static IBicepConfiguration BuiltInWithAllAnalyzersDisabled => TestConfigurationBuilder
        .Create()
        .WithAllAnalyzersDisabled()
        .Build();

    public static IBicepConfiguration BuiltInWithStableAnalyzers => TestConfigurationBuilder
        .Create()
        .WithAllAnalyzers()
        .WithAnalyzersDisabled(
            UseRecentApiVersionRule.Code,
            UseRecentModuleVersionsRule.Code,
            NoHardcodedOutputsRule.Code,
            UseDescriptionParametersRule.Code,
            UseDescriptionVarsRule.Code,
            UseDescriptionOutputRule.Code,
            UseDescriptionTypeRule.Code,
            UseDescriptionTypePropertyRule.Code)
        .Build();
}
