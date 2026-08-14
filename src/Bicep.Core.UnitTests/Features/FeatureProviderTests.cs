// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.UnitTests.Assertions;
using Bicep.IO.FileSystem;
using Bicep.Testing;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Features;

[TestClass]
public class FeatureProviderTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void PropertyLookup_WithNothingConfigured_ReturnsDefault()
    {
        var fileSet = InMemoryTestFileSet.Create(("repo/bicepconfig.json", """
            {
              "experimentalFeaturesEnabled": {}
            }
            """));

        var configManager = new ConfigurationManager(fileSet.FileExplorer);
        var configuration = configManager.GetConfiguration(fileSet.GetUri("repo/main.bicep"));
        var fpm = new FeatureProviderFactory(configManager, fileSet.FileExplorer);

        var control = fpm.GetFeatureProvider(fileSet.GetUri("main.bicep"));
        var sut = fpm.GetFeatureProvider(fileSet.GetUri("repo/main.bicep"));
        sut.SymbolicNameCodegenEnabled.Should().Be(control.SymbolicNameCodegenEnabled);
    }

    [TestMethod]
    public void PropertyLookup_WithFeatureEnabledViaBicepConfig_ReturnsTrue()
    {
        var fileSet = InMemoryTestFileSet.Create(
            ("repo/bicepconfig.json", """
                {
                  "experimentalFeaturesEnabled": {}
                }
                """),
            ("repo/subdir/bicepconfig.json", """
                {
                  "experimentalFeaturesEnabled": {
                    "symbolicNameCodegen": true
                  }
                }
                """));

        var configManager = new ConfigurationManager(fileSet.FileExplorer);
        var configuration = configManager.GetConfiguration(fileSet.GetUri("repo/main.bicep"));
        var fpm = new FeatureProviderFactory(configManager, fileSet.FileExplorer);

        var control = fpm.GetFeatureProvider(fileSet.GetUri("main.bicep"));
        control.SymbolicNameCodegenEnabled.Should().BeFalse();
        var mainDirFeatures = fpm.GetFeatureProvider(fileSet.GetUri("repo/main.bicep"));
        mainDirFeatures.SymbolicNameCodegenEnabled.Should().BeFalse();
        var subDirFeatures = fpm.GetFeatureProvider(fileSet.GetUri("repo/subdir/module.bicep"));
        subDirFeatures.SymbolicNameCodegenEnabled.Should().BeTrue();
    }

    [TestMethod]
    public void DocsGeneration_feature_is_exposed_by_feature_providers()
    {
        var enabled = ExperimentalFeaturesEnabled.AllDisabled with { DocsGeneration = true };
        var recordProvider = new RecordBasedFeatureProvider(enabled);

        recordProvider.DocsGenerationEnabled.Should().BeTrue();

        var overridden = new OverriddenFeatureProvider(
            recordProvider,
            new(DocsGenerationEnabled: false));
        overridden.DocsGenerationEnabled.Should().BeFalse();

        var assemblyVersionFactory = TestFeatureProviderFactory.WithAssemblyVersion(
            IFeatureProviderFactory.WithStaticFeatureProvider(recordProvider),
            "test");
        var sourceFileUri = InMemoryTestFileSet.Create(("main.bicep", "")).GetUri("main.bicep");
        assemblyVersionFactory.GetFeatureProvider(sourceFileUri)
            .DocsGenerationEnabled.Should().BeTrue();

        IFeatureProvider legacyProvider = new LegacyFeatureProvider();
        legacyProvider.DocsGenerationEnabled.Should().BeFalse();

        var legacyConfiguration = new ExperimentalFeaturesEnabled(
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false);
        legacyConfiguration.DocsGeneration.Should().BeFalse();
    }

    private sealed class LegacyFeatureProvider : IFeatureProvider
    {
        public string AssemblyVersion => throw new NotImplementedException();
        public Bicep.IO.Abstraction.IDirectoryHandle CacheRootDirectory => throw new NotImplementedException();
        public bool OciEnabled => false;
        public bool SymbolicNameCodegenEnabled => false;
        public bool ResourceTypedParamsAndOutputsEnabled => false;
        public bool SourceMappingEnabled => false;
        public bool LegacyFormatterEnabled => false;
        public bool TestFrameworkEnabled => false;
        public bool AssertsEnabled => false;
        public bool WaitUntilEnabled => false;
        public bool LocalDeployEnabled => false;
        public bool ResourceInfoCodegenEnabled => false;
        public bool ModuleExtensionConfigsEnabled => false;
        public bool UserDefinedConstraintsEnabled => false;
        public bool DeployCommandsEnabled => false;
        public bool PatchEnabled => false;
        public bool RuntimeValuesInTagsAndSkuEnabled => false;
        public bool AzExtensionConfigEnabled => false;
    }
}
