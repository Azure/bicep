// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests.Handlers;

[TestClass]
public class BicepGetConfigInfoHandlerTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    private RootConfiguration TestConfig = IConfigurationManager.GetBuiltInConfiguration().WithAnalyzersConfiguration(new AnalyzersConfiguration("""
              {
                "core": {
                  "enabled": true,
                    "rules": {
                    "artifacts-parameters": {
                        "level": "info"
                    },
                    "max-asserts": {
                        "level": "error"          
                    },
                    "use-recent-api-versions":{
                        "level": "warning",
                        "maxAgeInDays": 30
                    },
                    "max-variables":{
                        "level": "off"
                    }
                  }
                }
              }
              """));

    [TestMethod]
    public async Task MergedConfiguration()
    {
        var handler = new BicepGetConfigInfoHandler(new LinterAnalyzer(BicepTestConstants.EmptyServiceProvider), IConfigurationManager.WithStaticConfiguration(TestConfig));

        var result = await handler.Handle(new BicepGetConfigInfoParams("file:///main.bicep"), default);
        result.ConfigPath.Should().BeNull();

        result.EffectiveConfig.Should().ContainIgnoringNewlines("""
              "formatting": {
                "indentKind": "Space",
                "newlineKind": "LF",
                "insertFinalNewline": true,
                "indentSize": 2,
                "width": 120
              }
            """.ReplaceLineEndings());
        result.EffectiveConfig.Should().ContainIgnoringNewlines("""
                "analyzers": {
                  "core": {
                    "enabled": true,
                    "rules": {
                      "artifacts-parameters": {
                        "level": "info"
                      },
                      "max-asserts": {
                        "level": "error"
                      },
                      "use-recent-api-versions": {
                        "level": "warning",
                        "maxAgeInDays": 30
                      },
                      "max-variables": {
                        "level": "off"
                      }
                    }
                  }
                }
              """.ReplaceLineEndings());

    }

    [TestMethod]
    public async Task LinterState()
    {
        var handler = new BicepGetConfigInfoHandler(new LinterAnalyzer(BicepTestConstants.EmptyServiceProvider), IConfigurationManager.WithStaticConfiguration(TestConfig));

        var result = await handler.Handle(new BicepGetConfigInfoParams("file:///main.bicep"), default);
        result.ConfigPath.Should().BeNull();

        // Example output:
        /*
            Analyzer enabled: True

            Enabled rules:
              adminusername-should-not-be-literal: Warning (default)
              artifacts-parameters: Warning (default)
              decompiler-cleanup: Warning (default)
              max-asserts: Error (default)
              max-outputs: Error (default)
              max-params: Error (default)
              max-resources: Error (default)
              max-variables: Error (default)
              no-conflicting-metadata: Warning (default)
              no-deployments-resources: Warning (default)
              no-hardcoded-env-urls: Warning (default)
              nested-deployment-template-scoping: Error (default)
              no-unnecessary-dependson: Warning (default)
              no-unused-existing-resources: Warning (default)
              no-unused-params: Warning (default)
              no-unused-vars: Warning (default)
              outputs-should-not-contain-secrets: Warning (default)
              prefer-interpolation: Warning (default)
              prefer-unquoted-property-names: Warning (default)
              protect-commandtoexecute-secrets: Warning (default)
              secure-secrets-in-params: Warning (default)
              secure-parameter-default: Warning (default)
              secure-params-in-nested-deploy: Warning (default)
              simplify-interpolation: Warning (default)
              simplify-json-null: Warning (default)
              use-parent-property: Warning (default)
              use-resource-id-functions: Warning (default)
              use-resource-symbol-reference: Warning (default)
              use-safe-access: Warning (default)
              use-secure-value-for-secure-inputs: Warning (default)
              use-stable-resource-identifiers: Warning (default)
              use-stable-vm-image: Warning (default)
  
            Disabled rules:
              explicit-values-for-loc-params: Off (default)
              no-hardcoded-location: Off (default)
              no-loc-expr-outside-params: Off (default)
              use-recent-api-versions: Off (default)
              use-recent-module-versions: Off (default)
              use-user-defined-types: Off (default)
              what-if-short-circuiting: Off (default)
  
        */

        result.LinterState.Should().StartWith("Analyzer enabled: True");
    }

    [TestMethod]
    public async Task IfAnalyzerDisabled_ThenAllRulesOff()
    {
        var config = IConfigurationManager.GetBuiltInConfiguration().WithAnalyzersConfiguration(new AnalyzersConfiguration("""
            {
              "analyzers": {
                "core": {
                  "enabled": false
                }
              }
            }
            """));
        var handler = new BicepGetConfigInfoHandler(new LinterAnalyzer(BicepTestConstants.EmptyServiceProvider), IConfigurationManager.WithStaticConfiguration(config));

        var result = await handler.Handle(new BicepGetConfigInfoParams("file:///main.bicep"), default);
        var linterState = result.LinterState.ReplaceLineEndings();

        linterState.Should().ContainIgnoringNewlines("Analyzer enabled: False");
        linterState.Should().ContainIgnoringNewlines("""
            Enabled rules:
              
            Disabled rules:
              adminusername-should-not-be-literal: Off (analyzer disabled)
            """);
    }


}
