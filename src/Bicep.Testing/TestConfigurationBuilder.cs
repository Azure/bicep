// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Testing.Extensions;
using Bicep.Core.Json;

namespace Bicep.Testing;

public sealed class TestConfigurationBuilder
{
    private IBicepConfiguration configuration;

    private TestConfigurationBuilder(IBicepConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public static TestConfigurationBuilder Create(IBicepConfiguration? configuration = null) =>
        new(configuration ?? BicepConfiguration.BuiltIn);

    public TestConfigurationBuilder WithAllAnalyzers()
    {
        foreach (var (code, ruleInfo) in new LinterRulesProvider().GetLinterRules())
        {
            if (ruleInfo.defaultDiagnosticLevel == DiagnosticLevel.Off)
            {
                this.WithAnalyzer(code, DiagnosticLevel.Warning);
            }
        }

        return this;
    }

    public TestConfigurationBuilder WithAllAnalyzersDisabled() =>
        this.WithAnalyzersConfiguration(AnalyzersConfiguration.Empty);

    public TestConfigurationBuilder WithAnalyzer(string analyzerCode, DiagnosticLevel level) =>
        this.WithAnalyzersConfiguration(((AnalyzersConfiguration)this.configuration.Analyzers).SetValue($"core.rules.{analyzerCode}.level", level.ToString().ToLowerInvariant()));

    public TestConfigurationBuilder WithAnalyzersDisabled(params string[] analyzerCodes)
    {
        foreach (var code in analyzerCodes)
        {
            this.WithAnalyzer(code, DiagnosticLevel.Off);
        }

        return this;
    }

    public TestConfigurationBuilder WithAnalyzersConfiguration(AnalyzersConfiguration analyzersConfiguration) =>
        this.With(analyzers: analyzersConfiguration);

    public TestConfigurationBuilder WithCloudConfiguration(CloudConfiguration cloudConfiguration) =>
        this.With(cloud: cloudConfiguration);

    public TestConfigurationBuilder WithExtensions(string extensionsJson) =>
        this.With(extensions: ExtensionsConfiguration.Bind(JsonElementFactory.CreateElement(extensionsJson)));

    public TestConfigurationBuilder WithImplicitExtensions(string implicitExtensionsJson) =>
        this.With(implicitExtensions: ImplicitExtensionsConfiguration.Bind(JsonElementFactory.CreateElement(implicitExtensionsJson)));

    public TestConfigurationBuilder WithExperimentalFeaturesEnabled(ExperimentalFeaturesEnabled experimentalFeaturesEnabled) =>
        this.With(experimentalFeaturesEnabled: experimentalFeaturesEnabled);

    public IBicepConfiguration Build() => this.configuration;

    private TestConfigurationBuilder With(
        CloudConfiguration? cloud = null,
        ExtensionsConfiguration? extensions = null,
        ImplicitExtensionsConfiguration? implicitExtensions = null,
        AnalyzersConfiguration? analyzers = null,
        ExperimentalFeaturesEnabled? experimentalFeaturesEnabled = null)
    {
        this.configuration = BicepConfigurationExtensions.With(
            this.configuration,
            cloud: cloud,
            extensions: extensions,
            implicitExtensions: implicitExtensions,
            analyzers: analyzers,
            experimentalFeaturesEnabled: experimentalFeaturesEnabled);

        return this;
    }
}
