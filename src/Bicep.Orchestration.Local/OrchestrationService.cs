// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Azure.Core;
using Azure.Deployments.Core.Definitions;
using Bicep.Cli.Helpers;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.SourceGraph;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Local.Deploy.Azure;
using Bicep.Local.Deploy.Extensibility;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Orchestration.Local;

public record LaunchOptions(string Workspace, string EntryPoint, string StatePath);
public record SourceLocation(string Uri, int Line);
public record StackSource(SourceLocation Declaration, SourceLocation? Requires);
public record StackView(string Name, string Region, string? Stage, int? StageOrder, JsonElement Labels, string[] Dependencies, JsonElement[] Observed, StackSource? Source);
public record Topology(string Target, string TargetInstance, long TargetRevision, StackView[] Stacks);
public record StackChanges(string Name, JsonElement[] Changes, string SelectionReason, string[] RequiredBy);
public record DeploymentPlan(string Id, string Fingerprint, string Source, string SourceFingerprint, string ParametersFingerprint,
    string ObservedFingerprint, string[] Selectors, string Target, string TargetInstance, long TargetRevision, DateTimeOffset Created, StackChanges[] Stacks);
public record ProgressEvent(long Sequence, DateTimeOffset Time, string? Stack, string Status);
public record OperationView(string Id, string PlanId, string Target, string TargetInstance, long TargetRevision, string Status, string[] Stacks, ProgressEvent[] Events);
public record PendingApproval(DeploymentPlan Plan, DateTimeOffset SubmittedAt);
public record ActivityView(string Target, string TargetInstance, long TargetRevision, DeploymentPlan? Plan, OperationView? Operation, PendingApproval[] PendingApprovals);

public sealed class OrchestrationRequestException(string message) : InvalidOperationException(message)
{
}

