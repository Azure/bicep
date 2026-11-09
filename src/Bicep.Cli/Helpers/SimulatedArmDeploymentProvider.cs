// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Entities;
using Bicep.Cli.Helpers.Snapshots;
using Bicep.Core.Configuration;
using Bicep.Core.Json;
using Bicep.Core.TypeSystem;
using Bicep.Core.Utils.Snapshots;
using Bicep.Local.Deploy;
using Bicep.Local.Deploy.Azure;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;
using StacksConfig = Bicep.Local.Deploy.Extensibility.NestedDeploymentExtension.StacksConfig;

namespace Bicep.Cli.Helpers;

public sealed class SimulatedArmDeploymentProvider : IArmDeploymentProvider, IAsyncDisposable
{
    public bool SupportsExternalExtensions => false;

    public const string EnvironmentVariable = "BICEP_ORCHESTRATION_SIMULATION_STATE";

    private readonly string path;
    private readonly FileStream fileLock;
    private readonly SemaphoreSlim gate = new(1);
    private JArray state;

    public SimulatedArmDeploymentProvider(string path)
    {
        this.path = Path.GetFullPath(path);
        fileLock = new FileStream(this.path + ".lock", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        try
        {
            state = File.Exists(this.path) ? JArray.Parse(File.ReadAllText(this.path)) : new JArray();
            var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in state)
            {
                if (entry is not JObject resource || resource["id"] is not JValue { Type: JTokenType.String } idValue ||
                    idValue.Value<string>() is not { } id || !id.StartsWith('/'))
                {
                    throw new CommandLineException("Simulation state must be a JSON array of resource objects, each with an 'id' string starting with '/'.");
                }
                if (!ids.Add(id))
                {
                    throw new CommandLineException($"Simulation state contains duplicate resource ID '{id}'.");
                }
            }
        }
        catch
        {
            fileLock.Dispose();
            throw;
        }
    }

    private static string Scope(DeploymentLocator locator)
    {
        if (locator.SubscriptionId is null || locator.ResourceGroupName is null)
        {
            throw new CommandLineException("Simulation currently supports resource-group deployment scopes only.");
        }
        return $"/subscriptions/{locator.SubscriptionId}/resourceGroups/{locator.ResourceGroupName}";
    }

    private static string StackId(DeploymentLocator locator) =>
        $"{Scope(locator)}/providers/Microsoft.Resources/deploymentStacks/{locator.DeploymentName}";

    private static JObject? Find(JArray resources, string id) =>
        resources.OfType<JObject>().FirstOrDefault(resource => string.Equals(resource["id"]?.Value<string>(), id, StringComparison.OrdinalIgnoreCase));

    private static void Put(JArray resources, string id, JObject value)
    {
        Remove(resources, id);
        value["id"] = id;
        resources.Add(value);
    }

    private static void Remove(JArray resources, string id) =>
        Find(resources, id)?.Remove();

