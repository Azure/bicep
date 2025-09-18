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
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Utils;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.Commands.Helpers.Deploy;

public record UsingConfig(
    string? Name,
    string Scope,
    StacksConfig? StacksConfig);

public record StacksConfig(
    string? Description,
    ActionOnUnmanage? ActionOnUnmanage,
    DenySettings? DenySettings);

public record DeployCommandsConfig(
    string Template,
    string Parameters,
    UsingConfig UsingConfig);

public class DeploymentProcessor(ArmClient armClient)
{
    public static async Task<DeployCommandsConfig> GetDeployCommandsConfig(IEnvironment environment, IReadOnlyDictionary<string, string> additionalArgs, ParametersResult result)
    {
        if (result.Template?.Template is not { } template ||
            result.Parameters is not { } parameters)
        {
            throw new Exception($"Failed to compile Bicep parameters");
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
            StacksConfig: stacksConfig);

        return new(
            Template: template,
            Parameters: paramsDefinition.ToJson(),
            UsingConfig: config);
    }

    public async Task Deploy(DeployCommandsConfig config, Action<ImmutableArray<DeploymentView>> onRefresh, CancellationToken cancellationToken)
    {
        var (template, parameters, usingConfig) = config;
        var deploymentName = usingConfig.Name ?? "main";
        var paramsDefinition = parameters.FromJson<DeploymentParametersDefinition>();

        var deploymentsClient = armClient.GetResourceGroupResource(new ResourceIdentifier(usingConfig.Scope)).GetArmDeployments();
        string entrypointDeploymentId;
        if (usingConfig.StacksConfig is { } stacksConfig)
        {
            var stacksClient = armClient.GetResourceGroupResource(new ResourceIdentifier(usingConfig.Scope)).GetDeploymentStacks();

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
            SortedDictionary<string, DeploymentView> deployments = new(StringComparer.OrdinalIgnoreCase);
            Queue<string> deploymentsToFetch = [];
            deploymentsToFetch.Enqueue(entrypointDeploymentId);

            // TODO track stack to completion (including post-deployment resource cleanup stuff + error handling)
            while (deploymentsToFetch.Any())
            {
                var id = deploymentsToFetch.Dequeue();
                if (deployments.ContainsKey(id))
                {
                    continue;
                }

                var isEntryPoint = id.Equals(entrypointDeploymentId, StringComparison.OrdinalIgnoreCase);
                var deployment = await GetDeployment(id, isEntryPoint, cancellationToken);
                deployments.Add(deployment.Id, deployment);

                foreach (var op in deployment.Operations)
                {
                    if (op.Type.Equals("Microsoft.Resources/deployments", StringComparison.OrdinalIgnoreCase))
                    {
                        deploymentsToFetch.Enqueue(op.Id);
                    }
                }
            }

            onRefresh([.. deployments.Values]);

            complete = IsTerminal(deployments[entrypointDeploymentId].State);
            pollAttempt++;
            if (!complete)
            {
                // optimize for quick polls at the start, then back off (2s -> 4s -> 8s -> 10s)
                var waitTimeSeconds = Math.Max(pollAttempt * 2, 10);
                await Task.Delay(TimeSpan.FromSeconds(waitTimeSeconds), cancellationToken);
            }
        }
    }

    private async Task<DeploymentView> GetDeployment(string resourceId, bool isEntryPoint, CancellationToken cancellationToken)
    {
        List<DeploymentOperationView> cliOperations = [];

        var id = ResourceIdentifier.Parse(resourceId);
        var deploymentsClient = armClient.GetArmDeploymentResource(id);

        var deployment = await deploymentsClient.GetAsync(cancellationToken);
        await foreach (var operation in deploymentsClient.GetDeploymentOperationsAsync(cancellationToken: cancellationToken))
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
            cliOperations.Add(new(
                Id: operation.Properties.TargetResource.Id,
                Name: operation.Properties.TargetResource.ResourceName,
                Type: operation.Properties.TargetResource.ResourceType!,
                State: operationState,
                StartTime: operation.Properties.Timestamp!.Value.DateTime,
                EndTime: IsTerminal(operationState) ? operation.Properties.Timestamp!.Value.Add(operation.Properties.Duration!.Value).DateTime : null,
                Error: GetError(operation)));
        }

        var deploymentState = deployment.Value.Data.Properties.ProvisioningState!.Value.ToString();
        return new(
            Id: deployment.Value.Data.Id.ToString(),
            Name: deployment.Value.Data.Name,
            StartTime: deployment.Value.Data.Properties.Timestamp!.Value.DateTime,
            EndTime: IsTerminal(deploymentState) ? deployment.Value.Data.Properties.Timestamp!.Value.Add(deployment.Value.Data.Properties.Duration!.Value).DateTime : null,
            Operations: [.. cliOperations],
            IsEntryPoint: isEntryPoint,
            State: deploymentState,
            Error: GetError(deployment.Value.Data),
            Outputs: GetOutputs(deployment.Value.Data));
    }

    private static string? GetError(ArmDeploymentOperation operation)
    {
        if (operation.Properties.StatusMessage?.Error is not { } error)
        {
            return null;
        }

        return $"{error.Code}: {error.Message}";
    }

    private static string? GetError(ArmDeploymentData deployment)
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
            return ImmutableDictionary<string, JsonNode>.Empty;
        }

        var outputs = outputsData.ToString().FromJson<Dictionary<string, DeploymentParameterDefinition>>();

        return outputs.ToImmutableDictionary(
            kvp => kvp.Key,
            kvp => JsonNode.Parse(kvp.Value.Value.ToJson())!);
    }

    public static bool IsTerminal(string state)
        => state.ToLowerInvariant() is "succeeded" or "failed" or "canceled";

    public static bool IsSuccess(IEnumerable<DeploymentView> deployments)
        => deployments.FirstOrDefault(x => x.IsEntryPoint)?.State.ToLowerInvariant() is "succeeded";
}
