// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.Core.UnitTests.Diagnostics
{
    // Tests the bicepconfig.schema.json file

    [TestClass]
    public class BicepConfigSchemaTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private (IBicepAnalyzerRule[] rules, JObject configSchema) GetRulesAndSchema()
        {
            var linter = new LinterAnalyzer();
            var ruleSet = linter.GetRuleSet();
            ruleSet.Should().NotBeEmpty();

            var configStream = typeof(BicepConfigSchemaTests).Assembly.GetManifestResourceStream(
                $"{typeof(BicepConfigSchemaTests).Assembly.GetName().Name}.bicepconfig.schema.json");
            Assert.IsNotNull(configStream);
            var configContents = new StreamReader(configStream).ReadToEnd();
            Assert.IsNotNull(configContents);
            var configSchema = JObject.Parse(configContents);

            return (ruleSet.ToArray(), configSchema);
        }

        [TestMethod]
        public void Config_ShouldIncludeCurrentCoreRules()
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
                key.Should().MatchRegex("^[a-z][a-z-]*[a-z]$", "all rule keys should be lower-cased with hyphens, and not start or end with hyphen");
                key.Should().HaveLengthLessThanOrEqualTo(36, "rule ids should have a reasonable length");
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
        public void NoHardCodedEnvUrls_DefaultsShouldMatchInConfigAndSchema()
        {
            const string configPath = "src/Bicep.Core/Configuration/bicepconfig.json";
            const string schemaPath = "src/vscode-bicep/schemas/bicepconfig.schema.json";

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

            disallowedHostsInSchema.Should().BeEquivalentTo(disallowedHostsInConfig, $"default of no-hardcoded-env-urls.disallowedHosts should be the same in {configPath} and {schemaPath}");
            disallowedHostsInSchema.Should().BeInAscendingOrder($"default of no-hardcoded-env-urls.disallowedHosts should be in alphabetical order in {schemaPath}");
            disallowedHostsInConfig.Should().BeInAscendingOrder($"default of no-hardcoded-env-urls.disallowedHosts should be in alphabetical order in {configPath}");

            excludedHostsInSchema.Should().BeEquivalentTo(excludedHostsInConfig, $"default of no-hardcoded-env-urls.excluded should be the same in {configPath} and {schemaPath}");
            excludedHostsInSchema.Should().BeInAscendingOrder($"default of no-hardcoded-env-urls.excluded should be in alphabetical order in {schemaPath}");
            excludedHostsInConfig.Should().BeInAscendingOrder($"default of no-hardcoded-env-urls.excluded should be in alphabetical order in {configPath}");
        }
    }
}
