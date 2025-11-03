// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Decompiler.IntegrationTests;

[TestClass]
public class BicepDecompilerServiceCollectionExtensionsTests : TestBase
{
    [TestMethod]
    public async Task BicepDecompiler_can_be_constructed()
    {
        var fileExplorer = new InMemoryFileExplorer();
        var bicepUri = IOUri.FromFilePath("/main.bicep");

        var services = new ServiceCollection()
            .AddSingleton<IFileExplorer>(fileExplorer)
            .AddBicepDecompiler()
            .BuildServiceProvider();

        var decompiler = services.GetRequiredService<BicepDecompiler>();

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
}