public sealed class OrchestrationService(
    LaunchOptions options,
    IServiceScopeFactory scopes,
    IHostApplicationLifetime lifetime) : IAsyncDisposable
{
    private sealed record Prepared(Compilation Compilation, OrchestrationSelection Selection, OrchestrationSelection All, string Parameters,
        IReadOnlyDictionary<string, (string Name, int Order)> RegionStages, FileStamp[] Inputs, bool InputsAreLocal, string Fingerprint);
    private sealed record FileStamp(string Path, bool Exists, long Length, long LastWriteTicks);
    internal sealed class Operation(string id, string planId, string target, string targetInstance, long targetRevision, string[] stacks)
    {
        public readonly object Sync = new();
        public readonly List<ProgressEvent> Events = [];
        public string Status = "Queued";
        public OperationView View() { lock (Sync) { return new(id, planId, target, targetInstance, targetRevision, Status, stacks, [.. Events]); } }
        public void Add(string? stack, string status)
        {
            lock (Sync)
            {
                if (stack is not null && Events.LastOrDefault(e => e.Stack == stack)?.Status == status) { return; }
                if (stack is null) { Status = status; }
                Events.Add(new(Events.Count + 1, DateTimeOffset.UtcNow, stack, status));
            }
        }
        public void Complete(string status)
        {
            lock (Sync)
            {
                var unfinished = Events.Where(e => e.Stack is not null).GroupBy(e => e.Stack)
                    .Select(group => group.Last()).Where(e => e.Status is "Queued" or "Running").ToArray();
                foreach (var progress in unfinished)
                {
                    Add(progress.Stack, status == "Canceled" ? "Canceled" : progress.Status == "Queued" ? "Skipped" : "Failed");
                }
                Add(null, status);
            }
        }
    }

    private readonly SemaphoreSlim gate = new(1);
    private readonly SemaphoreSlim approvals = new(1);
    private readonly object stateLock = new();
    private readonly ConcurrentDictionary<string, DeploymentPlan> plans = new();
    private readonly Dictionary<string, PendingApproval> pendingApprovals = new();
    private readonly ConcurrentDictionary<string, Operation> operations = new();
    private readonly string targetInstance = Guid.NewGuid().ToString("N");
    private const long targetRevision = 0;
    private Topology? cached;
    private Prepared? cachedPrepared;
    private FileStamp? cachedStateStamp;
    private Task? running;
    private DeploymentPlan? latestPlan;
    private Operation? latestOperation;
    private const string Target = "JSON SIMULATION";

    private ArmDeploymentProviderFactory.Session Session()
    {
        var provider = new SimulatedArmDeploymentProvider(options.StatePath);
        return new(provider, provider);
    }

    private async Task<Prepared> Prepare(IServiceProvider services, string[] selectors, CancellationToken ct)
    {
        services.GetRequiredService<IBicepConfigurationManager>().PurgeAllCaches();
        var compiler = services.GetRequiredService<BicepCompiler>();
        var compilation = await compiler.CreateCompilation(new Uri(options.EntryPoint).ToIOUri());
        var errors = compilation.GetAllDiagnosticsByBicepFile().Values.SelectMany(x => x)
            .Where(x => x.Level == DiagnosticLevel.Error).Select(x => x.Code).Distinct().ToArray();
        if (errors.Length > 0) { throw new InvalidOperationException($"Compilation failed ({string.Join(", ", errors)}). See diagnostics in VS Code."); }
        if (compilation.GetEntrypointSemanticModel().TargetScope != ResourceScope.Orchestrator)
        {
            throw new InvalidOperationException("The entry point must target orchestrator scope.");
        }
        var emitted = compilation.Emitter.Parameters();
        var template = emitted.Template?.Template ?? throw new InvalidOperationException("No orchestration template.");
        var parameters = emitted.Parameters ?? throw new InvalidOperationException("No orchestration parameters.");
        var regionStages = ReadRegionStages(template, parameters);
        var selection = await OrchestrationSelection.Create(template, parameters, OrchestrationSelection.ParseLabels(selectors), false, ct);
        var (inputs, inputsAreLocal) = GetFileStamps(compilation, options.Workspace);
        // Includes source text (even edits with unchanged emitted output), resolved parameters and auxiliary inputs.
        var sources = compilation.SourceFileGrouping.SourceFiles.OrderBy(x => x.FileHandle.Uri.ToString())
            .Select(x => x.FileHandle.Uri + "\n" + x.Text);
        var auxiliary = compilation.SourceFileGrouping.GetAllReferencedAuxiliaryFileUris().OrderBy(x => x.ToString())
            .Select(uri => services.GetRequiredService<Bicep.IO.Abstraction.IFileExplorer>().GetFile(uri).ReadAllText());
        var all = selectors.Length == 0 ? selection : await OrchestrationSelection.Create(template, parameters, OrchestrationSelection.ParseLabels([]), false, ct);
        return new(compilation, selection, all, parameters, regionStages, inputs, inputsAreLocal,
            Hash(string.Join("\n", sources.Concat(auxiliary)) + template + parameters
            + JsonSerializer.Serialize(compilation.GetEntrypointSemanticModel().Configuration) + string.Join("\n", selectors) + Target));
    }

    private static (FileStamp[] Stamps, bool AreLocal) GetFileStamps(Compilation compilation, string workspace)
    {
        var sourceUris = compilation.SourceFileGrouping.SourceFiles.Select(file => file.FileHandle.Uri);
        var auxiliaryUris = compilation.SourceFileGrouping.GetAllReferencedAuxiliaryFileUris();
        var uris = sourceUris.Concat(auxiliaryUris).Distinct().ToArray();
        var paths = uris.Select(uri => uri.TryGetFilePath()).ToArray();
        if (paths.Any(path => path is null))
        {
            return ([], false);
        }

        var localPaths = paths.Select(path => path ?? throw new InvalidOperationException("Expected a local source path.")).ToArray();
        var workspacePath = Path.GetFullPath(workspace);
        var comparer = OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
        var configPaths = localPaths.SelectMany(path => ConfigFiles(path, workspacePath));
        var trackedPaths = localPaths.Concat(configPaths).Select(Path.GetFullPath)
            .Distinct(comparer).Order(comparer);
        return ([.. trackedPaths.Select(CaptureFileStamp)], true);
    }

    private static IEnumerable<string> ConfigFiles(string filePath, string workspace)
    {
        var directory = Path.GetDirectoryName(Path.GetFullPath(filePath));
        while (directory is not null && IsWithinWorkspace(directory, workspace))
        {
            yield return Path.Combine(directory, "bicepconfig.json");
            if (string.Equals(directory, workspace, OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
            {
                yield break;
            }
            directory = Path.GetDirectoryName(directory);
        }
    }

    private static bool IsWithinWorkspace(string path, string workspace)
    {
        var relative = Path.GetRelativePath(workspace, path);
        return relative == "." || (!Path.IsPathRooted(relative) &&
            relative != ".." && !relative.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal));
    }

    private static FileStamp CaptureFileStamp(string path)
    {
        var info = new FileInfo(path);
        return info.Exists
            ? new(path, true, info.Length, info.LastWriteTimeUtc.Ticks)
            : new(path, false, 0, 0);
    }

    private static bool InputsAreCurrent(Prepared prepared) =>
        prepared.InputsAreLocal && prepared.Inputs.All(stamp => CaptureFileStamp(stamp.Path) == stamp);

    private static IReadOnlyDictionary<string, (string Name, int Order)> ReadRegionStages(string template, string parameters)
    {
        using var templateDocument = JsonDocument.Parse(template);
        using var parametersDocument = JsonDocument.Parse(parameters);
        var templateRoot = templateDocument.RootElement;
        var parametersRoot = parametersDocument.RootElement.GetProperty("parameters");
        JsonElement mappings;
        if (parametersRoot.TryGetProperty("stageMappings", out var direct) && direct.TryGetProperty("value", out var directValue))
        {
            mappings = directValue;
        }
        else if (parametersRoot.TryGetProperty("config", out var config) &&
            config.TryGetProperty("value", out var configValue) &&
            configValue.TryGetProperty("stageMappings", out var configMappings))
        {
            mappings = configMappings;
        }
        else if (templateRoot.TryGetProperty("parameters", out var templateParameters) &&
            templateParameters.TryGetProperty("stageMappings", out var stageParameter) &&
            stageParameter.TryGetProperty("defaultValue", out var defaultMappings))
        {
            mappings = defaultMappings;
        }
        else
        {
            return new Dictionary<string, (string Name, int Order)>(StringComparer.OrdinalIgnoreCase);
        }

        if (mappings.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidOperationException("stageMappings must be an array of named region groups.");
        }

        var regionStages = new Dictionary<string, (string Name, int Order)>(StringComparer.OrdinalIgnoreCase);
        var order = 0;
        foreach (var mapping in mappings.EnumerateArray())
        {
            var name = RequiredString(mapping, "name");
            if (!mapping.TryGetProperty("regions", out var regions) || regions.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidOperationException($"Stage mapping '{name}' must contain a regions array.");
            }
            foreach (var region in regions.EnumerateArray())
            {
                var regionName = region.GetString() ?? throw new InvalidOperationException($"Stage mapping '{name}' contains a non-string region.");
                if (!regionStages.TryAdd(regionName, (name, order)))
                {
                    throw new InvalidOperationException($"Region '{regionName}' appears in more than one stage mapping.");
                }
            }
            order++;
        }
        return regionStages;
    }

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    private static string RequiredString(JsonElement element, string property) => element.GetProperty(property).GetString()
        ?? throw new InvalidOperationException($"Missing required property: {property}.");
    private static string Symbol(JsonElement stack) => RequiredString(stack, "symbolicName");
    private static IReadOnlyDictionary<string, StackSource> GetStackSources(Compilation compilation)
    {
        if (!compilation.GetEntrypointSemanticModel().Root.TryGetBicepFileSemanticModelViaUsing()
            .IsSuccess(out var model, out _) || model is not SemanticModel orchestration)
        {
            throw new InvalidOperationException("Unable to resolve the orchestration source file.");
        }
        if (orchestration.SourceFile.FileHandle.Uri.TryGetFilePath() is not { } path)
        {
            return new Dictionary<string, StackSource>();
        }

        var uri = new Uri(path).AbsoluteUri;
        SourceLocation Location(int position) => new(uri, TextCoordinateConverter.GetPosition(orchestration.SourceFile.LineStarts, position).line + 1);
        return orchestration.Root.AllStackDeclarations.ToDictionary(stack => stack.SymbolicName, stack =>
        {
            var syntax = stack.DeclaringStack;
            var requires = syntax.TryGetBody()?.TryGetPropertyByName("requires");
            return new StackSource(Location(syntax.Keyword.Span.Position), requires is null ? null : Location(requires.Span.Position));
        }, StringComparer.Ordinal);
    }

    private static async Task<(DeploymentLocator Locator, string Parameters, NestedDeploymentExtension.StacksConfig Config)> StackConfig(JsonElement stack)
    {
        var properties = stack.GetProperty("properties");
        var processed = await NestedDeploymentExtension.ProcessParameters(RequiredString(properties, "parameters"),
            JObject.Parse(properties.GetProperty("body").GetProperty("inputs").GetRawText()));
        var id = new ResourceIdentifier(processed.UsingConfig.Scope);
        if (id.SubscriptionId is null) { throw new InvalidOperationException("Above-subscription scope is unsupported."); }
        var config = processed.UsingConfig.StacksConfig ?? throw new InvalidOperationException("Stack configuration is required.");
        config = config with { Version = properties.GetProperty("body").TryGetProperty("version", out var version) ? version.GetString() : null };
        return (new("", null, id.SubscriptionId, id.ResourceGroupName, processed.UsingConfig.Name ?? "main"),
            processed.Parameters, config);
    }

    private async Task<Topology> Observe(Prepared prepared, IArmDeploymentProvider provider, CancellationToken ct)
    {
        var views = new List<StackView>();
        var sources = GetStackSources(prepared.Compilation);
        foreach (var stack in prepared.All.Resources)
        {
            var name = Symbol(stack);
            var (locator, _, _) = await StackConfig(stack);
            var raw = await provider.ObserveStack(prepared.Compilation.GetEntrypointSemanticModel().Configuration, locator, ct);
            var body = stack.GetProperty("properties").GetProperty("body");
            var region = RequiredString(body, "region");
            var stage = prepared.RegionStages.TryGetValue(region, out var assignment) ? assignment : ((string Name, int Order)?)null;
            sources.TryGetValue(Regex.Replace(name, @"\[\d+\]", ""), out var source);
            views.Add(new(name, region, stage?.Name, stage?.Order,
                body.TryGetProperty("labels", out var labels) ? labels : JsonSerializer.SerializeToElement(new { }),
                [.. prepared.All.Dependencies[name].Order()], JsonSerializer.Deserialize<JsonElement[]>(raw) ?? throw new InvalidOperationException("Invalid observation."), source));
        }
        return new(Target, targetInstance, targetRevision, [.. views]);
    }

    private string ObservedFingerprint(Topology topology) => Hash(JsonSerializer.Serialize(topology)
        // Include unmanaged simulation entries too: an intervening import must not be overwritten by an old plan.
        + (File.Exists(options.StatePath) ? File.ReadAllText(options.StatePath) : "<absent>"));
    private string Fingerprint(Prepared prepared, Topology topology) => Hash(prepared.Fingerprint + ObservedFingerprint(topology));

    private void ValidateTarget(string? expectedInstance, long? expectedRevision)
    {
        if (expectedInstance is null && expectedRevision is null) { return; }
        if (expectedInstance != targetInstance || expectedRevision != targetRevision)
        {
            throw new OrchestrationRequestException("Target changed. Wait for automatic refresh and try again.");
        }
    }

    private Topology Cache(Topology topology)
    {
        lock (stateLock) { cached = topology; }
        return topology;
    }

    public async Task<Topology> GetTopology(CancellationToken ct, string? expectedInstance = null, long? expectedRevision = null)
    {
        if (!await gate.WaitAsync(0, ct))
        {
            lock (stateLock)
            {
                ValidateTarget(expectedInstance, expectedRevision);
                return cached ?? throw new OrchestrationRequestException("An operation is in progress. Retry topology shortly.");
            }
        }
        try
        {
            ValidateTarget(expectedInstance, expectedRevision);
            lock (stateLock)
            {
                if (cached is not null && cachedPrepared is { } current &&
                    InputsAreCurrent(current) && cachedStateStamp == CaptureFileStamp(options.StatePath))
                {
                    return cached;
                }
            }

            using var scope = scopes.CreateScope();
            Prepared? prepared;
            lock (stateLock)
            {
                prepared = cachedPrepared is { } current && InputsAreCurrent(current) ? current : null;
            }
            prepared ??= await Prepare(scope.ServiceProvider, [], ct);
            await using var session = Session();
            var topology = await Observe(prepared, session.Provider, ct);
            lock (stateLock)
            {
                cached = topology;
                cachedPrepared = prepared;
                cachedStateStamp = CaptureFileStamp(options.StatePath);
            }
            return topology;
        }
        finally { gate.Release(); }
    }

    public async Task<DeploymentPlan> Plan(string[] selectors, CancellationToken ct, string? expectedInstance = null, long? expectedRevision = null)
    {
        // Prediction is read-only. Queue behind topology refreshes (including the automatic
        // refresh after completion), rather than making an ordinary user action fail at random.
        // Approval rejects concurrent deployment attempts.
        await gate.WaitAsync(ct);
        try
        {
            ValidateTarget(expectedInstance, expectedRevision);
            using var scope = scopes.CreateScope();
            var prepared = await Prepare(scope.ServiceProvider, selectors, ct);
            await using var session = Session();
            var topology = await Observe(prepared, session.Provider, ct);
            var changes = new List<StackChanges>();
            var dependents = prepared.Selection.Dependencies
                .Where(dependency => prepared.Selection.Selected.Contains(dependency.Key))
                .SelectMany(dependency => dependency.Value.Select(name => (Name: name, Dependent: dependency.Key)))
                .ToLookup(item => item.Name, item => item.Dependent, StringComparer.Ordinal);
            foreach (var stack in prepared.Selection.Resources)
            {
                var (locator, parameters, config) = await StackConfig(stack);
                var properties = stack.GetProperty("properties");
                var result = await session.Provider.WhatIf(prepared.Compilation.GetEntrypointSemanticModel().Configuration,
                    locator, RequiredString(properties.GetProperty("body"), "region"),
                    RequiredString(properties, "template"), parameters, config, ct);
                var resourceChanges = JsonSerializer.Deserialize<JsonElement[]>(result.ToJson()) ?? throw new InvalidOperationException("Invalid prediction.");
                var name = Symbol(stack);
                var matched = prepared.Selection.Matched.Contains(name);
                var requiredBy = dependents[name].Order(StringComparer.Ordinal).ToArray();
                changes.Add(new(name, resourceChanges, matched ? "Selected" : "Prerequisite", requiredBy));
            }
            var plan = new DeploymentPlan(Guid.NewGuid().ToString("N"), Fingerprint(prepared, topology),
                Path.GetRelativePath(options.Workspace, options.EntryPoint), prepared.Fingerprint, Hash(prepared.Parameters),
                ObservedFingerprint(topology), selectors, Target, targetInstance, targetRevision, DateTimeOffset.UtcNow, [.. changes]);
            // Bounded in-memory plan cache. Plans expire on process restart.
            if (plans.Count >= 100) { plans.Clear(); }
            plans[plan.Id] = plan;
            lock (stateLock) { latestPlan = plan; }
            return plan;
        }
        finally { gate.Release(); }
    }

    private async Task<Prepared> ValidatePlan(DeploymentPlan plan, IServiceProvider services, IArmDeploymentProvider provider, CancellationToken ct)
    {
        ValidateTarget(plan.TargetInstance, plan.TargetRevision);
        var prepared = await Prepare(services, plan.Selectors, ct);
        var topology = await Observe(prepared, provider, ct);
        if (Fingerprint(prepared, topology) != plan.Fingerprint)
        {
            throw new OrchestrationRequestException("Stale plan: source, parameters, target, or observed state changed. Predict and submit a new plan.");
        }
        return prepared;
    }

    public async Task<PendingApproval> Submit(string planId, CancellationToken ct)
    {
        await gate.WaitAsync(ct);
        try
        {
            lock (stateLock)
            {
                if (pendingApprovals.ContainsKey(planId)) { throw new OrchestrationRequestException("This plan is already awaiting approval."); }
                if (pendingApprovals.Count >= 100) { throw new OrchestrationRequestException("The approval queue is full. Approve or reject a pending plan first."); }
            }
            if (!plans.TryGetValue(planId, out var plan)) { throw new OrchestrationRequestException("Plan not found; predict again."); }
            using var scope = scopes.CreateScope();
            await using var session = Session();
            await ValidatePlan(plan, scope.ServiceProvider, session.Provider, ct);
            var submission = new PendingApproval(plan, DateTimeOffset.UtcNow);
            lock (stateLock)
            {
                pendingApprovals.Add(planId, submission);
                plans.TryRemove(planId, out _);
                if (latestPlan?.Id == planId) { latestPlan = null; }
            }
            return submission;
        }
        finally { gate.Release(); }
    }

    public PendingApproval Reject(string planId)
    {
        lock (stateLock)
        {
            if (!pendingApprovals.Remove(planId, out var submission)) { throw new OrchestrationRequestException("Plan is not awaiting approval."); }
            return submission;
        }
    }

    public async Task<OperationView> Approve(string planId, bool approved, CancellationToken ct)
    {
        if (!approved) { throw new OrchestrationRequestException("Explicit approval is required."); }
        if (!await approvals.WaitAsync(0, ct)) { throw new OrchestrationRequestException("An operation is already in progress."); }
        try
        {
            lock (stateLock)
            {
                if (latestOperation?.View().Status is "Queued" or "Running") { throw new OrchestrationRequestException("An operation is already in progress."); }
            }
            return await ApproveCore(planId, ct);
        }
        finally { approvals.Release(); }
    }

    private async Task<OperationView> ApproveCore(string planId, CancellationToken ct)
    {
        // Wait for read-only topology/prediction work without accepting concurrent approvals.
        await gate.WaitAsync(ct);
        IServiceScope? scope = null;
        ArmDeploymentProviderFactory.Session? session = null;
        try
        {
            PendingApproval submission;
            lock (stateLock)
            {
                if (!pendingApprovals.TryGetValue(planId, out var pending)) { throw new OrchestrationRequestException("Plan is not awaiting approval. Submit it first."); }
                submission = pending;
            }
            var plan = submission.Plan;
            scope = scopes.CreateScope();
            session = Session();
            var prepared = await ValidatePlan(plan, scope.ServiceProvider, session.Provider, ct);
            var operation = new Operation(Guid.NewGuid().ToString("N"), planId, Target, targetInstance, targetRevision, [.. plan.Stacks.Select(stack => stack.Name)]);
            var view = operation.View();
            lock (stateLock)
            {
                if (!pendingApprovals.Remove(planId)) { throw new OrchestrationRequestException("Plan is no longer awaiting approval."); }
                operations[view.Id] = operation;
                latestOperation = operation;
            }
            if (operations.Count > 100)
            {
                foreach (var previous in operations.Where(x => x.Key != view.Id).Take(operations.Count - 100)) { operations.TryRemove(previous.Key, out _); }
            }
            var ownedScope = scope;
            var ownedSession = session;
            // Execution owns its provider and lock until completion, independently of the HTTP request.
            running = Task.Run(async () =>
            {
                var terminal = "Failed";
                try
                {
                    operation.Add(null, "Running");
                    foreach (var name in prepared.Selection.Selected.Order()) { operation.Add(name, "Queued"); }
                    var progressProvider = new ProgressProvider(ownedSession.Provider, async (locator, status) =>
                    {
                        var name = locator.DeploymentName;
                        foreach (var stack in prepared.Selection.Resources)
                        {
                            if ((await StackConfig(stack)).Locator == locator) { name = Symbol(stack); break; }
                        }
                        operation.Add(name, status);
                        if (status == "Succeeded") { Cache(await Observe(prepared, ownedSession.Provider, lifetime.ApplicationStopping)); }
                    });
                    await using var dispatcher = ownedScope.ServiceProvider.GetRequiredService<LocalExtensionDispatcherFactory>().Create(progressProvider);
                    await dispatcher.InitializeExtensions(prepared.Compilation);
                    var result = await dispatcher.Deploy(prepared.Selection.Template, prepared.Parameters, _ => { }, lifetime.ApplicationStopping);
                    foreach (var resource in result.Operations)
                    {
                        var status = resource.Properties.ProvisioningState.ToString();
                        if (resource.Properties.TargetResource?.SymbolicName is { } name && prepared.Selection.Selected.Contains(name) &&
                            status is "Failed" or "Canceled")
                        {
                            operation.Add(name, status);
                        }
                    }
                    Cache(await Observe(prepared, ownedSession.Provider, lifetime.ApplicationStopping));
                    terminal = result.Deployment.Properties.ProvisioningState?.ToString() == "Succeeded" ? "Succeeded" : "Failed";
                }
                catch (OperationCanceledException) { terminal = "Canceled"; }
                catch { terminal = "Failed"; } // No provider exception payloads (may contain credentials) in public events/logs.
                finally
                {
                    try { await ownedSession.DisposeAsync(); }
                    catch { terminal = "Failed"; }
                    finally
                    {
                        try { ownedScope.Dispose(); }
                        catch { terminal = "Failed"; throw; }
                        finally
                        {
                            lock (stateLock)
                            {
                                operation.Complete(terminal);
                                gate.Release();
                            }
                        }
                    }
                }
            }, CancellationToken.None);
            scope = null;
            session = null;
            return view;
        }
        catch
        {
            try { if (session is not null) { await session.DisposeAsync(); } }
            finally
            {
                try { scope?.Dispose(); }
                finally { gate.Release(); }
            }
            throw;
        }
    }

    public OperationView GetOperation(string id)
    {
        lock (stateLock)
        {
            return operations.TryGetValue(id, out var operation) ? operation.View() : throw new KeyNotFoundException("Operation not found.");
        }
    }

    public ActivityView GetActivity()
    {
        lock (stateLock) { return new(Target, targetInstance, targetRevision, latestPlan, latestOperation?.View(), [.. pendingApprovals.Values.OrderBy(item => item.SubmittedAt)]); }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD003", Justification = "Shutdown joins the owned background operation without a synchronization context.")]
    public async ValueTask DisposeAsync()
    {
        if (running is not null) { await running; }
        gate.Dispose();
        approvals.Dispose();
    }
}
