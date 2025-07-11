// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.SourceGraph;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.IO.Abstraction;
using Bicep.McpServer.ResourceProperties;
using Bicep.McpServer.ResourceProperties.Entities;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using ModelContextProtocol.Server;
using Newtonsoft.Json.Linq;

namespace Bicep.McpServer;

[McpServerToolType]
public sealed class BicepTools(
    AzResourceTypeLoader azResourceTypeLoader,
    ResourceVisitor resourceVisitor,
    ISourceFileFactory sourceFileFactory,
    BicepCompiler bicepCompiler,
    ArmClient armClient)
{
    private static Lazy<BinaryData> BestPracticesMarkdownLazy { get; } = new(() =>
        BinaryData.FromStream(
            typeof(BicepTools).Assembly.GetManifestResourceStream("Files/bestpractices.md") ??
            throw new InvalidOperationException("Could not find embedded resource 'Files/bestpractices.md'")));

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    [McpServerTool(Title = "List available Azure resource types", Destructive = false, Idempotent = true, OpenWorld = false, ReadOnly = true)]
    [Description("""
    Lists all available Azure resource types for a specific provider.
    The return value is a newline-separated list of resource types including their API version, e.g. Microsoft.KeyVault/vaults@2024-11-01.
    Such information is the most accurate and up-to-date as it is sourced from the Azure Resource Provider APIs.
    """)]
    public string ListAzResourceTypesForProvider(
        [Description("The resource provider (or namespace) of the Azure resource; e.g. Microsoft.KeyVault")] string providerNamespace)
    {
        var azResourceTypes = azResourceTypeLoader.GetAvailableTypes();

        if (providerNamespace is { })
        {
            azResourceTypes = azResourceTypes.Where(type => type.TypeSegments[0].Equals(providerNamespace, StringComparison.OrdinalIgnoreCase));
            return string.Join("\n", azResourceTypes.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase));
        }
        else
        {
            return string.Empty;
        }
    }

    [McpServerTool(Title = "Get Azure resource type schema", Destructive = false, Idempotent = true, OpenWorld = false, ReadOnly = true)]
    [Description("""
    Gets the schema for a specific Azure resource type and API version.
    Such information is the most accurate and up-to-date as it is sourced from the Azure Resource Provider APIs.
    """)]
    public string GetAzResourceTypeSchema(
        [Description("The resource type of the Azure resource; e.g. Microsoft.KeyVault/vaults")] string azResourceType,
        [Description("The API version of the resource type; e.g. 2024-11-01 or 2024-12-01-preview")] string apiVersion)
    {
        TypesDefinitionResult typesDefinition = resourceVisitor.LoadSingleResourceType(azResourceType, apiVersion);

        var allComplexTypes = new List<ComplexType>();
        allComplexTypes.AddRange(typesDefinition.ResourceTypeEntities);
        allComplexTypes.AddRange(typesDefinition.ResourceFunctionTypeEntities);
        allComplexTypes.AddRange(typesDefinition.OtherComplexTypeEntities);

        return JsonSerializer.Serialize(allComplexTypes, JsonSerializerOptions);
    }

    [McpServerTool(Title = "Get Bicep best-practices", Destructive = false, Idempotent = true, OpenWorld = false, ReadOnly = true)]
    [Description("""
    Lists up-to-date recommended Bicep best-practices for authoring templates.
    These practices help improve maintainability, security, and reliability of your Bicep files.
    This is helpful additional context if you've been asked to generate Bicep code.
    """)]
    public string GetBicepBestPractices() => BestPracticesMarkdownLazy.Value.ToString();

    [McpServerTool(Title = "Get bicep file diagnostics", Destructive = false, Idempotent = true, OpenWorld = false, ReadOnly = true)]
    [Description("""
    Obtains diagnostics for a Bicep file.
    The diagnostics include errors, warnings, and other messages that can help identify issues in the Bicep code.
    The diagnostics are returned as a newline-separated list of messages, each containing the file name, line number, character position, severity level, code, and message.
    This can be helpful for assessing the accuracy of generated Bicep code an iterating on it to improve quality.
    """)]
    public async Task<string> GetBicepFileDiagnostics(
        [Description("The raw contents of the .bicep file")] string bicepContents)
    {
        var uri = new Uri("inmemory:///main.bicep");
        var bicepFile = sourceFileFactory.CreateBicepFile(uri, bicepContents);

        var workspace = new Workspace();
        workspace.UpsertSourceFile(bicepFile);

        var compilation = await bicepCompiler.CreateCompilation(bicepFile.Uri, workspace, skipRestore: true);
        var diagnostics = compilation.GetAllDiagnosticsByBicepFile()[bicepFile];

        var sb = new StringBuilder();
        foreach (var diagnostic in diagnostics)
        {
            (var line, var character) = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, diagnostic.Span.Position);

            // build a a code description link if the Uri is assigned
            var codeDescription = diagnostic.Uri == null ? string.Empty : $" [{diagnostic.Uri.AbsoluteUri}]";

            var message = $"{bicepFile.FileHandle.Uri}({line + 1},{character + 1}) : {diagnostic.Level} {diagnostic.Code}: {diagnostic.Message}{codeDescription}";

            sb.AppendLine(message);
        }

        return sb.ToString();
    }

    [McpServerTool(Title = "Get Bicep what-if results", Destructive = false, Idempotent = true, OpenWorld = true, ReadOnly = true)]
    [Description("""
    Runs live what-if analysis for a Bicep file.
    This tool allows you to see the potential changes that would be made by deploying a Bicep file without actually applying those changes.
    It provides a preview of the resources that would be created, updated, or deleted based on the current state of the Azure environment.
    This is useful for validating the impact of a Bicep deployment before executing it.
    """)]
    public async Task<string> GetBicepWhatIfResults(
        [Description("The Azure subscription Id, in Guid form. You must use a real value, do not supply a placeholder. You can ask the user or use other tools to obtain this in advance, if you don't already have it in context.")] string subscriptionId,
        [Description("The Azure resource group name. You must use a real value, do not supply a placeholder. You can ask the user or use other tools to obtain this in advance, if you don't already have it in context.")] string resourceGroupName,
        [Description("The fully-qualified path to a .bicep file on disk")] string pathToBicepFile,
        [Description("The deployment parameters, if required. They key of the dictionary is the name of the parameter to supply, and the value is the parameter value to use.")] ImmutableDictionary<string, object>? deploymentParameters = null)
    {
        deploymentParameters ??= ImmutableDictionary<string, object>.Empty;
        var fileUri = IOUri.FromLocalFilePath(pathToBicepFile);
        var compilation = await bicepCompiler.CreateCompilation(fileUri.ToUri(), skipRestore: true);

        var sb = new StringBuilder();
        foreach (var (bicepFile, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
        {
            foreach (var diagnostic in diagnostics)
            {
                (var line, var character) = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, diagnostic.Span.Position);

                // build a a code description link if the Uri is assigned
                var codeDescription = diagnostic.Uri == null ? string.Empty : $" [{diagnostic.Uri.AbsoluteUri}]";

                var message = $"{bicepFile.FileHandle.Uri}({line + 1},{character + 1}) : {diagnostic.Level} {diagnostic.Code}: {diagnostic.Message}{codeDescription}";

                sb.AppendLine(message);
            }
        }

        var result = compilation.Emitter.Template();

        if (!result.Success || result.Template is null)
        {
            sb.AppendLine($"Bicep compilation failed. Correct the error diagnostics and try again.");
            return sb.ToString();
        }

        var resourceGroupId = ResourceGroupResource.CreateResourceIdentifier(subscriptionId, resourceGroupName);
        var deploymentId = ArmDeploymentResource.CreateResourceIdentifier(resourceGroupId.ToString(), "main");

        var deploymentContent = new ArmDeploymentWhatIfContent(new(ArmDeploymentMode.Incremental)
        {
            Template = BinaryData.FromString(result.Template),
            Parameters = BinaryData.FromString(deploymentParameters.ToDictionary(x => x.Key, x => new
            {
                Value = x.Value
            }).ToJson()),
        });

        var deployment = armClient.GetArmDeploymentResource(deploymentId);
        var whatIfResults = await deployment.WhatIfAsync(WaitUntil.Completed, deploymentContent);

        return whatIfResults.GetRawResponse().Content.ToString();
    }
}
