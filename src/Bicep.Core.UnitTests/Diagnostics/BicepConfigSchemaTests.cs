// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics
{
    // Tests the src/vscode-bicep/schemas/bicepconfig.schema.json file

    [TestClass]
    public class BicepConfigSchemaTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private const string BicepRootConfigFilePath = "src/Bicep.Core/Configuration/bicepconfig.json";
        private const string BicepConfigSchemaFilePath = "src/vscode-bicep/schemas/bicepconfig.schema.json";

        private (IBicepAnalyzerRule[] rules, JsonElement configSchema) GetRulesAndSchema()
        {
            var linter = new LinterAnalyzer();
            var ruleSet = linter.GetRuleSet();
            ruleSet.Should().NotBeEmpty();

            var configStream = typeof(BicepConfigSchemaTests).Assembly.GetManifestResourceStream(
                $"{typeof(BicepConfigSchemaTests).Assembly.GetName().Name}.bicepconfig.schema.json");
            Assert.IsNotNull(configStream);

            var document = JsonDocument.Parse(configStream);
            return (ruleSet.ToArray(), document.RootElement);
        }

        //asdfg
        //private IEnumerable<JsonProperty> GetRuleCustomConfigurationProperties(JsonElement ruleConfigSchema)
        //{
        //    return ruleConfigSchema
        //        .GetProperty("allOf")[0]
        //        .GetProperty("properties")
        //        .EnumerateObject();
        //}

        [TestMethod]
        public void Config_ShouldIncludeCurrentAllCoreRules()
        {
            var (rules, schema) = GetRulesAndSchema();
            var ruleConfigs = schema.GetProperty("properties")
                .GetProperty("analyzers").GetProperty("properties").GetProperty("core").GetProperty("properties")
                .GetProperty("rules").GetProperty("properties").EnumerateObject().ToDictionary(p => p.Name);
            Assert.IsNotNull(ruleConfigs);
            ruleConfigs.Keys.Should().BeEquivalentTo(rules.Select(r => r.Code), "config schema should include definitions for core rules");
        }

        [TestMethod]
        public void RuleConfigs_ShouldBeCorrectlyDefined()
        {
            var (rules, schema) = GetRulesAndSchema();
            var ruleConfigs = schema
                .GetProperty("properties")
                .GetProperty("analyzers")
                .GetProperty("properties")
                .GetProperty("core")
                .GetProperty("properties")
                .GetProperty("rules")
                .GetProperty("properties")
                .EnumerateObject()
                .ToDictionary(p => p.Name);

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

                var allOf = ruleConfig.Value.GetProperty("allOf");
                Assert.IsNotNull(allOf, "Each rule should have a top-level allOf");
                allOf.EnumerateArray().Count().Should().BeGreaterOrEqualTo(2);

                var description = allOf[0].GetProperty("description").ToString();
                Assert.IsNotNull(description);
                description.Should().EndWith($" See https://aka.ms/bicep/linter/{configKey}", "each rule's description should end with 'See <help-link>' using the link to the rule's docs");

                var matchingRule = rules.SingleOrDefault(r => r.Code == configKey);
                matchingRule.Should().NotBeNull("Rule's key in config does not match any linter rule's code");
                description.Should().Contain(matchingRule!.Description, "each rule's description should contain the same description as is specified in the rule");

                var lastAllOf = allOf[allOf.GetArrayLength() - 1];
                var refString = lastAllOf.GetProperty("$ref").ToString();
                Assert.IsNotNull(refString, "each rule's last allOf should be a ref to the definition of a rule");
                refString.Should().MatchRegex("^#/definitions/rule-def-level-(warning|error|off)$", "each rule's last allOf should be a ref to the definition of a rule, one of '#/definitions/rule-def-level-warning', '#/definitions/rule-def-level-error' or '#/definitions/rule-def-error-off'");
            }
        }

        // Other methods...

        [TestMethod]
        public void NoHardCodedEnvUrls_DefaultsShouldMatchInConfigAndSchema()
        {
            // From schema
            var (rules, schema) = GetRulesAndSchema();
            var ruleConfigs = schema
                .GetProperty("properties")
                .GetProperty("analyzers")
                .GetProperty("properties")
                .GetProperty("core")
                .GetProperty("properties")
                .GetProperty("rules")
                .GetProperty("properties")
                .EnumerateObject()
                .ToDictionary(p => p.Name, p => p.Value);

            Assert.IsNotNull(ruleConfigs);
            string[] disallowedHostsInSchema = ruleConfigs["no-hardcoded-env-urls"].GetProperty("allOf")[0]
                .GetProperty("properties").GetProperty("disallowedhosts").GetProperty("default")
                .EnumerateArray().Select(v => v.ToString()).ToArray();
            string[] excludedHostsInSchema = ruleConfigs["no-hardcoded-env-urls"].GetProperty("allOf")[0]
                .GetProperty("properties").GetProperty("excludedhosts").GetProperty("default")
                .EnumerateArray().Select(v => v.ToString()).ToArray();

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
        public void ExperimentalFeatures_SameAsGenerated()
        {
            // From schema
            var (rules, schema) = GetRulesAndSchema();
            var ruleConfigs = schema
                .GetProperty("properties")
                .GetProperty("analyzers")
                .GetProperty("properties")
                .GetProperty("core")
                .GetProperty("properties")
                .GetProperty("rules")
                .GetProperty("properties")
                .EnumerateObject()
                .ToDictionary(p => p.Name, p => p.Value); Assert.IsNotNull(ruleConfigs);
            string[] disallowedHostsInSchema = ruleConfigs["no-hardcoded-env-urls"].GetProperty("allOf")[0]
                .GetProperty("properties").GetProperty("disallowedhosts").GetProperty("default")
                .EnumerateArray().Select(v => v.ToString()).ToArray();
            string[] excludedHostsInSchema = ruleConfigs["no-hardcoded-env-urls"].GetProperty("allOf")[0]
                .GetProperty("properties").GetProperty("excludedhosts").GetProperty("default")
                .EnumerateArray().Select(v => v.ToString()).ToArray();

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
    }
}


///////////////////////////////////////////////////////////////////
//// Generation functions
////
//// Ideally these will grow into a code generator that simply generates bicepconfig.schema.json for us

//private void GenerateExperimentalFeaturesSection() {
//            json
//            "experimentalFeaturesEnabled": {
//              "type": "object",
//              "description": "The experimental Bicep features that should be enabled or disabled for templates using this configuration file",
//              "properties": {
//                "symbolicNameCodegen": {
//                  "type": "boolean"
//                },
//        "extensibility": {
//          "type": "boolean"
//        },
//        "resourceTypedParamsAndOutputs": {
//          "type": "boolean"
//        },
//        "sourceMapping": {
//          "type": "boolean"
//        },
//        "userDefinedTypes": {
//          "type": "boolean"
//        },
//        "userDefinedFunctions": {
//          "type": "boolean"
//        },
//        "prettyPrinting": {
//          "type": "boolean"
//        },
//        "testFramework": {
//          "type": "boolean"
//        },
//        "asserts": {
//          "type": "boolean"
//        },
//        "dynamicTypeLoading": {
//          "type": "boolean"
//        },
//        "compileTimeImports": {
//          "type": "boolean"
//        },
//        "publishSource": {
//          "type": "boolean"
//        }
//      }
//    },
//        }
//    }
//}
