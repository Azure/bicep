// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text;

namespace Bicep.Cli.Commands.Helpers.Deploy;

public class DeploymentRenderer
{
    private static string RewindLines(int count) => $"{Esc}[{count}F";
    public static string EraseLine => $"{Esc}[K";

    public const char Esc = (char)27;
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

    public static string Format(DateTime utcNow, ImmutableArray<DeploymentView> deployments, int prevLineCount)
    {
        var sb = new StringBuilder();

        if (prevLineCount > 0)
        {
            sb.Append(DeploymentRenderer.HideCursor);
            sb.Append(RewindLines(prevLineCount));
        }

        if (deployments.SingleOrDefault(d => d.IsEntryPoint) is { } entrypoint)
        {
            var indent = 0;
            AppendLine(indent, $"{EraseLine}{entrypoint.Name} {GetState(entrypoint.State)} ({GetDuration(utcNow, entrypoint.StartTime, entrypoint.EndTime)})", sb);
            RenderOperations(utcNow, sb, deployments, entrypoint, indent + 1);
            RenderOutputs(sb, entrypoint, indent);
        }

        if (prevLineCount > 0)
        {
            sb.Append(DeploymentRenderer.ShowCursor);
        }

        return sb.ToString();
    }

    private static void RenderOutputs(StringBuilder sb, DeploymentView deployment, int indent)
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

    private static void RenderOperations(DateTime utcNow, StringBuilder sb, ImmutableArray<DeploymentView> deployments, DeploymentView deployment, int indent)
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
            AppendLine(indent, $"{EraseLine}{operation.Name} {GetState(operation.State)} ({GetDuration(utcNow, operation.StartTime, operation.EndTime)})", sb);
            if (operation.Error is not null)
            {
                AppendLine(indent + 1, $"{EraseLine}{Red}{operation.Error}{Reset}", sb);
            }

            if (operation.Type.Equals("Microsoft.Resources/deployments", StringComparison.OrdinalIgnoreCase))
            {
                var subDeployment = deployments.Single(d => d.Id == operation.Id);
                RenderOperations(utcNow, sb, deployments, subDeployment, indent + 1);
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

    private static string GetDuration(DateTime utcNow, DateTime startTime, DateTime? endTime)
    {
        var duration = (endTime ?? utcNow) - startTime;
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