    public async Task CreateResourceGroup(IBicepConfiguration configuration, DeploymentLocator locator, string location, CancellationToken cancellationToken)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            var next = (JArray)state.DeepClone();
            var id = Scope(locator);
            Put(next, id, new JObject
            {
                ["id"] = id,
                ["name"] = locator.ResourceGroupName,
                ["type"] = "Microsoft.Resources/resourceGroups",
                ["location"] = location,
                ["properties"] = new JObject { ["provisioningState"] = "Succeeded" },
            });
            await Save(next, cancellationToken);
        }
        finally
        {
            gate.Release();
        }
    }

    private async Task<JObject> Predict(JArray resources, DeploymentLocator locator, string template, string parameters, HashSet<string> managed, CancellationToken cancellationToken)
    {
        var snapshot = await SnapshotHelper.GetSnapshot(
            ResourceScope.ResourceGroup,
            template,
            parameters,
            tenantId: null,
            managementGroupId: null,
            subscriptionId: locator.SubscriptionId,
            resourceGroup: locator.ResourceGroupName,
            deploymentName: locator.DeploymentName,
            location: Find(resources, Scope(locator))?["location"]?.Value<string>(),
            externalInputs: [],
            includeSymbolicNames: false,
            cancellationToken: cancellationToken);
        if (snapshot.Diagnostics.Length > 0)
        {
            throw new CommandLineException($"Simulation prediction failed for '{locator.DeploymentName}': {string.Join(Environment.NewLine, snapshot.Diagnostics)}");
        }
        foreach (var prediction in snapshot.PredictedResources)
        {
            var resource = JObject.Parse(prediction.GetRawText());
            if (resource["id"]?.Value<string>() is not { } id || !id.StartsWith('/'))
            {
                throw new CommandLineException("Simulation cannot persist a prediction without a resolved resource ID.");
            }
            if (!managed.Add(id))
            {
                throw new CommandLineException($"Simulation predicted duplicate resource ID '{id}'.");
            }
            Put(resources, id, resource);
        }
        var outputDefinitions = JObject.Parse(template)["outputs"] as JObject;
        return new JObject(snapshot.Outputs.Select(output => new JProperty(output.Key, new JObject
        {
            ["type"] = outputDefinitions?[output.Key]?["type"]?.DeepClone()
                ?? throw new CommandLineException($"Simulation output '{output.Key}' has no type."),
            ["value"] = JToken.Parse(output.Value.GetRawText()),
        })));
    }

    public async Task StartDeploymentStack(IBicepConfiguration configuration, DeploymentLocator locator, string template, string parameters, StacksConfig stacksConfig, CancellationToken cancellationToken)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            var next = (JArray)state.DeepClone();
            var managed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var outputs = await Predict(next, locator, template, parameters, managed, cancellationToken);
            var id = StackId(locator);
            var previous = Find(next, id);
            foreach (var removed in ManagedResources(previous).Except(managed, StringComparer.OrdinalIgnoreCase))
            {
                Unmanage(next, removed, stacksConfig);
            }
            Put(next, id, new JObject
            {
                ["id"] = id,
                ["name"] = locator.DeploymentName,
                ["type"] = "Microsoft.Resources/deploymentStacks",
                ["properties"] = new JObject
                {
                    ["provisioningState"] = "Succeeded",
                    ["resources"] = new JArray(managed.Order(StringComparer.OrdinalIgnoreCase).Select(resourceId => new JObject { ["id"] = resourceId })),
                    ["outputs"] = outputs,
                    ["template"] = JObject.Parse(template),
                    ["parameters"] = JObject.Parse(parameters)["parameters"],
                },
            });
            await Save(next, cancellationToken);
        }
        finally
        {
            gate.Release();
        }
    }

    private static IEnumerable<string> ManagedResources(JObject? stack) =>
        (stack?["properties"]?["resources"] as JArray ?? new JArray())
            .Select(resource => resource["id"]?.Value<string>() ?? throw new CommandLineException("Simulation stack contains an invalid managed resource ID."));

    private static void Unmanage(JArray resources, string id, StacksConfig config)
    {
        var action = Find(resources, id)?["type"]?.Value<string>()?.Equals("Microsoft.Resources/resourceGroups", StringComparison.OrdinalIgnoreCase) == true
            ? config.ActionOnUnmanage?.ResourceGroups?.ToString()
            : config.ActionOnUnmanage?.Resources.ToString();
        if (action is null)
        {
            throw new CommandLineException("Simulation requires the applicable actionOnUnmanage setting to be set.");
        }
        if (action.Equals("delete", StringComparison.OrdinalIgnoreCase))
        {
            Remove(resources, id);
        }
        else if (!action.Equals("detach", StringComparison.OrdinalIgnoreCase))
        {
            throw new CommandLineException($"Unsupported simulation actionOnUnmanage '{action}'.");
        }
    }

    public async Task TeardownStack(IBicepConfiguration configuration, DeploymentLocator locator, StacksConfig config, CancellationToken cancellationToken)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            var next = (JArray)state.DeepClone();
            var id = StackId(locator);
            var stack = Find(next, id) ?? throw new CommandLineException($"Simulation stack '{id}' does not exist.");
            foreach (var resource in ManagedResources(stack))
            {
                Unmanage(next, resource, config);
            }
            Remove(next, id);
            await Save(next, cancellationToken);
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task<IReadOnlyList<DeploymentWhatIfResourceChangeDefinition>> WhatIf(IBicepConfiguration configuration, DeploymentLocator locator, string location, string template, string parameters, StacksConfig config, CancellationToken cancellationToken)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            var next = (JArray)state.DeepClone();
            Put(next, Scope(locator), new JObject
            {
                ["id"] = Scope(locator),
                ["name"] = locator.ResourceGroupName,
                ["type"] = "Microsoft.Resources/resourceGroups",
                ["location"] = location,
                ["properties"] = new JObject { ["provisioningState"] = "Succeeded" },
            });
            var managed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            await Predict(next, locator, template, parameters, managed, cancellationToken);
            var removed = ManagedResources(Find(state, StackId(locator))).Except(managed, StringComparer.OrdinalIgnoreCase).ToArray();
            foreach (var id in removed)
            {
                Unmanage(next, id, config);
            }
            var ids = managed.Concat(removed).Order(StringComparer.OrdinalIgnoreCase).ToArray();
            Snapshot GetSnapshot(JArray resources) => new(
                [.. ids.Select(id =>
                {
                    var resource = Find(resources, id);
                    if (resource is null)
                    {
                        return null;
                    }
                    var body = (JObject)resource.DeepClone();
                    body["apiVersion"] ??= Find(next, id)?["apiVersion"]?.DeepClone()
                        ?? throw new CommandLineException($"Simulation resource '{id}' has no apiVersion.");
                    return body;
                }).OfType<JObject>().Select(resource => JsonElementFactory.CreateElement(resource.ToString()))],
                [], []);
            var beforeSnapshot = GetSnapshot(state);
            var afterSnapshot = GetSnapshot(next);
            var changes = SnapshotDiffer.CalculateChanges(beforeSnapshot, afterSnapshot).ToList();
            foreach (var id in ids.Where(id => !changes.Any(change => string.Equals(change.ResourceId, id, StringComparison.OrdinalIgnoreCase))))
            {
                changes.Add(new DeploymentWhatIfResourceChangeDefinition
                {
                    ResourceId = id,
                    ChangeType = managed.Contains(id) ? DeploymentWhatIfChangeType.NoChange : DeploymentWhatIfChangeType.Ignore,
                    Before = Find(state, id)?.DeepClone(),
                    After = Find(next, id)?.DeepClone(),
                });
            }
            return changes;
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task<LocalDeploymentResult> CheckDeploymentStack(IBicepConfiguration configuration, DeploymentLocator locator, CancellationToken cancellationToken)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            var stack = Find(state, StackId(locator)) ?? throw new CommandLineException($"Simulation stack '{StackId(locator)}' does not exist.");
            var content = new JObject { ["properties"] = new JObject
            {
                ["mode"] = "Incremental",
                ["provisioningState"] = "Succeeded",
                ["outputs"] = stack["properties"]?["outputs"]?.DeepClone() ?? new JObject(),
            } }.ToString().FromJson<DeploymentContent>();
            return new(content, [], []);
        }
        finally
        {
            gate.Release();
        }
    }

    public Task StartDeployment(IBicepConfiguration configuration, DeploymentLocator locator, string template, string parameters, CancellationToken cancellationToken) =>
        throw new CommandLineException("Simulation supports orchestration stacks, not ordinary Azure deployments.");

    public Task<LocalDeploymentResult> CheckDeployment(IBicepConfiguration configuration, DeploymentLocator locator, CancellationToken cancellationToken) =>
        throw new CommandLineException("Simulation supports orchestration stacks, not ordinary Azure deployments.");

    private async Task Save(JArray next, CancellationToken cancellationToken)
    {
        var temporary = $"{path}.{Guid.NewGuid():N}.tmp";
        try
        {
            await File.WriteAllTextAsync(temporary, next.ToString(), cancellationToken);
            if (!OperatingSystem.IsWindows())
            {
                File.SetUnixFileMode(temporary, UnixFileMode.UserRead | UnixFileMode.UserWrite);
            }
            File.Move(temporary, path, overwrite: true);
            state = next;
        }
        finally
        {
            if (File.Exists(temporary))
            {
                File.Delete(temporary);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await fileLock.DisposeAsync();
        gate.Dispose();
    }
}
