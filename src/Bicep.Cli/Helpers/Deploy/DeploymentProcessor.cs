// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json.Nodes;
using Azure.Core;
using Azure.Deployments.ClientTools;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Expression.Intermediate;
using Azure.Deployments.Expression.Intermediate.Extensions;
using Azure.Deployments.Templates.Expressions.PartialEvaluation;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers.WhatIf;
using Bicep.Core;
using Bicep.Core.AzureApi;
using Bicep.Core.Configuration;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.TypeSystem;
using Bicep.Core.Utils;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.Helpers.Deploy;

public record UsingConfig(
    string? Name,
    string Scope,
    ResourceScope ScopeType,
    StacksConfig? StacksConfig);

public record StacksConfig(
    string? Description,
    ActionOnUnmanage? ActionOnUnmanage,
    DenySettings? DenySettings);

public record DeployCommandsConfig(
    string Template,
    string Parameters,
    UsingConfig UsingConfig);

public class DeploymentProcessor(IArmClientProvider armClientProvider) : IDeploymentProcessor
{
    public static async Task<DeployCommandsConfig> GetDeployCommandsConfig(IEnvironment environment, IReadOnlyDictionary<string, string> additionalArgs, ParametersResult result, ResourceScope scopeType)
    {
        if (result.Template?.Template is not { } template ||
            result.Parameters is not { } parameters)
        {
            throw new InvalidOperationException($"Failed to compile Bicep parameters");
        }

        var foundCliArgs = new HashSet<string>();
        parameters = await ParametersProcessor.Process(parameters, async (kind, config) =>
        {
            await Task.CompletedTask;
            switch (kind)
            {
                case "sys.cliArg":
                    var argKey = config!.Value<string>()!;
                    var expectedArgument = $"{ArgumentConstants.CliArgPrefix}{argKey}";
                    foundCliArgs.Add(argKey);
                    return additionalArgs.TryGetValue(argKey) ?? throw new CommandLineException($"CLI argument '{expectedArgument}' must be provided.");
                case "sys.envVar":
                    var envKey = config!.Value<string>()!;
                    return environment.GetVariable(envKey) ?? throw new CommandLineException($"Environment variable '{envKey}' is not set.");
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        });

        if (additionalArgs.Keys.Except(foundCliArgs).ToImmutableArray() is { Length: > 0 } extraArgs)
        {
            throw new CommandLineException($"The following CLI argument(s) were provided but not required: {string.Join(", ", extraArgs.Select(a => $"{ArgumentConstants.CliArgPrefix}{a}"))}");
        }

        var paramsDefinition = parameters.FromJson<DeploymentParametersDefinition>();

        ExpressionEvaluationContext evalContext = new(
        [
            Azure.Deployments.Expression.Expressions.ExpressionBuiltInFunctions.Functions,
            new ParametersScope(paramsDefinition.ExternalInputs),
        ]);

        var usingConfigJToken = parameters.FromJson<JObject>().GetProperty(ParametersJsonWriter.UsingConfigPropertyName);
        if (usingConfigJToken is null)
        {
            // This should have been validated by the compilation process
            throw new UnreachableException();
        }
        var usingConfig = usingConfigJToken.FromJToken<DeploymentParameterDefinition>() switch
        {
            { Expression: { } expression } => ToJTokenExpressionSerializer.Serialize(evalContext.EvaluateExpression(ExpressionParser.ParseLanguageExpression(expression))),
            { Value: { } value } => value,
            _ => throw new InvalidOperationException($"Failed to evaluate {ParametersJsonWriter.UsingConfigPropertyName}"),
        };

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
            Scope: usingConfig.GetProperty("scope")?.Value<string>() ?? throw new UnreachableException(),
            ScopeType: scopeType,
            StacksConfig: stacksConfig);

        return new(
            Template: template,
            Parameters: paramsDefinition.ToJson(),
            UsingConfig: config);
    }

    private static DeploymentStackCollection GetStacksClient(ArmClient armClient, UsingConfig usingConfig)
        => usingConfig.ScopeType switch
        {
            ResourceScope.ResourceGroup => armClient.GetResourceGroupResource(new ResourceIdentifier(usingConfig.Scope)).GetDeploymentStacks(),
            ResourceScope.Subscription => armClient.GetSubscriptionResource(new ResourceIdentifier(usingConfig.Scope)).GetDeploymentStacks(),
            ResourceScope.ManagementGroup => armClient.GetManagementGroupResource(new ResourceIdentifier(usingConfig.Scope)).GetDeploymentStacks(),
            _ => throw new CommandLineException($"Target scope {usingConfig.ScopeType} is not supported."),
        };

