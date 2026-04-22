// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.ComponentModel;
using System.Text.Json;
using Azure.Core;
using Azure.Deployments.Core.Entities;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core;
using Bicep.Core.AzureApi;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Utils.Snapshots;
using Bicep.Decompiler;
using Bicep.IO.Abstraction;
using ModelContextProtocol.Server;

namespace Bicep.McpServer;

[McpServerToolType]
public sealed class ConnectedTools(
    BicepDecompiler decompiler,
    IArmClientProvider armClientProvider)
{
    public record ExportResourceGroupResult(
        [Description("The bicep file representing the exported resource group")]
        string BicepFile);

    [McpServerTool(Title = "Export a resource group", Destructive = false, Idempotent = true, OpenWorld = true, ReadOnly = true, UseStructuredContent = true)]
    [Description("""
    Exports all resources in an Azure resource group as a Bicep file.
    Use this to generate Bicep code representing an existing resource group's infrastructure.
    """)]
    public async Task<ExportResourceGroupResult> ExportResourceGroup(
        [Description("The subscription ID of the resource group to export.")] string subscriptionId,
        [Description("The name of the resource group to export.")] string resourceGroup,
        CancellationToken cancellationToken = default)
    {
        var configuration = IConfigurationManager.GetBuiltInConfiguration();
        var armClient = armClientProvider.CreateArmClient(configuration, null);

        ExportTemplate export = new()
        {
            Options = ExportTemplateOptions.IncludeParameterDefaultValue.ToString(),
        };
        export.Resources.Add("*");

        var response = await armClient
            .GetResourceGroupResource(new ResourceIdentifier($"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}"))
            .ExportTemplateAsync(Azure.WaitUntil.Completed, export, cancellationToken);

        var template = response.Value.Template.ToString();
        var tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.bicep");
        var decompileResult = await decompiler.Decompile(IOUri.FromFilePath(tempFilePath).WithExtension(LanguageConstants.LanguageFileExtension), template);

        var bicepFile = decompileResult.FilesToSave[decompileResult.EntrypointUri];

        return new(bicepFile);
    }
}
