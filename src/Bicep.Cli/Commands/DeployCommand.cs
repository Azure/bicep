// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Azure.Core;
using Azure.Deployments.ClientTools;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Json;
using Azure.Deployments.Expression.Intermediate;
using Azure.Deployments.Expression.Intermediate.Extensions;
using Azure.Deployments.Extensibility.Core.V2.Models;
using Azure.Deployments.Templates.Export;
using Azure.Deployments.Templates.Expressions.PartialEvaluation;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.AzureApi;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.TypeSystem;
using Bicep.Core.Utils;
using Bicep.Local.Deploy;
using Bicep.Local.Deploy.Extensibility;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.Commands;

public class DeployCommand(
    IOContext io,
    IEnvironment environment,
    IArmClientProvider armClientProvider,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler,
    InputOutputArgumentsResolver inputOutputArgumentsResolver) : ICommand
{
    public async Task<int> RunAsync(DeployArguments args, CancellationToken cancellationToken)
    {
        var paramsFileUri = inputOutputArgumentsResolver.PathToUri(args.ParamsFile);
        ArgumentHelper.ValidateBicepParamFile(paramsFileUri);

        var compilation = await compiler.CreateCompilation(paramsFileUri, skipRestore: args.NoRestore);

        var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, compilation);
        var parameters = compilation.Emitter.Parameters();

        if (summary.HasErrors)
        {
            return 1;
        }

        var model = compilation.GetEntrypointSemanticModel();

        var armClient = armClientProvider.CreateArmClient(model.Configuration, null);
        var renderer = new Renderer();
        var processor = new DeploymentProcessor(environment, armClient, renderer);

        var onComplete = new CancellationTokenSource();
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, onComplete.Token);
        await Task.WhenAll([
            renderer.RenderLoop(io, TimeSpan.FromMilliseconds(50), linkedCts.Token),
            Process(processor, args, parameters, cancellationToken, onComplete),
        ]);

        return 0;
    }

    private async Task Process(DeploymentProcessor processor, DeployArguments args, ParametersResult result, CancellationToken cancellationToken, CancellationTokenSource onComplete)
    {
        try
        {
            await processor.Process(result, args, cancellationToken);
        }
        finally
        {
            await onComplete.CancelAsync();
        }
    }
}

public class Renderer
{
    public record Deployment(
        string Id,
        string Name,
        string State,
        DateTime StartTime,
        DateTime? EndTime,
        ImmutableArray<DeploymentOperation> Operations,
        bool IsEntryPoint,
        string? Error,
        ImmutableDictionary<string, JsonNode> Outputs);

    public record DeploymentOperation(
        string Id,
        string Name,
        string Type,
        string State,
        DateTime StartTime,
        DateTime? EndTime,
        string? Error);

    private static string RewindLines(int count) => $"{Esc}[{count}F";
    private static string EraseLine => $"{Esc}[K";

    private const char Esc = (char)27;
    public static string Orange { get; } = $"{Esc}[38;5;208m";
    public static string Green { get; } = $"{Esc}[38;5;77m";
    public static string Purple { get; } = $"{Esc}[38;5;141m";
    public static string Blue { get; } = $"{Esc}[38;5;39m";
    public static string Gray { get; } = $"{Esc}[38;5;246m";
    public static string Reset { get; } = $"{Esc}[0m";
    public static string Red { get; } = $"{Esc}[38;5;203m";
    public static string Bold { get; } = $"{Esc}[1m";

    public static string HideCursor { get; } = $"{Esc}[?25l";
    public static string ShowCursor { get; } = $"{Esc}[?25h";

    private ImmutableArray<Deployment> deployments = [];

    public void Refresh(ImmutableArray<Deployment> newDeployments)
    {
        lock (this)
        {
            deployments = newDeployments;
        }
    }

