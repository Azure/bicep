// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Helpers;
using Azure.Deployments.Extensibility.Contract;
using Azure.Deployments.Extensibility.Core.V2.Models;
using Azure.Deployments.Extensibility.Data;
using Azure.Deployments.Extensibility.Messages;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.AzureApi;
using Bicep.Local.Deploy.Azure;
using Bicep.Local.Deploy.Engine;
using Bicep.Local.Deploy.Types;
using Json.More;
using Json.Pointer;
using Microsoft.WindowsAzure.ResourceStack.Common.Algorithms;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;
using Azure.ResourceManager.Resources.Models;
using Azure.Deployments.Expression.Intermediate;
using Azure.Deployments.Templates.Expressions.PartialEvaluation;
using Azure.Deployments.Expression.Intermediate.Extensions;
using Newtonsoft.Json;

namespace Bicep.Local.Deploy.Extensibility;

public class NestedDeploymentExtension(
    IArmDeploymentProvider armDeploymentProvider,
    LocalDeploymentEngine localDeploymentEngine,
    IConfigurationManager configurationManager) : ILocalExtension
{
    private record DeploymentIdentifiers(
        string? SubscriptionId,
        string? ResourceGroup,
        string Name,
        string? SourceUri);

    private static DeploymentIdentifiers GetDeploymentIdentifiers(JsonObject properties)
    {
        var name = properties["name"]?.GetValue<string>();
        if (name == null)
        {
            throw new InvalidOperationException("Deployment name must have been set.");
        }

        var subscriptionId = properties["subscriptionId"]?.GetValue<string>();
        var resourceGroupName = properties["resourceGroup"]?.GetValue<string>();

        var scope = properties["scope"]?.GetValue<string>();
        if (scope != null)
        {
            throw new InvalidOperationException("Above-subscription scope is not supported yet.");
        }

        var sourceUri = properties["sourceUri"]?.GetValue<string>();

        return new(subscriptionId, resourceGroupName, name, sourceUri);
    }

    private static JsonObject FromIdentifiers(DeploymentIdentifiers identifiers)
        => JsonObject.Parse(identifiers.ToJsonStream())?.AsObject() ?? throw new UnreachableException("Serialization is not expected to fail.");

    private static void EnsureDeploymentType(string type)
    {
        if (type != "Microsoft.Resources/deployments")
        {
            throw new InvalidOperationException($"Unsupported type: {type}");
        }
    }

    private static LocalExtensionOperationResponse GetResponse(string type, string? apiVersion, DeploymentIdentifiers identifiers, LocalDeploymentResult result)
    {
        if (result.Deployment.Properties.ProvisioningState == ProvisioningState.Failed)
        {
            return new LocalExtensionOperationResponse(
                Resource: null,
                ErrorData: new ErrorData(
                    error: new Error(
                    result.Deployment.Properties.Error.Code,
                    result.Deployment.Properties.Error.Message,
                    JsonPointer.Empty,
                    result.Deployment.Properties.Error.Details.SelectArray(x => new ErrorDetail(x.Code, x.Message, JsonPointer.Empty)))));
        }

        return new LocalExtensionOperationResponse(
            Resource: new Resource(
                identifiers: FromIdentifiers(identifiers),
                type: type,
                apiVersion: apiVersion,
                status: result.Deployment.Properties.ProvisioningState.ToString(),
                properties: JsonObject.Parse(result.Deployment.Properties.ToJsonStream())?.AsObject() ?? throw new UnreachableException("Serialization is not expected to fail.")),
            ErrorData: null);
    }

    public async Task<LocalExtensionOperationResponse> CreateOrUpdate(ResourceSpecification request, CancellationToken cancellationToken)
    {
        if (request.Type == "OrchestrationStack")
        {
            return await CreateOrUpdateStack(request, cancellationToken);
        }

        EnsureDeploymentType(request.Type);

        var template = request.Properties["template"]!.ToString();
        var parameters = new JsonObject
        {
            ["$schema"] = "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
            ["contentVersion"] = "1.0.0.0",
            ["parameters"] = request.Properties["parameters"]?.DeepClone(),
        };

        var identifiers = GetDeploymentIdentifiers(request.Properties);

        if (identifiers.SubscriptionId != null)
        {
            try
            {
                GuardHelper.ArgumentNotNull(identifiers.SourceUri);
                var configuration = configurationManager.GetConfiguration(new Uri(identifiers.SourceUri).ToIOUri());
                DeploymentLocator locator = new("", null, identifiers.SubscriptionId, identifiers.ResourceGroup, identifiers.Name);

                await armDeploymentProvider.StartDeployment(configuration, locator, template, parameters.ToJsonString(), cancellationToken);
                var result = await armDeploymentProvider.CheckDeployment(configuration, locator, cancellationToken);

                return GetResponse(request.Type, request.ApiVersion, identifiers, result);
            }
            catch (Exception ex)
            {
                return new LocalExtensionOperationResponse(
                    Resource: null,
                    ErrorData: new(new("InvalidDeployment", $"Failed to deploy to Azure. {ex}")));
            }
        }
        else
        {
            await localDeploymentEngine.StartDeployment(identifiers.Name, template, parameters.ToJsonString(), cancellationToken);
            var result = await localDeploymentEngine.CheckDeployment(identifiers.Name);

            return GetResponse(request.Type, request.ApiVersion, identifiers, result);
        }
    }

    private async Task<LocalExtensionOperationResponse> CreateOrUpdateStack(ResourceSpecification request, CancellationToken cancellationToken)
    {
        var templateFile = request.Properties["template"]!.ToString();
        var parametersFile = request.Properties["parameters"]!.ToString();
        var sourceUri = request.Properties["sourceUri"]!.ToString();
        var body = request.Properties["body"]!.ToString().FromJson<JObject>();
        var region = body!["region"]!.Value<string>();
        var deploy = body!["deploy"]!.Value<string>();
        var inputs = body!["inputs"]!.Value<JObject>();

        var paramsResult = await ProcessParameters(parametersFile, inputs!);
        var subscriptionId = paramsResult.UsingConfig.Scope.Split('/')[2];
        var resourceGroup = paramsResult.UsingConfig.Scope.Split('/')[4];

        DeploymentIdentifiers identifiers = new(
            SubscriptionId: subscriptionId,
            ResourceGroup: resourceGroup,
            Name: paramsResult.UsingConfig.Name!,
            SourceUri: sourceUri);

        if (paramsResult.UsingConfig.StacksConfig is not { } stacksConfig)
        {
            throw new InvalidOperationException("Stacks configuration is not present.");
        }

        if (identifiers.SubscriptionId is null)
        {
            throw new InvalidOperationException("Stack deployments must be deployed to Azure.");
        }

        var descriptionJson = new JObject
        {
            ["version"] = "1.0",
            ["templateHash"] = TemplateHelpers.ComputeTemplateHash(templateFile.FromJson<JToken>()),
            ["parametersHash"] = TemplateHelpers.ComputeDeploymentParametersHash(paramsResult.Parameters.FromJson<DeploymentParametersDefinition>().Parameters),
        };

        stacksConfig = stacksConfig with { Description = descriptionJson.ToJson() };

        try
        {
            GuardHelper.ArgumentNotNull(identifiers.SourceUri);
            var configuration = configurationManager.GetConfiguration(new Uri(identifiers.SourceUri).ToIOUri());
            DeploymentLocator locator = new("", null, identifiers.SubscriptionId, identifiers.ResourceGroup, identifiers.Name);

            await armDeploymentProvider.CreateResourceGroup(configuration, locator, region!, cancellationToken);

            await armDeploymentProvider.StartDeploymentStack(configuration, locator, templateFile, paramsResult.Parameters, stacksConfig, cancellationToken);
            var result = await armDeploymentProvider.CheckDeploymentStack(configuration, locator, cancellationToken);

            return GetResponse(request.Type, request.ApiVersion, identifiers, result);
        }
        catch (Exception ex)
        {
            return new LocalExtensionOperationResponse(
                Resource: null,
                ErrorData: new(new("InvalidDeployment", $"Failed to deploy to Azure. {ex}")));
        }
    }

    public async Task<LocalExtensionOperationResponse> Get(ResourceReference request, CancellationToken cancellationToken)
    {
        if (request.Type == "OrchestrationStack")
        {
            return await GetStack(request, cancellationToken);
        }

        EnsureDeploymentType(request.Type);

        var identifiers = GetDeploymentIdentifiers(request.Identifiers);

        if (identifiers.SubscriptionId != null)
        {
            try
            {
                GuardHelper.ArgumentNotNull(identifiers.SourceUri);
                var configuration = configurationManager.GetConfiguration(new Uri(identifiers.SourceUri).ToIOUri());
                DeploymentLocator locator = new("", null, identifiers.SubscriptionId, identifiers.ResourceGroup, identifiers.Name);

                var result = await armDeploymentProvider.CheckDeployment(configuration, locator, cancellationToken);

                return GetResponse(request.Type, request.ApiVersion, identifiers, result);
            }
            catch (Exception ex)
            {
                return new LocalExtensionOperationResponse(
                    Resource: null,
                    ErrorData: new(new("InvalidDeployment", $"Failed to deploy to Azure. {ex}")));
            }
        }
        else
        {
            var result = await localDeploymentEngine.CheckDeployment(identifiers.Name);

            return GetResponse(request.Type, request.ApiVersion, identifiers, result);
        }
    }

    private async Task<LocalExtensionOperationResponse> GetStack(ResourceReference request, CancellationToken cancellationToken)
    {
        var identifiers = GetDeploymentIdentifiers(request.Identifiers);

        if (identifiers.SubscriptionId is null)
        {
            throw new InvalidOperationException("Stack deployments must be deployed to Azure.");
        }

        try
        {
            GuardHelper.ArgumentNotNull(identifiers.SourceUri);
            var configuration = configurationManager.GetConfiguration(new Uri(identifiers.SourceUri).ToIOUri());
            DeploymentLocator locator = new("", null, identifiers.SubscriptionId, identifiers.ResourceGroup, identifiers.Name);

            var result = await armDeploymentProvider.CheckDeploymentStack(configuration, locator, cancellationToken);

            return GetResponse(request.Type, request.ApiVersion, identifiers, result);
        }
        catch (Exception ex)
        {
            return new LocalExtensionOperationResponse(
                Resource: null,
                ErrorData: new(new("InvalidDeployment", $"Failed to deploy to Azure. {ex}")));
        }
    }

    public async Task<LocalExtensionOperationResponse> Preview(ResourceSpecification request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Type != "Microsoft.Resources/deployments" && request.Type != "OrchestrationStack")
        {
            throw new InvalidOperationException($"Unsupported type: {request.Type}");
        }

        var identifiers = GetDeploymentIdentifiers(request.Properties);

        return new LocalExtensionOperationResponse(
            Resource: new Resource(
                identifiers: FromIdentifiers(identifiers),
                type: request.Type,
                apiVersion: request.ApiVersion,
                status: ProvisioningState.Succeeded.ToString(),
                properties: request.Properties),
            ErrorData: null);
    }

    public Task<LocalExtensionOperationResponse> Delete(ResourceReference request, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;

    public Task<TypeFiles> GetTypeFiles(CancellationToken cancellationToken)
        => throw new InvalidOperationException($"Extension {nameof(NestedDeploymentExtension)} does not support type files.");

    public record UsingConfig(
        string? Name,
        string Scope,
        StacksConfig? StacksConfig);

    public record StacksConfig(
        string? Description,
        ActionOnUnmanage? ActionOnUnmanage,
        DenySettings? DenySettings);

    public record ProcessParametersResult(
        string Parameters,
        UsingConfig UsingConfig);

    public static async Task<ProcessParametersResult> ProcessParameters(string parameters, JObject inputs)
    {
        parameters = await ParametersProcessor.Process(parameters, async (kind, config) =>
        {
            await Task.CompletedTask;
            switch (kind)
            {
                case "stack.setting":
                    var argKey = config!.Value<string>()!;
                    return inputs.GetProperty(argKey);

                // TODO implement eval
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        });

        var paramsDefinition = parameters.FromJson<DeploymentParametersDefinition>();
        if (paramsDefinition.Parameters.TryGetValue("$usingConfig") is not { } usingConfigParam)
        {
            // TODO add earlier validation that the 'with <config>' syntax is used
            throw new UnreachableException();
        }

        ExpressionEvaluationContext context = new(
        [
            global::Azure.Deployments.Expression.Expressions.ExpressionBuiltInFunctions.Functions,
            new ParametersScope(paramsDefinition.ExternalInputs),
        ]);

        if (usingConfigParam.Expression is { } expression)
        {
            usingConfigParam.Value = ToJTokenExpressionSerializer.Serialize(
                context.EvaluateExpression(ExpressionParser.ParseLanguageExpression(expression)));
            usingConfigParam.Expression = null;
        }

        foreach (var param in paramsDefinition.Parameters.Values)
        {
            if (param.Expression is { } expr)
            {
                param.Value = ToJTokenExpressionSerializer.Serialize(
                    context.EvaluateExpression(ExpressionParser.ParseLanguageExpression(expr)));
                param.Expression = null;
            }
        }

        var usingConfig = usingConfigParam.Value ?? throw new UnreachableException("Should have already been evaluated");

        // required property - should have already been validated
        var mode = usingConfig.GetProperty("mode")?.Value<string>() ?? throw new UnreachableException();

        StacksConfig? stacksConfig = null;
        if (mode == "stack")
        {
            ActionOnUnmanage? actionOnUnmanage = null;
            if (usingConfig.GetProperty("actionOnUnmanage") is JObject actionOnUnmanageConfig)
            {
                DeploymentStacksDeleteDetachEnum actionOnUnmanageResources = actionOnUnmanageConfig?.GetProperty("resources")?.Value<string>() is { } resources ? new(resources) : throw new UnreachableException();
                DeploymentStacksDeleteDetachEnum? actionOnUnmanageResourceGroups = actionOnUnmanageConfig?.GetProperty("resourceGroups")?.Value<string>() is { } resourceGroups ? new(resourceGroups) : null;
                DeploymentStacksDeleteDetachEnum? actionOnUnmanageManagementGroups = actionOnUnmanageConfig?.GetProperty("managementGroups")?.Value<string>() is { } managementGroups ? new(managementGroups) : null;

                actionOnUnmanage = new(actionOnUnmanageResources)
                {
                    ResourceGroups = actionOnUnmanageResourceGroups,
                    ManagementGroups = actionOnUnmanageManagementGroups
                };
            }

            DenySettings? denySettings = null;
            if (usingConfig.GetProperty("denySettings") is JObject denySettingsConfig)
            {
                DenySettingsMode denySettingsMode = denySettingsConfig?.GetProperty("mode")?.Value<string>() is { } dsMode ? new(dsMode) : throw new UnreachableException();
                var applyToChildScopes = denySettingsConfig?.GetProperty("applyToChildScopes")?.Value<bool>() ?? false;
                var excludedActions = denySettingsConfig?.GetProperty("excludedActions")?.FromJToken<string[]>();
                var excludedPrincipals = denySettingsConfig?.GetProperty("excludedPrincipals")?.FromJToken<string[]>();

                denySettings = new(denySettingsMode)
                {
                    ApplyToChildScopes = applyToChildScopes,
                };
                denySettings.ExcludedActions.AddRange(excludedActions ?? []);
                denySettings.ExcludedPrincipals.AddRange(excludedPrincipals ?? []);
            }

            stacksConfig = new(
                Description: usingConfig.GetProperty("description")?.Value<string>(),
                ActionOnUnmanage: actionOnUnmanage,
                DenySettings: denySettings);
        }

        UsingConfig config = new(
            Name: usingConfig.GetProperty("name")?.Value<string>(),
            // required property - should have already been validated
            Scope: usingConfig.GetProperty("scope")?.Value<string>() ?? throw new UnreachableException(),
            StacksConfig: stacksConfig);

        // TODO: This is a hack to pass config around in Bicep - should not be sent on the wire.
        paramsDefinition.Parameters.Remove("$usingConfig");

        return new(
            Parameters: paramsDefinition.ToJson(),
            UsingConfig: config);
    }



    private static class ParametersProcessor
    {
        public delegate Task<JToken> ResolveExternalInput(string kind, JToken? config);

        public static async Task<string> Process(string parametersFile, ResolveExternalInput onResolveExternalInput)
        {
            GuardHelper.ArgumentNotNull(parametersFile);
            GuardHelper.ArgumentNotNull(onResolveExternalInput);

            // Intentionally avoid using a typed class here, because we don't want to interfere
            // with properties that aren't part of the official schema.
            var parameters = TryParse(parametersFile) ?? throw new InvalidOperationException("Failed to read the parameters file");

            if (TryGetPropertyInsensitive(parameters, "externalInputDefinitions") is not { } externalInputDefinitions)
            {
                return parametersFile;
            }

            if (externalInputDefinitions is not JObject inputDefinitions)
            {
                throw new InvalidOperationException("externalInputDefinitions must be an object");
            }

            if (TryGetPropertyInsensitive(parameters, "externalInputs") is { })
            {
                throw new InvalidOperationException("externalInputs must not already be defined");
            }

            var inputs = new JObject();
            foreach (var kvp in inputDefinitions)
            {
                if (kvp.Value is not JObject inputDefinition)
                {
                    throw new InvalidOperationException($"externalInputDefinitions[{kvp.Key}] must be an object");
                }

                if (TryGetPropertyInsensitive(inputDefinition, "kind") is not { } kindToken ||
                    kindToken.Type != JTokenType.String ||
                    kindToken.Value<string>() is not { } kind)
                {
                    throw new InvalidOperationException($"externalInputDefinitions[{kvp.Key}].kind must be defined");
                }

                var config = TryGetPropertyInsensitive(inputDefinition, "config");
                var value = await onResolveExternalInput(kind, config).ConfigureAwait(false);

                inputs[kvp.Key] = new JObject
                {
                    ["value"] = value,
                };
            }

            parameters["externalInputs"] = inputs;

            return JsonConvert.SerializeObject(parameters, Formatting.Indented);
        }

        private static JToken? TryGetPropertyInsensitive(JObject jobject, string propertyName)
            => jobject
                .Properties()
                .FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase))?.Value;

        private static JObject? TryParse(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<JObject>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}