    private static ArmDeploymentCollection GetDeploymentsClient(ArmClient armClient, UsingConfig usingConfig)
        => usingConfig.ScopeType switch
        {
            ResourceScope.ResourceGroup => armClient.GetResourceGroupResource(new ResourceIdentifier(usingConfig.Scope)).GetArmDeployments(),
            ResourceScope.Subscription => armClient.GetSubscriptionResource(new ResourceIdentifier(usingConfig.Scope)).GetArmDeployments(),
            ResourceScope.ManagementGroup => armClient.GetManagementGroupResource(new ResourceIdentifier(usingConfig.Scope)).GetArmDeployments(),
            _ => throw new CommandLineException($"Target scope {usingConfig.ScopeType} is not supported."),
        };

    public async Task Deploy(RootConfiguration bicepConfig, DeployCommandsConfig config, Action<DeploymentWrapperView> onRefresh, CancellationToken cancellationToken)
    {
        try
        {
            var armClient = armClientProvider.CreateArmClient(bicepConfig, null);

            var (template, parameters, usingConfig) = config;
            var deploymentName = usingConfig.Name ?? "main";
            var paramsDefinition = parameters.FromJson<DeploymentParametersDefinition>();

            string entrypointDeploymentId;
            if (usingConfig.StacksConfig is { } stacksConfig)
            {
                var stacksClient = GetStacksClient(armClient, usingConfig);

                DeploymentStackData stacksData = new()
                {
                    Description = stacksConfig.Description,
                    ActionOnUnmanage = stacksConfig.ActionOnUnmanage,
                    DenySettings = stacksConfig.DenySettings,
                    Template = BinaryData.FromString(template),
                };

                foreach (var kvp in paramsDefinition.Parameters ?? [])
                {
                    stacksData.Parameters[kvp.Key] = new DeploymentParameter()
                    {
                        // TODO handle expressions, external inputs
                        Value = BinaryData.FromString(kvp.Value.Value.ToJson()),
                    };
                }

                await stacksClient.CreateOrUpdateAsync(Azure.WaitUntil.Started, deploymentName, stacksData, cancellationToken);
                while (true)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                    var response = await stacksClient.GetAsync(deploymentName, cancellationToken);
                    if (response.Value.Data.DeploymentId is { })
                    {
                        entrypointDeploymentId = response.Value.Data.DeploymentId;
                        break;
                    }
                }
            }
            else
            {
                var deploymentsClient = GetDeploymentsClient(armClient, usingConfig);
                var deploymentProperties = new ArmDeploymentProperties(ArmDeploymentMode.Incremental)
                {
                    Template = BinaryData.FromString(template),
                    Parameters = BinaryData.FromString(paramsDefinition.Parameters.ToJson()),
                };

                foreach (var kvp in paramsDefinition.ExternalInputs ?? [])
                {
                    deploymentProperties.ExternalInputs[kvp.Key] = new(BinaryData.FromString(kvp.Value.ToJson()));
                }

                var armDeploymentContent = new ArmDeploymentContent(deploymentProperties);

                await deploymentsClient.CreateOrUpdateAsync(Azure.WaitUntil.Started, deploymentName, armDeploymentContent, cancellationToken);
                entrypointDeploymentId = $"{usingConfig.Scope}/providers/Microsoft.Resources/deployments/{deploymentName}";
            }

            var pollAttempt = 0;
            var complete = false;
            while (!complete)
            {
                var deployment = await GetDeployment(armClient, entrypointDeploymentId, cancellationToken);

                onRefresh(new(deployment, null));

                complete = IsTerminal(deployment.State);
                pollAttempt++;
                if (!complete)
                {
                    // optimize for quick polls at the start, then back off (2s -> 4s -> 8s -> 10s)
                    var waitTimeSeconds = Math.Max(pollAttempt * 2, 10);
                    await Task.Delay(TimeSpan.FromSeconds(waitTimeSeconds), cancellationToken);
                }
            }
        }
        catch (Exception exception)
        {
            // ensure we report the error
            onRefresh(new(null, exception.Message));
        }
    }

    public async Task<DeploymentWhatIfResponseDefinition> WhatIf(RootConfiguration bicepConfig, DeployCommandsConfig config, CancellationToken cancellationToken)
    {
        var armClient = armClientProvider.CreateArmClient(bicepConfig, null);

        var (template, parameters, usingConfig) = config;
        var deploymentName = usingConfig.Name ?? "main";
        var paramsDefinition = parameters.FromJson<DeploymentParametersDefinition>();

        if (config.UsingConfig.StacksConfig is { })
        {
            throw new CommandLineException("What-If analysis is not currently supported for stack deployments.");
        }

        var deploymentResource = armClient.GetArmDeploymentResource(new ResourceIdentifier($"{usingConfig.Scope}/providers/Microsoft.Resources/deployments/{deploymentName}"));
        var deploymentProperties = new ArmDeploymentWhatIfProperties(ArmDeploymentMode.Incremental)
        {
            Template = BinaryData.FromString(template),
            Parameters = BinaryData.FromString(paramsDefinition.Parameters.ToJson()),
        };
        var armDeploymentContent = new ArmDeploymentWhatIfContent(deploymentProperties);

        var response = await deploymentResource.WhatIfAsync(Azure.WaitUntil.Completed, armDeploymentContent, cancellationToken);

        return response.GetRawResponse().Content.ToString().FromJson<DeploymentWhatIfResponseDefinition>();
    }

    public async Task Teardown(RootConfiguration bicepConfig, DeployCommandsConfig config, Action<GeneralOperationView> onRefresh, CancellationToken cancellationToken)
    {
        try
        {
            var armClient = armClientProvider.CreateArmClient(bicepConfig, null);

            var (template, parameters, usingConfig) = config;
            var deploymentName = usingConfig.Name ?? "main";
            var paramsDefinition = parameters.FromJson<DeploymentParametersDefinition>();

            if (config.UsingConfig.StacksConfig is not { } stacksConfig)
            {
                throw new CommandLineException("What-If analysis is not currently supported for stack deployments.");
            }

            var stackResource = armClient.GetDeploymentStackResource(new ResourceIdentifier($"{usingConfig.Scope}/providers/Microsoft.Resources/deploymentStacks/{deploymentName}"));
            onRefresh(new("Teardown", "Running", null));

            await stackResource.DeleteAsync(Azure.WaitUntil.Completed,
                unmanageActionResources: stacksConfig.ActionOnUnmanage?.Resources switch
                {
                    // TODO simplify
                    { } val when val.Equals(DeploymentStacksDeleteDetachEnum.Delete) => UnmanageActionResourceMode.Delete,
                    { } val when val.Equals(DeploymentStacksDeleteDetachEnum.Detach) => UnmanageActionResourceMode.Detach,
                    _ => (UnmanageActionResourceMode?)null,
                },
                unmanageActionResourceGroups: stacksConfig.ActionOnUnmanage?.ResourceGroups switch
                {
                    // TODO simplify
                    { } val when val.Equals(DeploymentStacksDeleteDetachEnum.Delete) => UnmanageActionResourceGroupMode.Delete,
                    { } val when val.Equals(DeploymentStacksDeleteDetachEnum.Detach) => UnmanageActionResourceGroupMode.Detach,
                    _ => (UnmanageActionResourceGroupMode?)null,
                },
                unmanageActionManagementGroups: stacksConfig.ActionOnUnmanage?.ManagementGroups switch
                {
                    // TODO simplify
                    { } val when val.Equals(DeploymentStacksDeleteDetachEnum.Delete) => UnmanageActionManagementGroupMode.Delete,
                    { } val when val.Equals(DeploymentStacksDeleteDetachEnum.Detach) => UnmanageActionManagementGroupMode.Detach,
                    _ => (UnmanageActionManagementGroupMode?)null,
                },
                // TODO figure out what to do with this
                // bypassStackOutOfSyncError: true,
                cancellationToken: cancellationToken);

            onRefresh(new("Teardown", "Succeeded", null));
        }
        catch (Exception exception)
        {
            // ensure we report the error
            onRefresh(new("Teardown", "Failed", exception.Message));
        }
    }

    private static async Task<DeploymentView> GetDeployment(ArmClient armClient, string resourceId, CancellationToken cancellationToken)
    {
        var id = ResourceIdentifier.Parse(resourceId);
        var deploymentsClient = armClient.GetArmDeploymentResource(id);

        var deployment = await deploymentsClient.GetAsync(cancellationToken);
        List<ArmDeploymentOperation> operations = [];
        await foreach (var operation in deploymentsClient.GetDeploymentOperationsAsync(cancellationToken: cancellationToken))
        {
            operations.Add(operation);
        }

        return GetDeploymentView(deployment, operations);
    }

    private static string? GetError(ArmDeploymentOperation operation)
    {
        if (operation.Properties.StatusMessage?.Error is not { } error)
        {
            return null;
        }

        return $"{error.Code}: {error.Message}";
    }

    private static string? GetError(DeploymentOperationDefinition operation)
    {
        if (operation.Properties.StatusMessage?.GetProperty("error") is not { } error)
        {
            return null;
        }

        return $"{error.GetProperty("code")}: {error.GetProperty("message")}";
    }

    private static string? GetError(ArmDeploymentData deployment)
    {
        if (deployment.Properties.Error is not { } error)
        {
            return null;
        }

        return $"{error.Code}: {error.Message}";
    }

    private static string? GetError(DeploymentContent deployment)
    {
        if (deployment.Properties.Error is not { } error)
        {
            return null;
        }

        return $"{error.Code}: {error.Message}";
    }

    private static ImmutableDictionary<string, JsonNode> GetOutputs(ArmDeploymentData deployment)
    {
        if (deployment.Properties.Outputs is not { } outputsData)
        {
            return [];
        }

        var outputs = outputsData.ToString().FromJson<Dictionary<string, DeploymentParameterDefinition>>();

        return outputs.ToImmutableDictionary(
            kvp => kvp.Key,
            kvp => JsonNode.Parse(kvp.Value.Value.ToJson())!);
    }

    private static ImmutableDictionary<string, JsonNode> GetOutputs(DeploymentContent deployment)
    {
        if (deployment.Properties.Outputs is not { } outputs)
        {
            return [];
        }

        return outputs.ToImmutableDictionary(
            kvp => kvp.Key,
            kvp => JsonNode.Parse(kvp.Value.Value.ToJson())!);
    }

    public static bool IsTerminal(string? state)
        => state?.ToLowerInvariant() is "succeeded" or "failed" or "canceled";

    public static bool IsActive(string? state)
        => !IsTerminal(state);

    public static bool IsSuccess(string? state)
        => state?.ToLowerInvariant() is "succeeded";

    private static DeploymentView GetDeploymentView(ArmDeploymentResource deployment, IEnumerable<ArmDeploymentOperation> operations)
    {
        List<DeploymentOperationView> operationViews = [];
        foreach (var operation in operations)
        {
            if (operation.Properties.TargetResource is null)
            {
                continue;
            }

            if (operation.Properties.ProvisioningOperation == ProvisioningOperationKind.Read)
            {
                // Hide read operations for now
                // TODO re-visit this
                continue;
            }

            var operationState = operation.Properties.ProvisioningState.ToString();
            operationViews.Add(new(
                Id: operation.Properties.TargetResource.Id,
                Name: operation.Properties.TargetResource.ResourceName,
                SymbolicName: operation.Properties.TargetResource.SymbolicName,
                Type: operation.Properties.TargetResource.ResourceType!,
                State: operationState,
                StartTime: operation.Properties.Timestamp!.Value.DateTime,
                EndTime: IsTerminal(operationState) ? operation.Properties.Timestamp!.Value.Add(operation.Properties.Duration!.Value).DateTime : null,
                Error: GetError(operation)));
        }

        var deploymentState = deployment.Data.Properties.ProvisioningState!.Value.ToString();
        return new(
            Id: deployment.Data.Id.ToString(),
            Name: deployment.Data.Name,
            StartTime: deployment.Data.Properties.Timestamp!.Value.DateTime,
            EndTime: IsTerminal(deploymentState) ? deployment.Data.Properties.Timestamp!.Value.Add(deployment.Data.Properties.Duration!.Value).DateTime : null,
            Operations: [.. operationViews],
            State: deploymentState,
            Error: GetError(deployment.Data),
            Outputs: GetOutputs(deployment.Data));
    }

    public static DeploymentView GetDeploymentView(DeploymentContent deployment, IEnumerable<DeploymentOperationDefinition> operations)
    {
        List<DeploymentOperationView> operationViews = [];
        foreach (var operation in operations)
        {
            if (operation.Properties.TargetResource is null)
            {
                continue;
            }

            if (operation.Properties.ProvisioningOperation == ProvisioningOperation.Read)
            {
                // Hide read operations for now
                // TODO re-visit this
                continue;
            }

            var operationState = operation.Properties.ProvisioningState.ToString();
            operationViews.Add(new(
                Id: operation.Properties.TargetResource.Id,
                Name: operation.Properties.TargetResource.ResourceName,
                SymbolicName: operation.Properties.TargetResource.SymbolicName,
                Type: operation.Properties.TargetResource.ResourceType!,
                State: operationState,
                StartTime: operation.Properties.Timestamp!.Value,
                EndTime: IsTerminal(operationState) ? operation.Properties.Timestamp!.Value.Add(operation.Properties.Duration!.Value) : null,
                Error: GetError(operation)));
        }

        var deploymentState = deployment.Properties.ProvisioningState!.Value.ToString();
        return new(
            Id: deployment.Id.ToString(),
            Name: deployment.Name,
            StartTime: deployment.Properties.Timestamp!.Value,
            EndTime: IsTerminal(deploymentState) ? deployment.Properties.Timestamp!.Value.Add(deployment.Properties.Duration!.Value) : null,
            Operations: [.. operationViews],
            State: deploymentState,
            Error: GetError(deployment),
            Outputs: GetOutputs(deployment));
    }
}
