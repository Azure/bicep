// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Diagnostics
{
    // Tests the bicepconfig.schema.json file

    [TestClass]
    public class BicepConfigSchemaTests
    {
        private (IBicepAnalyzerRule[] rules, JObject configSchema) GetRulesAndSchema()
        {
            var linter = new LinterAnalyzer(BicepTestConstants.BuiltInConfiguration);
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
        public void DefaultValueForCoreAnalyzer_ShouldIncludeCurrentCoreRules()
        {
            var (rules, schema) = GetRulesAndSchema();
            var defaultRules = schema.SelectToken("properties.analyzers.default.core.rules")!.ToObject<IDictionary<string, object>>();
            Assert.IsNotNull(defaultRules);
            defaultRules.Keys.Should().BeEquivalentTo(rules.Select(r => r.Code), "default rules in config schema should be same as the set of internal rules");
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
                                "$ref": "#/definitions/rule"
                            }
                        ]
                    },

                */

                var allOf = ruleConfig!.SelectToken("allOf");
                Assert.IsNotNull(allOf, "Each rule should have a top-level allOf");
                allOf.Count().Should().BeGreaterOrEqualTo(2);

                var description = allOf[0]?.SelectToken("description")?.ToString();
                Assert.IsNotNull(description);
                description.Should().Contain($"https://aka.ms/bicep/linter/{configKey}", "each rule's description should contain a link to the rule's docs");

                var matchingRule = rules.SingleOrDefault(r => r.Code == configKey);
                matchingRule.Should().NotBeNull("Rule's key in config does not match any linter rule's code");
                description.Should().Contain(matchingRule!.Description, "each rule's description should contain the same description as is specified in the rule");

                var lastAllOf = allOf[allOf.Count() - 1];
                var refString = lastAllOf?.SelectToken("$ref")?.ToString();
                Assert.IsNotNull(refString, "each rule's last allOf should be a ref to the definition of a rule");
                refString.Should().Be("#/definitions/rule", "each rule's last allOf should be a ref to the definition of a rule");
            }
        }

        [TestMethod]
        public void RuleConfigs_RuleNamesShouldUseCorrectCasing()
        {
            var (rules, schema) = GetRulesAndSchema();
            var ruleConfigs = schema.SelectToken("properties.analyzers.properties.core.properties.rules.properties")!.ToObject<IDictionary<string, JObject>>();
            Assert.IsNotNull(ruleConfigs);
            foreach (var (key, rule) in ruleConfigs)
            {
                key.Should().MatchRegex("^[a-z][a-z-]*[a-z]$", "all rule keys should be lower-cased with hyphens, not start or end with hyphen");
            }
        }
    }
}
