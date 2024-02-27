// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Bicep.Core.UnitTests.Configuration
{
    // Tests the src/vscode-bicep/schemas/bicepconfig.schema.json file

    [TestClass]
    public class BicepConfigSchemaTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private const string BicepRootConfigFilePath = "src/Bicep.Core/Configuration/bicepconfig.json";
        private const string BicepConfigSchemaFilePath = "src/vscode-bicep/schemas/bicepconfig.schema.json";

        private const string HelpFileName = "experimental-features.md";

        // TODO: Remove these when they're fixed
        private readonly string[] GrandfatheredFeaturesNeedingHelpOrDescription = {
                "microsoftGraphPreview",
                "providerRegistry",
            };

        private static string GetBicepConfigSchemaContents()
        {
            var configStream = typeof(BicepConfigSchemaTests).Assembly.GetManifestResourceStream(
           $"{typeof(BicepConfigSchemaTests).Assembly.GetName().Name}.bicepconfig.schema.json");
            Assert.IsNotNull(configStream);
            var configContents = new StreamReader(configStream).ReadToEnd();
            Assert.IsNotNull(configContents);
            return configContents;
        }
        private static JSchema GetConfigSchema() => JSchema.Parse(GetBicepConfigSchemaContents());

        private (IBicepAnalyzerRule[] rules, JObject configSchema) GetRulesAndSchema()
        {
            var linter = new LinterAnalyzer();
            var ruleSet = linter.GetRuleSet();
            ruleSet.Should().NotBeEmpty();

            return (ruleSet.ToArray(), JObject.Parse(GetBicepConfigSchemaContents()));
        }

        private IEnumerable<JProperty> GetRuleCustomConfigurationProperties(JObject ruleConfigSchema)
        {
            var properties = ruleConfigSchema.SelectToken("allOf[0].properties")?.OfType<JProperty>();
            return properties ?? Enumerable.Empty<JProperty>();
        }

        private string GetExperimentalFeaturesHelpContents()
        {
            var helpStream = typeof(BicepConfigSchemaTests).Assembly.GetManifestResourceStream(
                               $"{typeof(BicepConfigSchemaTests).Assembly.GetName().Name}.Configuration.Links.{HelpFileName}");
            Assert.IsNotNull(helpStream);
            var helpContents = new StreamReader(helpStream).ReadToEnd();
            Assert.IsNotNull(helpContents);
            return helpContents;
        }

        private string[] GetExperimentalFeatureIdsFromHelpContents()
        {
            var helpContents = GetExperimentalFeaturesHelpContents();

            var experimentalFeaturesSection = new Regex("## List of feature(.*?)^## ", RegexOptions.Multiline | RegexOptions.Singleline).Match(helpContents).Groups[1].Value;
            experimentalFeaturesSection.Should().NotBeNullOrWhiteSpace();

            var featureIds = new Regex(@"^### `(?<fragment>[a-zA-Z]+)`", RegexOptions.Multiline).Matches(experimentalFeaturesSection!).Select(match => match.Groups["fragment"].Value).ToArray();
            return featureIds;
        }

        [TestMethod]
        public void Config_ShouldIncludeCurrentAllCoreRules()
        {
            var (rules, schema) = GetRulesAndSchema();
            var ruleConfigs = schema.SelectToken("properties.analyzers.properties.core.properties.rules.properties")!.ToObject<IDictionary<string, object>>();
            Assert.IsNotNull(ruleConfigs);
            ruleConfigs.Keys.Should().BeEquivalentTo(rules.Select(r => r.Code), "config schema should include definitions for core rules");
        }

        [TestMethod]
        public void RuleConfigs_ShouldBeCorrectlyDefined()
        {
            var (rules, schema) = GetRulesAndSchema();
            var ruleConfigs = schema.SelectToken("properties.analyzers.properties.core.properties.rules.properties")!.ToObject<IDictionary<string, JObject>>();
            Assert.IsNotNull(ruleConfigs);
            foreach (var (configKey, ruleConfig) in ruleConfigs)
            {
                // Example of minimum expected format for each rule definition
                /*
                    "no-unused-params": {
                        "allOf": [
                            {
                                "description": "No unused parameters. See https://aka.ms/bicep/linter/no-unused-params",
                                ...
                            },
                            {
                                "$ref": "#/definitions/rule-def-level-{warning,error,off}"
                            }
                        ]
                    },

                */

                var allOf = ruleConfig!.SelectToken("allOf");
                Assert.IsNotNull(allOf, "Each rule should have a top-level allOf");
                allOf.Count().Should().BeGreaterOrEqualTo(2);

                var description = allOf[0]?.SelectToken("description")?.ToString();
                Assert.IsNotNull(description);
                description.Should().EndWith($" See https://aka.ms/bicep/linter/{configKey}", "each rule's description should end with 'See <help-link>' using the link to the rule's docs");

                var matchingRule = rules.SingleOrDefault(r => r.Code == configKey);
                matchingRule.Should().NotBeNull("Rule's key in config does not match any linter rule's code");
                description.Should().Contain(matchingRule!.Description, "each rule's description should contain the same description as is specified in the rule");

                var lastAllOf = allOf[allOf.Count() - 1];
                var refString = lastAllOf?.SelectToken("$ref")?.ToString();
                Assert.IsNotNull(refString, "each rule's last allOf should be a ref to the definition of a rule");
                refString.Should().MatchRegex("^#/definitions/rule-def-level-(warning|error|off)$", "each rule's last allOf should be a ref to the definition of a rule, one of '#/definitions/rule-def-level-warning', '#/definitions/rule-def-level-error' or '#/definitions/rule-def-error-off'");
            }
        }

        [TestMethod]
        public void RuleConfigs_RuleNamesShouldBeConsistent()
        {
            var (rules, schema) = GetRulesAndSchema();
            var ruleConfigs = schema.SelectToken("properties.analyzers.properties.core.properties.rules.properties")!.ToObject<IDictionary<string, JObject>>();
            Assert.IsNotNull(ruleConfigs);
            foreach (var (key, rule) in ruleConfigs)
            {
                key.Should().BeInKebabCasing("all rule keys should be lower-cased with hyphens, and not start or end with hyphen");
                key.Should().HaveLengthLessThanOrEqualTo(36, "rule ids should have a reasonable length");
            }
        }

        [TestMethod]
        public void RuleConfigs_RuleCustomConfigurationPropertiesShouldBeConsistent()
        {
            var (rules, schema) = GetRulesAndSchema();
            var ruleConfigs = schema.SelectToken("properties.analyzers.properties.core.properties.rules.properties")!.ToObject<IDictionary<string, JObject>>();
            Assert.IsNotNull(ruleConfigs);
            foreach (var (key, rule) in ruleConfigs)
            {
                var rulePropertyConfigs = GetRuleCustomConfigurationProperties(rule);
                foreach (JProperty rulePropertyConfig in rulePropertyConfigs)
                {
                    string rulePropertyName = rulePropertyConfig.Name;
                    string rulePlusPropertyName = $"{key} -> {rulePropertyConfig.Name}";

                    rulePropertyConfig.Name.Should().BeInCamelCasing($"all rule custom configuration property names should be mixed case with no hyphens ({rulePlusPropertyName})");
                    rulePropertyConfig.Name.Should().HaveLengthLessThanOrEqualTo(25, $"all rule custom configuration property names should have a reasonable length {rulePlusPropertyName})");
                }
            }
        }

        [TestMethod]
        public void RuleConfigs_RuleDescriptionsShouldBeConsistent()
        {
            var (rules, schema) = GetRulesAndSchema();
            var ruleConfigs = schema.SelectToken("properties.analyzers.properties.core.properties.rules.properties")!.ToObject<IDictionary<string, JObject>>();
            Assert.IsNotNull(ruleConfigs);
            foreach (var (key, rule) in ruleConfigs)
            {
                string descriptionWithLink = rule.SelectToken("allOf[0].description")!.ToString();
                string description = new Regex("^(.+) See https://.+").Match(descriptionWithLink)?.Groups[1].Value ?? "<couldn't find rule description>";
                description.Should().MatchRegex("^[A-Z]", "all rule descriptions should start with a capital letter");
                description.Should().EndWith(".", "all rule descriptions should end with a period");
                description.Should().NotContainAny(new[] { "don't", "do not" }, StringComparison.InvariantCultureIgnoreCase, "Use \"Should\" type of language generally (less impolite)");
                description.Should().NotContain("\"", "use single quotes instead of double quotes in rule descriptions");
            }
        }

        [TestMethod]
        public void RuleConfigs_RulesShouldBeAlphabetizedForEasierMaintenance()
        {
            var (rules, schema) = GetRulesAndSchema();
            var ruleConfigs = schema.SelectToken("properties.analyzers.properties.core.properties.rules.properties")!.ToArray();
            var ruleNames = ruleConfigs.Select(c => ((JProperty)c).Name);
            var alphabetizedNames = ruleNames.OrderBy(n => n);
        }

        [TestMethod]
        public void NoHardCodedEnvUrls_DefaultsShouldMatchInConfigAndSchema()
        {
            // From schema
            var (rules, schema) = GetRulesAndSchema();
            IDictionary<string, JObject>? ruleConfigs = schema.SelectToken("properties.analyzers.properties.core.properties.rules.properties")!.ToObject<IDictionary<string, JObject>>();
            Assert.IsNotNull(ruleConfigs);
            string[] disallowedHostsInSchema = ruleConfigs["no-hardcoded-env-urls"].SelectToken("allOf[0].properties.disallowedhosts.default")!.Values().Select(v => v.ToString()).ToArray();
            string[] excludedHostsInSchema = ruleConfigs["no-hardcoded-env-urls"].SelectToken("allOf[0].properties.excludedhosts.default")!.Values().Select(v => v.ToString()).ToArray();

            // From config
            RootConfiguration builtinConfig = IConfigurationManager.GetBuiltInConfiguration();
            string[]? disallowedHostsInConfig = builtinConfig.Analyzers.GetValue<string[]?>("core.rules.no-hardcoded-env-urls.disallowedhosts", null);
            disallowedHostsInConfig.Should().NotBeNull();
            string[]? excludedHostsInConfig = builtinConfig.Analyzers.GetValue<string[]?>("core.rules.no-hardcoded-env-urls.excludedhosts", null);
            excludedHostsInConfig.Should().NotBeNull();

            disallowedHostsInSchema.Should().BeEquivalentTo(disallowedHostsInConfig, $"default of no-hardcoded-env-urls.disallowedHosts should be the same in {BicepRootConfigFilePath} and {BicepConfigSchemaFilePath}");
            disallowedHostsInSchema.Should().BeInAscendingOrder($"default of no-hardcoded-env-urls.disallowedHosts should be in alphabetical order in {BicepConfigSchemaFilePath}");
            disallowedHostsInConfig.Should().BeInAscendingOrder($"default of no-hardcoded-env-urls.disallowedHosts should be in alphabetical order in {BicepRootConfigFilePath}");

            excludedHostsInSchema.Should().BeEquivalentTo(excludedHostsInConfig, $"default of no-hardcoded-env-urls.excluded should be the same in {BicepRootConfigFilePath} and {BicepConfigSchemaFilePath}");
            excludedHostsInSchema.Should().BeInAscendingOrder($"default of no-hardcoded-env-urls.excluded should be in alphabetical order in {BicepConfigSchemaFilePath}");
            excludedHostsInConfig.Should().BeInAscendingOrder($"default of no-hardcoded-env-urls.excluded should be in alphabetical order in {BicepRootConfigFilePath}");
        }

        private ICollection<string> GetExperimentalFeatureIdsFromSchema()
        {
            IDictionary<string, JObject>? experimentalFeatures = GetExperimentalFeaturesFromSchema();
            return experimentalFeatures.Keys;
        }

        [TestMethod]
        public void ExperimentalFeatures_ShouldHaveDescription()
        {
            // From schema
            IDictionary<string, JObject>? experimentalFeatures = GetExperimentalFeaturesFromSchema();

            foreach (var (featureName, featureValue) in experimentalFeatures)
            {
                /* Example:

                    "publishSource": {
                        "type": "boolean",
                        "description": "Enables publishing source code with modules using the bicep publish `--with-source` option. See https://aka.ms/bicep/experimental-features#publishsource"
                    },

                */

                featureValue.SelectToken("type")!.ToString().Should().Be("boolean", $"all experimental feature values should be of type boolean: {featureName}");

                var descriptionProp = featureValue.SelectToken("description");

                if (descriptionProp is null)
                {
                    if (GrandfatheredFeaturesNeedingHelpOrDescription.Contains(featureName))
                    {
                        continue;
                    }
                    else
                    {
                        descriptionProp.Should().NotBeNull($"all experimental feature values should have a description: {featureName}");
                    }
                }
                else if (GrandfatheredFeaturesNeedingHelpOrDescription.Contains(featureName))
                {
                    GrandfatheredFeaturesNeedingHelpOrDescription.Should().NotContain(featureName, $"grandfathered experimental features should be removed from {nameof(GrandfatheredFeaturesNeedingHelpOrDescription)} when they have descriptions added");
                }

                descriptionProp.Should().NotBeNull($"all experimental feature values should have a description: {featureName}");

                string? description = featureValue.SelectToken("description")?.ToString();
                description.Should().NotBeNullOrWhiteSpace($"all experimental feature descriptions should not be empty: {featureName}");
                description.Should().MatchRegex($"^[A-Z]", "all feature descriptions should start with a capital letter: {featureName}");
                description.Should().NotContain($"\"", "use single quotes instead of double quotes in feature descriptions: {featureName}");

                description.Should().NotContainAny(new[] { "don't", "do not" }, "Use \"Should\" type of language generally (less impolite)");
            }
        }

        private IDictionary<string, JObject> GetExperimentalFeaturesFromSchema()
        {
            var (rules, schema) = GetRulesAndSchema();
            IDictionary<string, JObject>? experimentalFeatures = schema.SelectToken("properties.experimentalFeaturesEnabled.properties")!.ToObject<IDictionary<string, JObject>>();
            Assert.IsNotNull(experimentalFeatures);
            return experimentalFeatures;
        }

        [TestMethod]
        public void ExperimentalFeatures_DescriptionsShouldEndWithLinkToHelpPage()
        {
            // From schema
            IDictionary<string, JObject>? experimentalFeatures = GetExperimentalFeaturesFromSchema();

            foreach (var (featureName, featureValue) in experimentalFeatures)
            {
                /* Example:

                    "publishSource": {
                        "type": "boolean",
                        "description": "Enables publishing source code with modules using the bicep publish `--with-source` option. See https://aka.ms/bicep/experimental-features#publishsource"
                    },

                */

                var description = featureValue.SelectToken("description")?.ToString();
                if (description is { })
                {
                    string regex = ".* See https://aka.ms/bicep/experimental-features#([a-zA-Z]+)$";
                    description.Should().MatchRegex(regex, $"all experimental feature descriptions should end with a link to the feature's documentation (with a fragment): {featureName}");
                    var experimentalFeatureIdInLink = new Regex(regex).Match(description).Groups[1].Value;
                    experimentalFeatureIdInLink.Should().Be(featureName.ToLower(), $"Fragment in experimental feature description link should match the feature's id, but in lowercase: {featureName}");
                }
            }
        }

        [TestMethod]
        public void ExperimentalFeatures_ShouldBeDocumentedInHelpFile()
        {
            // From schema
            var experimentalFeaturesIdsFromSchema = GetExperimentalFeatureIdsFromSchema();

            // From help
            var experimentalFeatureIdsFromHelp = GetExperimentalFeatureIdsFromHelpContents().OrderBy(s => s).ToArray();

            GrandfatheredFeaturesNeedingHelpOrDescription.Should().NotContainAny(experimentalFeatureIdsFromHelp, $"grandfathered experimental features should be removed from {nameof(GrandfatheredFeaturesNeedingHelpOrDescription)} when they are documented in help");

            foreach (var featureId in experimentalFeaturesIdsFromSchema.Where(id => !GrandfatheredFeaturesNeedingHelpOrDescription.Contains(id)))
            {
                experimentalFeatureIdsFromHelp.Should().Contain(featureId, $"all experimental features in the schema should be documented in the help file {HelpFileName}");
            }
        }

        [TestMethod]
        public void ExperimentalFeatures_ShouldBeSortedInHelpFile()
        {
            var experimentalFeatureIdsFromHelp = GetExperimentalFeatureIdsFromHelpContents();

            experimentalFeatureIdsFromHelp.Should().BeInAscendingOrder($"experimental feature ids in the help file {HelpFileName} should be in alphabetical order");
        }

        [TestMethod]
        public void DefaultConfig_ShouldValidateAgainstSchema()
        {
            // Arrange
            var builtinConfig = IConfigurationManager.GetBuiltInConfiguration().ToUtf8Json();
            builtinConfig.Should().NotBeNull();
            var bicepConfigJson = JObject.Parse(builtinConfig!);
            var schema = GetConfigSchema();

            // Act & Assert
            bool isValid = bicepConfigJson.IsValid(schema, out IList<ValidationError> errors);
            errors.Should().BeEmpty();
            isValid.Should().BeTrue();
        }
    }
}
