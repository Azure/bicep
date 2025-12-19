// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.McpServer.UnitTests;

[TestClass]
public class BicepDecompilerToolsTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    private static IServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services
            .AddBicepMcpServer();

        return services.BuildServiceProvider();
    }

    private readonly BicepDecompilerTools tools = GetServiceProvider().GetRequiredService<BicepDecompilerTools>();

    [TestMethod]
    public async Task DecompileArmParametersFile_returns_bicep_parameters()
    {
        var paramsFilePath = FileHelper.SaveResultFile(TestContext, "parameters.json", """
            {
              "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
              "contentVersion": "1.0.0.0",
              "parameters": {
                "adminUsername": {
                  "value": "tim"
                },
                "dnsLabelPrefix": {
                  "value": "newvm79347a"
                }
              }
            }
            """);

        var response = await tools.DecompileArmParametersFile(paramsFilePath);
        response.FilesToSave[response.EntrypointUri].Should().Contain("param adminUsername = 'tim'");
        response.FilesToSave[response.EntrypointUri].Should().Contain("param dnsLabelPrefix = 'newvm79347a'");
    }

    [TestMethod]
    public async Task DecompileArmTemplateFile_returns_bicep()
    {
        var templateFilePath = FileHelper.SaveResultFile(TestContext, "template.json", """
            {
              "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
              "contentVersion": "1.0.0.0",
              "parameters": {
                "inputObject": {
                  "type": "object"
                }
              },
              "resources": [],
              "outputs": {
                "outputObject": {
                  "type": "object",
                  "value": "[parameters('inputObject')]"
                }
              }
            }
            """);

        var response = await tools.DecompileArmTemplateFile(templateFilePath);
        response.FilesToSave[response.EntrypointUri].Should().Contain("param inputObject object");
        response.FilesToSave[response.EntrypointUri].Should().Contain("output outputObject object = inputObject");
    }
}