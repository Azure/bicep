// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
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

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        public class RuleAndSchemaTestDataAttribute : Attribute, ITestDataSource
        {
            public IEnumerable<object[]> GetData(MethodInfo methodInfo)
            {
                var analyzer = new LinterAnalyzer(BicepTestConstants.EmptyServiceProvider);
                var ruleSet = analyzer.GetRuleSet().ToArray();

                return AllRulesAndSchemasById.Values.Select(value => new object[] { value.Rule.Code, value.Rule, value.Schema });
            }

            public string? GetDisplayName(MethodInfo methodInfo, object?[]? data)
            {
                var id = (data?[0] as string)!;

                return $"{methodInfo.Name} ({id})";
            }
        }

        private const string BicepRootConfigFilePath = "src/Bicep.Core/Configuration/bicepconfig.json";
        private const string BicepConfigSchemaFilePath = "src/vscode-bicep/schemas/bicepconfig.schema.json";

        private const string HelpFileName = "experimental-features.md";

        // TODO: Remove these when they're fixed
        private readonly string[] GrandfatheredFeaturesNeedingHelpOrDescription = [
        ];

        private static string GetBicepConfigSchemaContents()
        {
            var configStream = typeof(BicepConfigSchemaTests).Assembly.GetManifestResourceStream(
                $"{typeof(BicepConfigSchemaTests).Assembly.GetName().Name}.bicepconfig.schema.json");
            Assert.IsNotNull(configStream);
            var configContents = new StreamReader(configStream).ReadToEnd();
            Assert.IsNotNull(configContents);
            return configContents;
        }

        private static JObject BicepConfigSchema => JObject.Parse(GetBicepConfigSchemaContents());

        private static JSchema BicepConfigSchemaAsJSchema => JSchema.Parse(GetBicepConfigSchemaContents());

        private static ImmutableArray<IBicepAnalyzerRule> AllRules
        {
            get
            {
                var linter = new LinterAnalyzer(BicepTestConstants.EmptyServiceProvider);
                var ruleSet = linter.GetRuleSet();
                ruleSet.Should().NotBeEmpty();

                return ruleSet.ToImmutableArray();
            }
        }

        private static (string Id, JObject Schema)[] AllRuleSchemas =>
            (BicepConfigSchema.SelectToken("properties.analyzers.properties.core.properties.rules.properties") as JObject)!
            .Children<JProperty>()
            .Select(prop => (prop.Name, (JObject)prop.Value))
            .ToArray();

        private static IImmutableDictionary<string, JObject> AllRuleSchemasById =>
            BicepConfigSchema.SelectToken("properties.analyzers.properties.core.properties.rules.properties")!.ToObject<IDictionary<string, JObject>>()!
            .ToImmutableDictionary();

        private static IImmutableDictionary<string, (IBicepAnalyzerRule Rule, JObject Schema)> AllRulesAndSchemasById =>
                AllRules
                    .Join(AllRuleSchemas,
                          rule => rule.Code,
                          ruleSchema => ruleSchema.Id,
                          (rule, ruleSchema) => new { Id = rule.Code, Rule = rule, ruleSchema.Schema })
                    .ToImmutableDictionary(rs => rs.Id, rs => (rs.Rule, rs.Schema));

        private static IEnumerable<JProperty> GetRuleCustomConfigurationProperties(JObject ruleConfigSchema)
        {
            var properties = ruleConfigSchema.SelectToken("allOf[0].properties")?.OfType<JProperty>();
            return properties ?? [];
        }

        private IImmutableDictionary<string, JObject> GetExperimentalFeaturesFromSchema()
        {
            IDictionary<string, JObject>? experimentalFeatures = BicepConfigSchema.SelectToken("properties.experimentalFeaturesEnabled.properties")!.ToObject<IDictionary<string, JObject>>();
            Assert.IsNotNull(experimentalFeatures);
            return experimentalFeatures.ToImmutableDictionary();
        }

        private IEnumerable<string> GetExperimentalFeatureIdsFromSchema()
        {
            IImmutableDictionary<string, JObject>? experimentalFeatures = GetExperimentalFeaturesFromSchema();
            return experimentalFeatures.Keys;
        }

        private string? GetRuleDefaultLevel(JObject ruleConfigSchema)
        {
            return ruleConfigSchema.SelectToken("allOf[1].$ref")?.Value<string>() switch
            {
                "#/definitions/rule-def-level-off" => "Off",
                "#/definitions/rule-def-level-warning" => "Warning",
                "#/definitions/rule-def-level-error" => "Error",
                "#/definitions/rule-def-level-info" => "Info",
                _ => throw new Exception("Unexpected value for #/definitions/rule-def-level-xxx for rule configuration schema")
            };
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
        public void SanityCheck_Rules()
        {
            AllRules.Should().NotBeEmpty();
        }

        [TestMethod]
        public void SanityCheck_Schemas()
        {
            BicepConfigSchema.Children().Should().NotBeEmpty();
        }

        [TestMethod]
        public void SanityCheck_RuleSchemas()
        {
            AllRuleSchemas.Should().NotBeEmpty();
        }

        [TestMethod]
        public void SanityCheck_AllRulesAndSchemaById()
        {
            AllRulesAndSchemasById.Should().NotBeEmpty();

            AllRules.Length.Should().Be(AllRuleSchemas.Length, "the count of rules and rule configuration schemas should match");

            AllRulesAndSchemasById.Count.Should().Be(AllRules.Length, "the Code of rules and should match the Id of each corresponding rule configuration schema");
        }

        [TestMethod]
        public void Config_EachRuleShouldHaveOneSchemaAndViceVersa()
        {
            AllRuleSchemas.Select(s => s.Id).Should().BeEquivalentTo(AllRules.Select(r => r.Code), "each core linter rule should have one corresponding configuration schema entry and vice versa");
        }

        [TestMethod]
        public void RuleConfigs_ShouldBeCorrectlyDefined()
        {
            foreach (var (configKey, ruleConfig) in AllRuleSchemas)
            {
                // Example of minimum expected format for each rule definition
                /*
                    "no-unused-params": {
                        "allOf": [
                            {
                                "description": "No unused parameters. See https://aka.ms/bicep/linter-diagnostics#no-unused-params",
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
                description.Should().EndWith($" See https://aka.ms/bicep/linter-diagnostics#{configKey}", "each rule's description should end with 'See <help-link>' using the link to the rule's docs");

                var matchingRule = AllRules.SingleOrDefault(r => r.Code == configKey);
                matchingRule.Should().NotBeNull("Rule's key in config does not match any linter rule's code");
                description.Should().Contain(matchingRule!.Description, "each rule's description should contain the same description as is specified in the rule");

                var lastAllOf = allOf[allOf.Count() - 1];
                var refString = lastAllOf?.SelectToken("$ref")?.ToString();
                Assert.IsNotNull(refString, "each rule's last allOf should be a ref to the definition of a rule");
                refString.Should().MatchRegex("^#/definitions/rule-def-level-(warning|error|info|off)$", "each rule's last allOf should be a ref to the definition of a rule, one of '#/definitions/rule-def-level-warning', '#/definitions/rule-def-level-error', ''#/definitions/rule-def-level-info', or '#/definitions/rule-def-error-off'");
            }
        }

        [TestMethod]
        public void RuleConfigs_RuleNamesShouldBeConsistent()
        {
            foreach (var rule in AllRules)
            {
                rule.Code.Should().BeInKebabCasing("all rule ids should be lower-cased with hyphens, and not start or end with hyphen");
                rule.Code.Should().HaveLengthLessThanOrEqualTo(36, "rule ids should have a reasonable length");
            }
        }

        [TestMethod]
        public void RuleConfigs_RuleCustomConfigurationPropertiesShouldBeConsistent()
        {
            foreach (var (ruleId, ruleSchema) in AllRuleSchemas)
            {
                var customRuleConfigProps = GetRuleCustomConfigurationProperties(ruleSchema);
                foreach (JProperty rulePropertyConfig in customRuleConfigProps)
                {
                    string rulePlusPropertyName = $"{ruleId} -> {rulePropertyConfig.Name}";

                    rulePropertyConfig.Name.Should().BeInCamelCasing($"all rule custom configuration property names should be mixed case with no hyphens ({rulePlusPropertyName})");
                    rulePropertyConfig.Name.Should().HaveLengthLessThanOrEqualTo(25, $"all rule custom configuration property names should have a reasonable length {rulePlusPropertyName})");
                }
            }
        }

        [TestMethod]
        public void RuleConfigs_RuleDescriptionsShouldBeConsistent()
        {
            foreach (var (_, ruleSchema) in AllRuleSchemas)
            {
                string descriptionWithLink = ruleSchema.SelectToken("allOf[0].description")!.ToString();
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
            var alphabetizedRuleIds = AllRuleSchemas.Select(r => r.Id).OrderBy(n => n);
            alphabetizedRuleIds.Should().BeInAscendingOrder();
        }

        [TestMethod]
        public void NoHardCodedEnvUrls_DefaultsShouldMatchInConfigAndSchema()
        {
            // From schema
            var ruleSchemas = AllRuleSchemasById;
            string[] disallowedHostsInSchema = ruleSchemas["no-hardcoded-env-urls"].SelectToken("allOf[0].properties.disallowedhosts.default")!.Values().Select(v => v.ToString()).ToArray();
            string[] excludedHostsInSchema = ruleSchemas["no-hardcoded-env-urls"].SelectToken("allOf[0].properties.excludedhosts.default")!.Values().Select(v => v.ToString()).ToArray();

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

        [TestMethod]
        public void ExperimentalFeatures_ShouldHaveDescription()
        {
            // From schema
            IImmutableDictionary<string, JObject>? experimentalFeatures = GetExperimentalFeaturesFromSchema();

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

        [TestMethod]
        public void ExperimentalFeatures_DescriptionsShouldEndWithLinkToHelpPage()
        {
            // From schema
            IImmutableDictionary<string, JObject>? experimentalFeatures = GetExperimentalFeaturesFromSchema();

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
            var experimentalFeaturesIdsFromSchema = GetExperimentalFeatureIdsFromSchema().ToArray();

            // From help
            var experimentalFeatureIdsFromHelp = GetExperimentalFeatureIdsFromHelpContents().OrderBy(s => s).ToArray();

            GrandfatheredFeaturesNeedingHelpOrDescription.Should().NotContainAny(experimentalFeatureIdsFromHelp, $"grandfathered experimental features should be removed from {nameof(GrandfatheredFeaturesNeedingHelpOrDescription)} when they are documented in help");

            foreach (var featureId in experimentalFeaturesIdsFromSchema.Where(id => !GrandfatheredFeaturesNeedingHelpOrDescription.Contains(id)))
            {
                experimentalFeatureIdsFromHelp.Should().Contain(featureId, $"all experimental features in the schema should be documented in the help file {HelpFileName}");
            }

            foreach (var featureId in experimentalFeatureIdsFromHelp.Where(id => !GrandfatheredFeaturesNeedingHelpOrDescription.Contains(id)))
            {
                experimentalFeaturesIdsFromSchema.Should().Contain(featureId, $"all experimental features documented in the help file {HelpFileName} should be in the bicepconfig.schema.json file");
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

            // Act & Assert
            bool isValid = bicepConfigJson.IsValid(BicepConfigSchemaAsJSchema, out IList<ValidationError> errors);
            errors.Should().BeEmpty();
            isValid.Should().BeTrue();
        }

        [TestMethod]
        public void UserConfig_SysProviderIsProhibited()
        {
            var bicepConfigJson = JObject.Parse("""
            {
                "extensions": {
                    "sys": "example.azurecr.io/some/fake/path:1.0.0"
                }
            }
            """);

            bool isValid = bicepConfigJson.IsValid(BicepConfigSchemaAsJSchema, out IList<ValidationError> errors);
            errors.Should().HaveCount(1);
            errors.Single().Path.Should().Be("extensions.sys");
            isValid.Should().BeFalse();
        }

        [TestMethod]
        [RuleAndSchemaTestData]
        public void RuleConfigs_DefaultLevelShouldMatchRuleDefinition(string id, IBicepAnalyzerRule rule, JObject ruleSchema)
        {
            var defaultLevelInRuleDefinition = rule.DefaultDiagnosticLevel.ToString();
            var defaultLevelInSchema = GetRuleDefaultLevel(ruleSchema);

            defaultLevelInSchema.Should().Be(defaultLevelInRuleDefinition,
                $"the default diagnostic level of a rule's config schema should match that defined in the rule's class definition (make sure rule {id}'s #/definitions/rule-def-level-xxx reference is correct)");
        }

        [TestMethod]
        public void RuleConfigs_RuleDescriptionShouldIndicateDefaultDiagnosticLevel()
        {
            foreach (var (id, (rule, ruleSchema)) in AllRulesAndSchemasById)
            {
                var defaultLevel = rule.DefaultDiagnosticLevel.ToString();
                var description = ruleSchema.SelectToken("allOf")?[0]?.SelectToken("description")?.ToString();

                description.Should().MatchRegex($"\\. Defaults to '{defaultLevel}'\\. See https:",
                    "rule description should indicate its default diagnostic level");
            }
        }
    }
}
