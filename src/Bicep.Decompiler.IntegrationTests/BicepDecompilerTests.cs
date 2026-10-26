// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests;
using Bicep.Decompiler.ArmHelpers;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Decompiler.IntegrationTests;

[TestClass]
public class BicepDecompilerTests : TestBase
{
    [TestMethod]
    public async Task BicepDecompiler_can_be_constructed()
    {
        var fileExplorer = new InMemoryFileExplorer();
        var bicepUri = IOUri.FromFilePath("/main.bicep");

        var decompiler = BicepDecompiler.Create(s => s.AddSingleton<IFileExplorer>(fileExplorer));

        var result = await decompiler.Decompile(bicepUri, """
        {
          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
          "contentVersion": "1.0.0.0",
          "parameters": {
            "foo": {
              "type": "string"
            }
          },
          "resources": [],
          "outputs": {
            "foo": {
              "type": "string",
              "value": "[parameters('foo')]"
            }
          }
        }
        """);

        result.FilesToSave[result.EntrypointUri].Should().Contain("param foo string");
    }

    [TestMethod]
    public async Task Decompiler_emits_warning_comment_for_parameter_names_with_periods()
    {
        var fileExplorer = new InMemoryFileExplorer();
        var bicepUri = IOUri.FromFilePath("/main.bicep");

        var decompiler = BicepDecompiler.Create(s => s.AddSingleton<IFileExplorer>(fileExplorer));

        var result = await decompiler.Decompile(bicepUri, """
        {
          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
          "contentVersion": "1.0.0.0",
          "parameters": {
            "Security.Authentication.AAD.Tenant": {
              "type": "string"
            },
            "normalParam": {
              "type": "string"
            }
          },
          "resources": []
        }
        """);

        var bicepOutput = result.FilesToSave[result.EntrypointUri];

        // The dotted parameter name should be renamed to use underscores
        bicepOutput.Should().Contain("param Security_Authentication_AAD_Tenant string");

        // A warning comment should appear before the renamed parameter
        bicepOutput.Should().Contain(
            "// WARNING: This parameter was renamed during decompilation because its original name could not be used as a Bicep identifier.");

        // The warning should appear before the renamed param declaration
        var warningIndex = bicepOutput.IndexOf("// WARNING:");
        var paramIndex = bicepOutput.IndexOf("param Security_Authentication_AAD_Tenant");
        warningIndex.Should().BeLessThan(paramIndex);

        // Parameters with valid names should not have a warning
        var normalParamIndex = bicepOutput.IndexOf("param normalParam");
        var warningBeforeNormal = bicepOutput.LastIndexOf("// WARNING:", normalParamIndex);
        warningBeforeNormal.Should().BeLessThan(paramIndex,
            because: "the warning comment should only appear before the renamed parameter, not before normalParam");
    }

    [TestMethod]
    public async Task Decompiler_does_not_emit_warning_for_valid_parameter_names()
    {
        var fileExplorer = new InMemoryFileExplorer();
        var bicepUri = IOUri.FromFilePath("/main.bicep");

        var decompiler = BicepDecompiler.Create(s => s.AddSingleton<IFileExplorer>(fileExplorer));

        var result = await decompiler.Decompile(bicepUri, """
        {
          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
          "contentVersion": "1.0.0.0",
          "parameters": {
            "validParamName": {
              "type": "string"
            }
          },
          "resources": []
        }
        """);

        var bicepOutput = result.FilesToSave[result.EntrypointUri];
        bicepOutput.Should().Contain("param validParamName string");
        bicepOutput.Should().NotContain("// WARNING:");
    }

    [TestMethod]
    public void IsBicepGeneratedTemplate_returns_true_for_bicep_generated_template()
    {
        var template = """
        {
          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
          "contentVersion": "1.0.0.0",
          "metadata": {
            "_generator": {
              "name": "bicep",
              "version": "0.99.0.0",
              "templateHash": "12345678901234567"
            }
          },
          "resources": []
        }
        """;

        TemplateHelpers.IsBicepGeneratedTemplate(JObject.Parse(template)).Should().BeTrue();
    }

    [TestMethod]
    public void IsBicepGeneratedTemplate_returns_false_for_plain_arm_template()
    {
        var template = """
        {
          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
          "contentVersion": "1.0.0.0",
          "resources": []
        }
        """;

        TemplateHelpers.IsBicepGeneratedTemplate(JObject.Parse(template)).Should().BeFalse();
    }

    [TestMethod]
    public void IsBicepGeneratedTemplate_returns_false_for_non_bicep_generator()
    {
        var template = """
        {
          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
          "contentVersion": "1.0.0.0",
          "metadata": {
            "_generator": {
              "name": "arm-ttk",
              "version": "1.0.0"
            }
          },
          "resources": []
        }
        """;

        TemplateHelpers.IsBicepGeneratedTemplate(JObject.Parse(template)).Should().BeFalse();
    }
}