    public async Task RenderLoop(IOContext io, TimeSpan refreshInterval, CancellationToken cancellationToken)
    {
        var lineCount = 0;
        try
        {
            while (true)
            {
                Render(io, deployments, ref lineCount);
                await Task.Delay(refreshInterval, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Always render the final state before exiting
            Render(io, deployments, ref lineCount);
        }
    }

    public static void Render(IOContext io, ImmutableArray<Deployment> deployments, ref int lineCount)
    {
        io.Output.Write(Renderer.HideCursor);
        var output = GetOutput(deployments, lineCount);
        io.Output.Write(output);
        io.Output.Write(Renderer.ShowCursor);
        lineCount = output.Count(c => c == '\n');
    }

    public static string GetOutput(ImmutableArray<Deployment> deployments, int prevLineCount)
    {
        var sb = new StringBuilder();
        var entrypoint = deployments.SingleOrDefault(d => d.IsEntryPoint);
        if (entrypoint is null)
        {
            return "";
        }

        if (prevLineCount > 0)
        {
            sb.Append(RewindLines(prevLineCount));
        }

        var indent = 0;
        AppendLine(indent, $"{EraseLine}{entrypoint.Name} {GetState(entrypoint.State)} ({GetDuration(entrypoint.StartTime, entrypoint.EndTime)})", sb);
        RenderOperations(sb, deployments, entrypoint, indent + 1);
        RenderOutputs(sb, entrypoint, indent);

        return sb.ToString();
    }

    private static void RenderOutputs(StringBuilder sb, Deployment deployment, int indent)
    {
        var outputs = deployment.Outputs;
        if (!deployment.Outputs.Any())
        {
            return;
        }

        sb.AppendLine();
        AppendLine(indent, $"{EraseLine}Outputs:", sb);
        foreach (var output in outputs)
        {
            AppendLine(indent + 1, $"{EraseLine}{output.Key}: {output.Value}", sb);
        }
    }

    private static void RenderOperations(StringBuilder sb, ImmutableArray<Deployment> deployments, Deployment deployment, int indent)
    {
        var tenantId = Guid.NewGuid();
        var orderedOperations = deployment.Operations
            .OrderBy(x => x.EndTime is { } ? x.EndTime.Value : DateTime.MaxValue)
            .ThenBy(x => x.StartTime)
            .ToList();

        if (deployment.Error is { } error)
        {
            AppendLine(indent, $"{EraseLine}{Red}{error}{Reset}", sb);
        }

        foreach (var operation in orderedOperations)
        {
            AppendLine(indent, $"{EraseLine}{operation.Name} {GetState(operation.State)} ({GetDuration(operation.StartTime, operation.EndTime)})", sb);
            if (operation.Error is not null)
            {
                AppendLine(indent + 1, $"{EraseLine}{Red}{operation.Error}{Reset}", sb);
            }

            if (operation.Type.Equals("Microsoft.Resources/deployments", StringComparison.OrdinalIgnoreCase))
            {
                var subDeployment = deployments.Single(d => d.Id == operation.Id);
                RenderOperations(sb, deployments, subDeployment, indent + 1);
            }
        }
    }

    private static string GetState(string state) => state.ToLowerInvariant() switch
    {
        "succeeded" => $"{Bold}{Green}{state}{Reset}",
        "failed" => $"{Bold}{Red}{state}{Reset}",
        "accepted" => $"{Bold}{Gray}{state}{Reset}",
        "running" => $"{Bold}{Gray}{state}{Reset}",
        _ => $"{Bold}{state}{Reset}",
    };

    private static string GetDuration(DateTime startTime, DateTime? endTime)
    {
        var duration = (endTime ?? DateTime.UtcNow) - startTime;
        return $"{duration.TotalSeconds:0.0}s";
    }

    private static void AppendLine(int indent, string text, StringBuilder sb)
    {
        if (text.Length == 0)
        {
            sb.AppendLine();
            return;
        }

        while (text.Length > 0)
        {
            var indentString = new string(' ', indent * 2);
            var wrapIndex = Math.Min(text.Length + indentString.Length, TerminalWidth - 1);
            sb.Append(indentString);
            sb.Append(text[..(wrapIndex - indentString.Length)]);
            sb.AppendLine();
            text = text[(wrapIndex - indentString.Length)..];
        }
    }

    private static int TerminalWidth => (Console.IsOutputRedirected || Console.BufferWidth == 0) ? int.MaxValue : Console.BufferWidth;
}

public class DeploymentProcessor(IEnvironment environment, ArmClient armClient, Renderer renderer)
{
    public record ProcessParametersResult(
        string Parameters,
        string Scope,
        string DeploymentName);

    public static async Task<ProcessParametersResult> ProcessParameters(IEnvironment environment, LiveDeploymentArgumentsBase args, string parameters)
    {
        parameters = await ParametersProcessor.Process(parameters, async (kind, config) =>
        {
            await Task.CompletedTask;
            switch (kind)
            {
                case "sys.cliArg":
                    var argKey = config!.Value<string>()!;
                    return args.AdditionalArguments.TryGetValue(argKey) ?? throw new InvalidOperationException($"CLI argument '{argKey}' is not set.");
                case "sys.envVar":
                    var envKey = config!.Value<string>()!;
                    return environment.GetVariable(envKey) ?? throw new InvalidOperationException($"Environment variable '{envKey}' is not set.");
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
            Azure.Deployments.Expression.Expressions.ExpressionBuiltInFunctions.Functions,
            new ParametersScope(paramsDefinition.ExternalInputs),
        ]);

        if (usingConfigParam.Expression is { } expression)
        {
            usingConfigParam.Value = ToJTokenExpressionSerializer.Serialize(
                context.EvaluateExpression(ExpressionParser.ParseLanguageExpression(expression)));
            usingConfigParam.Expression = null;
        }

        var usingConfig = usingConfigParam.Value ?? throw new UnreachableException("Should have already been evaluated");

        // required property - should have already been validated
        var scope = usingConfig.GetProperty("scope")?.Value<string>() ?? throw new UnreachableException();
        // optional property - default to "main"
        var deploymentName = usingConfig.GetProperty("name")?.Value<string>() ?? "main";

        // TODO: This is a hack to pass config around in Bicep - should not be sent on the wire.
        paramsDefinition.Parameters.Remove("$usingConfig");

        return new(
            Parameters: paramsDefinition.ToJson(),
            Scope: scope,
            DeploymentName: deploymentName);
    }

    public async Task Process(
        ParametersResult result,
        DeployArguments args,
        CancellationToken cancellationToken = default)
    {
        if (result.Template?.Template is not { } template ||
            result.Parameters is not { } parameters)
        {
            throw new Exception($"Failed to compile Bicep parameters");
        }

        (parameters, var scope, var deploymentName) = await ProcessParameters(environment, args, parameters);

        var deploymentsClient = armClient.GetResourceGroupResource(new ResourceIdentifier(scope)).GetArmDeployments();
        var paramsDefinition = parameters.FromJson<DeploymentParametersDefinition>();
        var deploymentProperties = new ArmDeploymentProperties(ArmDeploymentMode.Incremental)
        {
            Template = BinaryData.FromString(template),
            Parameters = BinaryData.FromString(paramsDefinition.Parameters.ToJson()),
        };

        foreach (var kvp in paramsDefinition.ExternalInputs ?? [])
        {
            deploymentProperties.ExternalInputs[kvp.Key] = new(BinaryData.FromString(kvp.Value.ToJson()));
        }

        // foreach (var kvp in paramsDefinition.ExternalInputDefinitions ?? [])
        // {
        //     deploymentProperties.ExternalInputDefinitions[kvp.Key] = new(kvp.Value.Kind)
        //     {
        //         Config = BinaryData.FromString(kvp.Value.Config.ToJson()),
        //     };
        // }

        var armDeploymentContent = new ArmDeploymentContent(deploymentProperties);

        var started = await deploymentsClient.CreateOrUpdateAsync(Azure.WaitUntil.Started, deploymentName, armDeploymentContent, cancellationToken);

        var entrypointDeploymentId = $"{scope}/providers/Microsoft.Resources/deployments/{deploymentName}";
        var complete = false;
        while (!complete)
        {
            SortedDictionary<string, Renderer.Deployment> deployments = new(StringComparer.OrdinalIgnoreCase);
            Queue<string> deploymentsToFetch = [];
            deploymentsToFetch.Enqueue(entrypointDeploymentId);

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

            renderer.Refresh([.. deployments.Values]);

            complete = IsTerminal(deployments[entrypointDeploymentId].State);
            if (!complete)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }
        }
    }

    private async Task<Renderer.Deployment> GetDeployment(string resourceId, bool isEntryPoint, CancellationToken cancellationToken)
    {
        List<Renderer.DeploymentOperation> cliOperations = [];

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

    private static bool IsTerminal(string state)
        => state.ToLowerInvariant() is "succeeded" or "failed" or "canceled";
}