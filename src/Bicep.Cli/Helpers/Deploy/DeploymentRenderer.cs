// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Spectre.Console;

namespace Bicep.Cli.Helpers.Deploy;

public class DeploymentRenderer(IAnsiConsole console)
{
    public bool RenderDeployment(Table table, DeploymentWrapperView? view, ref bool isInitialized)
    {
        if (view?.Deployment is { } deployment)
        {
            if (!isInitialized)
            {
                table.AddColumn("Resource");
                table.AddColumn("Duration");
                table.AddColumn("Status");
                isInitialized = true;
            }

            var orderedOperations = deployment.Operations
                .OrderBy(x => x.EndTime is { } ? x.EndTime.Value : DateTime.MaxValue)
                .ThenBy(x => x.StartTime)
                .ToList();

            table.Rows.Clear();

            foreach (var operation in orderedOperations)
            {
                table.AddRow(
                    (operation.SymbolicName ?? operation.Name).EscapeMarkup(),
                    GetDuration(DateTime.UtcNow, operation.StartTime, operation.EndTime),
                    GetStatus(operation));
            }
        }

        return view?.Error is { } || DeploymentProcessor.IsTerminal(view?.Deployment?.State);
    }

    public async Task<bool> RenderDeployment(TimeSpan refreshInterval, Func<Action<DeploymentWrapperView>, Task> executeFunc, CancellationToken cancellationToken)
    {
        var isInitialized = false;
        var deploymentsTable = new Table().RoundedBorder();

        DeploymentWrapperView? view = null;
        await Task.WhenAll([
            RenderLive(refreshInterval, deploymentsTable, () => RenderDeployment(deploymentsTable, view, ref isInitialized), cancellationToken),
            executeFunc(newView => view = newView),
        ]);

        if (view?.Error is { } error)
        {
            var errorTable = new Table().RoundedBorder();
            errorTable.AddColumn("Error");

            errorTable.AddRow($"[bold red]{view.Error.EscapeMarkup()}[/]");
            console.Write(errorTable);
        }

        if (view?.Deployment is { } deployment && !deployment.Outputs.IsEmpty)
        {
            var outputsTable = new Table().RoundedBorder();
            outputsTable.AddColumn("Output");
            outputsTable.AddColumn("Value");

            foreach (var (name, value) in deployment.Outputs.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
            {
                outputsTable.AddRow(name.EscapeMarkup(), value.ToString().EscapeMarkup());
            }

            console.Write(outputsTable);
        }

        return DeploymentProcessor.IsSuccess(view?.Deployment?.State);
    }

    public async Task<bool> RenderOperation(TimeSpan refreshInterval, Func<Action<GeneralOperationView>, Task> executeFunc, CancellationToken cancellationToken)
    {
        var table = new Table().RoundedBorder();
        table.AddColumn("Operation");
        table.AddColumn("Duration");
        table.AddColumn("Status");

        var stopwatch = Stopwatch.StartNew();
        GeneralOperationView? view = null;
        await Task.WhenAll([
            RenderLive(refreshInterval, table, () => {
                table.Rows.Clear();
                if (view is { })
                {
                    table.AddRow(
                        view.Name.EscapeMarkup(),
                        GetDuration(stopwatch.Elapsed),
                        GetStatus(view));
                }

                return DeploymentProcessor.IsSuccess(view?.State);
            }, cancellationToken),
            executeFunc(newView => view = newView),
        ]);

        return DeploymentProcessor.IsSuccess(view?.State);
    }

    private async Task RenderLive(TimeSpan refreshInterval, Table table, Func<bool> refreshFunc, CancellationToken cancellationToken)
    {
        // Only support live updates in interactive consoles
        // This should allow this to render tables correctly in CI/CD pipelines where you can't ovewrite output
        if (console.Profile.Capabilities.Interactive)
        {
            await console.Live(table)
                .AutoClear(false)
                .Overflow(VerticalOverflow.Visible)
                .StartAsync(async ctx =>
                {
                    var isComplete = false;
                    while (!isComplete)
                    {
                        isComplete = refreshFunc();

                        ctx.Refresh();

                        if (!isComplete)
                        {
                            await Task.Delay(refreshInterval, cancellationToken);
                        }
                    }
                });
        }
        else
        {
            var isComplete = false;
            while (!isComplete)
            {
                isComplete = refreshFunc();

                if (!isComplete)
                {
                    await Task.Delay(refreshInterval, cancellationToken);
                }
            }

            console.Write(table);
        }
    }

    private static string GetStatus(GeneralOperationView operation)
        => GetStatus(operation.State, operation.Error);

    private static string GetStatus(DeploymentOperationView operation)
        => GetStatus(operation.State, operation.Error);

    private static string GetStatus(string state, string? error)
    {
        switch (state.ToLowerInvariant())
        {
            case "succeeded":
                return $"[bold green]{state.EscapeMarkup()}[/]";
            case "failed":
                return $"[bold red]{(error ?? state).EscapeMarkup()}[/]";
            default:
                return $"[bold gray]{state.EscapeMarkup()}[/]";
        }
    }

    private static string GetDuration(DateTime utcNow, DateTime startTime, DateTime? endTime)
        => GetDuration((endTime ?? utcNow) - startTime);
    
    private static string GetDuration(TimeSpan duration)
        => $"{duration.TotalSeconds:0.0}s";

    // private static string GetPortalUrl(DeploymentOperationView operation)
    // {
    //     switch (operation.Type.ToLowerInvariant())
    //     {
    //         case "microsoft.resources/deployments":
    //             // Deployments have a dedicated Portal blade to track progress
    //             return $"{portalBaseUrl}/#@{tenantId}/blade/HubsExtension/DeploymentDetailsBlade/overview/id/{Uri.EscapeDataString(operation.Id)}";
    //         default:
    //             return $"{portalBaseUrl}/#@{tenantId}/resource{operation.Id}";
    //     }
    // }
}