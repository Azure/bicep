// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace Bicep.Orchestration.Local;

[McpServerToolType]
public sealed class OrchestrationTools(OrchestrationService service)
{
    [McpServerTool(Name = "get_topology"), Description("Read desired dependencies and provider-observed resources for the shared target. Supply the target instance and revision from get_activity to reject a changed target. No deployment.")]
    public async Task<Topology> GetTopology(CancellationToken cancellationToken, string? expectedTargetInstance = null, long? expectedTargetRevision = null)
        => await SurfaceRequestErrors(() => service.GetTopology(cancellationToken, expectedTargetInstance, expectedTargetRevision));

    [McpServerTool(Name = "get_activity"), Description("Get the shared target, latest draft plan and operation, and plans pending approval. No deployment.")]
    public ActivityView GetActivity() => service.GetActivity();

    [McpServerTool(Name = "plan_deployment"), Description("Read-only prediction. Selectors are key=value label filters. Supply the target instance and revision from get_activity to reject a changed target. Incomplete prediction is an error, not a safety guarantee.")]
    public async Task<DeploymentPlan> PlanDeployment(CancellationToken cancellationToken, string[]? selectors = null, string? expectedTargetInstance = null, long? expectedTargetRevision = null)
        => await SurfaceRequestErrors(() => service.Plan(selectors ?? [], cancellationToken, expectedTargetInstance, expectedTargetRevision));

    [McpServerTool(Name = "submit_deployment"), Description("Submit a predicted plan for approval. Validates freshness and queues the exact plan. Does not approve or deploy anything.")]
    public async Task<PendingApproval> SubmitDeployment(string planId, CancellationToken cancellationToken)
        => await SurfaceRequestErrors(() => service.Submit(planId, cancellationToken));

    [McpServerTool(Name = "approve_deployment", Destructive = true), Description("Approve and execute a submitted plan. Set approved=true ONLY after explicit user approval for this exact plan. Rejects stale or unsubmitted plans. Returns an asynchronous operation ID.")]
    public async Task<OperationView> ApproveDeployment(string planId, bool approved, CancellationToken cancellationToken)
        => await SurfaceRequestErrors(() => service.Approve(planId, approved, cancellationToken));

    [McpServerTool(Name = "reject_deployment"), Description("Remove a submitted plan from the approval queue without deploying anything.")]
    public async Task<PendingApproval> RejectDeployment(string planId)
        => await SurfaceRequestErrors(() => Task.FromResult(service.Reject(planId)));

    [McpServerTool(Name = "get_operation"), Description("Get authoritative status and truthful stack-level progress; no invented resource-level progress.")]
    public OperationView GetOperation(string operationId) => service.GetOperation(operationId);

    private static async Task<T> SurfaceRequestErrors<T>(Func<Task<T>> request)
    {
        try { return await request(); }
        catch (OrchestrationRequestException exception) { throw new McpException(exception.Message, exception); }
    }
}
