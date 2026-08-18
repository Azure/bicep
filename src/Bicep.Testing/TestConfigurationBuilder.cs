// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Json;

namespace Bicep.Testing;

public sealed class TestConfigurationBuilder
{
    private RootConfiguration configuration;

    private TestConfigurationBuilder(RootConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public static TestConfigurationBuilder Create(RootConfiguration? configuration = null) =>
        new(configuration ?? IConfigurationManager.GetBuiltInConfiguration());

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
        this.WithAnalyzersConfiguration(this.configuration.Analyzers.SetValue($"core.rules.{analyzerCode}.level", level.ToString().ToLowerInvariant()));

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

    public RootConfiguration Build() => this.configuration;

    private TestConfigurationBuilder With(
        CloudConfiguration? cloud = null,
        ExtensionsConfiguration? extensions = null,
        ImplicitExtensionsConfiguration? implicitExtensions = null,
        AnalyzersConfiguration? analyzers = null,
        ExperimentalFeaturesEnabled? experimentalFeaturesEnabled = null)
    {
        this.configuration = new(
            cloud ?? this.configuration.Cloud,
            this.configuration.ModuleAliases,
            this.configuration.ModuleAliasesMock,
            extensions ?? this.configuration.Extensions,
            implicitExtensions ?? this.configuration.ImplicitExtensions,
            analyzers ?? this.configuration.Analyzers,
            this.configuration.CacheRootDirectory,
            this.configuration.ExperimentalFeaturesWarning,
            experimentalFeaturesEnabled ?? this.configuration.ExperimentalFeaturesEnabled,
            this.configuration.Formatting,
            this.configuration.Documentation,
            this.configuration.ConfigFileUri,
            this.configuration.Diagnostics);

        return this;
    }
}
